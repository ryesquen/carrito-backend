using AutoMapper;
using Carrito.Application.DTOs;
using Carrito.Application.Enums;
using Carrito.Application.Interfaces;
using Carrito.Domain.Entities;
using Carrito.Persistence.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carrito.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TiendaController : ControllerBase
    {
        private readonly ITiendaRepository _tiendaRepository;
        private readonly IMapper _mapper;
        public TiendaController(ITiendaRepository tiendaRepository, IMapper mapper)
        {
            _tiendaRepository = tiendaRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<TiendaDto>>> GetAllTiendas()
        {
            var response = await _tiendaRepository.GetAllTiendas();
            if (response.Object is not null)
            {
                var res = _mapper.Map<List<TiendaDto>>(response.Object);
                return Ok(res);
            }
            else return StatusCode(response.Status, response.Error);
        }
        [HttpGet("{id:int}", Name = "GetTiendaById")]
        public async Task<IActionResult> GetTiendaById(int id)
        {
            var response = await _tiendaRepository.GetTiendaById(id);
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
                return Ok(_mapper.Map<TiendaDto>(response.Object));
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertTienda([FromBody] TiendaDto tiendaDto)
        {
            if (tiendaDto is null || tiendaDto.Id > 0) return BadRequest(tiendaDto);
            var tienda = new Tienda
            {
                Sucursal = tiendaDto.Sucursal,
                Direccion = tiendaDto.Direccion
            };
            await _tiendaRepository.InsertTienda(tienda);
            return CreatedAtRoute("GetTiendaById", new { id = tienda.Id }, _mapper.Map<TiendaDto>(tienda));
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTienda(int id, [FromBody] TiendaDto tiendaDto)
        {
            if (tiendaDto is null || id != tiendaDto.Id) return BadRequest(tiendaDto);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await _tiendaRepository.GetTiendaById(id);
            var tienda = result.Object;
            tienda!.Sucursal = tiendaDto.Sucursal;
            tienda.Direccion = tiendaDto.Direccion;
            var response = await _tiendaRepository.UpdateTienda(id, tienda!);
            if (response.Exito) return Ok(response);
            else return StatusCode(response.Status, response.Error);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTienda(int id)
        {
            if (id == 0) { return BadRequest(); }
            var existe = await _tiendaRepository.GetTiendaById(id);
            if (existe is null) { return NotFound(); }
            var response = await _tiendaRepository.DeleteTienda(id);
            if (!response.Exito) return BadRequest(response);
            else return NoContent();
        }
    }
}