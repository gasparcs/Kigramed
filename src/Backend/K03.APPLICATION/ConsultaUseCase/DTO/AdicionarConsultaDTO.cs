using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class AdicionarConsultaDTO
{
   
    [Required(ErrorMessage = "Informe o médico especialista para a consulta")]
    public int Id_medico_especialiade { get; set; }

    [Required(ErrorMessage ="Informe o serviço para a consulta")]
    public int Id_servico { get; set; }

    [Required(ErrorMessage ="Informe o paciente")]
    public int Id_paciente { get; set; }

    [Required(ErrorMessage = "Informe o estado da consulta")]
    public int Id_estado_consulta { get; set; }

    [Required(ErrorMessage ="Informe a data para a consulta")]
    public DateTime Data_consulta { get; set; }
}
