using System;
using Backend.K03.APPLICATION.ClienteUseCase.Queries;
using Backend.K03.APPLICATION.FuncionarioUseCase.Queries;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.SMSUseCase.Comand;

public class AdicionarSMS(ICadastrarRepository<SMSModel> repository, PegarClientePeloNif clienterepository, PegarFuncionaarioPeloNif funcionariorepository)
{
     public async Task<string> ExecuteAsync(AdicionarSMSDTO dto)
    {
        var model = new SMSModel
        {
            Mensagem = dto.SMSMensagem,

            Nif_funcionario = dto.SMSNif_funcionario,
            
            Nif_cliente = dto.SMSNif_cliente,
        };

            return await repository.AddAsync(model);
    }
}
