using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
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
            var funcionario = await context.Tabelatb02_funcionario
                .Include(f => f.Contactos)
                .Include(f => f.MedicoEspecialidades)
                .FirstOrDefaultAsync(f => f.Nif == model.Nif);

            if (funcionario is null) return "Funcionário não encontrado";

            // Validar unicidade dos novos contactos (se já existem em OUTROS funcionários)
            foreach (var novoContacto in model.Contactos)
            {
                var existeEmOutro = await context.Tabelatb04_contato
                    .AnyAsync(c => c.Contacto == novoContacto.Contacto && c.Nif_funcionario != model.Nif);
                
                if (existeEmOutro)
                    return $"O contacto '{novoContacto.Contacto}' já está associado a outro funcionário.";
            }

            // Actualizar dados básicos
            funcionario.Nome = model.Nome;
            funcionario.Id_Perfil = model.Id_Perfil;
            funcionario.Estado = model.Estado;

            // Sincronizar Contactos
            var contactosParaRemover = funcionario.Contactos
                .Where(old => !model.Contactos.Any(n => n.Contacto == old.Contacto && n.Id_tipo_contacto == old.Id_tipo_contacto))
                .ToList();
            foreach (var c in contactosParaRemover) context.Tabelatb04_contato.Remove(c);

            foreach (var n in model.Contactos)
            {
                if (!funcionario.Contactos.Any(old => old.Contacto == n.Contacto && old.Id_tipo_contacto == n.Id_tipo_contacto))
                {
                    funcionario.Contactos.Add(new ContactoModel
                    {
                        Contacto = n.Contacto,
                        Id_tipo_contacto = n.Id_tipo_contacto,
                        Nif_funcionario = funcionario.Nif
                    });
                }
            }

            // Sincronizar Especialidades (se for médico)
            var espParaRemover = funcionario.MedicoEspecialidades
                .Where(old => !model.MedicoEspecialidades.Any(n => n.Id_especialidade == old.Id_especialidade))
                .ToList();
            foreach (var e in espParaRemover) context.Tabelatb07_medico_especialidade.Remove(e);

            foreach (var n in model.MedicoEspecialidades)
            {
                if (!funcionario.MedicoEspecialidades.Any(old => old.Id_especialidade == n.Id_especialidade))
                {
                    funcionario.MedicoEspecialidades.Add(new MedicoEspecilidadeModel
                    {
                        Id_especialidade = n.Id_especialidade,
                        Nif_funcionario = funcionario.Nif
                    });
                }
            }

            return await context.SaveChangesAsync() > 0 ?
                "Funcionário atualizado com sucesso." :
                "Nenhuma alteração detectada.";
        }
        catch (Exception ex)
        {
            return $"Erro técnico: {ex.Message}";
        }
    }
}
