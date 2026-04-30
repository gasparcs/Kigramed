using Backend.K03.APPLICATION.ConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class AdicionarConsulta(ICadastrarRepository<ConsultaModel> repository)
{
    public async Task<string> ExecuteAsync(AdicionarConsultaDTO dto)
    {
        var model = new ConsultaModel
        {
            Id_medico_especialiade = dto.Id_medico_especialiade,

            Id_servico = dto.Id_servico,

            Id_paciente = dto.Id_paciente,

            Id_estado_consulta = dto.Id_estado_consulta,
            
            Data_consulta = dto.Data_consulta
        };

        return await repository.AddAsync(model);
    }
}
