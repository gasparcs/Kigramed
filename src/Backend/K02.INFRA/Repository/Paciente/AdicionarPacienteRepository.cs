using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Paciente;

public class AdicionarPacienteRepository(KigramedDbContext context) : ICadastrarRepository<PacienteModel>
{
     public async Task<string> AddAsync(PacienteModel model)
    {
        try
        {
             await context.Tabelatb12_paciente.AddAsync(model);
             
             return await context.SaveChangesAsync() > 0 ?

             "Paciente cadastrado com sucesso." :

            "Não foi possível cadastrar o Paciente.";
        }
        catch(DbUpdateException ex)
        {
            return(ex.ToString());
        }
       
    }
}
