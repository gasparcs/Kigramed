using System;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ClienteUseCase.comand;

public class RemoverCliente(IRemoverRepository<ClienteModel> repository)
{
     public async Task<string> ExecuteAsync(string nif) 
    {
        var model = new ClienteModel
        { 
             Nif_cliente = nif
        };

        return await repository.RemoverAsync(model);
    }
}
