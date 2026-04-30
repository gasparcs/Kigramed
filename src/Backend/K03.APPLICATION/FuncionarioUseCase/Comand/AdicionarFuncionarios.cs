using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Comand;

public class AdicionarFuncionarios(ICadastrarRepository<FuncionarioModel> repository,
IPegarPeloNifRepository<FuncionarioModel> pesquisar)
{
    public async Task<string> ExecuteAsync(AdicionarFuncionarioDTO dto)
    {
        var usuario = await pesquisar.PegarpeloNifAsync(dto.FuncionaioNif);

        if (usuario is not null) return ("Funcionario que pretende cadastrar já existe");

        var model = new FuncionarioModel
        {
            Nif = dto.FuncionaioNif,

            Id_Perfil = dto.FuncionarioPerfil,

            Nome = dto.FuncionarioNome,

            Estado = dto.FuncionarioEstado,

            MedicoEspecialidades= dto.Especialidades.Select(e => new MedicoEspecilidadeModel
            {
                Id_especialidade= e.IdEspecialidade,

                Nif_funcionario = dto.FuncionaioNif
                
            }).ToList(),

            Contactos = dto.Contactos.Select( c => new ContactoModel
            {
                Id_tipo_contacto = c.TipoContacto,

                Contacto = c.Contacto,

                Nif_funcionario = dto.FuncionaioNif,

                Nif_cliente = null

            }).ToList(),

        };

         return await repository.AddAsync(model);
    } 
}
