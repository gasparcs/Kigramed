// using Backend.K03.APPLICATION.AgendamentoUseCase.Comand;
// using Backend.K03.APPLICATION.AgendamentoUseCase.Queries;
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
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.Comand;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.DTO;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.Queries;
using Backend.K03.APPLICATION.PagamentoUseCase.Comand;
using Backend.K03.APPLICATION.PagamentoUseCase.DTO;
using Backend.K03.APPLICATION.PagamentoUseCase.Queries;
using Backend.K03.APPLICATION.ServicosUseCase.Queries;
using Backend.K03.APPLICATION.SMSUseCase.Comand;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K03.APPLICATION.SMSUseCase.Queries;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS
{
    [Authorize(Roles = "Secretaria")]
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
        AtualizarConsulta atualizarconsultaServices,
        RemoverConsulta removerconsultaServices,

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
        AdicionarPagamento adicionarpagamentoServices,
        ListarPagamentoConsulta listarpagamentoconsultaServices,
        AdicionarPagamentoConsulta adicionarpagamentoconsultaServices,
        ListarSMS listarsmsServices,
        AdicionarSMS adicionarsmsServices,

        ListarMedicos listarmedicosServices,

        CriarConsulta criarConsulta,
        ConfirmarConsulta confirmarConsulta,
        CancelarConsulta cancelarConsulta,
        ValidarPagamentoConsulta validarPagamentoConsulta,
        RejeitarComprovativoConsulta rejeitarComprovativoConsulta,
        ListarConsultasPendentes listarConsultasPendentes,
        Backend.K02.INFRA.Data.KigramedDbContext context
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

        [HttpPut("consulta/{id}")]
        public async Task<IActionResult> AtualizarConsulta(int id, AtualizarConsultaDTO dto)
        {
            if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

            if (id != dto.IdConsulta)
            return StatusCode(400, "ID da consulta não corresponde");

            dto.IdConsulta = id;
            var resposta = await atualizarconsultaServices.ExecuteAsync(dto);
            if (resposta == "consulta_finalizada")
                return StatusCode(409, "Esta consulta já foi finalizada e não pode ser editada.");
            return resposta.Contains("sucesso") ? StatusCode(200, resposta) : StatusCode(400, resposta);
        }

        [HttpDelete("consulta/{id}")]
        public async Task<IActionResult> RemoverConsulta(int id)
        {
            var resposta = await removerconsultaServices.ExecuteAsync(id);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
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

         [AllowAnonymous]
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

         [AllowAnonymous]
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

        [HttpPost("pagamento")]
        public async Task<IActionResult> AdicionarPagamentos(AdicionarPagamentoDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpagamentoServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }

        [HttpGet("pagamentoconsulta")]
        public async Task<IActionResult> ListarPagamentoConsulta()
        {
            var resposta = await listarpagamentoconsultaServices.ExecuteAsync();
            return Ok(resposta);
        }

        [HttpPost("pagamentoconsulta")]
        public async Task<IActionResult> AdicionarPagamentoConsulta(AdicionarPagamentoConsultaDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpagamentoconsultaServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }

        [HttpGet("SMS")]
        public async Task<IActionResult> ListarSMS()
        {
            var resposta = await listarsmsServices.ExecuteAsync();
            return Ok(resposta);
        }

        [HttpPost("SMS")]
        public async Task<IActionResult> AdicionarSMS(AdicionarSMSDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarsmsServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }

        //----------------médicos------------//
        [HttpGet("medicos")]
         public async Task<IActionResult> Listarmedicos()
        {
            var resposta = await listarmedicosServices.ExecuteAsync();
            return Ok(resposta);
        }

    /// Lista todas as consultas pendentes
    [AllowAnonymous]
    [HttpGet("pedidos")]
    public async Task<IActionResult> ListarPedidos()
    {
        var resposta = await listarConsultasPendentes.ExecuteAsync();
        return Ok(new { mensagem = "sucesso", dados = resposta });
    }

    /// Confirma horário — reserva o horário e inicia prazo de 30 minutos
    [AllowAnonymous]
    [HttpPut("{id}/confirmar")]
    public async Task<IActionResult> Confirmar(int id)
    {
        var resposta = await confirmarConsulta.ExecuteAsync(id);
        return resposta switch
        {
            "sucesso"  => Ok(new { mensagem = "Horário confirmado. SMS enviado ao cliente com dados bancários. Prazo de pagamento: 30 minutos." }),
            "nao_encontrado" => StatusCode(404, new { mensagem = "Pedido não encontrado." }),
            "conflito" => StatusCode(409, new { mensagem = "⚠️ Conflito de horário! Este horário já foi reservado para outro paciente. Cancele este pedido e sugira outro horário ao cliente." }),
            var s when s.StartsWith("estado_invalido_confirmar:") => StatusCode(409, new { mensagem = $"Não é possível confirmar pedido no estado atual: {s.Split(':', 2)[1]}." }),
            _          => StatusCode(400, new { mensagem = resposta })
        };
    }

    /// Cancela pedido e liberta horário
    [HttpPut("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resposta = await cancelarConsulta.ExecuteAsync(id);
        return resposta.Contains("sucesso")
            ? Ok(new { mensagem = "Pedido cancelado. SMS enviado ao cliente." })
            : StatusCode(404, new { mensagem = resposta });
    }

    /// Valida comprovativo e cria consulta oficial
    [HttpPut("{id}/validar")]
    public async Task<IActionResult> Validar(int id)
    {
        var resposta = await validarPagamentoConsulta.ExecuteAsync(id);
        if (resposta == "sucesso")
            return Ok(new { mensagem = "Pagamento validado. Consulta registada com sucesso. SMS de confirmação enviado ao cliente." });
        else if (resposta == "erro_sms")
            return Ok(new { mensagem = "Pagamento validado. Consulta registada com sucesso. Erro ao enviar SMS de confirmação." });
        else if (resposta == "nao_encontrado")
            return StatusCode(404, new { mensagem = "Pedido não encontrado." });
        else if (resposta == "prazo_expirado")
            return StatusCode(400, new { mensagem = "O prazo de pagamento expirou. O pedido foi cancelado." });
        else if (resposta.StartsWith("estado_invalido_validar:"))
            return StatusCode(409, new { mensagem = $"Não é possível validar pedido no estado atual: {resposta.Split(':', 2)[1]}." });
        else
            return StatusCode(400, new { mensagem = resposta });
    }

    /// Rejeita comprovativo — cliente tem de reenviar
    [HttpPut("{id}/rejeitar")]
    public async Task<IActionResult> Rejeitar(int id)
    {
        var resposta = await rejeitarComprovativoConsulta.ExecuteAsync(id);
        return resposta.Contains("sucesso")
            ? Ok(new { mensagem = "Comprovativo rejeitado. SMS enviado ao cliente a pedir novo comprovativo." })
            : StatusCode(404, new { mensagem = resposta });
    }

    /// Ver comprovativo (imagem ou PDF)
    [AllowAnonymous]
    [HttpGet("{id}/comprovativo")]
    public async Task<IActionResult> VerComprovativo(int id)
    {
        var consulta = await context.Tabelatb15_consulta.FindAsync(id);
        if (consulta is null || string.IsNullOrEmpty(consulta.CaminhoComprovativo))
            return StatusCode(404, new { mensagem = "Comprovativo não encontrado." });

        var caminho = Path.Combine(Directory.GetCurrentDirectory(), consulta.CaminhoComprovativo);
        if (!System.IO.File.Exists(caminho))
            return StatusCode(404, new { mensagem = "Ficheiro não encontrado no servidor." });

        var ext  = Path.GetExtension(caminho).ToLower();
        var mime = ext == ".pdf" ? "application/pdf" : "image/jpeg";
        var bytes = await System.IO.File.ReadAllBytesAsync(caminho);
        return File(bytes, mime);
    }
    }
}