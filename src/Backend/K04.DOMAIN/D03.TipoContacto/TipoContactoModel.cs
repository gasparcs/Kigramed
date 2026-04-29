using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D04.Contacto;

namespace Backend.K04.DOMAIN.D03.TipoContacto;

[Table("tb03_tipo_contacto")]

public class TipoContactoModel
{
    [Key]
    [Column("id")]
    public int Id{ get; set; }

    [Column("descricao")]
    public String Descricao { get; set; }= string.Empty;

    public ICollection<ContactoModel> Contactos { get; set; } = [];

}
