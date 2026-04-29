using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class PegarPacientePeloTextoRepository(KigramedDbContext context) : IPegarPeloTextoRepository<PacienteModel>
{
   public async Task<IEnumerable<PacienteModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb12_paciente
                .Where(t => t.Nome.Contains(texto))
                .OrderBy(t => t.Nome)
                .Include(p => p.Cliente)
                .Include(c => c.ClientePaciente)
                .Include(a => a.Consultas)
                .ThenInclude(c => c.EstadoConsulta)
                .Include(g => g.Genero)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
