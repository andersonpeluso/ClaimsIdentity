using Innovativo.DTO;
using Innovativo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EficaciaCanaisController : ControllerBase
    {
        private readonly IEficaciaCanaisService _eficaciaCanaisService;

        public EficaciaCanaisController(IEficaciaCanaisService eficaciaCanaisService)
        {
            _eficaciaCanaisService = eficaciaCanaisService;
        }

        [HttpGet("{id}")]
        public ActionResult<EficaciaCanalDTO> ObterPorID(int id)
        {
            var eficaciaCanalDTO = _eficaciaCanaisService.ObterPorID(id);
            if (eficaciaCanalDTO is null)
                return NotFound();

            if (!_eficaciaCanaisService.PodeAcessarRelatorio(eficaciaCanalDTO, int.Parse(HttpContext.User.Identity.Name)))
                return Forbid();

            return eficaciaCanalDTO;
        }

        [HttpPost]
        public IActionResult Inserir(EficaciaCanalDTO dto)
        {

            var id = _eficaciaCanaisService.Inserir(dto, out string mensagem);

            if (mensagem != string.Empty)
                return Conflict(mensagem);

            return CreatedAtAction(nameof(ObterPorID), new { id }, dto);
        }

        [HttpGet]
        public ActionResult<List<EficaciaCanalRelatorioDTO>> Listar()
        {
            return _eficaciaCanaisService.Listar(int.Parse(HttpContext.User.Identity.Name));
        }

        [HttpGet(nameof(ObterUltimo))]
        public ActionResult<EficaciaCanalDTO> ObterUltimo()
        {
            return _eficaciaCanaisService.ObterUltimoDoCliente(int.Parse(HttpContext.User.Identity.Name));
        }
    }
}