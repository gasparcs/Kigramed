using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IPegarPeloTextoRepository<T>
{
    Task<IEnumerable<T>> PegarAsync(string texto); 
}
