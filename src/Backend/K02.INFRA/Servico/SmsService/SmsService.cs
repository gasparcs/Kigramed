using System;
using System.Text.Json.Serialization;
using Backend.K03.APPLICATION.Servico.ISmsService;

namespace Backend.K02.INFRA.Servico.SmsService;

public class SmsService(HttpClient httpClient) : ISmsService
{
    private const string Endpoint = "https://sms.gsaplatform.co/enviar/sms";

    public async Task<bool> EnviarAsync(string telefone, string mensagemTexto, string nif)
    {
        var request = new SmsRequest
        {
            Mensagem = [new SmsItem { Telefone = telefone, MensagemTexto = mensagemTexto }],
            Nif = nif
        };

        var response = await httpClient.PostAsJsonAsync(Endpoint, request);
        return response.IsSuccessStatusCode;
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

