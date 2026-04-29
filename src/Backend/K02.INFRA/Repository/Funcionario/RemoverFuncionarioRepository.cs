using System;
using System.Data;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class RemoverFuncionarioRepository(KigramedDbContext context) : IRemoverRepository<FuncionarioModel>
{
     public async Task<string> RemoverAsync(FuncionarioModel model)
    {
        try
        {
        var contactos = context.Tabelatb04_contato
        .Where(c => c.Nif_funcionario == model.Nif);
    
        context.Tabelatb04_contato.RemoveRange(contactos);
        await context.SaveChangesAsync();

        context.Tabelatb02_funcionario.Remove(model);
        return await context.SaveChangesAsync() >0 ?
        "Funcionário removido com sucesso." :
        "Não foi possível remover o funcionário.";
        }
        catch(DbUpdateException ex)
        {
            return (ex.ToString()); 
        }
        
    }
}
