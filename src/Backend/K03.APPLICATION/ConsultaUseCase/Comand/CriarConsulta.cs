using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class CriarConsulta(KigramedDbContext context)
{
    public async Task<(string numeroPedido, string mensagem)> ExecuteAsync(
        int idPaciente, int idEspecialidade, int idServico,
        DateTime horarioPreferencial, string? observacoes)
    {
        // 1. Busca idEstadoCancelada em query separada
        var idEstadoCancelada = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Cancelada")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        // 2. Busca MedicoEspecialidade para a especialidade pedida
        var medicoEspecialidade = await context.Tabelatb07_medico_especialidade
            .FirstOrDefaultAsync(me => me.Id_especialidade == idEspecialidade);

        if (medicoEspecialidade == null)
            return (string.Empty, "Nenhum médico disponível para esta especialidade.");

        // 3. Verifica conflito de horário
        var existeConflito = await context.Tabelatb15_consulta
            .AnyAsync(c =>
                c.Id_medico_especialiade == medicoEspecialidade.Id &&
                c.Data_consulta == horarioPreferencial &&
                c.Id_estado_consulta != idEstadoCancelada);

        if (existeConflito)
            return (string.Empty, "Este horário já está reservado. Por favor escolha outro.");

        // 4. Valida paciente
        var pacienteExiste = await context.Tabelatb12_paciente
            .AnyAsync(p => p.Id == idPaciente);

        if (!pacienteExiste)
            return (string.Empty, "Paciente não encontrado.");

        // 5. Busca idEstadoPendente
        var idEstadoPendente = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Pendente")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (idEstadoPendente == 0)
            return (string.Empty, "Estado 'Pendente' não encontrado na base de dados.");

        // 6. Gera NumeroPedido e cria consulta
        var numeroPedido = $"PED-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var consulta = new ConsultaModel
        {
            Id_medico_especialiade = medicoEspecialidade.Id,
            Id_servico             = idServico,
            Id_paciente            = idPaciente,
            Id_estado_consulta     = idEstadoPendente,
            Data_consulta          = horarioPreferencial,
            NumeroPedido           = numeroPedido,
            Observacoes            = observacoes,
            PrazoPagamento         = null,
            CriadoEm              = DateTime.UtcNow
        };

        context.Tabelatb15_consulta.Add(consulta);
        await context.SaveChangesAsync();

        return (numeroPedido, "Pedido criado com sucesso.");
    }
}
