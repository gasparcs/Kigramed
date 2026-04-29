using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D12.Paciente;

namespace Backend.K04.DOMAIN.D09.Genero;

[Table("tb10_genero")]
public class GeneroModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("nome")]
    public string Nome{get;set;}=string.Empty;

    public ICollection<PacienteModel> Pacientes { get; set; } = [];

}
