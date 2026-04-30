using System;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ClienteUseCase.Queries;

public class PegarClientePeloNif(IPegarPeloNifRepository<ClienteModel> repository)
{
        public async Task<LeituraClienteDTO?> ExecuteAsync(string nif)
    {
        var cliente = await repository.PegarpeloNifAsync(nif);

        if (cliente is null) return null;

        return new LeituraClienteDTO
        {
            ClienteNome = cliente.Nome,

            ClienteNif = cliente.Nif_cliente,

            Contactos = cliente.Contactos.Select( c => new ContactoDTO
            {
                TipoContacto = new TipoContactoDTO
                    {
                        Descricao = c.TipoContacto.Descricao
                    },

                Contacto = c.Contacto
            }),

            Pacientes = cliente.Pacientes.Select( p => new PacienteDTO
            {
                Nome = p.Nome
            })
        };
      
    }
}

