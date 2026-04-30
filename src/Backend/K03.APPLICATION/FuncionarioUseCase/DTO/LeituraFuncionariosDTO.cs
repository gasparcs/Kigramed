using System;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.DTO;

public class LeituraFuncionariosDTO
{
    public string FuncionarioNif {get;set;} = null!;

    public string FUncionarioPerfil {get;set;} = null!;

    public string FuncionarioNome {get;set;} = null!;

    public bool FuncionaroEstado {get;set;} 

    public IEnumerable<ContactoDTO> Contactos {get;set;} = [];
}

public class TipoContactoDTO
{
    public string Descricao {get;set;} = string.Empty;
}
public class ContactoDTO
{
    public TipoContactoDTO TipoContacto {get; set;} = null!;
    public string Contacto {get;set;} = string.Empty;
}

