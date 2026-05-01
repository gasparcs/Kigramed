using System;
using Backend.K03.APPLICATION.PacienteUseCase.DTO;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PacienteUseCase.Comand;

public class AdicionarPaciente(ICadastrarRepository<PacienteModel> repository)
{
   public async Task<string> ExecuteAsync(AdicionarPacienteDTO dto)
    {
          var model = new PacienteModel
        {
            Nome = dto.PacienteNome,
            Data_nascimento = DateTime.SpecifyKind(dto.PacienteData_nascimento.Date, DateTimeKind.Utc),
            Id_genero = dto.IdGenero,
            Nif_cliente = dto.Nif_cliente,
            Id_cliente_paciente = dto.IdCliente_Paciente
        };

        return await repository.AddAsync(model);
    }
}
