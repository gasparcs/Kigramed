using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class PegarPacientePeloIdRepository(KigramedDbContext context) : IPesquisarPeloIdRepository<PacienteModel>
{
   public async Task<PacienteModel?> PegarAsync(int id)
    {
        try
        {
            return await context.Tabelatb12_paciente
            .Include(p => p.Cliente)
            .Include(c => c.ClientePaciente)
            .Include(a => a.Consultas)
            .ThenInclude(c => c.EstadoConsulta)
            .Include(g => g.Genero)
            .FirstOrDefaultAsync(i =>i.Id ==id);
    
        }
        catch
        {
            return null;
        }
    
    }
}
