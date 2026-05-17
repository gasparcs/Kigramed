using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Queries;

public class ListarConsultasPendentes(KigramedDbContext context)
{
    public async Task<IEnumerable<LeituraConsultaPendenteDTO>> ExecuteAsync()
    {
        var estados = new[] { "Pendente", "Aguarda Pagamento", "Comprovativo Enviado", "Rejeitado" };

        var consultas = await context.Tabelatb15_consulta
            .Include(c => c.Paciente)
            .Include(c => c.EstadoConsulta)
            .Include(c => c.Servico)
            .Include(c => c.MedicoEspecialidade)
                .ThenInclude(me => me.Especialidade)
            .Where(c => estados.Contains(c.EstadoConsulta.Descricao))
            .OrderBy(c => c.Data_consulta)
            .Select(c => new LeituraConsultaPendenteDTO
            {
                Id = c.Id,
                NumeroPedido = c.NumeroPedido,
                NomePaciente = c.Paciente != null ? c.Paciente.Nome : string.Empty,
                Especialidade = c.MedicoEspecialidade != null && c.MedicoEspecialidade.Especialidade != null ? c.MedicoEspecialidade.Especialidade.Nome : string.Empty,
                Servico = c.Servico != null ? c.Servico.Nome : string.Empty,
                DataConsulta = c.Data_consulta,
                Estado = c.EstadoConsulta != null ? c.EstadoConsulta.Descricao : string.Empty,
                PrazoPagamento = c.PrazoPagamento,
                CaminhoComprovativo = c.CaminhoComprovativo,
                CriadoEm = c.CriadoEm
            })
            .ToListAsync();

        return consultas;
    }
}
