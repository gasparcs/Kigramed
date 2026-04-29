using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Pagamento;

public class AdicionarPagamentoRepository(KigramedDbContext context) : ICadastrarRepository<PagamentoModel>
{
     public async Task<string> AddAsync(PagamentoModel model)
    {
        try
        {
             await context.Tabelatb14_pagamento.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "Pagamento cadastrado com sucesso." :

            "Não foi possível cadastrar o pagamento.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
