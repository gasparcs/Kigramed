using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Backend.K03.APPLICATION.Servico.ITokenService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend.K02.INFRA.Servico.AuthService;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GerarToken(string nif, string nome, string telefone, string role, string perfil)
    {
        string chave = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");

        string issuer   = configuration["Jwt:Issuer"]   ?? "Kigramed.API";
        string audience = configuration["Jwt:Audience"] ?? "Kigramed.Client";

        int expiraMinutos = 120;
        if (int.TryParse(configuration["Jwt:ExpiraMinutos"], out int valor))
            expiraMinutos = valor;

        // CORRIGIDO: ClaimTypes.Role é o que o ASP.NET Core lê no [Authorize(Roles="...")]
        // O valor de 'role' vem do NormalizarPerfil() no LoginUsuario:
        //   "Admin" | "Medico" | "Secretaria" | "Funcionario"
        // Estes valores têm de coincidir EXACTAMENTE com os [Authorize(Roles="...")] nos controllers.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        nif),
            new(JwtRegisteredClaimNames.UniqueName, nome),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
            new("telefone",                          telefone),
            new(ClaimTypes.Role,                     role),   // lido pelo [Authorize(Roles="...")]
            new("perfil",                            perfil), // descrição original (ex: "Médico Especialista")
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:            issuer,
            audience:          audience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddMinutes(expiraMinutos),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
