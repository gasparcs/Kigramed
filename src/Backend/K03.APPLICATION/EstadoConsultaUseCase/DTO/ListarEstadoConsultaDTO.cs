using System;

namespace Backend.K03.APPLICATION.EstadoConsultaUseCase.DTO;

public class ListarEstadoConsultaDTO
{
    public int Id { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public IEnumerable<ConsultaDTO> Consultas { get; set; } = [];


}
public class ConsultaDTO
{
    public int ConsultaId { get; set; }

    public DateTime DataConsulta { get; set; }
}

public class DropdownDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
