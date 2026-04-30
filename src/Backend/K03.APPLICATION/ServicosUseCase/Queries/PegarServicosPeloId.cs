using System;
using Backend.K03.APPLICATION.ServicosUseCase.DTO;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ServicosUseCase.Queries;

public class PegarServicoPeloId(IPesquisarPeloIdRepository<ServicosModel> repository)
{
     public async Task<ListarServicosDTO ?> ExecuteAsync(int id)
    {
      var servico = await repository.PegarAsync(id); 

      if(servico is null) return null;

      return new ListarServicosDTO
      {
          ServicoNome = servico.Nome,

          ServicoDuracaoMinuto = servico.Duracao_minuto,

          ServicoEstado = servico.Estado,

          ServicoPreco = servico.Preco,

          IdEspecialidade = servico.Id_especialidade
      };


    }
}
