using System;
using Backend.K04.DOMAIN.Interfaces;

using Backend.K03.APPLICATION.Servico.ISmsService;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class RejeitarComprovativo(IAgendamentoRepository repository, ISmsService sms)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        // Volta ao estado anterior para o cliente reenviar
        pedido.Estado                = "Rejeitado";
        pedido.CaminhoComprovativo   = null;
        await repository.AtualizarAsync(pedido);

        var mensagem =
            $"Estimado(a) {pedido.NomeCliente},\n\n" +
            $"Informamos que o comprovativo de pagamento submetido para o seu " +
            $"pedido não foi aceite pela nossa equipa.\n\n" +
            $"Detalhes do pedido:\n" +
            $"  • Número do Pedido: {pedido.NumeroPedido}\n\n" +
            $"Possíveis motivos para a rejeição:\n" +
            $"  • Documento ilegível ou de qualidade insuficiente\n" +
            $"  • Valor transferido incorrecto\n" +
            $"  • Referência de pagamento em falta ou errada\n\n" +
            $"Por favor submeta um novo comprovativo válido através do portal " +
            $"www.kigramed.com para que a sua consulta possa ser confirmada.\n\n" +
            $"Para qualquer esclarecimento, a nossa equipa está ao seu dispor.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";

        await sms.EnviarAsync(pedido.Telefone, mensagem, "101010101010");

        return "sucesso";
    }
}

