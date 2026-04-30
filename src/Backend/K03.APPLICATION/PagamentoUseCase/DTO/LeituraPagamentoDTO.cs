using System;

namespace Backend.K03.APPLICATION.PagamentoUseCase.PagamentoDTO;

public class LeituraPagamentoDTO
{
    public int Id { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Secretaria { get; set; } = string.Empty;

    public string Comprovativo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; }
}
