using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.D19.MedicoConsulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class AtualizarConsulta(IAtualizarRepository<ConsultaModel> repository)
{
    public async Task<string> ExecuteAsync(AtualizarConsultaDTO dto)
    {
        var model = new ConsultaModel
        {
            Id = dto.IdConsulta,

            Data_consulta = dto.Data_consulta,

            MedicoConsulta = new MedicoConsultaModel
            {
                Id_medico_especialidade = dto.Id_medico_especialiade,

                Id_consulta = dto.IdConsulta,

                MedicoEspecialidade = null!,

                Consulta = null!
            },
            EstadoConsulta = new EstadoConsultaModel
            {
                Id = dto.Id_estado_consulta,
                
                Consultas = null!
            }
        };

        return await repository.ActualizarAsync(model);
    }
}
