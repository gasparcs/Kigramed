using System;

namespace Backend.K03.APPLICATION.Servico.IPasswordService;

public interface IPasswordVerify
{
     Task<bool> VerifyAsync(string senha, string hashArmazenado, string saltArmazenado);
}

