using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.ClienteUseCase.DTO;

public class AtualizarClienteDTO
{
    [Required(ErrorMessage = "Informe o NIF do cliente")]
    public string Nif_cliente { get; set; } = string.Empty; 

    [Required(ErrorMessage = "Informe o nome do Cliente")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe os contactos do cliente")]
    public IEnumerable<AdicionarContacto> Contactos { get; set; } = [];
}

public class AdicionarContacto
{
    public int TipoContacto { get; set; }

    public string Contacto { get; set; } = string.Empty;
}