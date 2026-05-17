using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class ValidarPagamentoConsulta(KigramedDbContext context, ISmsService smsService)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var consulta = await context.Tabelatb15_consulta
            .Include(c => c.Paciente)
                .ThenInclude(p => p.Cliente)
                .ThenInclude(cl => cl.Contactos)
            .Include(c => c.EstadoConsulta)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        if (consulta.EstadoConsulta.Descricao != "Comprovativo Enviado" && consulta.EstadoConsulta.Descricao != "Pagamento Enviado")
            return $"estado_invalido_validar:{consulta.EstadoConsulta.Descricao}";

        if (consulta.PrazoPagamento.HasValue && consulta.PrazoPagamento.Value < DateTime.UtcNow)
        {
            var estadoCancelada = await context.Tabelatb13_estado_consulta.FirstOrDefaultAsync(e => e.Descricao == "Cancelada");
            if (estadoCancelada != null)
            {
                consulta.Id_estado_consulta = estadoCancelada.Id;
                await context.SaveChangesAsync();
            }
            return "prazo_expirado";
        }

        var estadoConfirmada = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Confirmada");

        if (estadoConfirmada == null)
            return "Erro: Estado 'Confirmada' não configurado.";

        consulta.Id_estado_consulta = estadoConfirmada.Id;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?.FirstOrDefault()?.Contacto ?? string.Empty;

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
