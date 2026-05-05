using System;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.DTO;

public class LeituraPedidoDTO
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public DateTime HorarioPreferencial { get; set; }
    public string? Observacoes { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool HorarioReservado { get; set; }
    public DateTime? PrazoPagamento { get; set; }
    public string? CaminhoComprovativo { get; set; }
    public int? IdConsulta { get; set; }
    public DateTime CriadoEm { get; set; }
}