using System;
using System.Security.Cryptography;
using Backend.K03.APPLICATION.Servico.IPasswordService;

namespace Backend.K02.INFRA.Servico.PasswordService;

public class PasswordHashService : IPasswordHash
{
     public Task<(string Hash, string Salt)> HashAsync(string senha)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, 100000, HashAlgorithmName.SHA512, 64);

        return Task.FromResult((Convert.ToBase64String(hash), Convert.ToBase64String(salt)));
    }
}
