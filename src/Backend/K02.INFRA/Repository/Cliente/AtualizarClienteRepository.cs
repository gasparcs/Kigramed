using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class AtualizarClienteRepository(KigramedDbContext context) : IAtualizarRepository<ClienteModel>
{
      public async Task<string> ActualizarAsync(ClienteModel model)
    {

        try
        {
            var cliente = await context.Tabelatb09_cliente.FirstOrDefaultAsync(f=> f.Nif_cliente == model.Nif_cliente); 
            if(cliente is null) return "Cliente não encontrado";
            cliente.Nome = model.Nome;
            cliente.Contactos = model.Contactos;
            return await context.SaveChangesAsync() > 0 ?
            "Cliente Atualizado com sucesso." :
            "Não foi possível efectuar a atualização.";   
        }
        catch(DbUpdateException ex)
        {
            return ( ex.ToString());

        }
       
    }
}
