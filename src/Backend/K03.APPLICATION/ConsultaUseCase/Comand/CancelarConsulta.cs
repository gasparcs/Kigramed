using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class CancelarConsulta(KigramedDbContext context, ISmsService smsService)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var consulta = await context.Tabelatb15_consulta
            .Include(c => c.Paciente)
                .ThenInclude(p => p.Cliente)
                .ThenInclude(cl => cl.Contactos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        var estadoCancelada = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Cancelada");

        if (estadoCancelada == null)
            return "Erro: Estado 'Cancelada' nÃ£o configurado.";

        consulta.Id_estado_consulta = estadoCancelada.Id;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?
            .Select(c => c.Contacto)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && c.Any(char.IsDigit))
            ?? string.Empty;

        var mensagem =
            $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
            $"Informamos que o seu pedido de agendamento foi cancelado.\n\n" +
            $"Detalhes do pedido cancelado:\n" +
            $"Número do Pedido: {consulta.NumeroPedido}\n" +
            $"Data e Hora: {consulta.Data_consulta:dd/MM/yyyy 'às' HH:mm}\n\n" +
            $"Se o cancelamento foi inesperado ou deseja efectuar um novo " +
            $"agendamento, convidamo-lo(a) a aceder ao nosso portal em " +
            $"www.kigramed.com ou a contactar-nos directamente.\n\n" +
            $"Pedimos desculpa por qualquer inconveniente causado.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro Médico Kigramed";

        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

