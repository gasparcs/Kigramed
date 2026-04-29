using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.D16.Permissao;

namespace Backend.K04.DOMAIN.D17.PerfilPermissao;

[Table("tb17_perfil_permissoes")]
public class PerfilPermissaoModel
{
[Key]
[Column("id")]
public int Id { get; set; }

[Column("uuid_permissoes")]
public Guid UUID_permissao { get; set; }

[Column("id_perfil")]
public int Id_perfil { get; set; }

public PermissaoModel Permissao { get; set; } = null!;

public PerfilModel Perfil { get; set; } = null!;
}

