using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class ConfirmarConsulta(KigramedDbContext context, ISmsService smsService)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var consulta = await context.Tabelatb15_consulta
            .Include(c => c.Paciente!)
                .ThenInclude(p => p.Cliente)
                .ThenInclude(cl => cl.Contactos)
            .Include(c => c.EstadoConsulta)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        if (consulta.EstadoConsulta.Descricao != "Pendente")
            return $"estado_invalido_confirmar:{consulta.EstadoConsulta.Descricao}";

        var idEstadoCancelada = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Cancelada")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (idEstadoCancelada == 0)
            return "Erro: Estado 'Cancelada' nÃ£o configurado.";

        bool conflito = await context.Tabelatb15_consulta.AnyAsync(c =>
            c.Id_medico_especialiade == consulta.Id_medico_especialiade &&
            c.Data_consulta == consulta.Data_consulta &&
            c.Id_estado_consulta != idEstadoCancelada &&
            c.Id != consulta.Id);

        if (conflito)
            return "conflito";

        var estadoAguarda = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Aguarda Pagamento");

        if (estadoAguarda == null)
            return "Erro: Estado 'Aguarda Pagamento' nÃ£o configurado.";

        consulta.Id_estado_consulta = estadoAguarda.Id;
        consulta.PrazoPagamento = DateTime.UtcNow.AddHours(48);
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?
            .Select(c => c.Contacto)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && c.Any(char.IsDigit))
            ?? string.Empty;

        var deadline = consulta.PrazoPagamento?.ToLocalTime().ToString("dd/MM/yyyy HH:mm") 
                       ?? DateTime.UtcNow.AddMinutes(30).ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        var mensagem =
            $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
            $"O seu pedido de agendamento foi recebido e aceite com sucesso.\n\n" +
            $"Detalhes do pedido:\n" +
            $"  â€¢ NÃºmero do Pedido: {consulta.NumeroPedido}\n" +
            $"  â€¢ Data e Hora: {consulta.Data_consulta.ToLocalTime():dd/MM/yyyy 'Ã s' HH:mm}\n" +
            $"  â€¢ Prazo para pagamento: {deadline}\n\n" +
            $"Para confirmar a sua consulta, efectue o pagamento dentro do prazo " +
            $"indicado e envie o comprovativo atravÃ©s do portal.\n\n" +
            $"Dados bancÃ¡rios para transferÃªncia:\n" +
            $"  â€¢ Banco: [NOME DO BANCO]\n" +
            $"  â€¢ IBAN: [IBAN DA CLÃNICA]\n" +
            $"  â€¢ ReferÃªncia: {consulta.NumeroPedido}\n\n" +
            $"AtenÃ§Ã£o: o pedido serÃ¡ cancelado automaticamente caso o pagamento " +
            $"nÃ£o seja efectuado dentro do prazo.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro MÃ©dico Kigramed";

        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

