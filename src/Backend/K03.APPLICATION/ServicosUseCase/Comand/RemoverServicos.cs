using System;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ServicosUseCase.Comand;

public class RemoverServico(IRemoverRepository<ServicosModel> repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var model = new ServicosModel
        {
            Id = id
        };

        return await repository.RemoverAsync(model);

    }
}