using Innovativo.DTO;
using Innovativo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Innovativo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public ActionResult<List<ClienteDTO>> Listar()
        {
            return _clienteService.Listar();
        }

        [HttpGet("{id}")]
        public ActionResult<ClienteDTO> ObterPorID(int id)
        {
            var clienteDTO = _clienteService.ObterPorID(id);

            return clienteDTO == null ? NotFound() : (ActionResult<ClienteDTO>)clienteDTO;
        }

        [HttpPut("{id}")]
        public IActionResult Alterar(int id, ClienteDTO dto)
        {
            return _clienteService.Alterar(id, dto) ? (IActionResult)NoContent() : NotFound();
        }

        [HttpPost]
        public ActionResult<ClienteDTO> Inserir(ClienteDTO cvm)
        {
            var clienteDTO = _clienteService.Inserir(cvm);

            return CreatedAtAction(nameof(ObterPorID), new { id = clienteDTO.ID }, clienteDTO);
        }
    }
}