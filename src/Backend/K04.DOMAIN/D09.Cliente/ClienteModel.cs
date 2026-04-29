using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D20.SMS;

namespace Backend.K04.DOMAIN.D09.Cliente;

[Table("tb09_cliente")]
public class ClienteModel
{
    [Key]
    [Column("nif")]
    public string Nif_cliente { get; set; } = string.Empty;

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    public ICollection<ContactoModel> Contactos { get; set; } = [];

    public ICollection<PacienteModel> Pacientes { get; set; } = []; 

    public ICollection<PagamentoModel> Pagamentos { get; set; } = [];

    public ICollection<SMSModel> Mensagens { get; set; } = [];

}
