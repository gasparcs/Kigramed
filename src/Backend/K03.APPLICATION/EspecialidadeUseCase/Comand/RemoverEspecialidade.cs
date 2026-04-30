using System;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;

public class RemoverEspecialidade(IRemoverRepository<EspecialidadeModel> repository)
{
      public async Task<string> ExecuteAsync(int id)
    {
        var model = new EspecialidadeModel
        {
            Id = id
        };

        return await repository.RemoverAsync(model);
    }
}
