using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Comand;

public class AtualizarFuncionario(IAtualizarRepository<FuncionarioModel> repository)
{
      public async Task<string> ExecuteAsync(AtualizarFuncionarioDTO dto)
    {
        var model = new FuncionarioModel
        {
            Nif = dto.FuncionarioNif,

            Id_Perfil = dto.FuncionarioPerfil,

            Nome = dto.FuncionarioNome,

            Estado = dto.FuncionarioEstado,

             MedicoEspecialidades= dto.Especialidades.Select(e => new MedicoEspecilidadeModel
            {
                Id_especialidade= e.IdEspecialidade,

                Nif_funcionario = dto.FuncionarioNif
                
            }).ToList(),

            Contactos= dto.Contactos.Select(c => new ContactoModel
            {
                Id_tipo_contacto = c.TipoContacto,

                Contacto = c.Contacto

            }).ToList() 
        };

        return await repository.ActualizarAsync(model);
}
}