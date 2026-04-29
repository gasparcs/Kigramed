using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class AdicionarConsultaRepository(KigramedDbContext context) : ICadastrarRepository<ConsultaModel>
{

        public async Task<string> AddAsync(ConsultaModel model)
    {
          try
           {
              await context.Tabelatb15_consulta.AddAsync(model);
              return await context.SaveChangesAsync()>0?
              "Consulta adicionada com sucesso" :
              "Não foi possível adicionar consulta";
          }
         catch(DbUpdateException ex)
         {
            return (ex.ToString());
         }
    }
  
}
