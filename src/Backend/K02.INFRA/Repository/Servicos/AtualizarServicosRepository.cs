using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class AtualizarServicosRepository(KigramedDbContext context) : IAtualizarRepository<ServicosModel>
{
    public async Task<string> ActualizarAsync(ServicosModel model)
    {
        try
        {
             var servico = await context.Tabelatb08_servico.FirstOrDefaultAsync(s=>s.Id == model.Id);
        if(servico is null) return "Serviço não encontrado";
        servico.Nome=model.Nome;
        servico.Duracao_minuto=model.Duracao_minuto;
        servico.Preco=model.Preco;
        servico.Estado=model.Estado;
        return await context.SaveChangesAsync()>0?
        "Actualização feita com sucesso" :
        "Não foi possível efectuar a actualização";
        }
        catch (DbUpdateException ex)
        {
             return ( ex.ToString());
        }
       
    }
}
