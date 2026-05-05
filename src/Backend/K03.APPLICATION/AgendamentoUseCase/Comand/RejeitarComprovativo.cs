using System;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class RejeitarComprovativo(IAgendamentoRepository repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        // Volta ao estado anterior para o cliente reenviar
        pedido.Estado                = "Aguarda Pagamento";
        pedido.CaminhoComprovativo   = null;
        await repository.AtualizarAsync(pedido);

        return "sucesso";
    }
}
