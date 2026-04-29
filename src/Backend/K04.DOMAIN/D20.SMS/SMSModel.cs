using System;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D09.Cliente;

namespace Backend.K04.DOMAIN.D20.SMS;

[Table("tb20_sms")]
public class SMSModel
{
  [Column("id")]
  public int Id { get; set;}

  [Column("nif_funcionario")]
  public string Nif_funcionario = string.Empty;

  [Column("id_cliente")]
  public string Id_cliente = string.Empty;

  [Column("data_envio")]
  public DateTime Data_envio {get; set;}

  [Column("mensagem")]
  public string Mensagem = string.Empty;

  [Column("estado")]
  public bool Estado {get;set;}

  public FuncionarioModel Funcionario {get;set;} = null!;
  public ClienteModel Cliente {get;set;} = null!;
}
