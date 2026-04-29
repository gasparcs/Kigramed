using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class PegarClientePeloNifRepository (KigramedDbContext context) : IPegarPeloNifRepository<ClienteModel>
{
    public async Task<ClienteModel?> PegarpeloNifAsync(string nif)
    {
        try
        {
             return await context.Tabelatb09_cliente 
            .Include(p =>p.Pacientes)
            .Include(c =>c.Contactos)
            .FirstOrDefaultAsync(n =>n.Nif_cliente ==nif);
        }
        catch
        {
            return null;
        }
    
    }
}
