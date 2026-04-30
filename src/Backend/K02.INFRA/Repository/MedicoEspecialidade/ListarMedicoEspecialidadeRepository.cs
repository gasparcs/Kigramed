using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.MedicoEspecialidade;

public class ListarMedicoEspecialidadeRepository(KigramedDbContext context) : IlistagemRepository<MedicoEspecilidadeModel>
{
      public async Task<IEnumerable<MedicoEspecilidadeModel>> Listagem()
    {
        try
        {
            return await context.Tabelatb07_medico_especialidade
            .OrderBy(e => e.Funcionario) // ordena alfabeticamente
            .ToListAsync();
        }
        catch (System.Exception)
        {
            
            return [];
        }
       
    }

}