using System;
using System.Data;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class RemoverEspecialidadeRepository(KigramedDbContext context) : IRemoverRepository<EspecialidadeModel>
{
     public async Task<string> RemoverAsync(EspecialidadeModel model)
    {
        try
        {
         var medicoEspecialidades = context.Tabelatb07_medico_especialidade
        .Where(m => m.Id_especialidade == model.Id);
    
         context.Tabelatb07_medico_especialidade.RemoveRange(medicoEspecialidades);
         await context.SaveChangesAsync();

        context.Tabelatb06_especialidade.Remove(model);
        return await context.SaveChangesAsync() >0 ?
        "Especialidade removida com sucesso." :
        "Não foi possível remover a especialidade.";
        }
        catch(DbUpdateException ex)
        {
            return (ex.ToString()); 
        }
        
    }
}
