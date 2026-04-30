using System;

namespace Backend.K03.APPLICATION.Servico.IPasswordService;

public interface IPasswordCreate
{
       Task<string> GenerateAsync();
}
