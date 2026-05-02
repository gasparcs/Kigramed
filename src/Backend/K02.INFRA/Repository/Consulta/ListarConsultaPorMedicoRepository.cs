using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Consulta;

public class ListarConsultaPorMedicoRepository(KigramedDbContext context)
    : IListarConsultaPorMedicoRepository
{
    public async Task<IEnumerable<ConsultaModel>> ListarPorNifMedicoAsync(string nifMedico)
    {
        return await context.Tabelatb15_consulta
            .Include(c => c.MedicoEspecialidade).ThenInclude(me => me.Funcionario)
            .Include(c => c.MedicoEspecialidade).ThenInclude(me => me.Especialidade)
            .Include(c => c.Servico)
            .Include(c => c.Paciente).ThenInclude(p => p.Cliente)
            .Include(c => c.EstadoConsulta)
            // Filtra pelo NIF do funcionário/médico autenticado
            .Where(c => c.MedicoEspecialidade.Funcionario.Nif == nifMedico)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }
}
