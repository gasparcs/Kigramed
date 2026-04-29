using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;

namespace Backend.K04.DOMAIN.D05.Auth;

[Table("tb05_auth")]
public class AuthModel
{
    [Key]
    [Column("nif_funcionario")]
    public string Nif_funcionario { get; set; } = string.Empty;

    [Column("senha_hash")]
    public string Senha_hash { get; set; } = string.Empty;

    [Column("senha_salt")]
    public string Senha_Salt { get; set; } = string.Empty;

    public FuncionarioModel Funcionario { get; set; } = null!;
    

}