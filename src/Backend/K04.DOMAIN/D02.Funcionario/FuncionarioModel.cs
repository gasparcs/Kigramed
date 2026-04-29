using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D05.Auth;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D20.SMS;

namespace Backend.K04.DOMAIN.D02.Funcionario;

[Table("tb02_funcionario")]
public class FuncionarioModel
{
    [Key]
    [Column("nif")]
    public String Nif { get; set; } = string.Empty;

    [Column("id_perfil")]
    public int Id_Perfil { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("estado")]
    public bool Estado { get; set; }

    [ForeignKey("Id_Perfil")]
    public PerfilModel Perfil { get; set; } = null!;

    public ICollection<ContactoModel> Contactos { get; set; } = [];

    public AuthModel Auth { get; set; } = null!;

    public ICollection<MedicoEspecilidadeModel> MedicoEspecialidades { get; set; } = [];

    public ICollection<PagamentoModel> Pagamentos { get; set; } = [];

    public ICollection<SMSModel> Mensagens { get; set; } = [];
}
