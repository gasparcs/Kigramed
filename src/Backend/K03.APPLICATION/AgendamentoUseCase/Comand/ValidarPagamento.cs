using System;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AgendamentoUseCase.Comand;

public class ValidarPagamento(IAgendamentoRepository repository, ISmsService sms)
{
    private readonly IAgendamentoRepository _repository = repository;
    private readonly ISmsService _sms = sms;

    public async Task<string> ExecuteAsync(int id)
    {
        var pedido = await _repository.BuscarPorIdAsync(id);
        if (pedido is null) return "nao_encontrado";

        if (!string.Equals(pedido.Estado, "Comprovativo Enviado", StringComparison.OrdinalIgnoreCase) && !string.Equals(pedido.Estado, "Pagamento Enviado", StringComparison.OrdinalIgnoreCase))
            return $"estado_invalido_validar:{pedido.Estado}";

        // Verifica se o prazo ainda é válido
        if (pedido.PrazoPagamento.HasValue && pedido.PrazoPagamento < DateTime.UtcNow)
        {
            pedido.Estado           = "Cancelado";
            pedido.HorarioReservado = false;
            await _repository.AtualizarAsync(pedido);
            return "O prazo de pagamento expirou. Pedido cancelado.";
        }

        var criado = await _repository.CriarConsultaPagamentoAsync(pedido);
        if (!criado)
            return "Não foi possível criar a consulta ou vincular o pagamento.";

        pedido.Estado = "Validado";
        var atualizado = await _repository.AtualizarAsync(pedido);
        if (!atualizado) return "erro";

        var mensagem =
            $"Estimado(a) {pedido.NomeCliente},\n\n" +
            $"Temos o prazer de informar que o seu pagamento foi validado " +
            $"com sucesso e a sua consulta está oficialmente confirmada.\n\n" +
            $"Detalhes da consulta:\n" +
            $"  • Número do Pedido: {pedido.NumeroPedido}\n" +
            $"  • Data e Hora: {pedido.HorarioPreferencial.ToLocalTime().ToString("dd/MM/yyyy 'às' HH:mm")}\n\n" +
            $"Recomendamos que se apresente com 10 minutos de antecedência " +
            $"munido do seu documento de identificação.\n\n" +
            $"Contamos com a sua presença.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "101010101010");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}
