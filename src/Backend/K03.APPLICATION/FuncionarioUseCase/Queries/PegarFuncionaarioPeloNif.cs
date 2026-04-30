using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Queries;

public class PegarFuncionaarioPeloNif(IPegarPeloNifRepository<FuncionarioModel> repository)
{
     public async Task<LeituraFuncionariosDTO ?> ExecuteAsync(string nif) 
    {
        var funcionario = await repository.PegarpeloNifAsync(nif);

        if (funcionario is null) return null;

        return new LeituraFuncionariosDTO
        {
            FuncionarioNif = funcionario.Nif ?? string.Empty,

            FuncionarioNome = funcionario.Nome ?? string.Empty,

            FUncionarioPerfil = funcionario.Perfil?.Descricao ?? string.Empty, 

            FuncionaroEstado = funcionario.Estado,

            Contactos = funcionario.Contactos.Select(c => new ContactoDTO
            {
                Contacto = c.Contacto ?? string.Empty,

                TipoContacto = new TipoContactoDTO          
                {
                    Descricao = c.TipoContacto?.Descricao ?? string.Empty
                }
            })
        };
    }
}
