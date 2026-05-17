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
            return "Erro: Estado 'Cancelada' não configurado.";

        consulta.Id_estado_consulta = estadoCancelada.Id;
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?.FirstOrDefault()?.Contacto ?? "";
        string mensagem = $"KIGRAMED: O seu pedido {consulta.NumeroPedido} foi cancelado.";
        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}
