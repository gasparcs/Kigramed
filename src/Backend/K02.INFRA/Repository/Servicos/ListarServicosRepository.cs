using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class ListarServicoRepository(KigramedDbContext context) : IlistagemRepository<ServicoModel>
{
    public async Task<IEnumerable<ServicoModel>> Listagem()
    {
        try
        {
            var servicos = await context.Tabelatb08_servico
            .Include(e => e.Especialidade)
            .ToListAsync();
            return servicos;
        }
        catch 
        {
            return [];
        }
        
    }
}
