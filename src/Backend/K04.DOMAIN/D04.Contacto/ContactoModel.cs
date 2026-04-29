using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D03.TipoContacto;
using Backend.K04.DOMAIN.D09.Cliente;

namespace Backend.K04.DOMAIN.D04.Contacto;

[Table("tb04_contacto")]
public class ContactoModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nif_funcionario")]
    public string? Nif_funcionario { get; set; } = null;

    [Column("id_tipo_contacto")]
    public int Id_tipo_contacto { get; set; } 

    [Column("id_cliente")]
    public string? Nif_cliente { get; set; } = null;

    [Column("contacto")]
    public string Contacto { get; set; } = string.Empty;

    public FuncionarioModel Funcionario { get; set; } = null!;

    public TipoContactoModel TipoContacto { get; set; } = null!;

    public ClienteModel Cliente { get; set; } = null!;

}
