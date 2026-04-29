using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IPegarPeloNifRepository<T>
{
            Task<T?> PegarpeloNifAsync(string nif);
}
