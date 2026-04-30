using System;
using Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.DTO;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.Queries;

public class ListarMedicos(IlistagemRepository<MedicoEspecilidadeModel> repository)
{
     public async Task<IEnumerable<LeituraMedicoEspecialidadeDTO>> ExecuteAsync()
    {
        var medicos = await repository.Listagem();

        return medicos.Select(p => new LeituraMedicoEspecialidadeDTO
            {
                Id = p.Id,

                Nomefuncionario = p.Funcionario.Nome,

                NomeEspecialidade = p.Especialidade.Nome
            
        });
    }
}