using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class PegarFuncionarioPeloNifRepository(KigramedDbContext context) : IPegarPeloNifRepository<FuncionarioModel>
{
    public async Task<FuncionarioModel?> PegarpeloNifAsync(string nif)
    {
        try
        {
             return await context.Tabelatb02_funcionario 
            .Include(p => p.Perfil)
            .Include(c => c.Contactos).ThenInclude(c => c.TipoContacto)
            .FirstOrDefaultAsync(n => n.Nif == nif);
        }
        catch
        {
            return null;
        }
    
    }
}
