using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.D19.MedicoConsulta;
using Backend.K04.DOMAIN.D21.Agendamento;

namespace Backend.K04.DOMAIN.D15.Consulta;

[Table("tb15_consulta")]
public class ConsultaModel
{
    [Key]
    [Column("id")]
    public int Id{ get; set; }
    
    [Column("id_medico_especialista")]
    public int Id_medico_especialiade{ get; set; }

    [Column("id_servico")]
    public int Id_servico{ get; set; }

    [Column("id_paciente")]
    public int Id_paciente{ get; set; }

    [Column("id_estado")]
    public int Id_estado_consulta{ get; set; }

    [Column("data_consulta")]
    public DateTime Data_consulta{ get; set; }

   public MedicoConsultaModel MedicoConsulta { get; set; }=null!; 

    public ServicosModel Servico{ get; set; }=null!;

    public PacienteModel Paciente{ get; set; }=null!;

    public EstadoConsultaModel EstadoConsulta{ get; set; }=null!;

    public PagamentoConsultaModel PagamentoConsulta{ get; set; }=null!;

    public MedicoEspecilidadeModel MedicoEspecialidade{get;set;}=null!;

    public AgendamentoModel Agendamento {get;set;}=null!;

}
