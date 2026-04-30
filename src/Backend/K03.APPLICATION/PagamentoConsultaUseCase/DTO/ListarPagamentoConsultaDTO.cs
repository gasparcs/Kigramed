using System;

namespace Backend.K03.APPLICATION.PagamentoConsultaUseCase.DTO;

public class ListarPagamentoConsultaDTO
{
    public int Id { get; set; }

    public int IdPagamento { get; set; }

    public int IdConsulta { get; set; }

    public DateTime DataConsulta { get; set; }

    public string Comprovativo { get; set; } = string.Empty;

}
