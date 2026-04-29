using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.D19.MedicoConsulta;

namespace Backend.K04.DOMAIN.D07.MedicoEspecialidade;

[Table("tb07_medico_especialidade")]
public class MedicoEspecilidadeModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("nif_funcionario")]
    public string Nif_funcionario{get;set;}=string.Empty;

    [Column("id_especialidade")]
    public int Id_especialidade{get;set;}

    public FuncionarioModel Funcionario{get;set;}=null!;

    public EspecialidadeModel Especialidade{get;set;}=null!;

    public ICollection<MedicoConsultaModel> MedicoConsultas{get;set;}=null!;

    public ICollection<ConsultaModel> Consultas {get;set;}=null!;

}
