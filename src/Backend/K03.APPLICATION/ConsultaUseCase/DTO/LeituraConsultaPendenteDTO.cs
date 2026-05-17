using System;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class LeituraConsultaPendenteDTO
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Servico { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? PrazoPagamento { get; set; }
    public string? CaminhoComprovativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
