using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class CriarConsultaDTO
{
    [Required]
    public int IdPaciente { get; set; }

    [Required]
    public int IdEspecialidade { get; set; }

    [Required]
    public int IdServico { get; set; }

    [Required]
    public DateTime HorarioPreferencial { get; set; }

    public string? Observacoes { get; set; }
}
