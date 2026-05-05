using Backend.K03.APPLICATION.AgendamentoUseCase.Comand;
using Backend.K03.APPLICATION.AgendamentoUseCase.DTO;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController (

         CriarPedido criarPedido,
          IAgendamentoRepository repository): ControllerBase
    { 



         // ─── PÚBLICO ────────────────────────────────────────────────────────────

    /// Cliente cria pedido de agendamento
    [HttpPost("pedido")]
    public async Task<IActionResult> CriarPedido(CriarPedidoDTO dto)
    {
        if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

        var (numeroPedido, mensagem) = await criarPedido.ExecuteAsync(dto);

        if (!mensagem.Contains("sucesso"))
            return StatusCode(400, new { mensagem });

        return StatusCode(201, new
        {
            mensagem = "Pedido criado com sucesso! Guarde o número do seu pedido.",
            numeroPedido
        });
    }

    /// Cliente envia comprovativo de pagamento
    [HttpPost("{numeroPedido}/comprovativo")]
    public async Task<IActionResult> EnviarComprovativo(string numeroPedido, IFormFile ficheiro)
    {
        if (ficheiro is null || ficheiro.Length == 0)
            return StatusCode(400, new { mensagem = "Ficheiro inválido." });

        // Apenas PDF e imagens
        var extensoesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(ficheiro.FileName).ToLower();
        if (!extensoesPermitidas.Contains(ext))
            return StatusCode(400, new { mensagem = "Apenas PDF, JPG e PNG são permitidos." });

        // Tamanho máximo 5MB
        if (ficheiro.Length > 5_000_000)
            return StatusCode(400, new { mensagem = "O ficheiro não pode exceder 5MB." });

        var pedido = await repository.BuscarPorNumeroPedidoAsync(numeroPedido);
        if (pedido is null)
            return StatusCode(404, new { mensagem = "Pedido não encontrado." });

        if (pedido.Estado != "Aguarda Pagamento")
            return StatusCode(400, new { mensagem = $"Este pedido está com estado '{pedido.Estado}' e não aceita comprovativo." });

        // Verifica prazo
        if (pedido.PrazoPagamento.HasValue && pedido.PrazoPagamento < DateTime.UtcNow)
            return StatusCode(400, new { mensagem = "O prazo de pagamento de 2 horas expirou. O pedido foi cancelado." });

        // Guarda o ficheiro
        var pasta = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "comprovativos");
        Directory.CreateDirectory(pasta);
        var nomeFicheiro = $"{numeroPedido}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
        var caminho = Path.Combine(pasta, nomeFicheiro);

        await using var stream = new FileStream(caminho, FileMode.Create);
        await ficheiro.CopyToAsync(stream);

        pedido.Estado               = "Comprovativo Enviado";
        pedido.CaminhoComprovativo  = Path.Combine("uploads", "comprovativos", nomeFicheiro);
        await repository.AtualizarAsync(pedido);

        return Ok(new { mensagem = "Comprovativo enviado com sucesso! A secretaria irá validar em breve." });
    }

    /// Cliente consulta estado do pedido pelo número
    [HttpGet("pedido/{numeroPedido}/estado")]
    public async Task<IActionResult> ConsultarEstado(string numeroPedido)
    {
        var pedido = await repository.BuscarPorNumeroPedidoAsync(numeroPedido);
        if (pedido is null)
            return StatusCode(404, new { mensagem = "Pedido não encontrado." });

        return Ok(new
        {
            numeroPedido  = pedido.NumeroPedido,
            estado        = pedido.Estado,
            horario       = pedido.HorarioPreferencial,
            especialidade = pedido.Especialidade?.Nome,
            prazoPagamento = pedido.PrazoPagamento
        });
    }
    }
}
