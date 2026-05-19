using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class CriarConsultaDTO
{
    [Required] public string NifCliente { get; set; } = string.Empty;
    [Required] public string NomePaciente { get; set; } = string.Empty;
    [Required] public DateTime DataNascimentoPaciente { get; set; }
    [Required] public int IdGeneroPaciente { get; set; }
    [Required] public int IdClientePaciente { get; set; }
    [Required] public int IdEspecialidade { get; set; }
    [Required] public int IdServico { get; set; }
    [Required] public DateTime HorarioPreferencial { get; set; }
    public string? Observacoes { get; set; }
}
