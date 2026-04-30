using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.PagamentoConsulta;

public class ListarPagamentoConsultaRepository(KigramedDbContext context) : IlistagemRepository<PagamentoConsultaModel>
{
     public async Task<IEnumerable<PagamentoConsultaModel>> Listagem()
    {
        try
        {
            return await context.Tabelatb18_pagamento_consulta
                .Include(x => x.Pagamento)
                .Include(x => x.Consulta)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
