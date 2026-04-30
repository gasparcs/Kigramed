using System;

namespace Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;

public class LeituraEspecialidadeDTO
{
    
    public int EspecialidadeId {get;set;} 

    public string EspecialidadeNome {get;set;} = string.Empty;

    public string EspecialidadeDescricao {get;set;} = string.Empty;

    public bool EspecialidadeEstado {get;set;}

    public IEnumerable<MedicoEspecialidadeDTO> MedicoEspecialidade {get;set;} = [];

    public IEnumerable<ServicoDTO> Servicos {get;set;} = [];
}
public class MedicoEspecialidadeDTO
{
    public string FuncionarioNif {get;set;} = string.Empty;

    public string FuncionarioNome {get;set;} = string.Empty;
}

public class ServicoDTO
{
    public string ServicoDescricao {get;set;} = string.Empty;
}