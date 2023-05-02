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
    //[Authorize(Roles = "Admin,Basic")]
    public class ClientesArticuloController : ControllerBase
    {
        private readonly IClientesArticuloRepository _clientesArticuloRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IArticuloRepository _articuloRepository;
        private readonly IMapper _mapper;
        public ClientesArticuloController(IClientesArticuloRepository clientesArticuloRepository, IMapper mapper, IClienteRepository clienteRepository, IArticuloRepository articuloRepository)
        {
            _clientesArticuloRepository = clientesArticuloRepository;
            _clienteRepository = clienteRepository;
            _articuloRepository = articuloRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<ClientesArticuloDto>>> GetAllClientesArticulos()
        {
            var response = await _clientesArticuloRepository.GetAllClientesArticulos();
            if (response.Object is not null)
            {
                var res = _mapper.Map<List<ClientesArticuloDto>>(response.Object);
                return Ok(res);
            }
            else return StatusCode(response.Status, response.Error);
        }
        [HttpGet("{id:int}", Name = "GetClientesArticuloById")]
        public async Task<IActionResult> GetClientesArticuloById(int id)
        {
            var response = await _clientesArticuloRepository.GetClientesArticuloById(id);
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
                return Ok(_mapper.Map<ClientesArticuloDto>(response.Object));
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertClientesArticulo([FromBody] ClientesArticuloDto clientesArticuloDto)
        {
            if (clientesArticuloDto is null) return BadRequest(clientesArticuloDto);
            var cliente = await _clienteRepository.GetClienteById(clientesArticuloDto.ClienteId);
            var articulo = await _articuloRepository.GetArticuloById(clientesArticuloDto.ArticuloId);
            if (cliente is null || articulo is null) return BadRequest(clientesArticuloDto);
            if(articulo?.Object?.Stock == 0) return BadRequest(clientesArticuloDto);
            var clienteArticulo = new ClientesArticulo
            {
                ClienteId = clientesArticuloDto.ClienteId,
                ArticuloId = clientesArticuloDto.ArticuloId,
                Fecha = DateTime.UtcNow
            };
            await _clientesArticuloRepository.InsertClientesArticulo(clienteArticulo);
            articulo!.Object!.Stock--;
            await _articuloRepository.UpdateArticulo(clienteArticulo.ArticuloId, articulo!.Object);
            return CreatedAtRoute(nameof(GetClientesArticuloById), new { id = clienteArticulo.Id }, _mapper.Map<ClientesArticuloDto>(clienteArticulo));
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClientesArticulo(int id)
        {
            if (id == 0) { return BadRequest(); }
            var existe = await _clientesArticuloRepository.GetClientesArticuloById(id);
            if (existe is null) { return NotFound(); }
            var articulo = await _articuloRepository.GetArticuloById(existe!.Object!.ArticuloId);
            articulo!.Object!.Stock++;
            var response = await _clientesArticuloRepository.DeleteClientesArticulo(id);
            await _articuloRepository.UpdateArticulo(existe!.Object!.ArticuloId, articulo!.Object);
            if (!response.Exito) return BadRequest(response);
            else return NoContent();
        }
    }
}