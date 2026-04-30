using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D08.Servicos;

namespace Backend.K04.DOMAIN.D06.Especialidade;

[Table("tb06_especialidade")]
public class EspecialidadeModel
{
    [Key]
    [Column("id")]
     public int Id{get;set;}

     [Column("nome")]
     public string Nome{get;set;}=string.Empty;

    [Column("descricao")]
     public string Descricao{get;set;}=string.Empty;

     [Column("estado")]
     public bool Estado{get;set;}

     public ICollection<MedicoEspecilidadeModel> MedicoEspecialidades { get; set; } = [];

     public ICollection<ServicosModel> Servicos { get; set; } = [];
}
