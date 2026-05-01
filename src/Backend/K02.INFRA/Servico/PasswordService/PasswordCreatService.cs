using System;
using Backend.K03.APPLICATION.Servico.IPasswordService;

namespace Backend.K02.INFRA.Servico.PasswordService;

public class PasswordCreateService : IPasswordCreate
{
    public Task<string> GenerateAsync() =>

    Task.FromResult(new Random().NextInt64(100000, 999999).ToString());
}
