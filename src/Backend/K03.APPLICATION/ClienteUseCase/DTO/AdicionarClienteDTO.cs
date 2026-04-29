using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ClienteUseCase.DTO;

public class AdicionarClienteDTO
{
    [Required(ErrorMessage = "Informe o nome do Cliente")]
     public string ClienteNome{get;set;}=string.Empty;

    [Required(ErrorMessage = "Informe o Nif do Cliente")]
    public string ClienteNif{get;set;}=string.Empty;

    [Required(ErrorMessage = "Informe os contactos do Cliente")]
    public IEnumerable<AdicionarContactoDTO> Contactos { get; set; } = [];
}

public class AdicionarContactoDTO
{
    public int TipoContacto { get; set; }

    public string Contacto { get; set; } = string.Empty;
}