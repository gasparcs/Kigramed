using Backend.K02.INFRA.Data;
using Backend.K02.INFRA.Repository.Cliente;
using Backend.K02.INFRA.Repository.Consulta;
using Backend.K02.INFRA.Repository.Especialidade;
using Backend.K02.INFRA.Repository.EstadoConsulta;
using Backend.K02.INFRA.Repository.Funcionario;
using Backend.K02.INFRA.Repository.MedicoEspecialidade;
using Backend.K02.INFRA.Repository.Paciente;
using Backend.K02.INFRA.Repository.Pagamento;
using Backend.K02.INFRA.Repository.Perfil;
using Backend.K02.INFRA.Repository.Servicos;
using Backend.K03.APPLICATION.ClienteUseCase.comand;
using Backend.K03.APPLICATION.ClienteUseCase.Queries;
using Backend.K03.APPLICATION.MedicoEspecialidadeUseCase.Queries;
using Backend.K03.APPLICATION.PacienteUseCase.Comand;
using Backend.K03.APPLICATION.PacienteUseCase.Queries;
using Backend.K03.APPLICATION.PagamentoUseCase.Queries;
using Backend.K03.APPLICATION.PerfilUseCase.Queries;
using Backend.K03.APPLICATION.ServicosUseCase.Comand;
using Backend.K03.APPLICATION.ServicosUseCase.Queries;
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
using Backend.K04.DOMAIN.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
string conexao= builder.Configuration.GetConnectionString("ConexaoLocal")!;
builder.Services.AddDbContext<KigramedDbContext>(options => options.UseNpgsql(conexao));

//Contratos do Cliente 
builder.Services.AddScoped<ICadastrarRepository<ClienteModel>, AdicionarClienteRepository>();
builder.Services.AddScoped<IAtualizarRepository<ClienteModel>, AtualizarClienteRepository>();
builder.Services.AddScoped<IlistagemRepository<ClienteModel>, ListarClienteRepository>();
builder.Services.AddScoped<IPegarPeloNifRepository<ClienteModel>, PegarClientePeloNifRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<ClienteModel>,PegarClientePeloTextoRepository >();
builder.Services.AddScoped<IRemoverRepository<ClienteModel>, RemoverClienteRepository>();
//Contratos da Consulta
builder.Services.AddScoped<ICadastrarRepository<ConsultaModel>, AdicionarConsultaRepository>();
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
builder.Services.AddScoped<IPegarPeloTextoRepository<FuncionarioModel>, PegarFuncionarioPeloTexto>();
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

//Contratos para o Serviço
builder.Services.AddScoped<ICadastrarRepository<ServicosModel>, AdicionarServicoRepository>();
builder.Services.AddScoped<IAtualizarRepository<ServicosModel>, AtualizarServicosRepository>();
builder.Services.AddScoped<IlistagemRepository<ServicosModel>,ListarServicosRepository>();
builder.Services.AddScoped<IPesquisarPeloIdRepository<ServicosModel>, PegarIdServicosRepository>();
builder.Services.AddScoped<IPegarPeloTextoRepository<ServicosModel>, PegarTextoServicosRepository>();
builder.Services.AddScoped<IRemoverRepository<ServicosModel>, RemoverServicosRepository>();


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
//casos de uso medico
builder.Services.AddTransient<ListarMedicos>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
