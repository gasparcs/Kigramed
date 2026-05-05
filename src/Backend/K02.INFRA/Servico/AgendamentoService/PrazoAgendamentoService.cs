using System;
using Backend.K02.INFRA.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Servico.AgendamentoService;

public class PrazoAgendamentoService(
    IServiceScopeFactory scopeFactory,
    ILogger<PrazoAgendamentoService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<KigramedDbContext>();

                var agora = DateTime.UtcNow;

                // Busca pedidos com prazo expirado
                var expirados = await context.Tabelatb21_agendamento
                    .Where(a =>
                        a.Estado == "Aguarda Pagamento" &&
                        a.PrazoPagamento != null &&
                        a.PrazoPagamento < agora)
                    .ToListAsync(stoppingToken);

                foreach (var pedido in expirados)
                {
                    pedido.Estado = "Cancelado";
                    pedido.HorarioReservado = false; // liberta o horário
                    logger.LogWarning(
                        "Pedido {NumeroPedido} cancelado automaticamente por falta de pagamento.",
                        pedido.NumeroPedido);
                }

                if (expirados.Count > 0)
                    await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao verificar prazos de pagamento.");
            }

            // Verifica a cada 5 minutos
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
