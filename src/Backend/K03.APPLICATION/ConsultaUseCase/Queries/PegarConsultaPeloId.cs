using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Queries;

public class PegarConsultaPeloId(IPesquisarPeloIdRepository<ConsultaModel> repository)
{
    public async Task<LeituraConsultaDTO?> ExecuteAsync(int id)
    {
        var consulta = await repository.PegarAsync(id);

        if (consulta is null) return null;

        return new LeituraConsultaDTO
        {
            ConsultaId = consulta.Id,

            Data_consulta = consulta.Data_consulta,

            EstadoDescricao = consulta.EstadoConsulta?.Descricao ?? string.Empty,

            ServicoNome = consulta.Servico?.Nome ?? string.Empty,

            PacienteNome = consulta.Paciente?.Nome ?? string.Empty,

            ClienteNome = consulta.Paciente?.Cliente?.Nome ?? string.Empty,

            MedicoNome = consulta.MedicoEspecialidade?.Funcionario?.Nome ?? string.Empty,
            
            Especialidade = consulta.MedicoEspecialidade?.Especialidade?.Nome ?? string.Empty
        };
    }
}
