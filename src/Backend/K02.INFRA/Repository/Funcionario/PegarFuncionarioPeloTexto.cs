using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Funcionario;

public class PegarFuncionarioPeloTexto(KigramedDbContext context) : IPegarPeloTextoRepository<FuncionarioModel>
{
     public async Task<IEnumerable<FuncionarioModel>> PegarAsync(string texto)
    {
        try
        {
            return await context.Tabelatb02_funcionario
                .Where(t => t.Nome.Contains(texto))
                .Include(p => p.Perfil)
                .Include(c => c.Contactos).ThenInclude(c => c.TipoContacto)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}