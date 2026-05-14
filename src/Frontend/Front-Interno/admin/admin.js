// ─────────────────────────────────────────────────────────────────────────────
// ADMIN CORE - GESTÃO DE ESTADO E INICIALIZAÇÃO
// ─────────────────────────────────────────────────────────────────────────────

const adminState = {
  consultas: new Map(),
  pacientes: new Map(),
  clientes: new Map(),
  funcionarios: new Map(),
  servicos: new Map()
};

const sectionLoaders = {
  'dashboard': () => loadTopLists(),
  'consultas': () => loadConsultas(),
  'pacientes': () => loadPacientes(),
  'clientes': () => loadClientes(),
  'funcionarios': () => loadFuncionarios(),
  'especialidades': () => loadEspecialidades(),
  'servicos': () => loadServicos(),
  'pagamentos': () => loadPagamentos(),
  'pagamentoconsulta': () => loadPagamentoConsulta(),
  'sms': () => loadSMS(),
  'pedidos': () => loadPedidos()
};

let activeSectionId = 'dashboard';
let pollingIntervalId = null;

function startPolling() {
  if (pollingIntervalId) clearInterval(pollingIntervalId);
  pollingIntervalId = setInterval(() => {
    if (!document.hidden && activeSectionId && sectionLoaders[activeSectionId]) {
      sectionLoaders[activeSectionId]();
    }
  }, 30000);
}

document.addEventListener('visibilitychange', () => {
  if (!document.hidden && activeSectionId && sectionLoaders[activeSectionId]) {
    sectionLoaders[activeSectionId]();
  }
});

async function initAdmin() {
  validateAccess();
  // Carregamento paralelo de dados iniciais
  await Promise.all([
    loadEstados(),
    loadTopLists(),
    loadConsultas(),
    loadPacientes(),
    loadClientes(),
    loadFuncionarios(),
    loadEspecialidades(),
    loadServicos(),
    loadPagamentos(),
    loadPagamentoConsulta(),
    loadSMS(),
    loadPedidos()
  ]);
  await populateFormSelects();
  startPolling();
}

function validateAccess() {
  if (!token || role !== 'Admin') redirectToLogin();
  document.getElementById('sidebarName').textContent = nome;
  document.getElementById('topbarName').textContent = nome;
  document.getElementById('sidebarAvatar').textContent = nome.slice(0, 2).toUpperCase();
}

function showSection(sectionId, button) {
  if (window.innerWidth <= 900) {
    document.getElementById('sidebar')?.classList.remove('open');
    document.getElementById('sidebarOverlay')?.classList.remove('open');
  }
  document.querySelectorAll('.section').forEach((el) => el.classList.remove('active'));
  document.getElementById(`sec-${sectionId}`)?.classList.add('active');
  document.getElementById('topbarTitle').textContent = button?.textContent.trim() || sectionId;
  document.querySelectorAll('.nav-item').forEach((el) => el.classList.remove('active'));
  if (button) button.classList.add('active');
  if (window.innerWidth <= 900) sidebarToggle();

  activeSectionId = sectionId;
  if (sectionLoaders[activeSectionId]) {
    sectionLoaders[activeSectionId]();
  }
}

async function loadTopLists() {
  try {
    const [consultas, pacientes, funcionarios, pagamentos] = await Promise.all([
      fetchJson('/Admin/consulta'),
      fetchJson('/Admin/paciente'),
      fetchJson('/Admin/funcionario'),
      fetchJson('/Admin/pagamento')
    ]);
    document.getElementById('statConsultas').textContent = consultas?.length ?? 0;
    document.getElementById('statPacientes').textContent = pacientes?.length ?? 0;
    document.getElementById('statFuncionarios').textContent = funcionarios?.length ?? 0;
    document.getElementById('statPagamentos').textContent = pagamentos?.length ?? 0;
  } catch { }
}

async function loadEstados() {
  try {
    const estados = await fetchJson('/Admin/estados');
    setSelectOptions('cEstado', estados || [], ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
  } catch { }
}

async function populateFormSelects() {
  try {
    const [clientes, medicos, especialidades, pagamentos, consultas, estados, perfis, servicos, pacientes, tiposClientePaciente] = await Promise.all([
      fetchJson('/Admin/cliente'),
      fetchJson('/Admin/medicos'),
      fetchJson('/Admin/especialidade'),
      fetchJson('/Admin/pagamento'),
      fetchJson('/Admin/consulta'),
      fetchJson('/Admin/estados'),
      fetchJson('/Admin/perfil'),
      fetchJson('/Admin/servicos'),
      fetchJson('/Admin/paciente'),
      fetchJson('/Admin/cliente-paciente').catch(() => [])
    ]);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', servicos || [], ['servicoId', 'ServicoId', 'id', 'Id'], ['servicoNome', 'ServicoNome', 'nome', 'Nome']);
    setSelectOptions('cPaciente', pacientes || [], ['id', 'Id', 'pacienteId', 'PacienteId'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('cEstado', estados || [], ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
    setSelectOptions('svcEsp', especialidades || [], ['especialidadeId', 'EspecialidadeId', 'id', 'Id'], ['especialidadeNome', 'EspecialidadeNome', 'nome', 'Nome']);
    setSelectOptions('esEsp', especialidades || [], ['especialidadeId', 'EspecialidadeId', 'id', 'Id'], ['especialidadeNome', 'EspecialidadeNome', 'nome', 'Nome']);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('fPerfil', perfis || [], ['perfilId', 'PerfilId'], ['perfilDescricao', 'PerfilDescricao']);
    setSelectOptions('fEspecialidade', especialidades || [], ['especialidadeId', 'EspecialidadeId'], ['especialidadeNome', 'EspecialidadeNome']);
    setSelectOptions('payCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('pcPagamento', pagamentos || [], ['id', 'Id'], ['comprovativo', 'Comprovativo']);
    setSelectOptions('pcConsulta', consultas || [], ['consultaId', 'ConsultaId', 'id', 'Id'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('smsCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('efPerfil', perfis || [], ['perfilId', 'PerfilId'], ['perfilDescricao', 'PerfilDescricao']);
    setSelectOptions('efEspecialidade', especialidades || [], ['especialidadeId', 'EspecialidadeId'], ['especialidadeNome', 'EspecialidadeNome']);
    setSelectOptions('ecMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('ecEstado', estados || [], ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
    setSelectOptions('epTipo', tiposClientePaciente || [], ['id', 'Id'], ['descricao', 'Descricao']);
  } catch (error) {
    console.warn('Falha ao preencher selects', error);
  }
}

// Helpers específicos do Admin
function parseJwt(token) {
  if (!token) return null;
  try { return JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))); } catch { return null; }
}

function getUserId() {
  const payload = parseJwt(token);
  return payload?.nameid || payload?.sub || null;
}

// Ouvintes Globais
window.addEventListener('load', initAdmin);

// Modal Genérico (Ainda usado por algumas funcionalidades menores)
let actionFormHandler = null;
function openActionFormModal(config) {
  document.getElementById('actionFormTitle').textContent = config.title || 'Editar Registo';
  document.getElementById('actionFieldNome').value = config.nome || '';
  document.getElementById('actionFieldDescricao').value = config.descricao || '';
  document.getElementById('actionFieldData').value = config.data || '';
  document.getElementById('actionGroupDescricao').style.display = config.showDescricao ? '' : 'none';
  document.getElementById('actionGroupData').style.display = config.showData ? '' : 'none';
  actionFormHandler = config.onSubmit || null;
  openModal('modalActionForm');
}

document.getElementById('actionForm')?.addEventListener('submit', async (event) => {
  event.preventDefault();
  if (!actionFormHandler) return;
  const nome = document.getElementById('actionFieldNome').value.trim();
  const descricao = document.getElementById('actionFieldDescricao').value.trim();
  const data = document.getElementById('actionFieldData').value;
  try {
    await actionFormHandler({ nome, descricao, data });
    closeModal('modalActionForm');
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao guardar alterações.'), 'error');
  }
});