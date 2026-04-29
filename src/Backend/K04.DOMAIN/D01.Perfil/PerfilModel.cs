using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D17.PerfilPermissao;

namespace Backend.K04.DOMAIN.D01.Perfil;

[Table("tb01_perfil")]
public class PerfilModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    public ICollection<FuncionarioModel> Funcionarios { get; set; } =[];

    public ICollection<PerfilPermissaoModel> PerfisPermissoes { get; set; } = [];
}
