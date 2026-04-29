using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class AdicionarClienteRepository(KigramedDbContext context) : ICadastrarRepository<ClienteModel>
{
     public async Task<string> AddAsync(ClienteModel model)
    {
        try
        {
             await context.Tabelatb09_cliente.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "Cliente cadastrado com sucesso." :

            "Não foi possível cadastrar o cliente.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
