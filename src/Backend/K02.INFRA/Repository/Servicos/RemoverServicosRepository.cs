using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class RemoverServicoRepository(KigramedDbContext context) : IRemoverRepository<ServicosModel>
{
    public async Task<string> RemoverAsync(ServicosModel model)
    {
        try
        {
             context.Tabelatb08_servico.Remove(model);
             return await context.SaveChangesAsync()>0?
             "Serviço removido com sucesso" :
             "Não foi possível remover serviço";
        }
        catch (DbUpdateException ex)
        {
            return (ex.ToString()); 
        }
       
    }
}