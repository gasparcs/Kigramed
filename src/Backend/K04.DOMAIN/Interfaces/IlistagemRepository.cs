using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IlistagemRepository<T>
{
    Task<IEnumerable<T>> Listagem(); 
}
