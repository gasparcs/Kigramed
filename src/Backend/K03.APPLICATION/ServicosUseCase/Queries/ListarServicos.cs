using System;
using Backend.K03.APPLICATION.ServicosUseCase.DTO;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ServicosUseCase.Queries;

public class ListarServicos(IlistagemRepository<ServicosModel> repository)
{
    public async Task<IEnumerable<ListarServicosDTO>> ExecuteAsync()
    {
      var servicos = await repository.Listagem();

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