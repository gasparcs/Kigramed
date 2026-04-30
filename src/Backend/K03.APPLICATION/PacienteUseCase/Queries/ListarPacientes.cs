using System;
using Backend.K03.APPLICATION.PacienteUseCase.DTO;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PacienteUseCase.Queries;

public class ListarPacientes(IlistagemRepository<PacienteModel> repository)
{
   public async Task<IEnumerable<LeituraPacienteDTO>> ExecuteAsync()
    {
        var pacientes= await repository.Listagem();

        return pacientes.Select(p=> new LeituraPacienteDTO
        {

            PacienteId= p.Id,

            PacienteNome= p.Nome,

            PacienteData_nascimento= p.Data_nascimento,

            Cliente_Paciente = p.ClientePaciente?.Descricao ?? string.Empty,

            Cliente = p.Cliente?.Nome ?? string.Empty,

            Genero = p.Genero?.Nome ?? string.Empty,

            Consultas = p.Consultas?.Select( c => new ConsultaDTO
            {
                IdConsuta = c.Id,

                Data_Consulta = c.Data_consulta,

                Estado = c.EstadoConsulta?.Descricao ?? string.Empty
            }) ?? Enumerable.Empty<ConsultaDTO>()
    
        });
    }
}

