using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class PegarClientePeloTextoRepository(KigramedDbContext context) : IPegarPeloTextoRepository<ClienteModel>
{
     public async Task<IEnumerable<ClienteModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb09_cliente
                .Where(t => t.Nome.Contains(texto))
                .Include(p => p.Pacientes)
                .Include(c => c.Contactos)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
