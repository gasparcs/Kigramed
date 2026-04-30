using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class PegarEspecialidadePeloTextoRepository(KigramedDbContext context) : IPegarPeloTextoRepository<EspecialidadeModel>
{
     public async Task<IEnumerable<EspecialidadeModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb06_especialidade
                .Where(t => t.Nome.Contains(texto) || t.Descricao.Contains(texto))
                .Include(m => m.MedicoEspecialidades)
                .Include(s => s.Servicos)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
