using System;
using Backend.K03.APPLICATION.PacienteUseCase.DTO;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PacienteUseCase.Comand;

public class AtualizarPaciente(IAtualizarRepository<PacienteModel> repository)
{
    public async Task<string> ExecuteAsync(AtualizarPacienteDTO dto)
    {
        var model = new PacienteModel
        {
            Id = dto.IdPaciente,
            Nome = dto.PacienteNome,
            Data_nascimento = dto.DataNascimento,
            Id_genero = dto.IdGenero,
            Id_cliente_paciente = dto.IdClientePaciente
        };
        return await repository.ActualizarAsync(model);
    }
}
