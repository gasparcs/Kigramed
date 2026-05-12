using System;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K03.APPLICATION.Servico.IPasswordService;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D05.Auth;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.FuncionarioUseCase.Comand;

public class AdicionarFuncionarios(ICadastrarRepository<FuncionarioModel> repository,
IPegarPeloNifRepository<FuncionarioModel> pesquisar,
 IPasswordCreate gerarSenha,
IPasswordHash criptoSenha,
ISmsService sms)
{
    public async Task<string> ExecuteAsync(AdicionarFuncionarioDTO dto)
    {
        var usuario = await pesquisar.PegarpeloNifAsync(dto.FuncionaioNif);

        if (usuario is not null) return ("Funcionario que pretende cadastrar já existe");

        string senha = await gerarSenha.GenerateAsync();
        var(senhaHash, saltHash) = await criptoSenha.HashAsync(senha);

        var model = new FuncionarioModel
        {
            Nif = dto.FuncionaioNif,

            Id_Perfil = dto.FuncionarioPerfil,

            Nome = dto.FuncionarioNome,

            Estado = dto.FuncionarioEstado,

            Auth = new AuthModel
                {
                Senha_hash = senhaHash,

                Senha_Salt = saltHash
                },

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

        var resultado = await repository.AddAsync(model);

        if (resultado != "Funcionário cadastrado com sucesso.")
         return resultado;

        string texto =
            $"Estimado(a) {model.Nome},\n\n" +
            $"É com satisfação que informamos que o seu registo na plataforma do " +
            $"Centro Médico Kigramed foi efectuado com sucesso.\n\n" +
            $"As suas credenciais de acesso são:\n" +
            $"  • NIF: {model.Nif}\n" +
            $"  • Senha: {senha}\n\n" +
            $"Aceda ao sistema através do portal: www.kigramed.com\n\n" +
            $"Por motivos de segurança, recomendamos que altere a sua senha " +
            $"imediatamente após o primeiro acesso.\n\n" +
            $"Caso necessite de apoio, contacte a nossa equipa de suporte.\n\n" +
            $"Atenciosamente,\n" +
            $"Equipa Kigramed";

        var contato = model.Contactos.FirstOrDefault();

        if (contato is null) return "Telefone não válido";

        var smsResponse = await sms.EnviarAsync(contato.Contacto, texto, "101010101010");

       return smsResponse ? "Funcionário cadastrado e SMS enviado com sucesso!" : "Funcionário cadastrado, mas SMS falhou.";
    } 
}
