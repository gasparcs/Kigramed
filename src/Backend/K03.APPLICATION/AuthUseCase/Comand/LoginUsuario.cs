using System;
using System.Globalization;
using System.Text;
using Backend.K03.APPLICATION.AuthUseCase.DTO;
using Backend.K03.APPLICATION.Servico.IPasswordService;
using Backend.K03.APPLICATION.Servico.ITokenService;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.AuthUseCase.Comand;

public class LoginUsuario(
    IPegarAuthPeloNifRepository authRepository,
    IPasswordVerify passwordVerify,
    ITokenService tokenService)
{
    public async Task<LoginResponseDTO?> ExecuteAsync(LoginRequestDTO request)
    {
        var auth = await authRepository.PegarPeloNifAsync(request.Nif);
        if (auth == null)
            return null;

        bool senhaValida = await passwordVerify.VerifyAsync(request.Senha, auth.Senha_hash, auth.Senha_Salt);
        if (!senhaValida)
            return null;

        string normalizedRole = NormalizarPerfil(auth.Funcionario.Perfil?.Descricao);

        string telefone = auth.Funcionario.Contactos?
            .FirstOrDefault(c => c.TipoContacto?.Descricao
                .Equals("telefone", StringComparison.OrdinalIgnoreCase) == true)?
            .Contacto ?? string.Empty;

        string token = tokenService.GerarToken(
            nif: auth.Nif_funcionario,
            nome: auth.Funcionario.Nome,
            telefone: telefone,
            role: normalizedRole,
            perfil: auth.Funcionario.Perfil?.Descricao ?? "Sem Perfil"
        );

        return new LoginResponseDTO
        {
            Nif = auth.Nif_funcionario,
            Nome = auth.Funcionario.Nome,
            Perfil = auth.Funcionario.Perfil?.Descricao ?? "Sem Perfil",
            telefone = telefone,
            Token = token,
            role = normalizedRole
        };
    }

    private static string NormalizarPerfil(string? descricaoPerfil)
    {
        if (string.IsNullOrWhiteSpace(descricaoPerfil))
            return "Funcionario";

        string semAcentos = RemoverAcentos(descricaoPerfil).Trim().ToLowerInvariant();

        if (semAcentos.Contains("admin"))
            return "Admin";

        if (semAcentos.Contains("medico"))
            return "Medico";

        if (semAcentos.Contains("secretaria") || semAcentos.Contains("secretario"))
            return "Secretaria";

        return "Funcionario";
    }

    private static string RemoverAcentos(string texto)
    {
        string normalized = texto.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}