using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Pagamento;

public class AdicionarPagamentoRepository(KigramedDbContext context) : ICadastrarRepository<PacienteModel>
{
    public async Task<string> AddAsync(PacienteModel model)
    {
        
        try
        {
             await context.Tabelatb12_paciente.AddAsync(model);
             return await context.SaveChangesAsync()>0?
             "Pagamento adicionado com sucesso" :
             "Não foi possível adicionar pagamento";
        }
        catch (DbUpdateException ex)
        {
            
            return(ex.ToString());
        }

    }
}
