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
            .Include(c => c.Paciente)
            .ThenInclude(p => p.Cliente)
            .ThenInclude(cl => cl.Contactos)
            .Include(c => c.EstadoConsulta)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
            return "nao_encontrado";

        if (consulta.EstadoConsulta.Descricao != "Pendente")
            return $"estado_invalido_confirmar:{consulta.EstadoConsulta.Descricao}";

        var estadoCancelada = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Cancelada");

        if (estadoCancelada == null)
            return "Erro: Estado 'Cancelada' não configurado.";

        bool conflito = await context.Tabelatb15_consulta.AnyAsync(c =>
            c.Id_medico_especialiade == consulta.Id_medico_especialiade &&
            c.Data_consulta == consulta.Data_consulta &&
            c.Id_estado_consulta != estadoCancelada.Id &&
            c.Id != consulta.Id);

        if (conflito)
            return "conflito";

        var estadoAguarda = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Aguarda Pagamento");

        if (estadoAguarda == null)
            return "Erro: Estado 'Aguarda Pagamento' não configurado.";

        consulta.Id_estado_consulta = estadoAguarda.Id;
        consulta.PrazoPagamento = DateTime.UtcNow.AddMinutes(30);
        await context.SaveChangesAsync();

        string telefone = consulta.Paciente?.Cliente?.Contactos?.FirstOrDefault()?.Contacto ?? "";
        string mensagem = $"KIGRAMED: Pedido {consulta.NumeroPedido} confirmado. Por favor pague em 30 min. IBAN: AO06.0000.0000.0000.0000.0000.0";
        bool smsEnviado = await smsService.EnviarAsync(telefone, mensagem, "5417298387");

        return smsEnviado ? "sucesso" : "erro_sms";
    }
}
