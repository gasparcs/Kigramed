using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class PegarIdServicosRepository(KigramedDbContext context) : IPesquisarPeloIdRepository<ServicosModel>
{
    public async Task<ServicosModel?> PegarAsync(int id)
    {

        try
        {
            return await context.Tabelatb08_servico
            .Include(e=>e.Especialidade)
             .FirstAsync(s=>s.Id == id);
        }
        catch 
        {
            
            return null;
        }
      
    }
}