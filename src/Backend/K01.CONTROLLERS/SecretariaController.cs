using Backend.K03.APPLICATION.AgendamentoUseCase.Comand;
using Backend.K03.APPLICATION.AgendamentoUseCase.Queries;
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
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    
    [Route("api/[controller]")]
    [ApiController]
     
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

        ListarMedicos listarmedicosServices,

         ConfirmarPedido confirmarPedido,
    CancelarPedido cancelarPedido,
    ValidarPagamento validarPagamento,
    RejeitarComprovativo rejeitarComprovativo,
    ListarPedidos listarPedidos,
    IAgendamentoRepository repository
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
         [HttpGet("estado")]
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
        [HttpGet("medicos")]
         public async Task<IActionResult> Listarmedicos()
        {
            var resposta = await listarmedicosServices.ExecuteAsync();
            return Ok(resposta);
        }

        /// Lista todos os pedidos
    [AllowAnonymous]
    [HttpGet("pedidos")]
    public async Task<IActionResult> ListarPedidos()
    {
        var resposta = await listarPedidos.ExecuteAsync();
        return Ok(new { mensagem = "sucesso", dados = resposta });
    }

    /// Confirma horário — reserva o horário e inicia prazo de 2 horas
    [AllowAnonymous]
    [HttpPut("{id}/confirmar")]
    public async Task<IActionResult> Confirmar(int id)
    {
        var resposta = await confirmarPedido.ExecuteAsync(id);
        return resposta switch
        {
            "sucesso"  => Ok(new { mensagem = "Horário confirmado. SMS enviado ao cliente com dados bancários. Prazo de pagamento: 2 horas." }),
            "conflito" => StatusCode(409, new { mensagem = "⚠️ Conflito de horário! Este horário já foi reservado para outro paciente. Cancele este pedido e sugira outro horário ao cliente." }),
            _          => StatusCode(400, new { mensagem = resposta })
        };
    }

    /// Cancela pedido e liberta horário

    [HttpPut("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resposta = await cancelarPedido.ExecuteAsync(id);
        return resposta.Contains("sucesso")
            ? Ok(new { mensagem = "Pedido cancelado. SMS enviado ao cliente." })
            : StatusCode(404, new { mensagem = resposta });
    }

    /// Valida comprovativo e cria consulta oficial
    [HttpPut("{id}/validar")]
    public async Task<IActionResult> Validar(int id)
    {
        var resposta = await validarPagamento.ExecuteAsync(id);
        if (resposta == "sucesso")
            return Ok(new { mensagem = "Pagamento validado. Consulta registada com sucesso. SMS de confirmação enviado ao cliente." });
        else if (resposta == "erro_sms")
            return Ok(new { mensagem = "Pagamento validado. Consulta registada com sucesso. Erro ao enviar SMS de confirmação." });
        else
            return StatusCode(400, new { mensagem = resposta });
    }

    /// Rejeita comprovativo — cliente tem de reenviar
   
    [HttpPut("{id}/rejeitar")]
    public async Task<IActionResult> Rejeitar(int id)
    {
        var resposta = await rejeitarComprovativo.ExecuteAsync(id);
        return resposta.Contains("sucesso")
            ? Ok(new { mensagem = "Comprovativo rejeitado. SMS enviado ao cliente a pedir novo comprovativo." })
            : StatusCode(404, new { mensagem = resposta });
    }

    /// Ver comprovativo (imagem ou PDF)
    [HttpGet("{id}/comprovativo")]
    public async Task<IActionResult> VerComprovativo(int id)
    {
        var pedido = await repository.BuscarPorIdAsync(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.CaminhoComprovativo))
            return StatusCode(404, new { mensagem = "Comprovativo não encontrado." });

        var caminho = Path.Combine(Directory.GetCurrentDirectory(), pedido.CaminhoComprovativo);
        if (!System.IO.File.Exists(caminho))
            return StatusCode(404, new { mensagem = "Ficheiro não encontrado no servidor." });

        var ext  = Path.GetExtension(caminho).ToLower();
        var mime = ext == ".pdf" ? "application/pdf" : "image/jpeg";
        var bytes = await System.IO.File.ReadAllBytesAsync(caminho);
        return File(bytes, mime);
    }
    }
}
