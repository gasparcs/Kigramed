using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Queries;

public class ListarConsultas(IlistagemRepository<ConsultaModel> repository)
{
    public async Task<IEnumerable<LeituraConsultaDTO>> ExecuteAsync()
    {
        var consultas = await repository.Listagem();

        return consultas.Select(c => new LeituraConsultaDTO
        {
            ConsultaId = c.Id,

            Data_consulta = c.Data_consulta,

            EstadoDescricao = c.EstadoConsulta?.Descricao ?? string.Empty,

            ServicoNome = c.Servico?.Nome ?? string.Empty,

            PacienteNome = c.Paciente?.Nome ?? string.Empty,

            ClienteNome = c.Paciente?.Cliente?.Nome ?? string.Empty,

            MedicoNome = c.MedicoEspecialidade?.Funcionario?.Nome ?? string.Empty,

            Especialidade = c.MedicoEspecialidade?.Especialidade?.Nome ?? string.Empty
        });
    }
}