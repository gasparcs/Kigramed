using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Queries;

/// Use case exclusivo para o médico autenticado —
/// devolve apenas as suas próprias consultas.
public class ListarConsultasDoMedico(IListarConsultaPorMedicoRepository repository)
{
    public async Task<IEnumerable<LeituraConsultaDTO>> ExecuteAsync(string nifMedico)
    {
        if (string.IsNullOrWhiteSpace(nifMedico))
            return [];

        var consultas = await repository.ListarPorNifMedicoAsync(nifMedico);

        return consultas.Select(c => new LeituraConsultaDTO
        {
            ConsultaId       = c.Id,
            Data_consulta    = c.Data_consulta,
            EstadoDescricao  = c.EstadoConsulta?.Descricao       ?? string.Empty,
            ServicoNome      = c.Servico?.Nome                   ?? string.Empty,
            PacienteNome     = c.Paciente?.Nome                  ?? string.Empty,
            ClienteNome      = c.Paciente?.Cliente?.Nome         ?? string.Empty,
            MedicoNome       = c.MedicoEspecialidade?.Funcionario?.Nome ?? string.Empty,
            Especialidade    = c.MedicoEspecialidade?.Especialidade?.Nome ?? string.Empty,
        });
    }
}
