using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D05.Auth;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Auth;

public class PegarAuthPeloNifRepository(KigramedDbContext context) : IPegarAuthPeloNifRepository
{
    public async Task<AuthModel?> PegarPeloNifAsync(string nif)
    {
        try
        {
              return await context.Tabelatb05_auth
            .Include(a => a.Funcionario)
                .ThenInclude(f => f.Perfil)
            .Include(a => a.Funcionario)
                .ThenInclude(f => f.Contactos)
                    .ThenInclude(c => c.TipoContacto)
            .FirstOrDefaultAsync(a => a.Nif_funcionario == nif);
        }
        catch 
        {
            
            return null;
        }
      
    }
}
