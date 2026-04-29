using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class PegarServicosPeloIdRepository(KigramedDbContext context) : IPesquisarPeloIdRepository<ServicoModel>
{
    public async Task<ServicoModel?> PegarAsync(int id)
    {
        try
        {
             return await context.Tabelatb08_servico
              .Include(e => e.Especialidade)
            .FirstOrDefaultAsync(i => i.Id == id);
        }
        catch
        {
            return null;
        }
    
    }
}
