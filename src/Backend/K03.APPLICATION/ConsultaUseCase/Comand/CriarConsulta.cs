using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D04.Contacto;
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

        // 1. Verificar se o cliente existe; criar se não existir
        var cliente = await context.Tabelatb09_cliente
            .Include(c => c.Contactos)
            .FirstOrDefaultAsync(c => c.Nif_cliente == dto.NifCliente);

        if (cliente is null)
        {
            cliente = new ClienteModel
            {
                Nif_cliente = dto.NifCliente,
                Nome = dto.NomeCliente
            };
            context.Tabelatb09_cliente.Add(cliente);
            await context.SaveChangesAsync();
        }
        else if (string.IsNullOrWhiteSpace(cliente.Nome) && !string.IsNullOrWhiteSpace(dto.NomeCliente))
        {
            // Actualiza o nome se estava vazio
            cliente.Nome = dto.NomeCliente;
        }

        // 2. Guardar telefone se veio preenchido e ainda não existe
        if (!string.IsNullOrWhiteSpace(dto.TelefoneCliente))
        {
            var telefoneDigits = new string(dto.TelefoneCliente.Where(char.IsDigit).ToArray());
            var jaTemTelefone = cliente.Contactos?.Any(c => 
                new string(c.Contacto.Where(char.IsDigit).ToArray()) == telefoneDigits) ?? false;

            if (!jaTemTelefone)
            {
                // Id_tipo_contacto = 1 corresponde a "Telefone" na tb03_tipo_contacto
                var contacto = new ContactoModel
                {
                    Nif_cliente    = dto.NifCliente,
                    Id_tipo_contacto = 1,
                    Contacto       = dto.TelefoneCliente
                };
                context.Tabelatb04_contato.Add(contacto);
            }
        }

        await context.SaveChangesAsync();

        // 3. Reutilizar paciente se já existir com os mesmos dados
        var paciente = await context.Tabelatb12_paciente
            .FirstOrDefaultAsync(p =>
                p.Nif_cliente         == dto.NifCliente &&
                p.Nome                == dto.NomePaciente &&
                p.Id_genero           == dto.IdGeneroPaciente &&
                p.Id_cliente_paciente == dto.IdClientePaciente);

        if (paciente is null)
        {
            paciente = new PacienteModel
            {
                Nif_cliente          = dto.NifCliente,
                Nome                 = dto.NomePaciente,
                Data_nascimento      = dto.DataNascimentoPaciente,
                Id_genero            = dto.IdGeneroPaciente,
                Id_cliente_paciente  = dto.IdClientePaciente
            };
            context.Tabelatb12_paciente.Add(paciente);
            await context.SaveChangesAsync();
        }

        // 4. Seleccionar o médico com menos consultas activas na especialidade (load balancing)
        var idEstadoCancelada = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Cancelada")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        var medicoEspecialidade = await context.Tabelatb07_medico_especialidade
            .Where(me => me.Id_especialidade == dto.IdEspecialidade)
            .OrderBy(me => context.Tabelatb15_consulta
                .Count(c => c.Id_medico_especialiade == me.Id &&
                            c.Id_estado_consulta     != idEstadoCancelada))
            .FirstOrDefaultAsync();

        if (medicoEspecialidade == null)
            return (string.Empty, "Nenhum médico disponível para esta especialidade.");

        // 5. Verificar conflito de horário
        var existeConflito = await context.Tabelatb15_consulta
            .AnyAsync(c =>
                c.Id_medico_especialiade == medicoEspecialidade.Id &&
                c.Data_consulta          == dto.HorarioPreferencial &&
                c.Id_estado_consulta     != idEstadoCancelada);

        if (existeConflito)
            return (string.Empty, "Este horário já está reservado. Por favor escolha outro.");

        // 6. Criar consulta no estado Pendente
        var idEstadoPendente = await context.Tabelatb13_estado_consulta
            .Where(e => e.Descricao == "Pendente")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (idEstadoPendente == 0)
            return (string.Empty, "Estado 'Pendente' não encontrado na base de dados.");

        var numeroPedido = $"PED-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var consulta = new ConsultaModel
        {
            Id_paciente          = paciente.Id,
            Id_medico_especialiade = medicoEspecialidade.Id,
            Id_servico           = dto.IdServico,
            Id_estado_consulta   = idEstadoPendente,
            Data_consulta        = dto.HorarioPreferencial,
            NumeroPedido         = numeroPedido,
            Observacoes          = dto.Observacoes,
            PrazoPagamento       = null,
            CriadoEm             = DateTime.UtcNow
        };

        context.Tabelatb15_consulta.Add(consulta);
        await context.SaveChangesAsync();

        return (numeroPedido, "Pedido criado com sucesso.");
    }
}
