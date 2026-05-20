using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class RejeitarComprovativoConsulta(KigramedDbContext context, ISmsService smsService)
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

        var estadosValidos = new[] { "Comprovativo Enviado", "Pagamento Enviado" };
        if (!estadosValidos.Contains(consulta.EstadoConsulta?.Descricao ?? string.Empty))
            return $"estado_invalido_rejeitar:{consulta.EstadoConsulta?.Descricao}";

        var estadoRejeitado = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Rejeitado");

        if (estadoRejeitado == null)
            return "Erro: Estado 'Rejeitado' nÃ£o configurado.";

        consulta.Id_estado_consulta = estadoRejeitado.Id;
        consulta.CaminhoComprovativo = null;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?
            .Select(c => c.Contacto)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && c.Any(char.IsDigit))
            ?? string.Empty;

        var mensagem =
            $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
            $"Informamos que o comprovativo de pagamento submetido para o seu " +
            $"pedido nÃ£o foi aceite pela nossa equipa.\n\n" +
            $"Detalhes do pedido:\n" +
            $"  â€¢ NÃºmero do Pedido: {consulta.NumeroPedido}\n\n" +
            $"PossÃ­veis motivos para a rejeiÃ§Ã£o:\n" +
            $"  â€¢ Documento ilegÃ­vel ou de qualidade insuficiente\n" +
            $"  â€¢ Valor transferido incorrecto\n" +
            $"  â€¢ ReferÃªncia de pagamento em falta ou errada\n\n" +
            $"Por favor submeta um novo comprovativo vÃ¡lido atravÃ©s do portal " +
            $"www.kigramed.com para que a sua consulta possa ser confirmada.\n\n" +
            $"Para qualquer esclarecimento, a nossa equipa estÃ¡ ao seu dispor.\n\n" +
            $"Atenciosamente,\n" +
            $"Centro MÃ©dico Kigramed";

        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}

