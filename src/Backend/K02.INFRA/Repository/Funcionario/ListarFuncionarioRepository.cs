using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class ListarFuncionarioRepository(KigramedDbContext context) : IlistagemRepository<FuncionarioModel>
{
   
    public async Task<IEnumerable<FuncionarioModel>> Listagem()
    {

        try
        {
        var funcionarios = await context.Tabelatb02_funcionario
        .Include(p => p.Perfil)
        .Include(c => c.Contactos).ThenInclude(c => c.TipoContacto)
        .ToListAsync();
        return funcionarios;
        }
        catch
        {
            return [];
        }
    }
    
}
