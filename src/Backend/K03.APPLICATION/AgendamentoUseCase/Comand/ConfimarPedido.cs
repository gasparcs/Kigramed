using System;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class ConfirmarPedido(IAgendamentoRepository repository, ISmsService sms)
{
    private readonly IAgendamentoRepository _repository = repository;
    private readonly ISmsService _sms = sms;

    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await _repository.BuscarPorIdAsync(id);
        if (pedido is null) return "nao_encontrado";

        if (!string.Equals(pedido.Estado, "Pendente", StringComparison.OrdinalIgnoreCase))
            return $"estado_invalido_confirmar:{pedido.Estado}";

        // Tenta reservar o horário com transação atómica
        var reservado = await _repository.ReservarHorarioAsync(id);
        if (!reservado) return "conflito";

        // Define prazo de 30 minutos para pagamento
        pedido.Estado         = "Aguarda Pagamento";
        pedido.PrazoPagamento = DateTime.UtcNow.AddMinutes(30);
        var atualizado        = await _repository.AtualizarAsync(pedido);
        if (!atualizado) return "erro";

        var deadline = pedido.PrazoPagamento?.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? DateTime.UtcNow.AddMinutes(30).ToString("dd/MM/yyyy HH:mm");
        var mensagem =
            $"Estimado(a) {pedido.NomeCliente},\n\n" +
            $"O seu pedido de agendamento foi recebido e aceite com sucesso.\n\n" +
            $"Detalhes do pedido:\n" +
            $"  • Número do Pedido: {pedido.NumeroPedido}\n" +
            $"  • Prazo para pagamento: {deadline}\n\n" +
            $"Para confirmar a sua consulta, efectue o pagamento dentro do prazo " +
            $"indicado e envie o comprovativo através do portal.\n\n" +
            $"Dados bancários para transferência:\n" +
            $"  • Banco: [NOME DO BANCO]\n" +
            $"  • IBAN: [IBAN DA CLÍNICA]\n" +
            $"  • Referência: {pedido.NumeroPedido}\n\n" +
            $"Atenção: o pedido será cancelado automaticamente caso o pagamento " +
            $"não seja efectuado dentro do prazo.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

