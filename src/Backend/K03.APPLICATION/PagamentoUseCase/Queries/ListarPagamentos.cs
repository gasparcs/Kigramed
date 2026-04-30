using System;
using Backend.K03.APPLICATION.PagamentoUseCase.PagamentoDTO;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PagamentoUseCase.Queries;

public class ListarPagamentos(IlistagemRepository<PagamentoModel> repository)
{
    public async Task<IEnumerable<LeituraPagamentoDTO>> ExecuteAsync()
    {
        var pagamentos = await repository.Listagem();

        return pagamentos.Select(p => new LeituraPagamentoDTO
        {
            Id = p.Id,

            Cliente = p.Cliente.Nome,

            Secretaria = p.Funcionario.Nome,

            Comprovativo = p.Comprovativo,

            DataEnvio = p.Data_envio
        });
    }
}