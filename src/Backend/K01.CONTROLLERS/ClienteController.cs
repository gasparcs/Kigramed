using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController(CriarConsulta criarConsulta) : ControllerBase
    { 
        // ─── PÚBLICO ────────────────────────────────────────────────────────────

        /// Cliente cria pedido de agendamento
        [AllowAnonymous]
        [HttpPost("agendamento")]
        public async Task<IActionResult> CriarPedidoConsulta([FromBody] CriarConsultaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resposta = await criarConsulta.ExecuteAsync(
                dto.IdPaciente, 
                dto.IdEspecialidade, 
                dto.IdServico, 
                dto.HorarioPreferencial, 
                dto.Observacoes);

            return resposta.mensagem.Contains("sucesso")
                ? StatusCode(201, new { numeroPedido = resposta.numeroPedido, mensagem = resposta.mensagem })
                : StatusCode(400, new { mensagem = resposta.mensagem });
        }
    }
}
