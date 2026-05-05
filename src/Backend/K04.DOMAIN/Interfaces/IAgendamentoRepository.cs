using System;
using Backend.K04.DOMAIN.D21.Agendamento;

namespace Backend.K04.DOMAIN.Interfaces;

public class IAgendamentoRepository
{
    Task<AgendamentoModel> CriarPedidoAsync (AgendamentoModel pedido);
    Task<IEnumerable<AgendamentoModel>> ListarPedidosAsync();
    Task<AgendamentoModel?> BuscarPorIdAsync(int id);
    Task<AgendamentoModel?> BuscarPorNumeroPedidoAsync(string numeroPedido);
    Task<bool> AtualizarAsync(AgendamentoModel pedido);
    Task<bool> ExisteConflitoAsync(int idEspecialidade, DateTime horario, int? excluirId = null);
    Task<bool> ReservarHorarioAsync(int id);
}
