using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Especialidade;

public class ListarEspecialidadeRepository(KigramedDbContext context) : IlistagemRepository<EspecialidadeModel>
{
   
    public async Task<IEnumerable<EspecialidadeModel>> Listagem()
    {

        try
        {
        var especialidades = await context.Tabelatb06_especialidade
        .Include(m => m.MedicoEspecialidades)
        .Include(s => s.Servicos)
        .ToListAsync();
        return especialidades;
        }
        catch
        {
            return [];
        }
    }
    
}
