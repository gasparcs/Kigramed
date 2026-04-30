using System;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class LeituraConsultaDTO
{
    public int ConsultaId { get; set; }

    public DateTime Data_consulta { get; set; }

    public string EstadoDescricao { get; set; } = string.Empty;

    public string ServicoNome { get; set; } = string.Empty;

    public string PacienteNome { get; set; } = string.Empty;

    public string ClienteNome { get; set; } = string.Empty;

    public string MedicoNome { get; set; } = string.Empty;
    
    public string Especialidade { get; set; } = string.Empty;
}
