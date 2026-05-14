using System;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class CancelarPedido(IAgendamentoRepository repository, ISmsService sms)
{
    private readonly IAgendamentoRepository _repository = repository;
    private readonly ISmsService _sms = sms;

    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await _repository.BuscarPorIdAsync(id);
        if (pedido is null) return "Pedido não encontrado";

        pedido.Estado           = "Cancelado";
        pedido.HorarioReservado = false; // liberta o horário
        var atualizado = await _repository.AtualizarAsync(pedido);
        if (!atualizado) return "erro";

        var mensagem =
            $"Estimado(a) {pedido.NomeCliente},\n\n" +
            $"Informamos que o seu pedido de agendamento foi cancelado.\n\n" +
            $"Detalhes do pedido cancelado:\n" +
            $"  • Número do Pedido: {pedido.NumeroPedido}\n\n" +
            $"Se o cancelamento foi inesperado ou deseja efectuar um novo " +
            $"agendamento, convidamo-lo(a) a aceder ao nosso portal em " +
            $"www.kigramed.com ou a contactar-nos directamente.\n\n" +
            $"Pedimos desculpa por qualquer inconveniente causado.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

