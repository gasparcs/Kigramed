using System;
using Backend.K02.INFRA.Data;
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

