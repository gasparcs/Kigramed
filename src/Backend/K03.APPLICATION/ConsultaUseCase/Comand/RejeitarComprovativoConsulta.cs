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
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        var estadoRejeitado = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Rejeitado");

        if (estadoRejeitado == null)
            return "Erro: Estado 'Rejeitado' não configurado.";

        consulta.Id_estado_consulta = estadoRejeitado.Id;
        consulta.CaminhoComprovativo = null;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?.FirstOrDefault()?.Contacto ?? "";
        string mensagem = $"KIGRAMED: O comprovativo para o pedido {consulta.NumeroPedido} foi rejeitado. Por favor, envie novamente.";
        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return "sucesso";
    }
}
