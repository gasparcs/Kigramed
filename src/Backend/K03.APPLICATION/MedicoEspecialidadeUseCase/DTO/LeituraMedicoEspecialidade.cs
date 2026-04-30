using System;

namespace Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.DTO;

public class LeituraMedicoEspecialidadeDTO
{
    
    public int Id{get;set;}

    public string Nomefuncionario{get;set;}=string.Empty;

    public string NomeEspecialidade{get;set;}=string.Empty;
}