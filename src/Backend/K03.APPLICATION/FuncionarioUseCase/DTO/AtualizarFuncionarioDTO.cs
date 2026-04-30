using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.DTO;

public class AtualizarFuncionarioDTO
{
    [Required(ErrorMessage = "Informe o Nif do funcionário")]
    public string FuncionarioNif { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe algum perfil para o funcionário")]
    public int FuncionarioPerfil { get; set; }

    [Required(ErrorMessage = "Infomre o nome do funcionário")]
    public string FuncionarioNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o estado do funcionário")]
    public bool FuncionarioEstado { get; set; }

    public IEnumerable<AtualizarFuncionarioEspecialidadeDTO> Especialidades { get; set; } = [];
   
    [Required(ErrorMessage = "Informe os contactos do funcionario")]
    public IEnumerable<AtualizarContactoDTO> Contactos { get; set; } = [];

}

public class AtualizarContactoDTO
{
    public int TipoContacto { get; set; }

    public string Contacto { get; set; } = string.Empty;
}

public class AtualizarFuncionarioEspecialidadeDTO
{
    public int IdEspecialidade { get; set; }
}
