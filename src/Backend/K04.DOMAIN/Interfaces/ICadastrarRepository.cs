using System;

namespace Backend.K04.DOMAIN.Interfaces;

public interface ICadastrarRepository <T>
{
      Task<string> AddAsync(T model);
}
