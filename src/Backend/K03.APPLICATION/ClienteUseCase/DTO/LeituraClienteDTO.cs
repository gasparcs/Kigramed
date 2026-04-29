using System;

namespace Backend.K03.APPLICATION.ClienteUseCase.DTO;

public class LeituraClienteDTO
{
    public string ClienteNif {get;set;} = string.Empty;

    public string ClienteNome {get;set;} = string.Empty;

    
    public IEnumerable<ContactoDTO> Contactos {get;set;} = [];

    public IEnumerable<PacienteDTO> Pacientes {get;set;} = [];
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

public class PacienteDTO
{
    public string Nome {get;set;} = string.Empty;
}
