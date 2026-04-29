using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class RemoverPacienteRepository(KigramedDbContext context) : IRemoverRepository<PacienteModel>
{
    public async Task<string> RemoverAsync(PacienteModel model)
    {
        try
        {
               context.Tabelatb12_paciente.Remove(model);
               return await context.SaveChangesAsync() >0 ?
                "Paciente removido com sucesso." :
                "Não foi possível remover o Paciente.";
        }
        catch (DbUpdateException ex)
        {
            
             return (ex.ToString()); 
        }
    }
}
