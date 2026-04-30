using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K03.APPLICATION.ConsultaUseCase.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController (
        ListarConsultas listarConsultasServices,
        AtualizarConsulta atualizarConsultaServices
    ) 
    : ControllerBase
    {
         /// Lista todas as consultas do médico
         [HttpGet("consultas")]
        public async Task<IActionResult> ListarConsultas()
        {
            try
            {
                var resposta = await listarConsultasServices.ExecuteAsync();
                return Ok(new
                {
                    mensagem = "Consultas listadas com sucesso",
                    dados = resposta
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao listar consultas",
                    erro = ex.Message
                });
            }
        }

        /// Atualiza o status de uma consulta

        [HttpPut("consulta/{id}")]
        public async Task<IActionResult> AtualizarConsulta(int id, [FromBody] AtualizarConsultaDTO dto)
        {
            // Validar estado do modelo
            if (!ModelState.IsValid)
                return StatusCode(400, ModelState);

            // Validar se o ID da URL corresponde ao DTO
            if (id != dto.IdConsulta)
                return StatusCode(400, new
                {
                    mensagem = "ID da consulta não corresponde ao corpo da requisição"
                });

            try
            {
                var resposta = await atualizarConsultaServices.ExecuteAsync(dto);

                if (resposta.Contains("sucesso"))
                    return StatusCode(200, new
                    {
                        mensagem = "Consulta atualizada com sucesso",
                        detalhes = resposta
                    });
                else
                    return StatusCode(400, new
                    {
                        mensagem = "Erro ao atualizar consulta",
                        detalhes = resposta
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao processar atualização",
                    erro = ex.Message
                });
            }
        }

        /// Endpoint de teste para verificar se a API do médico está respondendo
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { mensagem = "API do Médico está operacional" });
        }
    }
}
