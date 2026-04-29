using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class AdicionarEspecialidadeRepository(KigramedDbContext context) : ICadastrarRepository<EspecialidadeModel>
{
     public async Task<string> AddAsync(EspecialidadeModel model)
    {
        try
        {
             await context.Tabelatb06_especialidade.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "Especialidade cadastrada com sucesso." :

            "Não foi possível cadastrar a especialidade.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
