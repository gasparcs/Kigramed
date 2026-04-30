using System;

namespace Backend.K03.APPLICATION.Servico.ISmsService;

public interface ISmsService
{
    Task<bool> EnviarAsync(string telefone, string mensagemTexto, string nif);
}
