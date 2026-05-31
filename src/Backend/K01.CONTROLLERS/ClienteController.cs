using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController(
        CriarConsulta criarConsulta,
        CancelarConsultaCliente cancelarConsultaCliente,
        KigramedDbContext context) : ControllerBase
    { 
        // ─── PÚBLICO ────────────────────────────────────────────────────────────

        /// Cliente cria pedido de agendamento
        [AllowAnonymous]
        [HttpPost("pedido")]
        public async Task<IActionResult> CriarPedido([FromBody] CriarConsultaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resposta = await criarConsulta.ExecuteAsync(dto);

            return resposta.mensagem.Contains("sucesso")
                ? StatusCode(201, new { numeroPedido = resposta.numeroPedido, mensagem = resposta.mensagem })
                : StatusCode(400, new { mensagem = resposta.mensagem });
        }

        [AllowAnonymous]
        [HttpGet("pedido/{numeroPedido}/estado")]
        public async Task<IActionResult> ConsultarEstado(string numeroPedido)
        {
            var consulta = await context.Tabelatb15_consulta
                .Include(c => c.EstadoConsulta)
                .Include(c => c.Servico).ThenInclude(s => s.Especialidade)
                .FirstOrDefaultAsync(c => c.NumeroPedido == numeroPedido);

            if (consulta is null)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            return Ok(new {
                numeroPedido = consulta.NumeroPedido,
                estado = consulta.EstadoConsulta.Descricao,
                horario = consulta.Data_consulta,
                especialidade = consulta.Servico?.Especialidade?.Nome ?? "—",
                servico = consulta.Servico?.Nome ?? "—",
                prazoPagamento = consulta.PrazoPagamento,
                observacoes = consulta.Observacoes
            });
        }

        [AllowAnonymous]
        [HttpGet("nif/{nif}")]
        public async Task<IActionResult> BuscarClientePorNif(string nif)
        {
            if (string.IsNullOrWhiteSpace(nif))
                return BadRequest(new { mensagem = "NIF inválido." });

            var cliente = await context.Tabelatb09_cliente
                .Where(c => c.Nif_cliente == nif.Trim())
                .Select(c => new { nif = c.Nif_cliente, nome = c.Nome })
                .FirstOrDefaultAsync();

            if (cliente is null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [AllowAnonymous]
        [HttpGet("cliente-paciente")]
        public async Task<IActionResult> ListarClientePaciente()
        {
            var items = await context.Tabelatb11_cliente_paciente
                .Select(cp => new { id = cp.Id, descricao = cp.Descricao })
                .ToListAsync();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("genero")]
        public async Task<IActionResult> ListarGeneros()
        {
            var items = await context.Tabelatb10_genero
                .Select(g => new { id = g.Id, nome = g.Nome })
                .ToListAsync();
            return Ok(items);
        }

        /// Cliente envia comprovativo de pagamento
        [AllowAnonymous]
        [HttpPost("{numeroPedido}/comprovativo")]
        public async Task<IActionResult> EnviarComprovativo(
            string numeroPedido, 
            IFormFile ficheiro)
        {
            if (ficheiro is null || ficheiro.Length == 0)
                return BadRequest(new { mensagem = "Ficheiro inválido." });

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(ficheiro.FileName).ToLower();

            if (!extensoesPermitidas.Contains(ext))
                return BadRequest(new { mensagem = "Formato não permitido. Use JPG, PNG ou PDF." });

            if (ficheiro.Length > 5_000_000)
                return BadRequest(new { mensagem = "Ficheiro demasiado grande. Máximo 5MB." });

            var consulta = await context.Tabelatb15_consulta
                .Include(c => c.EstadoConsulta)
                .FirstOrDefaultAsync(c => c.NumeroPedido == numeroPedido);

            if (consulta is null)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            if (!string.Equals(consulta.EstadoConsulta.Descricao, "Aguarda Pagamento", 
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(409, new { mensagem = $"Não é possível enviar comprovativo. Estado actual: {consulta.EstadoConsulta.Descricao}." });

            // Guarda o ficheiro
            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "comprovativos");
            Directory.CreateDirectory(pasta);

            var nomeFicheiro = $"{numeroPedido}{ext}";
            var caminhoCompleto = Path.Combine(pasta, nomeFicheiro);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                await ficheiro.CopyToAsync(stream);

            // Actualiza a consulta
            var idComprovativoEnviado = await context.Tabelatb13_estado_consulta
                .Where(e => e.Descricao == "Comprovativo Enviado")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (idComprovativoEnviado == 0)
                return StatusCode(500, new { mensagem = "Estado 'Comprovativo Enviado' não configurado." });

            consulta.CaminhoComprovativo = Path.Combine("uploads", "comprovativos", nomeFicheiro);
            consulta.Id_estado_consulta  = idComprovativoEnviado;

            await context.SaveChangesAsync();

            return Ok(new { mensagem = "Comprovativo enviado com sucesso." });
        }

        [AllowAnonymous]
        [HttpPut("consulta/{numeroPedido}/cancelar")]
        public async Task<IActionResult> CancelarConsulta(string numeroPedido)
        {
            var resposta = await cancelarConsultaCliente.ExecuteAsync(numeroPedido);
            return resposta.Contains("sucesso")
                ? StatusCode(200, new { mensagem = resposta })
                : StatusCode(400, new { mensagem = resposta });
        }
    }
}
