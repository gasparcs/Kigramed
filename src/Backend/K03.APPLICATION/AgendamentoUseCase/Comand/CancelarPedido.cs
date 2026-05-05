using System;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class CancelarPedido(IAgendamentoRepository repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        pedido.Estado           = "Cancelado";
        pedido.HorarioReservado = false; // liberta o horário
        await repository.AtualizarAsync(pedido);

        return "sucesso";
    }
}

