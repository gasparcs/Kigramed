using Backend.K03.APPLICATION.AuthUseCase.Comand;
using Backend.K03.APPLICATION.AuthUseCase.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(LoginUsuario loginUsuario) : ControllerBase
    {
         [HttpPost("login")]
          public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
            {
                if (!ModelState.IsValid) return StatusCode(400, ModelState);

               var resposta = await loginUsuario.ExecuteAsync(dto);

                if (resposta == null)
                return StatusCode(401, new { mensagem = "NIF ou senha inválidos" });

                
                return StatusCode(200, new
                {
                    mensagem = "Login realizado com sucesso",
                    
                    dados = resposta
                });
            }
    }
}
