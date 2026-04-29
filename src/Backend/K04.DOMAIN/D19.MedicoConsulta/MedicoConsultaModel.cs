using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D19.MedicoConsulta;

[Table("tb19_medico_consulta")]
public class MedicoConsultaModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("id_medico_especialidade")]
    public int Id_medico_especialidade { get; set; }

    [Column("id_consulta")]
    public int Id_consulta { get; set; }

    public MedicoEspecilidadeModel MedicoEspecialidade { get; set; } = null!;

    public ConsultaModel Consulta { get; set; } = null!;
}