using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class RemoverClienteRepository(KigramedDbContext context) : IRemoverRepository<ClienteModel>
{
     public async Task<string> RemoverAsync(ClienteModel model)
    {
        try
        {
             var contactos = context.Tabelatb04_contato
        .Where(c => c.Nif_cliente == model.Nif_cliente);
    
         context.Tabelatb04_contato.RemoveRange(contactos);
         await context.SaveChangesAsync();

        context.Tabelatb09_cliente.Remove(model);
        return await context.SaveChangesAsync() >0 ?
        "Cliente removido com sucesso." :
        "Não foi possível remover o cliente.";
        }
        catch(DbUpdateException ex)
        {
            return (ex.ToString());
        }
        
    }
}
