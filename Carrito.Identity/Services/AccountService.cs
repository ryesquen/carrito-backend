using Carrito.Application.DTOs.Users;
using Carrito.Application.Enums;
using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Settings;
using Carrito.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Carrito.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<JWTSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ResponseService<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            var response = new ResponseService<AuthenticationResponse>();
            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
            {
                response.AddBadRequest($"No hay una cuenta registrada con el email {request.Email}.");
            }

            var result = await _signInManager.PasswordSignInAsync(usuario!.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.AddBadRequest($"Las credenciales del usuario no son validas {request.Email}.");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWTToken(usuario);
            AuthenticationResponse responseAuthentication = new AuthenticationResponse();
            responseAuthentication.Id = usuario.Id;
            responseAuthentication.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            responseAuthentication.Email = usuario.Email;
            responseAuthentication.UserName = usuario.UserName;

            var rolesList = await _userManager.GetRolesAsync(usuario).ConfigureAwait(false);
            responseAuthentication.Roles = rolesList.ToList();
            responseAuthentication.IsVerified = usuario.EmailConfirmed;

            var refreshToken = GenerateRefreshToken();
            responseAuthentication.RefreshToken = refreshToken.Token;
            response.Object = responseAuthentication;
            return response;
        }

        public async Task<ResponseService<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var response = new ResponseService<string>();
            var usuarioConElMismoUserName = await _userManager.FindByNameAsync(request.UserName);
            if (usuarioConElMismoUserName != null)
            {
                response.AddBadRequest($"El nombre de usuario {request.UserName} ya fue registrado previamente.");
            }
            var usuario = new ApplicationUser
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var usuarioConElMismoCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoCorreo != null)
            {
                response.AddBadRequest($"El email {request.Email} ya fue registrado previamente.");
            }
            else
            {
                var result = await _userManager.CreateAsync(usuario, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usuario, Roles.Basic.ToString());
                    return response;
                }
                else
                {
                    response.AddInternalServerError($"{result.Errors}.");
                }
            }
            return response;
        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser usuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(usuario);
            var roles = await _userManager.GetRolesAsync(usuario);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("uid", usuario.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );
            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(50),
                Created = DateTime.Now
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}