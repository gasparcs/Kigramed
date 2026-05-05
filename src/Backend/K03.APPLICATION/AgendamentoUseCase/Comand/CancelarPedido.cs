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

        var mensagem = "Pedido cancelado. O horário foi libertado.";
        var smsEnviado = await _sms.EnviarAsync(pedido.Telefone, mensagem, "101010101010");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

