using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Servicos;

public class AtualizarServicosRepository(KigramedDbContext context) : IAtualizarRepository<ServicoModel>
{
      public async Task<string> ActualizarAsync(ServicoModel model)
    {

        try
        {
            var servico = await context.Tabelatb08_servico.FirstOrDefaultAsync(f=> f.Id == model.Id); 
            if(servico is null) return "Serviço não encontrado";
            servico.Nome = model.Nome;
            servico.Especialidade = model.Especialidade;
            servico.Duracao_minuto = model.Duracao_minuto;
            servico.Preco = model.Preco;
            servico.Estado = model.Estado;
            return await context.SaveChangesAsync() > 0 ?
            "Serviço atualizado com sucesso." :
            "Não foi possível efectuar a atualização.";
        }
        catch(DbUpdateException ex)
        {
            return ( ex.ToString());

        }
       
    }
}
