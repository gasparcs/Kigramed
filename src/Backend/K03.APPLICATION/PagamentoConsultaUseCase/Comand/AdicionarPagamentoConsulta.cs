using System;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PagamentoConsultaUseCase.Comand;

public class AdicionarPagamentoConsulta(ICadastrarRepository<PagamentoConsultaModel> repository)
{
    public async Task<string> ExecuteAsync(AdicionarPagamentoConsultaDTO dto)
    {
        var model = new PagamentoConsultaModel
        {
            Id_Pagamento = dto.IdPagamento,
            
            Id_Consulta = dto.IdConsulta
        };

        return await repository.AddAsync(model);
    }
}
