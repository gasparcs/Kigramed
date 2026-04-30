using System;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;

public class AtualizarEspecialidadeDTO
{
     public int EspecialidadeId { get; set; }

    public string EspecialidadeNome { get; set; } = string.Empty;

    public string EspecialidadeDescricao { get; set; } = string.Empty;

    public bool EspecialidadeEstado { get; set; }
}
