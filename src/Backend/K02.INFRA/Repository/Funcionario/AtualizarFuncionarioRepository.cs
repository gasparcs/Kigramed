using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class AtualizarFuncionarioRepository(KigramedDbContext context) : IAtualizarRepository<FuncionarioModel>
{
      public async Task<string> ActualizarAsync(FuncionarioModel model)
    {

        try
        {
            var funcionario = await context.Tabelatb02_funcionario.FirstOrDefaultAsync(f=> f.Nif == model.Nif); 
            if(funcionario is null) return "Funcionário não encontrado";
            funcionario.Nome = model.Nome;
            funcionario.Id_Perfil = model.Id_Perfil;
            funcionario.Contactos = model.Contactos;
            funcionario.Estado = model.Estado;
            return await context.SaveChangesAsync() > 0 ?
            "Funcionário atualizado com sucesso." :
            "Não foi possível efectuar a atualização.";   
        }
        catch(DbUpdateException ex)
        {
            return ( ex.ToString());

        }
       
    }
}
