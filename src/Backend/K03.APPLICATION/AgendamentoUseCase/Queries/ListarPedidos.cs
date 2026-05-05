using System;
using Backend.K03.APPLICATION.AgendamentoUseCase.DTO;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Queries;

public class ListarPedidos(IAgendamentoRepository repository)
{
    public async Task<IEnumerable<LeituraPedidoDTO>> ExecuteAsync()
    {
        var pedidos = await repository.ListarPedidosAsync();

        return pedidos.Select(p => new LeituraPedidoDTO
        {
            Id                  = p.Id,
            NumeroPedido        = p.NumeroPedido,
            NomeCliente         = p.NomeCliente,
            Telefone            = p.Telefone,
            Especialidade       = p.Especialidade?.Nome ?? string.Empty,
            HorarioPreferencial = p.HorarioPreferencial,
            Observacoes         = p.Observacoes,
            Estado              = p.Estado,
            HorarioReservado    = p.HorarioReservado,
            PrazoPagamento      = p.PrazoPagamento,
            CaminhoComprovativo = p.CaminhoComprovativo,
            IdConsulta          = p.IdConsulta,
            CriadoEm           = p.CriadoEm
        });
    }
}
