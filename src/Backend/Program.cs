using Backend.K02.INFRA.Data;
using Backend.K02.INFRA.Repository.Auth;
using Backend.K02.INFRA.Repository.Cliente;
using Backend.K02.INFRA.Repository.Consulta;
using Backend.K04.DOMAIN.Interfaces;
using Backend.K02.INFRA.Repository.Especialidade;
using Backend.K02.INFRA.Repository.EstadoConsulta;
using Backend.K02.INFRA.Repository.Funcionario;
using Backend.K02.INFRA.Repository.MedicoEspecialidade;
using Backend.K02.INFRA.Repository.Paciente;
using Backend.K02.INFRA.Repository.Pagamento;
using Backend.K02.INFRA.Repository.PagamentoConsulta;
using Backend.K02.INFRA.Repository.Perfil;
using Backend.K02.INFRA.Repository.Servicos;
using Backend.K02.INFRA.Repository.SMS;
using Backend.K03.APPLICATION.ClienteUseCase.comand;
using Backend.K03.APPLICATION.ClienteUseCase.Queries;
using Backend.K03.APPLICATION.ConsultaUseCase.Comand;
using Backend.K03.APPLICATION.ConsultaUseCase.Queries;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Comand;
using Backend.K03.APPLICATION.EspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.EstadoConsultaUseCase.Queries;
using Backend.K03.APPLICATION.FuncionarioUseCase.Comand;
using Backend.K03.APPLICATION.FuncionarioUseCase.DTO;
using Backend.K03.APPLICATION.FuncionarioUseCase.Queries;
using Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.PacienteUseCase.Comand;
using Backend.K03.APPLICATION.PacienteUseCase.Queries;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.Comand;
using Backend.K03.APPLICATION.PagamentoConsultaUseCase.Queries;
using Backend.K03.APPLICATION.PagamentoUseCase.Comand;
using Backend.K03.APPLICATION.PagamentoUseCase.Queries;
using Backend.K03.APPLICATION.PerfilUseCase.Queries;
using Backend.K03.APPLICATION.ServicosUseCase.Comand;
using Backend.K03.APPLICATION.ServicosUseCase.Queries;
using Backend.K03.APPLICATION.SMSUseCase.Comand;
using Backend.K03.APPLICATION.SMSUseCase.Queries;
using Backend.K04.DOMAIN.D01.Perfil;
using Backend.K04.DOMAIN.D02.Funcionario;
using Backend.K04.DOMAIN.D06.Especialidade;
using Backend.K04.DOMAIN.D07.MedicoEspecialidade;
using Backend.K04.DOMAIN.D08.Servicos;
using Backend.K04.DOMAIN.D09.Cliente;
using Backend.K04.DOMAIN.D12.Paciente;
using Backend.K04.DOMAIN.D13.EstadoConsulta;
using Backend.K04.DOMAIN.D14.Pagamento;
using Backend.K04.DOMAIN.D15.Consulta;
using Backend.K04.DOMAIN.D18.PagamentoConsulta;
using Backend.K04.DOMAIN.D20.SMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.K03.APPLICATION.Servico.IPasswordService;
using Backend.K02.INFRA.Servico.PasswordService;
using Backend.K03.APPLICATION.Servico.ISmsService;
using Backend.K02.INFRA.Servico.SmsService;
using Backend.K03.APPLICATION.Servico.ITokenService;
using Backend.K02.INFRA.Servico.AuthService;
using Backend.K03.APPLICATION.AuthUseCase.Comand;
using Backend.K02.INFRA.Repository.Agendamento;
using Backend.K03.APPLICATION.AgendamentoUseCase.Comand;
using Backend.K03.APPLICATION.AgendamentoUseCase.Queries;
using Backend.K02.INFRA.Servico.AgendamentoService;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string conexao= builder.Configuration.GetConnectionString("ConexaoLocal")!;
builder.Services.AddDbContext<KigramedDbContext>(options => options.UseNpgsql(conexao));

// Configurar autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "sua-chave-secreta-muito-longa-e-segura-aqui";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Kigramed";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "KigramedApi";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding
        .UTF8.GetBytes(jwtKey))
    };
});

//Contratos do Cliente 
builder.Services.AddScoped<ICadastrarRepository<ClienteModel>, AdicionarClienteRepository>();
builder.Services.AddScoped<IAtualizarRepository<ClienteModel>, AtualizarClienteRepository>();
builder.Services.AddScoped<IlistagemRepository<ClienteModel>, ListarClienteRepository>();
builder.Services.AddScoped<IPegarPeloNifRepository<ClienteModel>, PegarClientePeloNifRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<ClienteModel>,PegarClientePeloTextoRepository >();
builder.Services.AddScoped<IRemoverRepository<ClienteModel>, RemoverClienteRepository>();
//Contratos da Consulta
builder.Services.AddScoped<ICadastrarRepository<ConsultaModel>, AdicionarConsultaRepository>();
builder.Services.AddScoped<IListarConsultaPorMedicoRepository, ListarConsultaPorMedicoRepository>();
builder.Services.AddScoped<IAtualizarRepository<ConsultaModel>, AtualizarConsultaRepository>();
builder.Services.AddScoped<IlistagemRepository<ConsultaModel>, ListarConsultaRepository>();
builder.Services.AddScoped<IPesquisarPeloIdRepository<ConsultaModel>, PegarConsultaPeloIdRepository>();
builder.Services.AddScoped<IRemoverRepository<ConsultaModel>, RemoverConsultaRepository>();
//Contratos da Especialidade
builder.Services.AddScoped<ICadastrarRepository<EspecialidadeModel>, AdicionarEspecialidadeRepository>();
builder.Services.AddScoped<IAtualizarRepository<EspecialidadeModel>, AtualizarEspecialidadeRepository>();
builder.Services.AddScoped<IlistagemRepository<EspecialidadeModel>, ListarEspecialidadeRepository>();
builder.Services.AddScoped<IPesquisarPeloIdRepository<EspecialidadeModel>, PegarEspecialidadePeloIdRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<EspecialidadeModel>, PegarEspecialidadePeloTextoRepository>();
builder.Services.AddScoped<IRemoverRepository<EspecialidadeModel>, RemoverEspecialidadeRepository>();
//Contratos da Perfil
builder.Services.AddScoped<IlistagemRepository<PerfilModel>, ListarPerfilRepository>();
//Contratos do Funcionário
builder.Services.AddScoped<ICadastrarRepository<FuncionarioModel>, AdicionarFuncionarioRepository>();
builder.Services.AddScoped<IAtualizarRepository<FuncionarioModel>, AtualizarFuncionarioRepository>();
builder.Services.AddScoped<IlistagemRepository<FuncionarioModel>, ListarFuncionarioRepository>();
builder.Services.AddScoped<IPegarPeloNifRepository<FuncionarioModel>, PegarFuncionarioPeloNifRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<FuncionarioModel>, PegarFuncionarioPeloTextoRepository>();
builder.Services.AddScoped<IRemoverRepository<FuncionarioModel>, RemoverFuncionarioRepository>();

//Contratos de EstadoConsulta
builder.Services.AddScoped<IlistagemRepository<EstadoConsultaModel>, ListarEstadoConsultaRepository>();
//Contratos de MedicoEspecialidade
builder.Services.AddScoped<IlistagemRepository<MedicoEspecilidadeModel>, ListarMedicoEspecialidadeRepository>();
//Contrato do Paciente
builder.Services.AddScoped<ICadastrarRepository<PacienteModel>, AdicionarPacienteRepository>();
builder.Services.AddScoped<IAtualizarRepository<PacienteModel>, AtualizarPacienteRepository>();
builder.Services.AddScoped<IlistagemRepository<PacienteModel>, ListarPacienteRepository>();
builder.Services.AddScoped<IPesquisarPeloIdRepository<PacienteModel>, PegarPacientePeloIdRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<PacienteModel>, PegarPacientePeloTextoRepository>();
builder.Services.AddScoped<IRemoverRepository<PacienteModel>, RemoverPacienteRepository>();
//Contratos para o Pagamento
builder.Services.AddScoped<ICadastrarRepository<PagamentoModel>, AdicionarPagamentoRepository>();
builder.Services.AddScoped<IlistagemRepository<PagamentoModel>, ListarPagamentoRepository>();
//Contratos do Auth
builder.Services.AddScoped<IPegarAuthPeloNifRepository, PegarAuthPeloNifRepository>();
//Contratos para o Serviço
builder.Services.AddScoped<ICadastrarRepository<ServicosModel>, AdicionarServicoRepository>();
builder.Services.AddScoped<IAtualizarRepository<ServicosModel>, AtualizarServicosRepository>();
builder.Services.AddScoped<IlistagemRepository<ServicosModel>,ListarServicosRepository>();
builder.Services.AddScoped<IPesquisarPeloIdRepository<ServicosModel>, PegarIdServicosRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<ServicosModel>, PegarTextoServicosRepository>();
builder.Services.AddScoped<IRemoverRepository<ServicosModel>, RemoverServicosRepository>();

//contratos para a sms
builder.Services.AddScoped<ICadastrarRepository<SMSModel>, AdicionarSmsRepository>();
builder.Services.AddScoped<IlistagemRepository<SMSModel>, ListarSmsRepository>();
 
 //contratos para pagamentoconsulta
builder.Services.AddScoped<ICadastrarRepository<PagamentoConsultaModel>, AdicionarPagamentoConsultaRepository>();
builder.Services.AddScoped<IlistagemRepository<PagamentoConsultaModel>, ListarPagamentoConsultaRepository>();

//CASOS DE USO
//casos de usos cliendemodel
builder.Services.AddTransient<AdicionarCliente>();
builder.Services.AddTransient<AtualizarCliente>();
builder.Services.AddTransient<ListarClientes>();
builder.Services.AddTransient<RemoverCliente>();
builder.Services.AddTransient<PegarClientePeloNif>();
builder.Services.AddTransient<PegarClientePeloTexto>();
//casos de uso perfilmodel
builder.Services.AddTransient<ListarPerfis>();
//casos de uso servicosmodel
builder.Services.AddTransient<AdicionarServicos>();
builder.Services.AddTransient<AtualizarServicos>(); 
builder.Services.AddTransient<RemoverServico>();
builder.Services.AddTransient<ListarServicos>();
builder.Services.AddTransient<PegarServicoPeloId>();
builder.Services.AddTransient<PegarServicoPeloTexto>();
//casos de uso pacientemodel
builder.Services.AddTransient<AdicionarPaciente>();
builder.Services.AddTransient<AtualizarPaciente>();
builder.Services.AddTransient<RemoverPaciente>();
builder.Services.AddTransient<ListarPacientes>();
builder.Services.AddTransient<PegarPacientePeloID>();
builder.Services.AddTransient<PegarPacientePeloTexto>();
//caso de uso pagamento
builder.Services.AddTransient<ListarPagamentos>();
builder.Services.AddTransient<AdicionarPagamento>();
//casos de uso medico
builder.Services.AddTransient<ListarMedicos>();
//casos de uso consulta
builder.Services.AddTransient<AdicionarConsulta>();
builder.Services.AddTransient<AtualizarConsulta>();
builder.Services.AddTransient<ListarConsultas>();
builder.Services.AddTransient<ListarConsultasDoMedico>();
builder.Services.AddTransient<RemoverConsulta>();
builder.Services.AddTransient<PegarConsultaPeloId>();
//casos de uso especialidade
builder.Services.AddTransient<AdicionarEspecialidade>();
builder.Services.AddTransient<AtualizarEspecialidade>(); 
builder.Services.AddTransient<RemoverEspecialidade>();
builder.Services.AddTransient<ListarEspecialidade>();
builder.Services.AddTransient<PegarEspecialidadePeloId>();
builder.Services.AddTransient<PegarEspecialidadePeloTexto>();
//casos de uso estadoconsulta
builder.Services.AddTransient<ListarEstadoConsulta>();
//casos de uso funcionario
builder.Services.AddTransient<AdicionarFuncionarios>();
builder.Services.AddTransient<ListarFuncionario>(); 
builder.Services.AddTransient<PegarFuncionaarioPeloNif>();
builder.Services.AddTransient<PegarFuncionarioPeloTexto>();
builder.Services.AddTransient<AtualizarFuncionario>();
builder.Services.AddTransient<RemoverFuncionario>();
//casos de uso pagamentoconsulta
builder.Services.AddTransient<ListarPagamentoConsulta>();
builder.Services.AddTransient<AdicionarPagamentoConsulta>();
//contratos SMS
// contratos SMS
builder.Services.AddTransient<ListarSMS>();
builder.Services.AddTransient<AdicionarSMS>();

builder.Services.AddTransient<IPasswordCreate, PasswordCreateService>();
builder.Services.AddTransient<IPasswordHash, PasswordHashService>();

builder.Services.AddHttpClient<ISmsService, SmsService>();

builder.Services.AddTransient<ITokenService,JwtTokenService>();

builder.Services.AddTransient<LoginUsuario>();

builder.Services.AddTransient<IPasswordVerify, PasswordVerifyService>();

// 2. SERVIÇOS — adicionar junto aos outros builder.Services:
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddTransient<CriarPedido>();
builder.Services.AddTransient<ConfirmarPedido>();
builder.Services.AddTransient<CancelarPedido>();
builder.Services.AddTransient<ValidarPagamento>();
builder.Services.AddTransient<RejeitarComprovativo>();
builder.Services.AddTransient<ListarPedidos>();

// Background service que cancela pedidos com prazo expirado (verifica a cada 5 minutos)
builder.Services.AddHostedService<PrazoAgendamentoService>();

// Permitir upload de ficheiros até 5MB
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 5_000_000);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();