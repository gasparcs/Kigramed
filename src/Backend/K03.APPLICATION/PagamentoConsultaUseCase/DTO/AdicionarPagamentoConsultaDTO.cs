using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.PagamentoConsultaUseCase.DTO;

public class AdicionarPagamentoConsultaDTO
{
    [Required(ErrorMessage = "Informe o pagamento")]
    public int IdPagamento { get; set; }

    [Required(ErrorMessage = "Informe a consulta")]
    public int IdConsulta { get; set; }
}
