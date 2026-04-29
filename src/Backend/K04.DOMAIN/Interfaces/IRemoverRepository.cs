using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IRemoverRepository <T>
{
     Task<string> RemoverAsync(T model);
}
