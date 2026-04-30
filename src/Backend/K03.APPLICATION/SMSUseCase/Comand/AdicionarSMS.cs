using System;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.SMSUseCase.Comand;

public class AdicionarSMS(ICadastrarRepository<SMSModel> repository)
{
     public async Task<string> ExecuteAsync(AdicionarSMSDTO dto)
    {
        var model = new SMSModel
        {
            Mensagem = dto.SMSMensagem,

            Nif_funcionario = dto.SMSNif_funcionario,
            
            Id_cliente = dto.SMSId_cliente,
        };

            return await repository.AddAsync(model);
    }
}
