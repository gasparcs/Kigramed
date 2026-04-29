using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class ListarClienteRepository(KigramedDbContext context) : IlistagemRepository<ClienteModel>
{
   
    public async Task<IEnumerable<ClienteModel>> Listagem()
    {

        try
        {
        var clientes = await context.Tabelatb09_cliente
        .Include(p => p.Pacientes)
        .Include(c => c.Contactos).ThenInclude(c => c.TipoContacto)
        .ToListAsync();
        return clientes;
        }
        catch
        {
            return [];
        }
    }
    
}
