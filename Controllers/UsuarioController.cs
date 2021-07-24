using Innovativo.DTO;
using Innovativo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost("autenticar")]
        public IActionResult Autenticar([FromBody] UsuarioLoginDTO usvm)
        {
            UsuarioLogadoDTO ulDTO;

            if (!_usuarioService.Autenticar(usvm.Usuario, usvm.Senha, out ulDTO))
                return Unauthorized();

            return Ok(ulDTO);
        }

        [HttpPost()]
        public ActionResult<UsuarioDTO> Inserir(UsuarioDTO dto)
        {
            dto.ID = _usuarioService.Inserir(dto);
            return CreatedAtAction(nameof(ObterPorID), new { id = dto.ID }, dto);
        }

        [HttpGet]
        public ActionResult<List<UsuarioDTO>> GetAll()
        {
            return _usuarioService.Listar();
        }

        [HttpGet("{id}")]
        public ActionResult<UsuarioDTO> ObterPorID(int id)
        {
            var dto = _usuarioService.ObterPorIdDTO(id);

            return dto is null ? NotFound() : (ActionResult<UsuarioDTO>)dto;
        }

        [HttpPut("{id}")]
        public IActionResult Alterar(int id, UsuarioDTO dto)
        {
            return _usuarioService.Alterar(id, dto) ? (IActionResult)NoContent() : NotFound();
        }
    }
}