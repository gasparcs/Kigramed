using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class PegarConsultaPeloIdRepository(KigramedDbContext context) : IPesquisarPeloIdRepository<ConsultaModel>
{
    public async Task<ConsultaModel?> PegarAsync(int id)
    {
        try
        {
        return await context.Tabelatb15_consulta
        .Include(me=>me.MedicoConsulta)
        .Include(s=>s.Servico)
        .Include(p=>p.Paciente)
        .FirstAsync(c=>c.Id==id);
        }
        catch
        {
            return null;
        }
       
    }
}
