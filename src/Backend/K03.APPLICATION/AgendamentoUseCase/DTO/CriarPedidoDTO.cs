using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.DTO;

public class CriarPedidoDTO
{
    [Required(ErrorMessage ="Nome do cliente é obrigatório")]
    public string NomeCliente { get; set; } = string.Empty;

    [Required(ErrorMessage ="Número do cliente é obrigatório")]
    public string Telefone { get; set; } = string.Empty;
    [Required(ErrorMessage ="Especialidade é obrigatório")]
    public int IdEspecialidade { get; set; }
    [Required(ErrorMessage ="Horário é obrigatório")]
    public DateTime HorarioPreferencial { get; set; }
    public string? Observacoes { get; set; }
}