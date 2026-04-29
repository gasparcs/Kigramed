using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D18.PagamentoConsulta;

[Table("tb18_pagamento_consulta")]
public class PagamentoConsultaModel
{
    [Key]
    [Column("id")]
    public int Id{get;set;}

    [Column("id_pagamento")]
    public int Id_Pagamento{get;set;}

    [Column("id_consulta")]
    public int Id_Consulta{get;set;}

    public PagamentoModel Pagamento{get;set;}=null!;

    public ConsultaModel Consulta{get;set;}=null!;


}

