using Backend.K02.INFRA.Data;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.K02.INFRA.Servico.AgendamentoService;

public class PrazoAgendamentoService(
    IServiceScopeFactory scopeFactory,
    ILogger<PrazoAgendamentoService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PrazoAgendamentoService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await VerificarPrazosExpiradosAsync();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task VerificarPrazosExpiradosAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KigramedDbContext>();
        var sms     = scope.ServiceProvider.GetRequiredService<ISmsService>();

        try
        {
            var idAguardaPagamento = await context.Tabelatb13_estado_consulta
                .Where(e => e.Descricao == "Aguarda Pagamento")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            var idCancelada = await context.Tabelatb13_estado_consulta
                .Where(e => e.Descricao == "Cancelada")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (idAguardaPagamento == 0 || idCancelada == 0)
            {
                logger.LogWarning("Estados 'Aguarda Pagamento' ou 'Cancelada' não encontrados.");
                return;
            }

            var expiradas = await context.Tabelatb15_consulta
                .Include(c => c.Paciente)
                    .ThenInclude(p => p.Cliente)
                    .ThenInclude(cl => cl.Contactos)
                .Where(c =>
                    c.Id_estado_consulta == idAguardaPagamento &&
                    c.PrazoPagamento.HasValue &&
                    c.PrazoPagamento < DateTime.UtcNow)
                .ToListAsync();

            if (!expiradas.Any())
                return;

            logger.LogInformation("{Count} consulta(s) com prazo expirado encontrada(s).", expiradas.Count);

            foreach (var consulta in expiradas)
            {
                consulta.Id_estado_consulta = idCancelada;

                var telefone = consulta.Paciente?.Cliente?.Contactos
                    ?.FirstOrDefault()?.Contacto ?? string.Empty;

                if (!string.IsNullOrEmpty(telefone))
                {
                    var mensagem =
                        $"Estimado(a) {consulta.Paciente?.Nome ?? "Cliente"},\n\n" +
                        $"Informamos que o seu pedido {consulta.NumeroPedido} foi cancelado " +
                        $"automaticamente por não ter sido efectuado o pagamento dentro do prazo.\n\n" +
                        $"Pode efectuar um novo agendamento em www.kigramed.com.\n\n" +
                        $"Atenciosamente,\n" +
                        $"Centro Médico Kigramed";

                    await sms.EnviarAsync(telefone, mensagem, "5417298387");
                }

                logger.LogInformation("Consulta {NumeroPedido} cancelada por prazo expirado.", 
                    consulta.NumeroPedido);
            }

            // SaveChanges fora do loop — actualiza tudo numa única transacção
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao verificar prazos expirados.");
        }
    }
}
