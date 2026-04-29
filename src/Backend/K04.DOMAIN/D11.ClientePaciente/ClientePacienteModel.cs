using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D12.Paciente;

namespace Backend.K04.DOMAIN.D11.ClientePaciente;

[Table("tb11_cliente_paciente")]
public class ClientePacienteModel
{
    [Key]
    [Column("id")] 
    public int Id { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    public ICollection<PacienteModel> Pacientes { get; set; } = [];

}