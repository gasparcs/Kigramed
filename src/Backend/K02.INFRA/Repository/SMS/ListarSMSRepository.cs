using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.SMS;

public class ListarSmsRepository(KigramedDbContext context): IlistagemRepository<SMSModel>
{
     public async Task<IEnumerable<SMSModel>> Listagem()
    {
        try
        {
            var dados = await context.Tabelatb20_sms
            .Include(x=>x.Cliente)
                .ToListAsync();

            return dados;
        }
        catch ( Exception)
        {
            return [];
        }
    }
}