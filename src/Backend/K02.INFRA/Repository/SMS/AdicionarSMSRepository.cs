using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.SMS;

public class AdicionarSMSRepository(KigramedDbContext context) : ICadastrarRepository<SMSModel>
{
     public async Task<string> AddAsync(SMSModel model)
    {
        try
        {
             await context.Tabelatb20_sms.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "SMS cadastrado com sucesso." :

            "Não foi possível cadastrar o SMS.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
