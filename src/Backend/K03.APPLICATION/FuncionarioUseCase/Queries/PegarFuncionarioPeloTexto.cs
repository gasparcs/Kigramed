using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Queries;

public class PegarFuncionarioPeloTexto(IPegarPeloTextoRepository<FuncionarioModel> repository)
{
      public async Task<IEnumerable<LeituraFuncionariosDTO>?> ExecuteAsync(string texto)
    {
        var funcionario = await repository.PegarAsync(texto);

        if(funcionario is null) return null!;

        return funcionario.Select(f => new LeituraFuncionariosDTO
        {
            FuncionarioNif = f.Nif ?? string.Empty,

            FuncionarioNome = f.Nome ?? string.Empty,

            FUncionarioPerfil = f.Perfil?.Descricao ?? string.Empty, 

            FuncionaroEstado = f.Estado,

            Contactos = f.Contactos.Select(c => new ContactoDTO     
            {
                Contacto = c.Contacto ?? string.Empty,

                TipoContacto = new TipoContactoDTO
                {
                    Descricao = c.TipoContacto?.Descricao ?? string.Empty
                }
            })
        });
    }
}
