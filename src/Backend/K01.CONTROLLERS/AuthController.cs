using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (
        LoginUsuario loginUsuario
    ): ControllerBase
    {
         /// Realiza o login de um usuário e retorna um token JWT
         [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Validar estado do modelo
            if (!ModelState.IsValid)
                return StatusCode(400, ModelState);

            // Executar o caso de uso de login
            var resposta = await loginUsuario.ExecuteAsync(request);

            // Validar se o login foi bem-sucedido
            if (resposta == null)
                return StatusCode(401, new { mensagem = "NIF ou senha inválidos" });

            // Retornar sucesso com os dados do usuário e token
            return StatusCode(200, new
            {
                mensagem = "Login realizado com sucesso",
                dados = resposta
            });
        }

        
        /// Endpoint de teste para verificar se a API está respondendo
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { mensagem = "API de Autenticação está operacional" });
        }
    }
}
