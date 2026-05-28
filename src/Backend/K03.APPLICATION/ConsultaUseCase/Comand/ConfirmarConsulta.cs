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
        consulta.PrazoPagamento = DateTime.Now.AddMinutes(30);
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?
            .Select(c => c.Contacto)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && c.Any(char.IsDigit))
            ?? string.Empty;

        var deadline = consulta.PrazoPagamento?.ToString("dd/MM/yyyy HH:mm") 
                       ?? DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy HH:mm");

        var mensagem =
            $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
            $"O seu pedido de agendamento foi recebido e aceite com sucesso.\n\n" +
            $"Detalhes do pedido:\n" +
            $"Número do Pedido: {consulta.NumeroPedido}\n" +
            $"Data e Hora: {consulta.Data_consulta:dd/MM/yyyy 'ás' HH:mm}\n" +
            $"Prazo para pagamento: {deadline}\n\n" +
            $"Para confirmar a sua consulta, efectue o pagamento dentro do prazo " +
            $"indicado e envie o comprovativo através do portal.\n\n" +
            $"Dados bancários para transferência:\n" +
            $"    Banco: [NOME DO BANCO]\n" +
            $"    IBAN: [IBAN DA CLÍNICA]\n" +
            $" Referência: {consulta.NumeroPedido}\n\n" +
            $"Atenção: o pedido será cancelado automaticamente caso o pagamento " +
            $"não seja efectuado dentro do prazo.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";


        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

