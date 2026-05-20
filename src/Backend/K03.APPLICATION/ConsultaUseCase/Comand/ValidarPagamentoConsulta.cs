using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class ValidarPagamentoConsulta(KigramedDbContext context, ISmsService smsService)
{
    public async Task<string> ExecuteAsync(int id, string? nifSecretaria = null)
    {
        var consulta = await context.Tabelatb15_consulta
            .Include(c => c.Paciente)
                .ThenInclude(p => p.Cliente)
                .ThenInclude(cl => cl.Contactos)
            .Include(c => c.EstadoConsulta)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        if (consulta.EstadoConsulta.Descricao != "Comprovativo Enviado" &&
            consulta.EstadoConsulta.Descricao != "Pagamento Enviado")
            return $"estado_invalido_validar:{consulta.EstadoConsulta.Descricao}";

        if (consulta.PrazoPagamento.HasValue && consulta.PrazoPagamento.Value < DateTime.UtcNow)
        {
            var estadoCancelada = await context.Tabelatb13_estado_consulta
                .FirstOrDefaultAsync(e => e.Descricao == "Cancelada");

            if (estadoCancelada != null)
            {
                consulta.Id_estado_consulta = estadoCancelada.Id;
                await context.SaveChangesAsync();
            }
            return "prazo_expirado";
        }

        // Garante o registo financeiro ao validar comprovativo.
        var pagamentoConsulta = await context.Tabelatb18_pagamento_consulta
            .FirstOrDefaultAsync(pc => pc.Id_Consulta == consulta.Id);

        if (pagamentoConsulta == null)
        {
            var nifResponsavel = (nifSecretaria ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nifResponsavel))
            {
                nifResponsavel = await context.Tabelatb02_funcionario
                    .Where(f => f.Estado)
                    .Select(f => f.Nif)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(nifResponsavel))
                return "Erro: Nenhum funcionário disponível para registar o pagamento.";

            var nifCliente = consulta.Paciente?.Nif_cliente ?? consulta.Paciente?.Cliente?.Nif_cliente ?? string.Empty;
            if (string.IsNullOrWhiteSpace(nifCliente))
                return "Erro: Cliente associado ao pedido não encontrado.";

            var comprovativoNome = !string.IsNullOrWhiteSpace(consulta.CaminhoComprovativo)
                ? Path.GetFileName(consulta.CaminhoComprovativo)
                : $"Comprovativo {consulta.NumeroPedido}";

            var pagamento = new PagamentoModel
            {
                Id_cliente = nifCliente,
                Nif_funcionario = nifResponsavel,
                Comprovativo = string.IsNullOrWhiteSpace(comprovativoNome) ? $"Comprovativo {consulta.NumeroPedido}" : comprovativoNome,
                Data_envio = DateTime.UtcNow,
                CaminhoComprovativo = consulta.CaminhoComprovativo
            };

            context.Tabelatb14_pagamento.Add(pagamento);
            await context.SaveChangesAsync();

            pagamentoConsulta = new PagamentoConsultaModel
            {
                Id_Pagamento = pagamento.Id,
                Id_Consulta = consulta.Id
            };

            context.Tabelatb18_pagamento_consulta.Add(pagamentoConsulta);
            await context.SaveChangesAsync();
        }

        var estadoConfirmada = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Confirmada");

        if (estadoConfirmada == null)
            return "Erro: Estado 'Confirmada' não configurado.";

        consulta.Id_estado_consulta = estadoConfirmada.Id;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?
            .Select(c => c.Contacto)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && c.Any(char.IsDigit))
            ?? string.Empty;

        var mensagem =
            $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
            $"Temos o prazer de informar que o seu pagamento foi validado " +
            $"com sucesso e a sua consulta está oficialmente confirmada.\n\n" +
            $"Detalhes da consulta:\n" +
            $"  • Número do Pedido: {consulta.NumeroPedido}\n" +
            $"  • Data e Hora: {consulta.Data_consulta.ToLocalTime():dd/MM/yyyy 'às' HH:mm}\n\n" +
            $"Recomendamos que se apresente com 10 minutos de antecedência " +
            $"munido do seu documento de identificação.\n\n" +
            $"Contamos com a sua presença.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";

        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}
