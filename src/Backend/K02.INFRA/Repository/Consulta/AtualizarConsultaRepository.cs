using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class AtualizarConsultaRepository(KigramedDbContext context) : IAtualizarRepository<ConsultaModel>
{
     public async Task<string> ActualizarAsync(ConsultaModel model)
    {
        try
        {
                var consulta = await context.Tabelatb15_consulta
                
               .FirstOrDefaultAsync(c=>c.Id == model.Id);

                if(consulta is null) return "Consulta não encontrada";

                consulta.Data_consulta          = model.Data_consulta;

                consulta.Id_medico_especialiade = model.Id_medico_especialiade;

                consulta.Id_estado_consulta     = model.Id_estado_consulta;

                return await context.SaveChangesAsync() >0?
                "Consulta atualizada com sucesso" :
                "Não foi possível realizar a atualização";
        }
        catch(DbUpdateException ex)
        { 
            return ( ex.ToString());
        }
    }
}
