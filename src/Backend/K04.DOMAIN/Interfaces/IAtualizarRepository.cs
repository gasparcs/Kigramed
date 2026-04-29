using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IAtualizarRepository <T>
{
      Task<string> ActualizarAsync(T model);
}
