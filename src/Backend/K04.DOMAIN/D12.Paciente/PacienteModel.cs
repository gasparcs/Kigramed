using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D09.Genero;
using Backend.K04.DOMAIN.D11.ClientePaciente;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D12.Paciente;

[Table("tb12_paciente")]
public class PacienteModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("nif_cliente")]
    public string Nif_cliente {get;set;} = string.Empty;

    [Column("nome")]
    public String Nome {get;set;}=string.Empty;

    [Column("data_nascimento")]
    public DateTime Data_nascimento {get;set;}

    [Column("id_genero")]
    public int Id_genero {get;set;} 

    [Column("id_cliente_paciente")]
    public int Id_cliente_paciente {get;set;}

    public ClienteModel Cliente { get; set; } = null!;

    public GeneroModel Genero { get; set; } = null!;

    public ClientePacienteModel ClientePaciente { get; set; } = null!;

    public ICollection<ConsultaModel> Consultas { get; set; } = null!;

}
