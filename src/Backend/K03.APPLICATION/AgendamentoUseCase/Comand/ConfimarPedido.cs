using System;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class ConfirmarPedido(IAgendamentoRepository repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        if (pedido.Estado != "Pendente")
            return "Este pedido já foi processado.";

        // Tenta reservar o horário com transação atómica
        var reservado = await repository.ReservarHorarioAsync(id);
        if (!reservado) return "conflito";

        // Define prazo de 2 horas para pagamento
        pedido.Estado          = "Aguarda Pagamento";
        pedido.PrazoPagamento  = DateTime.UtcNow.AddHours(2);
        await repository.AtualizarAsync(pedido);

        return "sucesso";
    }
}