using AutoMapper;
using Carrito.Application.DTOs;
using Carrito.Domain.Entities;

namespace Carrito.WebApi.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Cliente, ClienteDto>().ReverseMap();
            CreateMap<Tienda, TiendaDto>().ReverseMap();
            CreateMap<Articulo, ArticuloDto>().ReverseMap();
            CreateMap<ClientesArticulo, ClientesArticuloDto>().ReverseMap();
            CreateMap<ArticulosTienda, ArticulosTiendaDto>().ReverseMap();
        }
    }
}