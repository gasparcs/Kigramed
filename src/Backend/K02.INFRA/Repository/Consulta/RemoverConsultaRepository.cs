using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class RemoverConsultaRepository(KigramedDbContext context) : IRemoverRepository<ConsultaModel>
{
       public async Task<string> RemoverAsync(ConsultaModel model)
    {

        try
        {
             context.Tabelatb15_consulta.Remove(model);
             return await context.SaveChangesAsync()>0?
             "Consulta removida com sucesso" :
             "Não foi possível remover consulta";
        }
        catch(DbUpdateException ex)
        {
             return (ex.ToString()); 
        }
      
    }
}
