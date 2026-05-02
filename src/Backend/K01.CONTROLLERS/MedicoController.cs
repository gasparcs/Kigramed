using System.IdentityModel.Tokens.Jwt;
using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K03.APPLICATION.ConsultaUseCase.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS;

[Authorize(Roles = "Medico")]
[Route("api/[controller]")]
[ApiController]
public class MedicoController(
    ListarConsultasDoMedico listarConsultasDoMedicoServices,
    AtualizarConsulta atualizarConsultaServices)
    : ControllerBase
{
    /// Lista apenas as consultas do médico autenticado.
    [HttpGet("consultas")]
    public async Task<IActionResult> ListarConsultas()
    {
        // Lê o NIF do claim "sub" do JWT (definido como JwtRegisteredClaimNames.Sub no login)
        string? nifMedico = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(nifMedico))
            return StatusCode(401, new { mensagem = "Não foi possível identificar o médico autenticado." });

        var resposta = await listarConsultasDoMedicoServices.ExecuteAsync(nifMedico);

        return Ok(new
        {
            mensagem = "Consultas listadas com sucesso",
            dados    = resposta
        });
    }

    /// Atualiza o estado de uma consulta.
    [HttpPut("consulta/{id}")]
    public async Task<IActionResult> AtualizarConsulta(int id, [FromBody] AtualizarConsultaDTO dto)
    {
        if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

        if (id != dto.IdConsulta)
            return StatusCode(400, new { mensagem = "ID da consulta não corresponde ao corpo da requisição." });

        var resposta = await atualizarConsultaServices.ExecuteAsync(dto);

        return resposta.Contains("sucesso")
            ? StatusCode(200, new { mensagem = "Consulta atualizada com sucesso.", detalhes = resposta })
            : StatusCode(400, new { mensagem = "Erro ao atualizar consulta.", detalhes = resposta });
    }

    /// Verifica se a API do médico está operacional.
    [HttpGet("status")]
    public IActionResult Status() =>
        Ok(new { mensagem = "API do Médico está operacional." });
}
