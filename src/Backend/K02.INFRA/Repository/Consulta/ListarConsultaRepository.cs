using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class ListarConsultaRepository(KigramedDbContext context) : IlistagemRepository<ConsultaModel>
{
    public async Task<IEnumerable<ConsultaModel>> Listagem()
    {
        try
        {
        var consultas = await context.Tabelatb15_consulta
        .Include(me => me.MedicoEspecialidade).ThenInclude(me => me.Funcionario)
        .Include(me => me.MedicoEspecialidade).ThenInclude(me => me.Especialidade)
        .Include(s => s.Servico)
        .Include(p => p.Paciente).ThenInclude(c => c.Cliente)
        .Include(e => e.EstadoConsulta)
        .OrderByDescending(c => c.Id)
        .ToListAsync();
        return consultas;
        }
        catch
        {
            return [];
        }
        
    
        
       
    }
}
