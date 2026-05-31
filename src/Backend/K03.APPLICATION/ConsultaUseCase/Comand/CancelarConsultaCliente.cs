using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class CancelarConsultaCliente(KigramedDbContext context)
{
    public async Task<string> ExecuteAsync(string numeroPedido)
    {
        var consulta = await context.Tabelatb15_consulta
            .Include(c => c.EstadoConsulta)
            .FirstOrDefaultAsync(c => c.NumeroPedido == numeroPedido);

        if (consulta is null)
            return "Pedido não encontrado.";

        var cancelaveis = new[] { "Confirmada", "Pendente", "Aguarda Pagamento" };

        if (!cancelaveis.Contains(consulta.EstadoConsulta.Descricao, StringComparer.OrdinalIgnoreCase))
            return $"Não é possível cancelar. Estado actual: {consulta.EstadoConsulta.Descricao}.";

        var idCancelada = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Cancelada")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (idCancelada == 0)
            return "Estado 'Cancelada' não configurado na base de dados.";

        consulta.Id_estado_consulta = idCancelada;
        await context.SaveChangesAsync();

        return "sucesso";
    }
}
