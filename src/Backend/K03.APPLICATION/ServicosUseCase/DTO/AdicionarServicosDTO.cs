using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ServicosUseCase.DTO;

public class AdicionarServicoDTO
{
    [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
    public String ServicoNome{get;set;}=string.Empty;

    [Required(ErrorMessage = "A duração do serviço é obrigatória.")]
    public int ServicoDuracaoMinuto{get;set;}

    [Required(ErrorMessage = "O preço do serviço é obrigatório.")]
    public decimal ServicoPreco{get;set;}

    [Required(ErrorMessage = "O estado do serviço é obrigatório.")]
    public bool ServicoEstado{get;set;}

    [Required(ErrorMessage = "A especialidade do serviço é obrigatória.")]
    public int IdEspecialidade{get;set;}
}
