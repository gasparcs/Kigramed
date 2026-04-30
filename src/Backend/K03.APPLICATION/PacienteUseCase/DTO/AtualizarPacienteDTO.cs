using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.PacienteUseCase.DTO;

public class AtualizarPacienteDTO
{
    public int IdPaciente {get;set;}

    [Required(ErrorMessage ="Nome do paciente é obrigatório")]
   public string PacienteNome{get;set;}=string.Empty;
   
}
