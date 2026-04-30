using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;

public class AdicionarEspecialidadeDTO
{
    [Required(ErrorMessage = "Informe o nome da especialidade.")]
    public string EspecialidadeNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a descrição da especialidade.")]
    public string EspecialidadeDescricao { get; set; } = string.Empty;

    public bool EspecialidadeEstado { get; set; }
}
