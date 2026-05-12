using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class AtualizarPacienteRepository(KigramedDbContext context) : IAtualizarRepository<PacienteModel>
{
    public async Task<string> ActualizarAsync(PacienteModel model)
    {
        try
        {
            var paciente = await context.Tabelatb12_paciente.FirstOrDefaultAsync(f => f.Id == model.Id);
            if (paciente is null) return "Paciente não encontrado";

            paciente.Nome = model.Nome;
            paciente.Data_nascimento = model.Data_nascimento;
            paciente.Id_genero = model.Id_genero;
            paciente.Id_cliente_paciente = model.Id_cliente_paciente;

            return await context.SaveChangesAsync() > 0 ?
                "Paciente atualizado com sucesso." :
                "Nenhuma alteração detectada.";
        }
        catch (Exception ex)
        {
            return $"Erro técnico: {ex.Message}";
        }
    }
}
