using System;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class ValidarPagamento(IAgendamentoRepository repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        if (pedido.Estado != "Comprovativo Enviado")
            return "O pedido não tem comprovativo para validar.";

        // Verifica se o prazo ainda é válido
        if (pedido.PrazoPagamento.HasValue && pedido.PrazoPagamento < DateTime.UtcNow)
        {
            pedido.Estado           = "Cancelado";
            pedido.HorarioReservado = false;
            await repository.AtualizarAsync(pedido);
            return "O prazo de pagamento expirou. Pedido cancelado.";
        }

        pedido.Estado = "Confirmado";
        await repository.AtualizarAsync(pedido);
        return "sucesso";
    }
}