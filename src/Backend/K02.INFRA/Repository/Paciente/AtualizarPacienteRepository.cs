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
            var paciente = await context.Tabelatb12_paciente.FirstOrDefaultAsync(f=> f.Id== model.Id); 
            if(paciente is null) return "Paciente não encontrado";
            paciente.Nome = model.Nome;
            return await context.SaveChangesAsync() > 0 ?
            "Cliente Atualizado com sucesso." :
            "Não foi possível efectuar a atualização.";   
        }
        catch(DbUpdateException ex)
        {
            return ( ex.ToString());

        }
       
    }
}
