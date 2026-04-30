using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.K03.APPLICATION.SMSUseCase.DTO;

public class AdicionarSMSDTO
{
    [Required(ErrorMessage = "A mensagem é obrigatória")]
    public string SMSMensagem { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o funcionario")]
    public string SMSNif_funcionario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o cliente")]
    public string SMSId_cliente { get; set; } = string.Empty;
}
