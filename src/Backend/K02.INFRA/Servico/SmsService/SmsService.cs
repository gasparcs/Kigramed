using System;
using System.Linq;
using System.Text.Json.Serialization;
using Backend.K03.APPLICATION.Servico.ISmsService;

namespace Backend.K02.INFRA.Servico.SmsService;

public class SmsService(HttpClient httpClient) : ISmsService
{
    private const string Endpoint = "https://sms.gsaplatform.co/enviar/sms";

    public async Task<bool> EnviarAsync(string telefone, string mensagemTexto, string nif)
    {
        var telefoneNormalizado = NormalizarTelefone(telefone);
        if (string.IsNullOrWhiteSpace(telefoneNormalizado) || string.IsNullOrWhiteSpace(mensagemTexto))
            return false;

        var request = new SmsRequest
        {
            Mensagem = [new SmsItem { Telefone = telefoneNormalizado, MensagemTexto = mensagemTexto }],
            Nif = nif
        };

        var response = await httpClient.PostAsJsonAsync(Endpoint, request);
        return response.IsSuccessStatusCode;
    }

    private static string NormalizarTelefone(string telefone)
    {
        var digits = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits))
            return string.Empty;

        if (digits.StartsWith("244") && digits.Length == 12)
            return digits;

        if (digits.Length == 9)
            return "244" + digits;

        if (digits.StartsWith("0") && digits.Length == 10)
            return "244" + digits[1..];

        return digits;
    }

    private sealed class SmsRequest
    {
        [JsonPropertyName("mensagem")]
        public List<SmsItem> Mensagem { get; set; } = [];

        [JsonPropertyName("nif")]
        public string Nif { get; set; } = string.Empty;
    }

    private sealed class SmsItem
    {
        [JsonPropertyName("telefone")]
        public string Telefone { get; set; } = string.Empty;

        [JsonPropertyName("mensagemTexto")]
        public string MensagemTexto { get; set; } = string.Empty;
    }
}

