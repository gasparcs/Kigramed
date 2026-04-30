using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class PegarTextoServicoRepository(KigramedDbContext context) : IPegarPeloTextoRepository<ServicosModel>
{
    public async Task<IEnumerable<ServicosModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb08_servico
            .Where(s=>s.Nome.Contains(texto))
            .Include(e=>e.Especialidade)
            .ToListAsync();
        }
        catch (Exception)
        {
            
            return [];
        }
        
    }
}