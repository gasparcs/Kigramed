using System;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.SMSUseCase.Queries;

public class ListarSMS(IlistagemRepository<SMSModel> repository)
{
    public async Task<IEnumerable<LeituraSMSDTO>> ExecuteAsync()
    {
        var sms = await repository.Listagem();

        return sms.Select(s => new LeituraSMSDTO
        {
            SmsId = s.Id,

            Mensagem = s.Mensagem ?? string.Empty,

            DataEnvio = s.Data_envio,

            Cliente = new ClienteSmsDTO
            {
                ClienteNif = s.Cliente?.Nif_cliente ?? string.Empty,

                ClienteNome = s.Cliente?.Nome ?? string.Empty
            }
        });
    }
}
