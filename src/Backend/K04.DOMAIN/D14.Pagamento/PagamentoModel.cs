using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;

namespace Backend.K04.DOMAIN.D14.Pagamento;

 [Table("tb14_pagamento")]
public class PagamentoModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("id_cliente")]
    public string Id_cliente{get;set;} = string.Empty;

    [Column("id_secretaria")]
    public string Nif_funcionario{get;set;}=string.Empty;
    
    [Column("comprovativo")]
    public string Comprovativo{get;set;}=string.Empty;

    [Column("data_envio")]
    public DateTime Data_envio{get;set;}

    public ClienteModel Cliente { get; set; } = null!;

    public FuncionarioModel Funcionario { get; set; } = null!;

    public ICollection<PagamentoConsultaModel> PagamentoConsultas { get; set; } = [];
    
    }
