using System;
using Backend.K02.INFRA.Data;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Repository.Cliente;

public class AtualizarClienteRepository(KigramedDbContext context) : IAtualizarRepository<ClienteModel>
{
    public async Task<string> ActualizarAsync(ClienteModel model)
    {
        try
        {
            var cliente = await context.Tabelatb09_cliente
                .Include(c => c.Contactos)
                .FirstOrDefaultAsync(f => f.Nif_cliente == model.Nif_cliente);

            if (cliente is null) return "Cliente não encontrado";

            // Validar unicidade dos novos contactos (se já existem em OUTROS clientes)
            foreach (var novoContacto in model.Contactos)
            {
                var existeEmOutro = await context.Tabelatb04_contato
                    .AnyAsync(c => c.Contacto == novoContacto.Contacto && c.Nif_cliente != model.Nif_cliente);
                
                if (existeEmOutro)
                    return $"O contacto '{novoContacto.Contacto}' já está associado a outro cliente.";
            }

            // Actualizar dados básicos
            cliente.Nome = model.Nome;

            // Sincronizar Contactos (Upsert/Delete)
            // 1. Remover contactos que não estão na nova lista
            var contactosParaRemover = cliente.Contactos
                .Where(old => !model.Contactos.Any(n => n.Contacto == old.Contacto && n.Id_tipo_contacto == old.Id_tipo_contacto))
                .ToList();
            
            foreach (var c in contactosParaRemover)
                context.Tabelatb04_contato.Remove(c);

            // 2. Adicionar novos contactos
            foreach (var n in model.Contactos)
            {
                if (!cliente.Contactos.Any(old => old.Contacto == n.Contacto && old.Id_tipo_contacto == n.Id_tipo_contacto))
                {
                    cliente.Contactos.Add(new ContactoModel
                    {
                        Contacto = n.Contacto,
                        Id_tipo_contacto = n.Id_tipo_contacto,
                        Nif_cliente = cliente.Nif_cliente
                    });
                }
            }

            return await context.SaveChangesAsync() > 0 ?
                "Cliente Atualizado com sucesso." :
                "Nenhuma alteração detectada ou erro ao salvar.";
        }
        catch (Exception ex)
        {
            return $"Erro técnico: {ex.Message}";
        }
    }
}
