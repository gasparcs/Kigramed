using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IPesquisarPeloIdRepository<T>
{
        Task<T?> PegarAsync(int id);
}
