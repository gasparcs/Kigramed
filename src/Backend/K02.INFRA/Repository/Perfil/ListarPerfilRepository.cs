using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Perfil;

public class ListarPerfilRepository(KigramedDbContext context) : IlistagemRepository<PerfilModel>
{
    public async Task<IEnumerable<PerfilModel>> Listagem()
    {
        try
        {
             var perfis = await context.Tabelatb01_perfil
            .ToListAsync();
              return perfis;
        }
        catch 
        {
            return [];
            
        }
        
         
    }
}
