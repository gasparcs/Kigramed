using System;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ClienteUseCase.Queries;

public class ListarClientes(IlistagemRepository<ClienteModel> repository)
{
     public async Task<IEnumerable<LeituraClienteDTO>> ExecuteAsync()
    {
      var clientes = await repository.Listagem();

      return clientes.Select(c => new LeituraClienteDTO
        {
            ClienteNome = c.Nome,

            ClienteNif = c.Nif_cliente,

            Contactos = c.Contactos.Select( ct => new ContactoDTO
                {
                    TipoContacto = new TipoContactoDTO
                    {
                        Descricao = ct.TipoContacto.Descricao
                    },

                    Contacto = ct.Contacto
                }),
            
            Pacientes = c.Pacientes.Select(p => new PacienteDTO
                {
                    Nome = p.Nome

                })

        });
    }
}
