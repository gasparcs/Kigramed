using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class PegarServicosPeloTextoRepository(KigramedDbContext context) : IPegarPeloTextoRepository<ServicoModel>
{
     public async Task<IEnumerable<ServicoModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb08_servico
                .Where(t => t.Nome.Contains(texto))
                 .Include(e => e.Especialidade)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
