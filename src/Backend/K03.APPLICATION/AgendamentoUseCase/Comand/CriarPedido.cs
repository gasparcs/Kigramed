using System;
using Backend.K03.APPLICATION.AgendamentoUseCase.DTO;
using Backend.K04.DOMAIN.D21.Agendamento;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class CriarPedido(IAgendamentoRepository repository)
{
    public async Task<(string numeroPedido, string mensagem)> ExecuteAsync(CriarPedidoDTO dto)
    {
        // Verifica se já existe horário reservado para esta especialidade
        var conflito = await repository.ExisteConflitoAsync(
            dto.IdEspecialidade,
            dto.HorarioPreferencial);

        if (conflito)
            return (string.Empty, "Este horário já está reservado. Por favor escolha outro horário.");

        // Gera número de pedido único
        var numeroPedido = $"PED-{DateTime.Now:yyyy}-{new Random().Next(1000, 9999)}";

        var pedido = new AgendamentoModel
        {
            NumeroPedido        = numeroPedido,
            NomeCliente         = dto.NomeCliente,
            Telefone            = dto.Telefone,
            IdEspecialidade     = dto.IdEspecialidade,
            Id_Servico           = dto.IdServico,
            HorarioPreferencial = dto.HorarioPreferencial,
            Observacoes         = dto.Observacoes,
            Estado              = "Pendente",
            HorarioReservado    = false,
            CriadoEm           = DateTime.UtcNow
        };

        await repository.CriarPedidoAsync(pedido);
        return (numeroPedido, "sucesso");
    }
}
