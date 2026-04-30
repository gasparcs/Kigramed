using System;

namespace Backend.K03.APPLICATION.ConsultaUseCase.DTO;

public class AtualizarConsultaDTO
{
    public int IdConsulta { get; set; }
    public int Id_medico_especialiade { get; set; }
    public int Id_estado_consulta { get; set; }
    public DateTime Data_consulta { get; set; }
}
