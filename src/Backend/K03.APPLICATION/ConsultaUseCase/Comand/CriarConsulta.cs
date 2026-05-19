using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Microsoft.EntityFrameworkCore;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class CriarConsulta(KigramedDbContext context)
{
    public async Task<(string numeroPedido, string mensagem)> ExecuteAsync(CriarConsultaDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NifCliente))
            return (string.Empty, "NIF do cliente é obrigatório.");

        // 1. Verificar se o cliente existe em tb09_cliente
        var cliente = await context.Tabelatb09_cliente
            .FirstOrDefaultAsync(c => c.Nif_cliente == dto.NifCliente);

        if (cliente is null)
        {
            cliente = new ClienteModel
            {
                Nif_cliente = dto.NifCliente,
                Nome = string.Empty
            };
            context.Tabelatb09_cliente.Add(cliente);
            await context.SaveChangesAsync();
        }

        // 2. Criar o paciente em tb12_paciente
        var paciente = new PacienteModel
        {
            Nif_cliente = dto.NifCliente,
            Nome = dto.NomePaciente,
            Data_nascimento = dto.DataNascimentoPaciente,
            Id_genero = dto.IdGeneroPaciente,
            Id_cliente_paciente = dto.IdClientePaciente
        };

        context.Tabelatb12_paciente.Add(paciente);
        await context.SaveChangesAsync();

        // 3. Verificar conflito de horário para o médico especialista da especialidade
        var medicoEspecialidade = await context.Tabelatb07_medico_especialidade
            .FirstOrDefaultAsync(me => me.Id_especialidade == dto.IdEspecialidade);

        if (medicoEspecialidade == null)
            return (string.Empty, "Nenhum médico disponível para esta especialidade.");

        var idEstadoCancelada = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Cancelada")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        var existeConflito = await context.Tabelatb15_consulta
            .AnyAsync(c =>
                c.Id_medico_especialiade == medicoEspecialidade.Id &&
                c.Data_consulta == dto.HorarioPreferencial &&
                c.Id_estado_consulta != idEstadoCancelada);

        if (existeConflito)
            return (string.Empty, "Este horário já está reservado. Por favor escolha outro.");

        // 4. Busca idEstadoPendente e cria a consulta
        var idEstadoPendente = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Pendente")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (idEstadoPendente == 0)
            return (string.Empty, "Estado 'Pendente' não encontrado na base de dados.");

        var numeroPedido = $"PED-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var consulta = new ConsultaModel
        {
            Id_paciente = paciente.Id,
            Id_medico_especialiade = medicoEspecialidade.Id,
            Id_servico = dto.IdServico,
            Id_estado_consulta = idEstadoPendente,
            Data_consulta = dto.HorarioPreferencial,
            NumeroPedido = numeroPedido,
            Observacoes = dto.Observacoes,
            PrazoPagamento = null,
            CriadoEm = DateTime.UtcNow
        };

        context.Tabelatb15_consulta.Add(consulta);
        await context.SaveChangesAsync();

        return (numeroPedido, "Pedido criado com sucesso.");
    }
}
