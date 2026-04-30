using System;
using Backend.K03.APPLICATION.PagamentoUseCase.DTO;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PagamentoUseCase.Comand;

public class AdicionarPagamento(ICadastrarRepository<PagamentoModel> repository)
{
     public async Task<string> ExecuteAsync(AdicionarPagamentoDTO dto)
    {
        var model = new PagamentoModel
        {
            Id_cliente = dto.IdCliente,

            Nif_funcionario = dto.IdSecretaria,

            Comprovativo = dto.Comprovativo,

            Data_envio = dto.Data
        };

        return await repository.AddAsync(model);
    }
}
