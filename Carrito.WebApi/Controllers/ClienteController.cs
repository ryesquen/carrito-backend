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
    [Authorize(Roles = "Admin")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        public ClienteController(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<ClienteDto>>> GetAllClientes()
        {
            var response = await _clienteRepository.GetAllClientes();
            if (response.Object is not null)
            {
                var res = _mapper.Map<List<ClienteDto>>(response.Object);
                return Ok(res);
            }
            else return StatusCode(response.Status, response.Error);
        }
        [HttpGet("{id:int}", Name = "GetClienteById")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var response = await _clienteRepository.GetClienteById(id);
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
                return Ok(_mapper.Map<ClienteDto>(response.Object));
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertCliente([FromBody] ClienteDto clienteDto)
        {
            if (clienteDto is null || clienteDto.Id > 0) return BadRequest(clienteDto);
            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Apellido = clienteDto.Apellido,
                Direccion = clienteDto.Direccion
            };
            await _clienteRepository.InsertCliente(cliente);
            return CreatedAtRoute(nameof(GetClienteById), new { id = cliente.Id }, _mapper.Map<ClienteDto>(cliente));
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteDto clienteDto)
        {
            if (clienteDto is null || id != clienteDto.Id) return BadRequest(clienteDto);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await _clienteRepository.GetClienteById(id);
            var cliente = result.Object;
            cliente!.Nombre = clienteDto.Nombre;
            cliente.Apellido = clienteDto.Apellido;
            cliente.Direccion = clienteDto.Direccion;
            var response = await _clienteRepository.UpdateCliente(id, cliente!);
            if (response.Exito) return Ok(response);
            else return StatusCode(response.Status, response.Error);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            if (id == 0) { return BadRequest(); }
            var existe = await _clienteRepository.GetClienteById(id);
            if (existe is null) { return NotFound(); }
            var response = await _clienteRepository.DeleteCliente(id);
            if(!response.Exito) return BadRequest(response);
            else return NoContent();
        }
    }
}