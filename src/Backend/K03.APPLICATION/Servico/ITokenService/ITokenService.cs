using System;

namespace Backend.K03.APPLICATION.Servico.ITokenService;

public interface ISmsService
{
    Task<bool> EnviarAsync(string telefone, string mensagemTexto, string nif);
}
