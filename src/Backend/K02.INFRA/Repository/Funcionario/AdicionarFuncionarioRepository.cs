using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class AdicionarFuncionarioRepository(KigramedDbContext context) : ICadastrarRepository<FuncionarioModel>
{
     public async Task<string> AddAsync(FuncionarioModel model)
    {
        try
        {
             await context.Tabelatb02_funcionario.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "Funcionário cadastrado com sucesso." :

            "Não foi possível cadastrar o funcionário.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
