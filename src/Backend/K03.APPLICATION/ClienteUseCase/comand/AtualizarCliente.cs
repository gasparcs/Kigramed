using System;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ClienteUseCase.comand;

public class AtualizarCliente(IAtualizarRepository<ClienteModel> repository)
{
     public async Task<string> ExecuteAsync(AtualizarClienteDTO dto)
    {
        var model = new ClienteModel
        {
            Nif_cliente = dto.Nif_cliente,
            
            Nome = dto.Nome,

            Contactos = dto.Contactos.Select( c => new ContactoModel
            {
                Id_tipo_contacto = c.TipoContacto,

                Contacto = c.Contacto,

                Nif_cliente = dto.Nif_cliente
                
            }).ToList()
        };
        return await repository.ActualizarAsync(model);
    }
}
