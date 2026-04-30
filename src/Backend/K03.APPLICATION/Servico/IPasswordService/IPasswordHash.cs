using System;

namespace Backend.K03.APPLICATION.Servico.IPasswordService;

public interface IPasswordHash
{
     Task<(string Hash, string Salt)> HashAsync(string senha);
}

