using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Pagamento;

public class ListarPagamentoRepository(KigramedDbContext context) : IlistagemRepository<PagamentoModel>
{
    public async Task<IEnumerable<PagamentoModel>> Listagem()
    {
        try
        {
             var pagamentos = await context.Tabelatb14_pagamento
             .Include(c => c.Cliente)
             .Include(f => f.Funcionario)
             .Include(p => p.PagamentoConsultas)
             .ToListAsync();
             return pagamentos;
        }

        catch 
        {
            
             return [];
        }
        
    }
}
