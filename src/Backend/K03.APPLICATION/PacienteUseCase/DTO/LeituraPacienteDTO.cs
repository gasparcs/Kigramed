using System;

namespace Backend.K03.APPLICATION.PacienteUseCase.DTO;

public class LeituraPacienteDTO
{
   public int PacienteId{get;set;}

   public string PacienteNome{get;set;}=string.Empty;

   public DateTime PacienteData_nascimento{get;set;}

   public string Cliente {get;set;}=null!;

   public string Cliente_Paciente{get;set;}=null!;

   public string  Genero{get;set;}=null!;

   public IEnumerable<ConsultaDTO> Consultas{get;set;}=null!;

}
public class ConsultaDTO
{
    public int IdConsuta {get;set;}

    public DateTime Data_Consulta {get;set;}

    public string Estado {get;set;} = null!;
}
