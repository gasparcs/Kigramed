using System;
using Backend.K03.APPLICATION.EstadoConsultaUseCase.DTO;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.Interfaces;

namespace Backend.K03.APPLICATION.EstadoConsultaUseCase.Queries;

public class ListarEstadoConsulta(IlistagemRepository<EstadoConsultaModel> repository)
{
      public async Task<IEnumerable<ListarEstadoConsultaDTO>> ExecuteAsync()
    {
        var estados = await repository.Listagem();

        return estados.Select(p => new ListarEstadoConsultaDTO
            {
                Id = p.Id,

                Descricao = p.Descricao ?? string.Empty,

                Consultas = p.Consultas.Select(c => new ConsultaDTO
                {
                    ConsultaId = c.Id,

                    DataConsulta = c.Data_consulta
                }) 
             });
    }

    public async Task<IEnumerable<DropdownDTO>> DropdownAsync()
    {
        var estados = await repository.Listagem();

        return estados.Select(e => new DropdownDTO
        {
            Id = e.Id,
            
            Nome = e.Descricao ?? string.Empty
        });
    }
}
