using Backend.K03.APPLICATION.ClienteUseCase.comand;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K03.APPLICATION.ClienteUseCase.Queries;
using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K03.APPLICATION.ConsultaUseCase.Queries;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.EstadoConsultaUseCase.Queries;
using Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.PacienteUseCase.Comand;
using Backend.K03.APPLICATION.PacienteUseCase.DTO;
using Backend.K03.APPLICATION.PacienteUseCase.Queries;
using Backend.K03.APPLICATION.PagamentoUseCase.Queries;
using Backend.K03.APPLICATION.ServicosUseCase.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Secretaria")]
     
    public class SecretariaController (
        AdicionarCliente adicionarServices,
        AtualizarCliente atualizarServices,
        ListarClientes listarServices,
        RemoverCliente removerServices,
        PegarClientePeloNif pegarnifServices,
        PegarClientePeloTexto pegartextoServices,

        AdicionarConsulta adicionarconsultaServices,
        ListarConsultas listarconsultaServices,

        AdicionarPaciente adicionarpacienteServices,
        AtualizarPaciente atualizarpacienteServices,
        RemoverPaciente removerpacienteServices,
        ListarPacientes listarpacientesServices,
        PegarPacientePeloID pegaridpacienteServices,
        PegarPacientePeloTexto pegartextopacienteServices,

        ListarServicos listarservicosServices,
        PegarServicoPeloId pegarservicoidServices,
        PegarServicoPeloTexto pegarservicotextoServices,

        ListarEspecialidade listarespecialidadesServices,
        PegarEspecialidadePeloId pegaridespecialidadeServices,
        PegarEspecialidadePeloTexto pegartextoespecialidadeServices,

        ListarEstadoConsulta listarestadosServices,
        ListarPagamentos listarpagamentosServices,

        ListarMedicos listarmedicosServices
    )
    : ControllerBase
    {
        // ------------ cliente -----------//

        //método adicionar
        [HttpPost("cliente")]
        public async Task<IActionResult> AdicionarCliente(AdicionarClienteDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }

        //método actualizar
        [HttpPut("cliente/{nif}")]
        public async Task<IActionResult> AtualizarCliente(string nif, AtualizarClienteDTO dto)
        {
            if (!ModelState.IsValid) 
            return StatusCode(400, ModelState);
            dto.Nif_cliente = nif;

            var resposta = await atualizarServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(200, resposta)
            : StatusCode(404, resposta);
        }

        //método listar
        [HttpGet("cliente")]
        public async Task<IActionResult> ListarCliente()
        {
            var resposta = await listarServices.ExecuteAsync();
            return Ok(resposta);
        }

        //método remover
        [HttpDelete("cliente/{nif}")]
        public async Task<IActionResult> RemoverCliente(string nif)
        {
            var resposta = await removerServices.ExecuteAsync(nif);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        //método pegar pelo nif
        [HttpGet("cliente/nif/{nif}")]
        public async Task<IActionResult> PegarClientePeloNif(string nif)
        {
            var resposta = await pegarnifServices.ExecuteAsync(nif);
            return resposta is null ? StatusCode(404, "Cliente não encontrado"):
            Ok(resposta);
        }

        //método pegar pelo texto
        [HttpGet("cliente/texto/{texto}")]
        public async Task<IActionResult> PegarClientePeloTexto(string texto)
        {
            var resposta = await pegartextoServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhum cliente encontrado") :
            Ok(resposta);
        }

        
         //  ------------- consulta ------------//

        [HttpPost("consulta")]
        public async Task<IActionResult> AdicionarConsulta(AdicionarConsultaDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarconsultaServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }

        [HttpGet("consulta")]
        public async Task<IActionResult> ListarConsultas()
        {
            var resposta = await listarconsultaServices.ExecuteAsync();
            return Ok(resposta);
        }

         //------------------ paciente ----------------//

         [HttpPost("paciente")]
        public async Task<IActionResult> AdicionarPaciente(AdicionarPacienteDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpacienteServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta) :
            StatusCode(500, resposta);            
        }

         [HttpPut("paciente/{id}")]
        public async Task<IActionResult> AtualizarPaciente(int id, AtualizarPacienteDTO dto)
        {
            if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

            dto.IdPaciente = id;

            var resposta = await atualizarpacienteServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(200, resposta)
            : StatusCode(404, resposta);
        }

         [HttpDelete("paciente/{id}")]
        public async Task<IActionResult> RemoverPaciente(int id)
        {
            var resposta = await removerpacienteServices.ExecuteAsync(id);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        [HttpGet("paciente")]
        public async Task<IActionResult> ListarPacientes()
        {
             var resposta = await listarpacientesServices.ExecuteAsync();
             return Ok(resposta);
        }
        
        [HttpGet("paciente/id/{id}")]
        public async Task<IActionResult> PegarPacientePeloId(int id)
        {
            var resposta = await pegaridpacienteServices.ExecuteAsync(id);
            return resposta is null ? StatusCode(404, "Paciente não encontrado"):
            Ok(resposta);
        }

         [HttpGet("paciente/texto/{texto}")]
        public async Task<IActionResult> PegarPacientePeloTexto(string texto)
        {
            var resposta = await pegartextopacienteServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhum paciente encontrado com o texto fornecido"):
            Ok(resposta);
        }

        //------------ serviço -------------//

         [HttpGet("servico")] 
        public async Task<IActionResult> ListarServicos()
        {
            var resposta = await listarservicosServices.ExecuteAsync();
            return Ok(resposta);
        }

         [HttpGet("servico/id/{id}")]
        public async Task<IActionResult> PegarServicoPeloId(int id)
        {
            var resposta = await pegarservicoidServices.ExecuteAsync(id);
            return resposta is null ? StatusCode(404, "Serviço não encontrado"):
            Ok(resposta);
        }

         [HttpGet("servico/texto/{texto}")]
        public async Task<IActionResult> PegarServicoPeloTexto(string texto)
        {
            var resposta = await pegarservicotextoServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhum serviço encontrado com o texto fornecido"):
            Ok(resposta);
        }

        //------------- especialidade -------------//

         [HttpGet("especialidade")] 
        public async Task<IActionResult> ListarEspecialidades()
        {
            var resposta = await listarespecialidadesServices.ExecuteAsync();
            return Ok(resposta);
        }

        [HttpGet("especialidade/id/{id}")]
        public async Task<IActionResult> PegarEspecialidadePeloId(int id)
        {
            var resposta = await pegaridespecialidadeServices.ExecuteAsync(id);
            return resposta is null ? StatusCode(404, "Especialidade não encontrada"):
            Ok(resposta);
        }

        [HttpGet("especialidade/texto/{texto}")] 
        public async Task<IActionResult> PegarEspecialidadePeloTexto(string texto)
        {
            var resposta = await pegartextoespecialidadeServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhuma especialidade encontrada com o texto fornecido"):
            Ok(resposta);
        }

        //---------------estado---------------//
         public async Task<IActionResult> ListarEstados()
        {
            var resposta = await listarestadosServices.ExecuteAsync();
            return Ok(resposta);
        }
        
        [HttpGet("pagamento")]
        public async Task<IActionResult> ListarPagamentos()
        {
            var resposta = await listarpagamentosServices.ExecuteAsync();
            return Ok(resposta);
        }

        //----------------médicos------------//
         public async Task<IActionResult> Listarmedicos()
        {
            var resposta = await listarmedicosServices.ExecuteAsync();
            return Ok(resposta);
        }
    }
}
