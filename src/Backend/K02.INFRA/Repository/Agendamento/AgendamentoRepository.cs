using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D09.Genero;
using Backend.K04.DOMAIN.D11.ClientePaciente;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.D21.Agendamento;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Agendamento;

public class AgendamentoRepository(KigramedDbContext context) : IAgendamentoRepository
{
    public async Task<bool> AtualizarAsync(AgendamentoModel pedido)
    {
        context.Tabelatb21_agendamento.Update(pedido);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<AgendamentoModel?> BuscarPorIdAsync(int id)
    {
       return await context.Tabelatb21_agendamento
            .Include(a => a.Especialidade)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AgendamentoModel?> BuscarPorNumeroPedidoAsync(string numeroPedido)
    {
         return await context.Tabelatb21_agendamento
            .Include(a => a.Especialidade)
            .FirstOrDefaultAsync(a => a.NumeroPedido == numeroPedido);
    }

    public async Task<AgendamentoModel> CriarPedidoAsync(AgendamentoModel pedido)
    {
        context.Tabelatb21_agendamento.Add(pedido);
        await context.SaveChangesAsync();
        return pedido;
    }

    public async Task<bool> CriarConsultaPagamentoAsync(AgendamentoModel pedido)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var cliente = await context.Tabelatb09_cliente.FirstOrDefaultAsync();
            var genero = await context.Tabelatb10_genero.FirstOrDefaultAsync();
            var clientePaciente = await context.Tabelatb11_cliente_paciente.FirstOrDefaultAsync();
            var estadoConsulta = await context.Tabelatb13_estado_consulta.FirstOrDefaultAsync();
            var medicoEspecialidade = await context.Tabelatb07_medico_especialidade.FirstOrDefaultAsync();
            var funcionario = await context.Tabelatb02_funcionario.FirstOrDefaultAsync();

            if (cliente is null || genero is null || clientePaciente is null || estadoConsulta is null || medicoEspecialidade is null || funcionario is null)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var paciente = new PacienteModel
            {
                Nif_cliente = cliente.Nif_cliente,
                Nome = pedido.NomeCliente,
                Data_nascimento = DateTime.UtcNow.Date,
                Id_genero = genero.Id,
                Id_cliente_paciente = clientePaciente.Id
            };
            context.Tabelatb12_paciente.Add(paciente);

            var consulta = new ConsultaModel
            {
                Id_medico_especialiade = medicoEspecialidade.Id,
                Id_servico = pedido.Id_Servico,
                Id_estado_consulta = estadoConsulta.Id,
                Data_consulta = pedido.HorarioPreferencial,
                Paciente = paciente
            };
            context.Tabelatb15_consulta.Add(consulta);

            var pagamento = new PagamentoModel
            {
                Id_cliente = cliente.Nif_cliente,
                Nif_funcionario = funcionario.Nif,
                Comprovativo = pedido.CaminhoComprovativo ?? string.Empty,
                Data_envio = DateTime.UtcNow
            };
            context.Tabelatb14_pagamento.Add(pagamento);

            var pagamentoConsulta = new PagamentoConsultaModel
            {
                Pagamento = pagamento,
                Consulta = consulta
            };
            context.Tabelatb18_pagamento_consulta.Add(pagamentoConsulta);

            await context.SaveChangesAsync();

            pedido.IdConsulta = consulta.Id;
            context.Tabelatb21_agendamento.Update(pedido);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public  async Task<bool> ExisteConflitoAsync(int idEspecialidade, DateTime horario, int? excluirId = null)
    {
        var margem = TimeSpan.FromMinutes(30);
        return await context.Tabelatb21_agendamento
            .Where(a =>
                a.IdEspecialidade == idEspecialidade &&
                a.HorarioReservado == true &&
                a.Estado != "Cancelado" &&
                (excluirId == null || a.Id != excluirId) &&
                a.HorarioPreferencial >= horario.Subtract(margem) &&
                a.HorarioPreferencial <= horario.Add(margem))
            .AnyAsync();
    }

    public async Task<IEnumerable<AgendamentoModel>> ListarPedidosAsync()
    {
        return await context.Tabelatb21_agendamento
            .Include(a => a.Especialidade)
            .Include(p => p.Servico)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
    }

    public async Task<bool> ReservarHorarioAsync(int id)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var pedido = await context.Tabelatb21_agendamento
                .Where(a => a.Id == id && !a.HorarioReservado)
                .FirstOrDefaultAsync();

            if (pedido is null)
            {
                await transaction.RollbackAsync();
                return false;
            }

            // Verifica conflito dentro da transação
            var conflito = await context.Tabelatb21_agendamento
                .AnyAsync(a =>
                    a.Id != id &&
                    a.IdEspecialidade == pedido.IdEspecialidade &&
                    a.HorarioReservado == true &&
                    a.Estado != "Cancelado" &&
                    a.HorarioPreferencial >= pedido.HorarioPreferencial.AddMinutes(-30) &&
                    a.HorarioPreferencial <= pedido.HorarioPreferencial.AddMinutes(30));

            if (conflito)
            {
                await transaction.RollbackAsync();
                return false;
            }

            pedido.HorarioReservado = true;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}

