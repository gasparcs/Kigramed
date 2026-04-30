using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class AdicionarServicoRepository(KigramedDbContext context) : ICadastrarRepository<ServicosModel>
{
    public async Task<string> AddAsync(ServicosModel model)
    {
        try
        {
              await context.Tabelatb08_servico.AddAsync(model);
              return await context.SaveChangesAsync()>0?
              "Serviço cadastrado com sucesso" :
              "Não foi posível cadastrar serviço";
        }
        catch (DbUpdateException ex)
        {
            return(ex.ToString());
        }
      

    }
}