using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.PacienteUseCase.DTO;

public class AtualizarPacienteDTO
{
    [Required(ErrorMessage = "ID do paciente é obrigatório")]
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "Nome do paciente é obrigatório")]
    public string PacienteNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "Género é obrigatório")]
    public int IdGenero { get; set; }

    [Required(ErrorMessage = "Tipo de cliente é obrigatório")]
    public int IdClientePaciente { get; set; }
}
