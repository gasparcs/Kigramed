using System;

namespace Backend.K03.APPLICATION.ServicosUseCase.DTO;

public class AtualizarServicosDTO
{
    public int ServicoId{get;set;}

    public String ServicoNome{get;set;}=string.Empty;
      
    public int ServicoDuracaoMinuto{get;set;}
   
    public decimal ServicoPreco{get;set;}

    public bool ServicoEstado{get;set;} 
}


