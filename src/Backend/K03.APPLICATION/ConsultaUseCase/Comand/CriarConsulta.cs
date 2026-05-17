using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D15.Consulta;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class CriarConsulta(KigramedDbContext context)
{
    public async Task<(string numeroPedido, string mensagem)> ExecuteAsync(
        int idPaciente, int idEspecialidade, int idServico,
        DateTime horarioPreferencial, string? observacoes)
    {
        // 1. Busca idEstadoCancelada
        var estadoCancelada = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Cancelada");
        
        if (estadoCancelada == null)
            return (string.Empty, "Erro: Estado 'Cancelada' não encontrado no sistema.");

        // 2. Busca o id_medico_especialiade (assumimos que idEspecialidade vem daqui ou precisa buscar o MedicoEspecialidade, 
        // mas a assinatura do método diz idEspecialidade e a tb15_consulta pede Id_medico_especialiade. Vamos verificar se temos que buscar um medico disponivel.
        // Espera, a tabela tb15_consulta tem Id_medico_especialiade e a tb07_medico_especialidade relaciona Medico e Especialidade.
        // Se o utilizador envia idEspecialidade, o correto é pegar um id_medico_especialiade disponível?
        // A regra diz: "Verifica conflito: mesmo Id_medico_especialiade, mesma Data_consulta, estado != Cancelada, id != o actual".
        // Vamos buscar um MedicoEspecialidade válido para a especialidade solicitada.
        var medicoEspecialidade = await context.Tabelatb07_medico_especialidade
            .FirstOrDefaultAsync(me => me.Id_especialidade == idEspecialidade);

        if (medicoEspecialidade == null)
            return (string.Empty, "Nenhum médico encontrado para esta especialidade.");

        int idMedicoEspecialidade = medicoEspecialidade.Id;

        // Verifica conflito
        var existeConflito = await context.Tabelatb15_consulta
            .AnyAsync(c => c.Id_medico_especialiade == idMedicoEspecialidade &&
                           c.Data_consulta == horarioPreferencial &&
                           c.Id_estado_consulta != estadoCancelada.Id);

        if (existeConflito)
            return (string.Empty, "conflito: Horário já reservado.");

        // 3. Valida que o paciente existe
        var pacienteExiste = await context.Tabelatb12_paciente.AnyAsync(p => p.Id == idPaciente);
        if (!pacienteExiste)
            return (string.Empty, "Paciente não encontrado.");

        // 4. Busca idEstadoPendente
        var estadoPendente = await context.Tabelatb13_estado_consulta
            .FirstOrDefaultAsync(e => e.Descricao == "Pendente");
        
        if (estadoPendente == null)
            return (string.Empty, "Erro: Estado 'Pendente' não encontrado no sistema.");

        // 5. Gera NumeroPedido
        string numeroPedido = $"PED-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        // 6. Cria e persiste ConsultaModel
        var novaConsulta = new ConsultaModel
        {
            Id_medico_especialiade = idMedicoEspecialidade,
            Id_servico = idServico,
            Id_paciente = idPaciente,
            Id_estado_consulta = estadoPendente.Id,
            Data_consulta = horarioPreferencial,
            NumeroPedido = numeroPedido,
            Observacoes = observacoes,
            PrazoPagamento = null,
            CriadoEm = DateTime.UtcNow
        };

        context.Tabelatb15_consulta.Add(novaConsulta);
        await context.SaveChangesAsync();

        // 7. Retorna
        return (numeroPedido, "Pedido criado com sucesso.");
    }
}
