using System;
using System.Linq;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.SMSUseCase.Comand;

public class AdicionarSMS(
    ICadastrarRepository<SMSModel> repository,
    IPegarPeloNifRepository<ClienteModel> clienteRepository,
    IPegarPeloNifRepository<FuncionarioModel> funcionarioRepository,
    ISmsService smsService)
{
    private const string NifSistema = "5417298387";

    public async Task<string> ExecuteAsync(AdicionarSMSDTO dto)
    {
        var nifCliente = dto.SMSNif_cliente.Trim();
        var nifFuncionario = dto.SMSNif_funcionario.Trim();

        var cliente = await clienteRepository.PegarpeloNifAsync(nifCliente);

        if (cliente is null)
            return "Cliente nao encontrado";

        var funcionario = await funcionarioRepository.PegarpeloNifAsync(nifFuncionario);

        if (funcionario is null)
            return "Funcionario nao encontrado";

        var telefone = cliente.Contactos?
            .FirstOrDefault(c => c.TipoContacto?.Descricao
                .Equals("telefone", StringComparison.OrdinalIgnoreCase) == true)?
            .Contacto
            ?? cliente.Contactos?
                .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Contacto) && c.Contacto.Any(char.IsDigit))
                ?.Contacto
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(telefone))
            return "Telefone do cliente nao encontrado";

        var model = new SMSModel
        {
            Mensagem = dto.SMSMensagem,
            Nif_funcionario = nifFuncionario,
            Nif_cliente = nifCliente,
            Data_envio = DateTime.Now
        };

        var smsEnviado = await smsService.EnviarAsync(telefone, dto.SMSMensagem, NifSistema);
        model.Estado = smsEnviado;

        var resultado = await repository.AddAsync(model);

        return smsEnviado
            ? resultado
            : "SMS cadastrada, mas o envio falhou.";
    }
}
