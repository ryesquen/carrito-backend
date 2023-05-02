using AutoMapper;
using Carrito.Application.DTOs;
using Carrito.Application.Enums;
using Carrito.Application.Interfaces;
using Carrito.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carrito.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ArticulosTiendaController : ControllerBase
    {
        private readonly IArticulosTiendaRepository _articulosTiendaRepository;
        private readonly ITiendaRepository _tiendaRepository;
        private readonly IArticuloRepository _articuloRepository;
        private readonly IMapper _mapper;
        public ArticulosTiendaController(IArticulosTiendaRepository articulosTiendaRepository, IArticuloRepository articuloRepository, IMapper mapper, ITiendaRepository tiendaRepository)
        {
            _articulosTiendaRepository = articulosTiendaRepository;
            _articuloRepository = articuloRepository;
            _mapper = mapper;
            _tiendaRepository = tiendaRepository;
        }
        [HttpGet]
        public async Task<ActionResult<List<ArticulosTiendaDto>>> GetAllArticulosTiendas()
        {
            var response = await _articulosTiendaRepository.GetAllArticulosTiendas();
            if (response.Object is not null)
            {
                var res = _mapper.Map<List<ArticulosTiendaDto>>(response.Object);
                return Ok(res);
            }
            else return StatusCode(response.Status, response.Error);
        }
        [HttpGet("{articuloId:int}/{tiendaId:int}", Name = "GetArticulosTiendaById")]
        public async Task<IActionResult> GetArticulosTiendaById(int articuloId, int tiendaId)
        {
            var response = await _articulosTiendaRepository.GetArticulosTiendaById(articuloId, tiendaId);
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
                return Ok(_mapper.Map<ArticulosTiendaDto>(response.Object));
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertArticulosTienda([FromBody] ArticulosTiendaDto articulosTiendaDto)
        {
            if (articulosTiendaDto is null) return BadRequest(articulosTiendaDto);
            var tienda = await _tiendaRepository.GetTiendaById(articulosTiendaDto.TiendaId);
            var articulo = await _articuloRepository.GetArticuloById(articulosTiendaDto.ArticuloId);
            if (!tienda.Exito || !articulo.Exito) return BadRequest(
                new
                {
                    tiendaError = tienda.Error,
                    articuloError = articulo.Error
                });
            var existe = await _articulosTiendaRepository.GetArticulosTiendaById(articulosTiendaDto.ArticuloId, articulosTiendaDto.TiendaId);
            if (existe.Object is null)
            {
                var articuloTienda = new ArticulosTienda
                {
                    TiendaId = articulosTiendaDto.TiendaId,
                    ArticuloId = articulosTiendaDto.ArticuloId,
                    Fecha = DateTime.UtcNow
                };
                await _articulosTiendaRepository.InsertArticulosTienda(articuloTienda);
                return CreatedAtRoute(nameof(GetArticulosTiendaById), new { articuloId = articuloTienda.ArticuloId, tiendaId = articuloTienda.TiendaId }, _mapper.Map<ArticulosTiendaDto>(articuloTienda));
            }
            else
            {
                articulo!.Object!.Stock++;
                await _articuloRepository.UpdateArticulo(articulo.Object.Id, articulo.Object);
                return Ok(articulosTiendaDto);
            }
        }
        [HttpDelete("{articuloId:int}/{tiendaId:int}")]
        public async Task<IActionResult> DeleteClientesArticulo(int articuloId, int tiendaId)
        {
            if (articuloId == 0 || tiendaId == 0) { return BadRequest(); }
            var existe = await _articulosTiendaRepository.GetArticulosTiendaById(articuloId, tiendaId);
            if (existe is null) { return NotFound(); }
            var response = await _articulosTiendaRepository.DeleteArticulosTienda(articuloId, tiendaId);
            if (!response.Exito) return BadRequest(response);
            else return NoContent();
        }
    }
}