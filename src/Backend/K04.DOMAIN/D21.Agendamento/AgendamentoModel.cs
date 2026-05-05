using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;
using Backend.K03.APPLICATION.EstadoConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.D15.Consulta;

namespace Backend.K04.DOMAIN.D21.Agendamento;

[Table("tb21_pedido_agendamento")]
public class AgendamentoModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("numero_pedido")]
    public string NumeroPedido { get; set; } = string.Empty;

    [Column("nome_cliente")]
    public string NomeCliente { get; set; } = string.Empty;

    [Column("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("id_especialidade")]
    public int IdEspecialidade { get; set; }

    [ForeignKey("IdEspecialidade")]
    public EspecialidadeModel? Especialidade { get; set; }

    [Column("horario_preferencial")]
    public DateTime HorarioPreferencial { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    // Estados: Pendente | Aguarda Pagamento | Comprovativo Enviado | Confirmado | Cancelado
    [Column("estado")]
    public string Estado { get; set; } = "Pendente";

    [Column("horario_reservado")]
    public bool HorarioReservado { get; set; } = false;

    // Prazo de 2 horas para pagamento após confirmação do horário
    [Column("prazo_pagamento")]
    public DateTime? PrazoPagamento { get; set; }

    [Column("caminho_comprovativo")]
    public string? CaminhoComprovativo { get; set; }

    // Preenchido após validação do pagamento
    [Column("id_consulta")]
    public int? IdConsulta { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [Column("id_servico")]
    public int Id_Servico  { get; set; }

    public ServicosModel? Servico { get; set; } = null!;

    public ConsultaModel? Consulta { get; set; } = null!;


}

