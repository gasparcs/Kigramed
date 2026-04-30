using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K02.INFRA.Repository.PagamentoConsulta;

public class AdicionarPagamentoConsultaRepository(KigramedDbContext context) : ICadastrarRepository<PagamentoConsultaModel>
{
     public async Task<string> AddAsync(PagamentoConsultaModel model)
    {
        try
        {
            await context.Tabelatb18_pagamento_consulta.AddAsync(model);

            return await context.SaveChangesAsync() > 0 ?

                "Pagamento associado à consulta com sucesso" :
                "Não foi possível associar o pagamento à consulta";
        }
        catch (Exception)
        {
            return "Erro ao associar pagamento à consulta";
        }
    }
}
