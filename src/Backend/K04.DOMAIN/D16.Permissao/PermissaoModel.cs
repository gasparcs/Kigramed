using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D17.PerfilPermissao;

namespace Backend.K04.DOMAIN.D16.Permissao;

[Table("tb16_permissoes")]
public class PermissaoModel
{
    [Key]
    [Column("uuid_permissoes")]
    public Guid UUID { get; set; }

    [Column("descricao")]
    public string Descricao{ get; set; } = string.Empty;

    public ICollection<PerfilPermissaoModel> PerfisPermissoes { get; set; } = [];

}
