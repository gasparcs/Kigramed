using System;

namespace Backend.K03.APPLICATION.SMSUseCase.DTO;

public class LeituraSMSDTO
{
    public int SmsId { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; }

    public ClienteSmsDTO Cliente { get; set; } = null!;
}

public class ClienteSmsDTO
{
    public string ClienteNif { get; set; } = string.Empty;

    public string ClienteNome { get; set; } = string.Empty;
}