using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.SMS;

public class ListarSMSRepository(KigramedDbContext context) : IlistagemRepository<SMSModel>
{
   
    public async Task<IEnumerable<SMSModel>> Listagem()
    {

        try
        {
        var sms = await context.Tabelatb20_sms
        .Include(p => p.Funcionario.Nome)
        .Include(c => c.Cliente.Nome)
        .ToListAsync();
        return sms;
        }
        catch
        {
            return [];
        }
    }
    
}
