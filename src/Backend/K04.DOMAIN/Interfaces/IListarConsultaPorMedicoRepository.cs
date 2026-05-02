namespace Backend.K04.DOMAIN.Interfaces;

using Backend.K04.DOMAIN.D15.Consulta;

/// Retorna apenas as consultas associadas ao médico com o NIF indicado.
public interface IListarConsultaPorMedicoRepository
{
    Task<IEnumerable<ConsultaModel>> ListarPorNifMedicoAsync(string nifMedico);
}
