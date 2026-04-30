using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.ConsultaUseCase.Comand;

public class RemoverConsulta(IRemoverRepository<ConsultaModel> repository)
{
    public async Task<string> ExecuteAsync(int id)
    {
        var model = new ConsultaModel
        {
            Id = id
        };

        return await repository.RemoverAsync(model);
    }
}
