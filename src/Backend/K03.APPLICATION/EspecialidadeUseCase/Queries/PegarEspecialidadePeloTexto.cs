using System;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;

public class PegarEspecialidadePeloTexto(IPegarPeloTextoRepository<EspecialidadeModel> repository)
{
      public async Task<LeituraEspecialidadeDTO?> ExecuteAsync(string texto)
    {
         var especialidade = await repository.PegarAsync(texto);

         if (especialidade is null) return null;

         return especialidade.Select( e => new LeituraEspecialidadeDTO
         {
             EspecialidadeId = e.Id,

            EspecialidadeNome = e.Nome,

            EspecialidadeDescricao = e.Descricao,

            EspecialidadeEstado = e.Estado,

            Servicos = e.Servicos.Select(s => new ServicoDTO
            {
                ServicoDescricao = s.Nome ?? string.Empty
            }),

            MedicoEspecialidade = e.MedicoEspecialidades.Select(me => new MedicoEspecialidadeDTO
            {
                FuncionarioNif = me.Nif_funcionario ?? string.Empty,

                FuncionarioNome = me.Funcionario?.Nome ?? string.Empty
            })   
         });
    }
}
