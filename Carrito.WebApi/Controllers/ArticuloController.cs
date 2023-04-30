using AutoMapper;
using Carrito.Application.DTOs;
using Carrito.Application.Interfaces;
using Carrito.Domain.Entities;
using Carrito.Persistence.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carrito.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ArticuloController : ControllerBase
    {
        private readonly IArticuloRepository _articuloRepository;
        private readonly IMapper _mapper;
        public ArticuloController(IArticuloRepository articuloRepository, IMapper mapper)
        {
            _articuloRepository = articuloRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Basic")]
        public async Task<ActionResult<List<ArticuloDto>>> GetAllArticulos()
        {
            var response = await _articuloRepository.GetAllArticulos();
            if (response.Object is not null)
            {
                var res = _mapper.Map<List<ArticuloDto>>(response.Object);
                return Ok(res);
            }
            else return StatusCode(response.Status, response.Error);
        }
        [HttpGet("{id:int}", Name = "GetArticuloById")]        
        public async Task<IActionResult> GetArticuloById(int id)
        {
            var response = await _articuloRepository.GetArticuloById(id);
            if (response.Object is null)
            {
                return response.Status switch
                {
                    StatusCodes.Status404NotFound => NotFound(response),
                    StatusCodes.Status400BadRequest => BadRequest(response),
                    StatusCodes.Status500InternalServerError => StatusCode(response.Status, response.Error),
                    _ => StatusCode(response.Status, response.Error),
                };
            }
            else
            {
                return Ok(_mapper.Map<ArticuloDto>(response.Object));
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertArticulo([FromBody] ArticuloDto articuloDto)
        {
            if (articuloDto is null || articuloDto.Id > 0) return BadRequest(articuloDto);
            var articulo = new Articulo
            {
                Codigo = articuloDto.Codigo,
                Descripcion = articuloDto.Descripcion,
                Precio = articuloDto.Precio,
                Imagen = articuloDto.Imagen,
                Stock = articuloDto.Stock
            };
            await _articuloRepository.InsertArticulo(articulo);
            return CreatedAtRoute(nameof(GetArticuloById), new { id = articulo.Id }, _mapper.Map<ArticuloDto>(articulo));
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ArticuloDto articuloDto)
        {
            if (articuloDto is null || id != articuloDto.Id) return BadRequest(articuloDto);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await _articuloRepository.GetArticuloById(id);
            var articulo = result.Object;
            articulo!.Codigo = articuloDto.Codigo;
            articulo.Descripcion = articuloDto.Descripcion;
            articulo.Precio = articuloDto.Precio;
            articulo.Imagen = articuloDto.Imagen;
            articulo.Stock = articuloDto.Stock;
            var response = await _articuloRepository.UpdateArticulo(id, articulo!);
            if (response.Exito) return Ok(response);
            else return StatusCode(response.Status, response.Error);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            if (id == 0) { return BadRequest(); }
            var existe = await _articuloRepository.GetArticuloById(id);
            if (existe is null) { return NotFound(); }
            var response = await _articuloRepository.DeleteArticulo(id);
            if (!response.Exito) return BadRequest(response);
            else return NoContent();
        }
    }
}