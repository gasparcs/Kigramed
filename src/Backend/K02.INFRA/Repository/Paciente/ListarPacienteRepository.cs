using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class ListarPacienteRepository(KigramedDbContext context) : IlistagemRepository<PacienteModel>
{
        public async Task<IEnumerable<PacienteModel>> Listagem()
    {

        try
        {
        var clientes = await context.Tabelatb12_paciente
        .Include(p => p.Cliente)
        .Include(P=> P.ClientePaciente)
        .Include(P=> P.Genero)
        .Include(p=> p.Consultas)
        .ToListAsync();
        return clientes;
        }
        catch
        {
            return [];
        }
    }
}
