using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.PacienteUseCase.DTO;

public class AdicionarPacienteDTO
{

   [Required(ErrorMessage = "Nome é do paciente é obrigatório")]
   public string PacienteNome{get;set;}=string.Empty;

   [Required(ErrorMessage = "Data de nascimento do paciente é obrigatório")]
   public DateTime PacienteData_nascimento{get;set;}

   public string IdCliente{get;set;} = string.Empty;

   public int  IdCliente_Paciente {get;set;}


   [Required(ErrorMessage ="O gênero é obrigatório")]
   public int IdGenero {get;set;}

}

