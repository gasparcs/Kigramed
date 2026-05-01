using System;
using Backend.K02.INFRA.Repository.EstadoConsulta;
using Backend.K03.APPLICATION.ClienteUseCase.comand;
using Backend.K03.APPLICATION.ClienteUseCase.DTO;
using Backend.K03.APPLICATION.ClienteUseCase.Queries;
using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K03.APPLICATION.ConsultaUseCase.Queries;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;
using Backend.K03.APPLICATION.EspecialidadeUseCase.DTO;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.EstadoConsultaUseCase.Queries;
using Backend.K03.APPLICATION.FuncionarioUseCase.Comand;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K03.APPLICATION.FuncionarioUseCase.Queries;
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
using Backend.K03.APPLICATION.PerfilUseCase.Queries;
using Backend.K03.APPLICATION.ServicosUseCase.Comand;
using Backend.K03.APPLICATION.ServicosUseCase.DTO;
using Backend.K03.APPLICATION.ServicosUseCase.Queries;
using Backend.K03.APPLICATION.SMSUseCase.Comand;
using Backend.K03.APPLICATION.SMSUseCase.DTO;
using Backend.K03.APPLICATION.SMSUseCase.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.K01.CONTROLLERS;
   
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController (AdicionarCliente adicionarclienteServices,
      AtualizarCliente atualizarclienteServices,
      ListarClientes listarclienteServices, 
      RemoverCliente removerclienteServices,
      PegarClientePeloNif pegarnifclienteServices,
      PegarClientePeloTexto pegartextoclienteServices,

      AdicionarConsulta adicionarconsultServices,
      ListarConsultas listarconsultaServices,
      AtualizarConsulta atualizarconsultaServices,
      RemoverConsulta removerconsultaServices,
      PegarConsultaPeloId pegarconsultapeloidServices,

      AdicionarEspecialidade adicionarespecialidadeServices,
      AtualizarEspecialidade atualizarespecialidadeServices,
      RemoverEspecialidade removerespecialidadeServices,
      ListarEspecialidade listarespecialidadesServices,
      PegarEspecialidadePeloId pegaridespecialidadeServices,
      PegarEspecialidadePeloTexto pegartextoespecialidadeServices,

      AdicionarFuncionarios adicionarfuncionarioServices,
      AtualizarFuncionario atualizarfuncionarioServices,
      RemoverFuncionario removerfuncionarioServices,
      ListarFuncionario listarfuncionariosServices,
      PegarFuncionaarioPeloNif pegarniffuncionarioServices,
      PegarFuncionarioPeloTexto pegartextofuncionarioServices,

      AdicionarPaciente adicionarpacientesServices,
      AtualizarPaciente atualizarpacienteServices,
      RemoverPaciente removerpacienteServices,
      ListarPacientes listarpacienteServices,
      PegarPacientePeloID pegaridpacienteServices,
      PegarPacientePeloTexto pegartextopacienteServices,

         ListarMedicos listarmedicosServices,

        ListarPerfis listarperfisServices,

        ListarEstadoConsulta listarestadosServices,

        ListarPagamentos listarpagamentosServices,
        AdicionarPagamento adicionarpagamentoServices,

        ListarPagamentoConsulta listarpagamentoconsultaServices,
        AdicionarPagamentoConsulta adicionarpagamentoconsultaServices,

        ListarSMS listarsmsServices,
        AdicionarSMS adicionarsmsServices,

        AdicionarServicos adicionarservicosServices,
        AtualizarServicos atualizarservicosServices,
        RemoverServico removerservicosServices,
        ListarServicos listarservicosServices,
        PegarServicoPeloId pegarservicosidServices,
        PegarServicoPeloTexto pegarservicostextoServices

      )
      : ControllerBase
    {
       
        // ------------ Cliente -----------//
        //método adicionar
        [HttpPost("cliente")]
        public async Task<IActionResult> AdicionarCliente(AdicionarClienteDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarclienteServices.ExecuteAsync(dto);
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

              var resposta = await atualizarclienteServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(200, resposta)
            : StatusCode(404, resposta);
        }
        //método listar
        [HttpGet("cliente")]
        public async Task<IActionResult> ListarCliente()
        {
            var resposta = await listarclienteServices.ExecuteAsync();
            return Ok(resposta);
        }

        //método remover
        [HttpDelete("cliente/{nif}")]
        public async Task<IActionResult> RemoverCliente(string nif)
        {
            var resposta = await removerclienteServices.ExecuteAsync(nif);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        //método pegar pelo nif
        [HttpGet("cliente/nif/{nif}")]
        public async Task<IActionResult> PegarClientePeloNif(string nif)
        {
            var resposta = await pegarnifclienteServices.ExecuteAsync(nif);
            return resposta is null ? StatusCode(404, "Cliente não encontrado"):
            Ok(resposta);
        }

        //método pegar pelo texto
        [HttpGet("cliente/texto/{texto}")]
        public async Task<IActionResult> PegarClientePeloTexto(string texto)
        {
            var resposta = await pegartextoclienteServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhum cliente encontrado") :
            Ok(resposta);
        }

        //  ------------- Consulta ------------//

        [HttpPost("consulta")]
        public async Task<IActionResult> AdicionarConsulta(AdicionarConsultaDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarconsultServices.ExecuteAsync(dto);
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
            return resposta.Contains("sucesso") ? StatusCode(200, resposta) : StatusCode(400, resposta);
        }

        [HttpDelete("consulta/{id}")]
        public async Task<IActionResult> RemoverConsulta(int id)
        {
            var resposta = await removerconsultaServices.ExecuteAsync(id);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        [HttpGet("consulta/id/{id}")]
        public async Task<IActionResult> PegarConsultaPeloId(int id)
        {
            var resposta = await pegarconsultapeloidServices.ExecuteAsync(id);
            return resposta is null ? StatusCode(404, "Especialidade não encontrada"):
            Ok(resposta);
        }

        //--------------- Especialidade -----------//

        [HttpPost("especialidade")]
        public async Task<IActionResult> AdicionarEspecialidade(AdicionarEspecialidadeDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarespecialidadeServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso") ? StatusCode(201, resposta) :
            StatusCode(500, resposta); 
        }

        [HttpPut("especialidade/{id}")]
        public async Task<IActionResult> AtualizarEspecialidade(int id, AtualizarEspecialidadeDTO dto)
        {
            if (!ModelState.IsValid)
            return StatusCode(400, ModelState);
            dto.EspecialidadeId = id;
            var resposta = await atualizarespecialidadeServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(500, resposta);
        }

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

         [HttpDelete("especialidade/{id}")]
        public async Task<IActionResult> RemoverEspecialidade(int id)
        {
            var resposta = await removerespecialidadeServices.ExecuteAsync(id);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        //--------------- funcionario --------------//
      
        [HttpPost("funcionario")]
        public async Task<IActionResult> AdicionarFuncionario(AdicionarFuncionarioDTO dto)
        {
            if(!ModelState.IsValid)
            
            return StatusCode(400, ModelState);

            var resposta = await adicionarfuncionarioServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta) :
            StatusCode(500, resposta);            
        }

        [HttpGet("funcionario")]
        public async Task<IActionResult> ListarFuncionarios()
        {
             var resposta = await listarfuncionariosServices.ExecuteAsync();
             return Ok(resposta);
        }

        [HttpGet("funcionario/nif/{nif}")] 
        public async Task<IActionResult> PegarFuncionarioPeloNif(string nif)
        {
            var resposta = await pegarniffuncionarioServices.ExecuteAsync(nif);
            return resposta is null? StatusCode(404, "Funcionário não encontrado"):
            Ok(resposta);
        } 

        [HttpGet("funcionario/texto/{texto}")]
        public async Task<IActionResult> PegarFuncionarioPeloTexto(string texto)
        {
            var resposta = await pegartextofuncionarioServices.ExecuteAsync(texto);
            return resposta is null? StatusCode(404, "Nenhum funcionário encontrado"):
            Ok(resposta); 
        } 

        [HttpPut("funcionario/{nif}")]
         public async Task<IActionResult> AtualizarFuncionario(string nif, AtualizarFuncionarioDTO dto)
        {
            if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

            dto.FuncionarioNif = nif;

            var resposta = await atualizarfuncionarioServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(200, resposta)
            : StatusCode(404, resposta);
        }

        [HttpDelete("funcionario/{nif}")]
        public async Task<IActionResult> RemoverFuncionario(string nif)
        {
            var resposta = await removerfuncionarioServices.ExecuteAsync(nif);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        //------------------ paciente ----------------//

         [HttpPost("paciente")]
        public async Task<IActionResult> AdicionarPaciente(AdicionarPacienteDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpacientesServices.ExecuteAsync(dto);
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
             var resposta = await listarpacienteServices.ExecuteAsync();
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

        //-------------- perfil -----------------//

        [HttpGet("perfil")]
        public async Task<IActionResult> ListarPerfis()
        {
            var resposta = await listarperfisServices.ExecuteAsync();
            return Ok(resposta);
        }

        //---------------estado---------------//
        [HttpGet("estados")]
         public async Task<IActionResult> ListarEstados()
        {
            var resposta = await listarestadosServices.ExecuteAsync();
            return Ok(resposta);
        }
        
        //---------------------pagamento--------------//
         [HttpPost("pagamento")]
        public async Task<IActionResult> AdicionarPagamentos(AdicionarPagamentoDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpagamentoServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }
        
        [HttpGet("pagamento")]
        public async Task<IActionResult> ListarPagamentos()
        {
            var resposta = await listarpagamentosServices.ExecuteAsync();
            return Ok(resposta);
        }

        //----------------medicos------------//
        [HttpGet("medicos")]
         public async Task<IActionResult> Listarmedicos()
        {
            var resposta = await listarmedicosServices.ExecuteAsync();
            return Ok(resposta);
        }

        //-------------- serviço --------------//

        [HttpPost("servicos")]
        public async Task<IActionResult> AdicionarServicos(AdicionarServicosDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarservicosServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }
        
        [HttpPut("servicos/{id}")]
        public async Task<IActionResult> AtualizarServico(int id, AtualizarServicosDTO dto)
        {
            if (!ModelState.IsValid)
            return StatusCode(400, ModelState);

            dto.ServicoId = id;    
            var resposta = await atualizarservicosServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(500, resposta);
        }

        [HttpDelete("servicos/{id}")]   
        public async Task<IActionResult> RemoverServicos(int id)
        {
            var resposta = await removerservicosServices.ExecuteAsync(id);
            return resposta.Contains("sucesso") ? StatusCode(200, resposta):
            StatusCode(404, resposta);
        }

        [HttpGet("servicos")] 
        public async Task<IActionResult> ListarServicos()
        {
            var resposta = await listarservicosServices.ExecuteAsync();
            return Ok(resposta);
        }

         [HttpGet("servicos/id/{id}")]
        public async Task<IActionResult> PegarServicoPeloId(int id)
        {
            var resposta = await pegarservicosidServices.ExecuteAsync(id);
            return resposta is null ? StatusCode(404, "Servico não encontrado"):
            Ok(resposta);
        }

         [HttpGet("servico/texto/{texto}")]
        public async Task<IActionResult> PegarServicosPeloTexto(string texto)
        {
            var resposta = await pegarservicostextoServices.ExecuteAsync(texto);
            return resposta is null ? StatusCode(404, "Nenhum servico encontrado com o texto fornecido"):
            Ok(resposta);
        }

          //---------------------pagamento consulta--------------//
         [HttpPost("pagamentoconsulta")]
        public async Task<IActionResult> AdicionarPagamentoConsulta(AdicionarPagamentoConsultaDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarpagamentoconsultaServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }
        
        [HttpGet("pagamentoconsulta")]
        public async Task<IActionResult> ListarPagamentoConsulta()
        {
            var resposta = await listarpagamentoconsultaServices.ExecuteAsync();
            return Ok(resposta);
        }

         //---------------------SMS--------------//
         [HttpPost("SMS")]
        public async Task<IActionResult> AdicionarSMS(AdicionarSMSDTO dto)
        {
            if(!ModelState.IsValid)
            return StatusCode(400, ModelState);
            var resposta = await adicionarsmsServices.ExecuteAsync(dto);
            return resposta.Contains("sucesso")? StatusCode(201, resposta): 
            StatusCode(500, resposta);
        }
        
        [HttpGet("SMS")]
        public async Task<IActionResult> ListarSMS()
        {
            var resposta = await listarsmsServices.ExecuteAsync();
            return Ok(resposta);
        }

    }


