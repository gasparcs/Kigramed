using System;
using Backend.K03.APPLICATION.ServicosUseCase.DTO;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ServicosUseCase.Queries;

public class PegarServicoPeloTexto(IPegarPeloTextoRepository<ServicosModel> repository)
{
     public async Task<IEnumerable<ListarServicosDTO>?> ExecuteAsync(string texto)
    {
      var servicos = await repository.PegarAsync(texto); 

      if(servicos is null) return null;

      return servicos.Select( s => new ListarServicosDTO
      {
           ServicoNome = s.Nome,

          ServicoDuracaoMinuto = s.Duracao_minuto,

          ServicoEstado = s.Estado,

          ServicoPreco = s.Preco,

          IdEspecialidade = s.Id_especialidade
      });
    }
}

