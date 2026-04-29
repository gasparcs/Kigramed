using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.EstadoConsulta;

public class ListarEstadoConsultaRepository(KigramedDbContext context) : IlistagemRepository<EstadoConsultaModel>
{
   
    public async Task<IEnumerable<EstadoConsultaModel>> Listagem()
    {

        try
        {
        var estadosConsulta = await context.Tabelatb13_estado_consulta
        .Include(c => c.Consultas)
        .ToListAsync();
        return estadosConsulta;
        }
        catch
        {
            return [];
        }
    }
    
}
