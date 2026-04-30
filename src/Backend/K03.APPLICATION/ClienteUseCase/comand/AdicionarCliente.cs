using System;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ClienteUseCase.comand;

public class AdicionarCliente(ICadastrarRepository<ClienteModel> repository)
{
     public async Task<string> ExecuteAsync(AdicionarClienteDTO dto)
    {
        var model = new ClienteModel
        {
            Nome = dto.ClienteNome,
            
            Nif_cliente = dto.ClienteNif,

             Contactos = dto.Contactos.Select( c => new ContactoModel
            {
                Id_tipo_contacto = c.TipoContacto,

                Contacto = c.Contacto,

                Nif_funcionario = null,

                Nif_cliente = dto.ClienteNif
                
            }).ToList()

        };
         return await repository.AddAsync(model);
    }          
}
