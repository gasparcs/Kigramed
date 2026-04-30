using System;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;

public class AdicionarEspecialidade(ICadastrarRepository<EspecialidadeModel> repository)
{
     public async Task<string> ExecuteAsync(AdicionarEspecialidadeDTO dto)
    {
        var model = new EspecialidadeModel
        {
          Nome = dto.EspecialidadeNome,

          Descricao = dto.EspecialidadeDescricao,

          Estado = dto.EspecialidadeEstado
        };

        return await repository.AddAsync(model);
    }
}
