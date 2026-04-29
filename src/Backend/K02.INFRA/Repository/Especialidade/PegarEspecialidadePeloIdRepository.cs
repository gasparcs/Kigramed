using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class PegarEspecialidadePeloIdRepository(KigramedDbContext context) : IPesquisarPeloIdRepository<EspecialidadeModel>
{
    public async Task<EspecialidadeModel?> PegarAsync(int id)
    {
        try
        {
             return await context.Tabelatb06_especialidade 
            .Include(m => m.MedicoEspecialidades)
            .Include(s => s.Servicos)
            .FirstOrDefaultAsync(i => i.Id == id);
        }
        catch
        {
            return null;
        }
    
    }
}
