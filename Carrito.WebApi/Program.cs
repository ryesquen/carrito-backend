using Carrito.Application.Interfaces;
using Carrito.Persistence.Contexts;
using Carrito.Persistence.Repository;
using Carrito.WebApi.Mappings;
using Microsoft.EntityFrameworkCore;
using Carrito.Identity;
using Carrito.Identity.Models;
using Carrito.Identity.Seeds;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CarritoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("cn"));
});
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITiendaRepository, TiendaRepository>();
builder.Services.AddScoped<IArticuloRepository, ArticuloRepository>();
builder.Services.AddScoped<IClientesArticuloRepository, ClientesArticuloRepository>();
builder.Services.AddScoped<IArticulosTiendaRepository, ArticulosTiendaRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DefaultRoles.SeedAsync(userManager, roleManager);
    await DefaultAdminUser.SeedAsync(userManager, roleManager);
    await DefaultBasicUser.SeedAsync(userManager, roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
