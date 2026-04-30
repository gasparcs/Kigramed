using System;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PagamentoConsultaUseCase.Queries;

public class ListarPagamentoConsulta(IlistagemRepository<PagamentoConsultaModel> repository)
{
     public async Task<IEnumerable<ListarPagamentoConsultaDTO>> ExecuteAsync()
    {
        var pagamentos = await repository.Listagem();

        return pagamentos.Select(p => new ListarPagamentoConsultaDTO
        {
            Id = p.Id,

            IdPagamento = p.Id_Pagamento,

            IdConsulta = p.Id_Consulta,

            DataConsulta = p.Consulta?.Data_consulta ?? DateTime.MinValue,
            
            Comprovativo = p.Pagamento?.Comprovativo ?? string.Empty,
       
        });
    }
}
