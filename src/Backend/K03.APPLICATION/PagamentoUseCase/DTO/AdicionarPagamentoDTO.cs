using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.PagamentoUseCase.DTO;

public class AdicionarPagamentoDTO
{
    [Required(ErrorMessage = "Informe o cliente")]
    public string IdCliente { get; set; }  = string.Empty;

    [Required(ErrorMessage = "Informe a secretaria")]
    public string IdSecretaria { get; set; }  = string.Empty;

    [Required(ErrorMessage = "Informe o comprovativo")]
    public string Comprovativo { get; set; } = string.Empty;

    public DateTime Data { get; set; } 
}
