using Carrito.Application.DTOs.Users;
using Carrito.Application.Response;

namespace Carrito.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseService<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<ResponseService<string>> RegisterAsync(RegisterRequest request, string origin);
    }
}
