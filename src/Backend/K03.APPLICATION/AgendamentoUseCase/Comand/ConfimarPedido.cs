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
        if (pedido is null) return "Pedido não encontrado";

        if (pedido.Estado != "Pendente")
            return "Este pedido já foi processado.";

        // Tenta reservar o horário com transação atómica
        var reservado = await _repository.ReservarHorarioAsync(id);
        if (!reservado) return "conflito";

        // Define prazo de 2 horas para pagamento
        pedido.Estado         = "Aguarda Pagamento";
        pedido.PrazoPagamento = DateTime.UtcNow.AddHours(2);
        var atualizado        = await _repository.AtualizarAsync(pedido);
        if (!atualizado) return "erro";

        var deadline = pedido.PrazoPagamento?.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? DateTime.UtcNow.AddHours(2).ToString("dd/MM/yyyy HH:mm");
        var mensagem = $"Pedido confirmado. Por favor pague até {deadline}. Dados bancários: Banco XYZ, IBAN PT50000201234567890123456, NIF 123456789. Após o pagamento, envie o comprovativo.";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "101010101010");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}