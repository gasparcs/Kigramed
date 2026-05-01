using System;

namespace Backend.K03.APPLICATION.Servico.ITokenService;

public interface ITokenService
{
    string GerarToken(string nif, string nome, string telefone, string role, string perfil);

}
