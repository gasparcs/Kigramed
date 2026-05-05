using System;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D03.TipoContacto;
using Backend.K04.DOMAIN.D04.Contacto;
using Backend.K04.DOMAIN.D05.Auth;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D09.Genero;
using Backend.K04.DOMAIN.D11.ClientePaciente;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.D16.Permissao;
using Backend.K04.DOMAIN.D17.PerfilPermissao;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.D19.MedicoConsulta;
using Backend.K04.DOMAIN.D20.SMS;
using Backend.K04.DOMAIN.D21.Agendamento;
using Microsoft.EntityFrameworkCore;

namespace Backend.K02.INFRA.Data;

public class KigramedDbContext(DbContextOptions<KigramedDbContext> options) : DbContext(options)
{
     public DbSet<PerfilModel> Tabelatb01_perfil{get;set;}
    public DbSet<FuncionarioModel> Tabelatb02_funcionario{get;set;}
    public DbSet<TipoContactoModel> Tabelatb03_tipo_contato{get;set;}
    public DbSet<ContactoModel> Tabelatb04_contato{get;set;}
    public DbSet<AuthModel> Tabelatb05_auth{get;set;}
    public DbSet<EspecialidadeModel> Tabelatb06_especialidade{get;set;}
    public DbSet<MedicoEspecilidadeModel> Tabelatb07_medico_especialidade{get;set;}
    public DbSet<ServicosModel> Tabelatb08_servico{get;set;}
    public DbSet<ClienteModel> Tabelatb09_cliente{get;set;} 
    public DbSet<GeneroModel> Tabelatb10_genero{get;set;}
    public DbSet<ClientePacienteModel> Tabelatb11_cliente_paciente{get;set;}
    public DbSet<PacienteModel> Tabelatb12_paciente{get;set;}
    public DbSet<EstadoConsultaModel> Tabelatb13_estado_consulta{get;set;}
    public DbSet<PagamentoModel> Tabelatb14_pagamento{get;set;}
    public DbSet<ConsultaModel> Tabelatb15_consulta{get;set;}
    public DbSet<PermissaoModel> Tabelatb16_permissoes{get;set;}
    public DbSet<PerfilPermissaoModel> Tabelatb17_peril_permissoes{get;set;}
    public DbSet<PagamentoConsultaModel> Tabelatb18_pagamento_consulta{get;set;}
    public DbSet<MedicoConsultaModel> Tabelatb19_medico_consulta{get;set;}
    public DbSet<SMSModel> Tabelatb20_sms{get;set;}
    public DbSet<AgendamentoModel> Tabelatb21_agendamento {get;set;}
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PerfilModel>(entity =>
        {
            entity.HasMany(pf =>pf.Funcionarios).WithOne(f =>f.Perfil).HasForeignKey(fk =>fk.Id_Perfil);

            entity.HasMany(pf =>pf.PerfisPermissoes).WithOne(pp =>pp.Perfil).HasForeignKey(fk =>fk.Id_perfil);
        });

        modelBuilder.Entity<FuncionarioModel>(entity =>
        {
            entity.HasMany(f => f.Contactos).WithOne(c => c.Funcionario ).HasForeignKey (fk => fk.Nif_funcionario);

            entity.HasOne(f => f.Auth).WithOne(a => a.Funcionario).HasForeignKey<AuthModel>(fk => fk.Nif_funcionario);

            entity.HasMany(f => f.MedicoEspecialidades).WithOne(me => me.Funcionario).HasForeignKey(fk => fk.Nif_funcionario);

            entity.HasMany(f => f.Pagamentos).WithOne(p => p.Funcionario).HasForeignKey(fk => fk.Nif_funcionario); 

            entity.HasMany(f => f.Mensagens).WithOne(m => m.Funcionario).HasForeignKey(fk => fk.Nif_funcionario);
        });


         modelBuilder.Entity<TipoContactoModel>(entity =>
        {
            entity.HasMany(tc =>tc.Contactos).WithOne(c => c.TipoContacto).HasForeignKey(fk => fk.Id_tipo_contacto);
        });

        modelBuilder.Entity<ContactoModel>( entity =>
        {
            entity.HasOne(c => c.Cliente).WithMany(cl => cl.Contactos).HasForeignKey(fk => fk.Nif_cliente);
        });

       modelBuilder.Entity<EspecialidadeModel>( entity =>
       {
            entity.HasMany(e => e.MedicoEspecialidades).WithOne(me => me.Especialidade).HasForeignKey(fk => fk.Id_especialidade);

            entity.HasMany(e => e.Servicos).WithOne(s => s.Especialidade).HasForeignKey(fk => fk.Id_especialidade);
            entity.HasMany(e => e.Agendamentos).WithOne(s => s.Especialidade).HasForeignKey(fk => fk.IdEspecialidade);

       });

       modelBuilder.Entity<MedicoEspecilidadeModel>( entity =>
       {
           entity.HasMany(me => me.MedicoConsultas).WithOne(mc => mc.MedicoEspecialidade).HasForeignKey(fk => fk.Id_medico_especialidade);

           entity.HasMany(me=>me.Consultas).WithOne(c=>c.MedicoEspecialidade).HasForeignKey(fk=> fk.Id_medico_especialiade);
       });

       modelBuilder.Entity<ServicosModel>( entity =>
       {
            entity.HasMany(s => s.Consultas).WithOne(c => c.Servico).HasForeignKey(fk => fk.Id_servico);

            entity.HasMany(s => s.Agendamento).WithOne(c => c.Servico).HasForeignKey(fk => fk.Id_Servico);
       });


       modelBuilder.Entity<ClienteModel>( entity =>
       {
           entity.HasMany(c => c.Pacientes).WithOne(p => p.Cliente).HasForeignKey(fk => fk.Nif_cliente);

           entity.HasMany(c => c.Pagamentos).WithOne(p => p.Cliente).HasForeignKey( fk => fk.Id_cliente);
           
           entity.HasMany(c => c.Mensagens).WithOne(m => m.Cliente).HasForeignKey(fk => fk.Id_cliente);
        });

        modelBuilder.Entity<GeneroModel>( entity =>
        {
            entity.HasMany(g => g.Pacientes).WithOne(p => p.Genero).HasForeignKey(fk => fk.Id_genero);
        });

        modelBuilder.Entity<ClientePacienteModel>( entity =>
        {
            entity.HasMany(cp => cp.Pacientes).WithOne(p => p.ClientePaciente).HasForeignKey(fk => fk.Id_cliente_paciente);
        });

        modelBuilder.Entity<PacienteModel>( entity =>
        {
           entity.HasMany(p => p.Consultas).WithOne(c => c.Paciente).HasForeignKey(fk => fk.Id_paciente);
        });

          modelBuilder.Entity<EstadoConsultaModel>( entity =>
        {
             entity.HasMany(ec => ec.Consultas).WithOne(c => c.EstadoConsulta).HasForeignKey(fk => fk.Id_estado_consulta);
        });

          modelBuilder.Entity<PagamentoModel>( entity =>
         {
              entity.HasMany(p => p.PagamentoConsultas).WithOne(pc => pc.Pagamento).HasForeignKey(fk => fk.Id_Pagamento);
        });

        modelBuilder.Entity<PermissaoModel>( entity =>
        {
              entity.HasMany(p => p.PerfisPermissoes).WithOne(pp => pp.Permissao).HasForeignKey(fk => fk.UUID_permissao).HasPrincipalKey(p=>p.UUID);
        });

        modelBuilder.Entity<PagamentoConsultaModel>( entity =>
        {
                 entity.HasOne(pc => pc.Consulta).WithOne(c => c.PagamentoConsulta).HasForeignKey<PagamentoConsultaModel>(fk => fk.Id_Consulta);
        });

        modelBuilder.Entity<MedicoConsultaModel>( entity =>
        {
                 entity.HasOne(mc => mc.Consulta).WithOne(c => c.MedicoConsulta).HasForeignKey<MedicoConsultaModel>(fk => fk.Id_consulta);
        });

        modelBuilder.Entity<AgendamentoModel>( entity =>
        {
                 entity.HasOne(mc => mc.Consulta).WithOne(c => c.Agendamento).HasForeignKey<AgendamentoModel>(fk => fk.IdConsulta);
        });
    }
}
