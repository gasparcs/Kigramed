using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class AtualizarEspecialidadeRepository(KigramedDbContext context) : IAtualizarRepository<EspecialidadeModel>
{
      public async Task<string> ActualizarAsync(EspecialidadeModel model)
    {

        try
        {
            var especialidade = await context.Tabelatb06_especialidade.FirstOrDefaultAsync(f=> f.Id == model.Id); 
            if(especialidade is null) return "Especialidade não encontrada";
            especialidade.Nome = model.Nome;
            especialidade.Descricao = model.Descricao;
            especialidade.Estado = model.Estado;
            return await context.SaveChangesAsync() > 0 ?
            "Especialidade atualizada com sucesso." :
            "Não foi possível efectuar a atualização.";   
        }
        catch(DbUpdateException ex)
        {
            return ( ex.ToString());

        }
       
    }
}
