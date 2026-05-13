using System;
using System.Data;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class RemoverFuncionarioRepository(KigramedDbContext context) : IRemoverRepository<FuncionarioModel>
{
    public async Task<string> RemoverAsync(FuncionarioModel model)
    {
        try
        {
            // IDs de estados que indicam consulta terminada (não bloqueiam)
            var estadosTerminados = new[] { 4, 6 }; // 4=Cancelada, 6=Finalizada

            // Verificar se o funcionário tem consultas ACTIVAS (como médico)
            var idsEspecialidades = await context.Tabelatb07_medico_especialidade
                .Where(me => me.Nif_funcionario == model.Nif)
                .Select(me => me.Id)
                .ToListAsync();

            if (idsEspecialidades.Any())
            {
                var temConsultasActivas = await context.Tabelatb15_consulta
                    .AnyAsync(c => idsEspecialidades.Contains(c.Id_medico_especialiade)
                                && !estadosTerminados.Contains(c.Id_estado_consulta));

                if (temConsultasActivas)
                    return "Não é possível remover este funcionário porque possui " +
                           "consultas activas associadas. " +
                           "Conclua ou cancele essas consultas primeiro.";
            }

            // Sem consultas activas — pode apagar com segurança

            // 1. Remover especialidades do médico
            var especialidades = context.Tabelatb07_medico_especialidade
                .Where(me => me.Nif_funcionario == model.Nif);
            context.Tabelatb07_medico_especialidade.RemoveRange(especialidades);
            await context.SaveChangesAsync();

            // 2. Remover contactos
            var contactos = context.Tabelatb04_contato
                .Where(c => c.Nif_funcionario == model.Nif);
            context.Tabelatb04_contato.RemoveRange(contactos);
            await context.SaveChangesAsync();

            // 3. Remover auth
            var auth = await context.Tabelatb05_auth
                .FirstOrDefaultAsync(a => a.Nif_funcionario == model.Nif);
            if (auth is not null)
            {
                context.Tabelatb05_auth.Remove(auth);
                await context.SaveChangesAsync();
            }

            // 4. Remover funcionário
            context.Tabelatb02_funcionario.Remove(model);
            return await context.SaveChangesAsync() > 0
                ? "Funcionário removido com sucesso."
                : "Não foi possível remover o funcionário.";
        }
        catch (DbUpdateException ex)
        {
            return ex.InnerException?.Message ?? ex.Message;
        }
    }
}
