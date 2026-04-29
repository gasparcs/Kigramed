using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D13.EstadoConsulta;

[Table("tb13_estado_consulta")]
public class EstadoConsultaModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    public ICollection<ConsultaModel> Consultas { get; set; } = null!;
}
