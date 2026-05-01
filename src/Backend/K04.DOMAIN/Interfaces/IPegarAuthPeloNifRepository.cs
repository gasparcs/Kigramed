using System;
using Backend.K04.DOMAIN.D05.Auth;

namespace Backend.K04.DOMAIN.Interfaces;

public interface IPegarAuthPeloNifRepository
{
    Task<AuthModel?> PegarPeloNifAsync(string nif);
}
