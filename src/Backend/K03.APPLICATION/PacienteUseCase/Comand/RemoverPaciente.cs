using System;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PacienteUseCase.Comand;

public class RemoverPaciente(IRemoverRepository<PacienteModel> repository)
{
  public async Task<string> ExecuteAsync(int id)
    {
        var model= new PacienteModel
        {
            Id= id
            
        };
        return await repository.RemoverAsync(model);
    }
}