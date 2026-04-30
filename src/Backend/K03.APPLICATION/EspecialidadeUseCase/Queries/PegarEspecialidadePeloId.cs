using System;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;

public class PegarEspecialidadePeloId(IPesquisarPeloIdRepository<EspecialidadeModel> repository)
{
     public async Task<LeituraEspecialidadeDTO?> ExecuteAsync(int id)
    {
         var especialidade = await repository.PegarAsync(id);

         if (especialidade is null) return null;

        return new LeituraEspecialidadeDTO
        {
            EspecialidadeId = especialidade.Id,

            EspecialidadeNome = especialidade.Nome,

            EspecialidadeDescricao = especialidade.Descricao,

            EspecialidadeEstado = especialidade.Estado,

            Servicos = especialidade.Servicos.Select(s => new ServicoDTO
            {
                ServicoDescricao = s.Nome ?? string.Empty
            }),

            MedicoEspecialidade = especialidade.MedicoEspecialidades.Select(me => new MedicoEspecialidadeDTO
            {
                FuncionarioNif = me.Nif_funcionario ?? string.Empty,
                
                FuncionarioNome = me.Funcionario?.Nome ?? string.Empty
            })
        };
    }
}
