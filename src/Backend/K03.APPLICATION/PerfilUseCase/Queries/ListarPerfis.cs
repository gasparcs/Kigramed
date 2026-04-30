using System;
using Backend.K03.APPLICATION.PerfilUseCase.DTO;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.PerfilUseCase.Queries;

public class ListarPerfis(IlistagemRepository<PerfilModel> repository)
{
    public async Task<IEnumerable<LeituraPerfilDTO>> ExecuteAsync()
    {
        var perfis = await repository.Listagem();

        return perfis.Select(p => new LeituraPerfilDTO
            {
                PerfilId = p.Id,

                PerfilDescricao = p.Descricao,

                Funcionarios = p.Funcionarios.Select( f => new FuncionarioDTO
                    {
                        Nome = f.Nome
                    }),
                
                PerfilPermissoes = p.PerfisPermissoes.Select( pp => new PerfilPermissaoDTO
                    {
                        Permissao = new PermissaoDTO
                        {
                            Descricao = pp.Permissao.Descricao
                        }
                    })
            
        });
    }
}

