using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Queries;

public class ListarFuncionario(IlistagemRepository<FuncionarioModel> repository)
{
     public async Task<IEnumerable<LeituraFuncionariosDTO>> ExecuteAsync()
    {
        var funcionarios = await repository.Listagem();

        return funcionarios.Select(f => new LeituraFuncionariosDTO
        {
            FuncionarioNif = f.Nif ?? string.Empty,

            FUncionarioPerfil = f.Perfil?.Descricao ?? string.Empty,

            FuncionarioNome = f.Nome ?? string.Empty,

            FuncionaroEstado = f.Estado,

            Contactos = f.Contactos.Select(c => new ContactoDTO
            {
                TipoContacto = new TipoContactoDTO
                {
                    Descricao = c.TipoContacto?.Descricao ?? string.Empty
                },
                Contacto = c.Contacto ?? string.Empty
            })
        });
    }
}
