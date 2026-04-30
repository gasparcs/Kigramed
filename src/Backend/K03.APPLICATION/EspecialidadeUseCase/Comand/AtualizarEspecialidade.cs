using System;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;

public class AtualizarEspecialidade(IAtualizarRepository<EspecialidadeModel> repository)
{
      public async Task<string> ExecuteAsync(AtualizarEspecialidadeDTO dto)
    {
        var model = new EspecialidadeModel
        {
            Id = dto.EspecialidadeId,

            Nome = dto.EspecialidadeNome,

            Descricao = dto.EspecialidadeDescricao,

            Estado = dto.EspecialidadeEstado
        };

        return await repository.ActualizarAsync(model);
    }
}
