using System;
using Backend.K03.APPLICATION.ServicosUseCase.DTO;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ServicosUseCase.Comand;

public class AtualizarServicos(IAtualizarRepository<ServicosModel> repository)
{
    public async Task<string> ExecuteAsync(AtualizarServicosDTO dto)
    {
        var model = new ServicosModel
        {
            Id = dto.ServicoId,

           Nome = dto.ServicoNome,

           Duracao_minuto = dto.ServicoDuracaoMinuto,

           Preco = dto.ServicoPreco,

           Estado = dto.ServicoEstado
        };

        return await repository.ActualizarAsync(model);
    }
}