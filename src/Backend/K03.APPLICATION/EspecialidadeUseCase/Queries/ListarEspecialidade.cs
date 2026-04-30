using System;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;

public class ListarEspecialidade(IlistagemRepository<EspecialidadeModel> repository)
{
     public async Task<IEnumerable<LeituraEspecialidadeDTO>> ExecuteAsync()
    {
        var especialidades = await repository.Listagem();

        return especialidades.Select(e => new LeituraEspecialidadeDTO
        {
              EspecialidadeId = e.Id,

              EspecialidadeNome = e.Nome,

              EspecialidadeDescricao = e.Descricao,
              
              EspecialidadeEstado = e.Estado,

              MedicoEspecialidade = e.MedicoEspecialidades.Select(me => new MedicoEspecialidadeDTO
              {
                FuncionarioNif = me.Nif_funcionario ?? string.Empty,

                FuncionarioNome = me.Funcionario?.Nome ?? string.Empty  
              }),

             Servicos = e.Servicos.Select(s => new ServicoDTO
             {
                  ServicoDescricao = s.Nome ?? string.Empty 
              })
        });
    }
}
