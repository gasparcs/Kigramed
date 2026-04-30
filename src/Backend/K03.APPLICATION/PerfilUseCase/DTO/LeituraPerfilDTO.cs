using System;

namespace Backend.K03.APPLICATION.PerfilUseCase.DTO;

public class LeituraPerfilDTO
{
    public int PerfilId { get; set; }

    public string PerfilDescricao { get; set; } = string.Empty;

    public IEnumerable<FuncionarioDTO> Funcionarios { get; set; } = [];

    public IEnumerable<PerfilPermissaoDTO> PerfilPermissoes { get; set; } = [];

}

public class FuncionarioDTO
{
    public string Nome { get; set; } = string.Empty;
}

public class PermissaoDTO
{
    public string  Descricao {get;set;} = string.Empty;
}
public class PerfilPermissaoDTO
{
    public PermissaoDTO Permissao {get;set;} = null!;
}


