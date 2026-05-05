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
        if (pedido is null) return "Pedido não encontrado";

        if (pedido.Estado != "Comprovativo Enviado")
            return "O pedido não tem comprovativo para validar.";

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

        pedido.Estado = "Confirmado";
        var atualizado = await _repository.AtualizarAsync(pedido);
        if (!atualizado) return "erro";

        var mensagem = $"Pagamento validado. Consulta confirmada para {pedido.HorarioPreferencial.ToLocalTime().ToString("dd/MM/yyyy HH:mm")}.";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "101010101010");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}