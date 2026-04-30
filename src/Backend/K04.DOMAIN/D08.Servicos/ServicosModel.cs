using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D08.Servicos;

[Table("tb08_servico")]
public class ServicosModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("id_especialidade")]
    public int Id_especialidade{get;set;}

    [Column("nome")]
    public string Nome{get;set;}=string.Empty;

    [Column("duracao_minuto")]
    public int Duracao_minuto{get;set;}

    [Column("preco")]
    public decimal Preco{get;set;}

    [Column("estado")]
    public bool Estado{get;set;}

    public EspecialidadeModel Especialidade{get;set;}=null!;

    public ICollection<ConsultaModel> Consultas{get;set;}=null!;

}
