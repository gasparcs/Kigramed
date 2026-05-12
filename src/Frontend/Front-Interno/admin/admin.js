function parseJwt(token) {
  if (!token) return null;
  try { return JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))); } catch { return null; }
}

function getUserId() {
  const payload = parseJwt(token);
  return payload?.nameid || payload?.sub || null;
}

function validateAccess() {
  if (!token || role !== 'Admin') redirectToLogin();
  document.getElementById('sidebarName').textContent = nome;
  document.getElementById('topbarName').textContent = nome;
  document.getElementById('sidebarAvatar').textContent = nome.slice(0, 2).toUpperCase();
}

function showSection(sectionId, button) {
  document.querySelectorAll('.section').forEach((el) => el.classList.remove('active'));
  document.getElementById(`sec-${sectionId}`)?.classList.add('active');
  document.getElementById('topbarTitle').textContent = button?.textContent.trim() || sectionId;
  document.querySelectorAll('.nav-item').forEach((el) => el.classList.remove('active'));
  if (button) button.classList.add('active');
  if (window.innerWidth <= 900) sidebarToggle();
}

function openModal(id) { document.getElementById(id)?.classList.add('open'); }
function closeModal(id) { document.getElementById(id)?.classList.remove('open'); }

let actionFormHandler = null;
let actionConfirmHandler = null;

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

function openActionConfirmModal(title, message, onConfirm) {
  document.getElementById('actionConfirmTitle').textContent = title || 'Confirmar Ação';
  document.getElementById('actionConfirmMessage').textContent = message || 'Tem a certeza?';
  actionConfirmHandler = onConfirm || null;
  openModal('modalActionConfirm');
}

function badgeEstado(estado) {
  const texto = estado || '-';
  const map = {
    'Aguarda Confirmacao': 'badge-amber',
    'Aguarda Pagamento': 'badge-amber',
    'Pago': 'badge-green',
    'Cancelado': 'badge-red',
    'Concluido': 'badge-blue',
    'Activo': 'badge-green',
    'Ativo': 'badge-green',
    'Inactivo': 'badge-red',
    'Inativo': 'badge-red',
  };
  return `<span class="badge ${map[texto] || 'badge-gray'}">${texto}</span>`;
}

function normalizePedidoEstado(rawEstado) {
  const estado = String(rawEstado || '').trim().toLowerCase();
  if (estado === 'pendente') return 'Pendente';
  if (estado === 'comprovativo enviado' || estado === 'pagamento enviado') return 'Pagamento Enviado';
  if (estado === 'confirmado' || estado === 'validado') return 'Validado';
  if (estado === 'cancelado') return 'Cancelado';
  if (estado === 'rejeitado') return 'Rejeitado';
  return rawEstado || '—';
}

function renderPedidoActions(id, estadoNormalizado) {
  if (estadoNormalizado === 'Pendente') {
    return `<button class="btn btn-sm btn-success" onclick="handlePedidoAction(${id}, 'confirmar')">Confirmar</button><button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${id}, 'cancelar')">Cancelar</button>`;
  }
  if (estadoNormalizado === 'Pagamento Enviado') {
    return `<button class="btn btn-sm btn-primary" onclick="handlePedidoAction(${id}, 'validar')">Validar</button><button class="btn btn-sm btn-danger" onclick="handlePedidoAction(${id}, 'rejeitar')">Rejeitar</button><button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Comprovativo</button>`;
  }
  if (estadoNormalizado === 'Validado') {
    return `<button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Comprovativo</button>`;
  }
  return '';
}

function filterTable(input, tableId) {
  const query = input.value.toLowerCase();
  document.querySelectorAll(`#${tableId} tbody tr`).forEach((row) => {
    row.style.display = row.textContent.toLowerCase().includes(query) ? '' : 'none';
  });
}

function pickValue(item, keys) {
  for (const key of keys) if (item && item[key] !== undefined && item[key] !== null) return item[key];
  return '';
}

function setSelectOptions(selectId, items, valueKeys, labelKeys) {
  const select = document.getElementById(selectId);
  if (!select) return;
  const v = Array.isArray(valueKeys) ? valueKeys : [valueKeys];
  const l = Array.isArray(labelKeys) ? labelKeys : [labelKeys];
  select.innerHTML = '<option value="">Seleccionar...</option>' + (items || []).map((item) => {
    const value = pickValue(item, v);
    const label = pickValue(item, l) || value;
    return `<option value="${value}">${label}</option>`;
  }).join('');
}

function renderComprovativoValue(value) {
  return value ? 'Comprovativo' : '—';
}

function renderComprovativoCell(value) {
  if (!value) return '—';
  const safe = String(value).replace(/'/g, "\\'");
  return `<button class="btn btn-sm btn-outline" onclick="openComprovativoFromPath('${safe}')">Ver comprovativo</button>`;
}

function getPrecoServico(item) {
  return item?.servicoPreco
    ?? item?.ServicoPreco
    ?? item?.precoServico
    ?? item?.PrecoServico
    ?? item?.preco
    ?? item?.Preco
    ?? item?.valor
    ?? item?.Valor
    ?? '—';
}

function toDatetimeLocal(value) {
  if (!value) return '';
  const d = new Date(value);
  if (Number.isNaN(d.valueOf())) return '';
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  const hh = String(d.getHours()).padStart(2, '0');
  const mi = String(d.getMinutes()).padStart(2, '0');
  return `${yyyy}-${mm}-${dd}T${hh}:${mi}`;
}

// ─────────────────────────────────────────────────────────────────────────────
// ESTADO ADMIN
// ─────────────────────────────────────────────────────────────────────────────
const adminState = {
  consultas: new Map(),
  pacientes: new Map(),
  clientes: new Map()
};

// ─────────────────────────────────────────────────────────────────────────────
// INIT
// ─────────────────────────────────────────────────────────────────────────────
async function initAdmin() {
  validateAccess();
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
}

// ─────────────────────────────────────────────────────────────────────────────
// LOAD FUNCTIONS
// ─────────────────────────────────────────────────────────────────────────────
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

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  const dashBody = document.getElementById('dashConsultas');
  if (!body && !dashBody) return;
  try {
    const items = await fetchJson('/Admin/consulta');
    const list = items || [];

    // Guardar no estado para edição
    adminState.consultas.clear();
    list.forEach((item, index) => {
      const id = item.consultaId || item.ConsultaId || index + 1;
      adminState.consultas.set(id, item);
    });

    if (body) {
      body.innerHTML = list.length
        ? list.map((item, i) => `<tr>
            <td>${item.consultaId || item.ConsultaId || i + 1}</td>
            <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
            <td>${item.medicoNome || item.MedicoNome || '—'}</td>
            <td>${item.servicoNome || item.ServicoNome || '—'}</td>
            <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
            <td>${badgeEstado(item.estadoDescricao || item.EstadoDescricao || '—')}</td>
            <td>
              <button class="btn btn-sm btn-outline" onclick="editarConsulta(${item.consultaId || item.ConsultaId})">Editar</button>
              <button class="btn btn-sm btn-danger" onclick="removerConsulta(${item.consultaId || item.ConsultaId})">Remover</button>
            </td>
          </tr>`).join('')
        : '<tr><td colspan="7" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
    }

    if (dashBody) {
      dashBody.innerHTML = list.length
        ? list.slice(0, 5).map((item, i) => `<tr>
            <td>${item.consultaId || item.ConsultaId || i + 1}</td>
            <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
            <td>${item.medicoNome || item.MedicoNome || '—'}</td>
            <td>${item.servicoNome || item.ServicoNome || '—'}</td>
            <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
            <td>${badgeEstado(item.estadoDescricao || item.EstadoDescricao || '—')}</td>
          </tr>`).join('')
        : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
    }
  } catch {
    if (body) body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
    if (dashBody) dashBody.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/paciente');

    // Guardar no estado para edição
    adminState.pacientes.clear();
    (items || []).forEach((item, index) => {
      const id = item.pacienteId || item.Id || index + 1;
      adminState.pacientes.set(id, item);
    });

    body.innerHTML = (items || []).length
      ? items.map((item, i) => `<tr>
          <td>${item.pacienteId || item.Id || i + 1}</td>
          <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
          <td>${formatDateShort(item.pacienteData_nascimento || item.Data_nascimento || item.DataNascimento)}</td>
          <td>${item.cliente || item.Cliente || item.clienteNome || item.ClienteNome || item.nif_cliente || item.Nif_cliente || '—'}</td>
          <td>
            <button class="btn btn-sm btn-outline" onclick="editarPaciente(${item.pacienteId || item.Id})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="removerPaciente(${item.pacienteId || item.Id})">Remover</button>
          </td>
        </tr>`).join('')
      : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum paciente encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pacientes.</td></tr>';
  }
}

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/cliente');

    // Guardar no estado para edição
    adminState.clientes.clear();
    (items || []).forEach((item) => {
      const nif = item.clienteNif || item.Nif_cliente || '';
      adminState.clientes.set(nif, item);
    });

    body.innerHTML = (items || []).length
      ? items.map((item) => `<tr>
          <td>${item.clienteNif || item.Nif_cliente || '—'}</td>
          <td>${item.clienteNome || item.Nome || '—'}</td>
          <td>${Array.isArray(item.contactos) ? item.contactos.map((c) => c.contacto || c.Contacto).join(', ') : '—'}</td>
          <td>
            <button class="btn btn-sm btn-outline" onclick="editarCliente('${item.clienteNif || item.Nif_cliente}')">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="removerCliente('${item.clienteNif || item.Nif_cliente}')">Remover</button>
          </td>
        </tr>`).join('')
      : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum cliente encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar clientes.</td></tr>';
  }
}

async function loadFuncionarios() {
  const body = document.getElementById('bodyFuncionarios');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/funcionario');
    body.innerHTML = (items || []).length ? items.map((item) => {
      const nif = item.funcionarioNif || item.FuncionarioNif || item.Nif || item.Nif_funcionario || '—';
      const perfil = item.fUncionarioPerfil || item.FUncionarioPerfil || item.funcionarioPerfil || '—';
      const estado = item.funcionaroEstado ?? item.FuncionaroEstado ?? item.funcionarioEstado;
      const estadoTxt = estado === true ? badgeEstado('Activo') : estado === false ? badgeEstado('Inactivo') : '—';
      const contactos = Array.isArray(item.contactos) ? item.contactos.map((c) => c.contacto || c.Contacto).filter(Boolean).join(', ') : '—';
      return `<tr>
        <td>${nif}</td>
        <td>${item.funcionarioNome || item.Nome || '—'}</td>
        <td>${perfil}</td>
        <td>${contactos || '—'}</td>
        <td>${estadoTxt}</td>
        <td>
          <button class="btn btn-sm btn-outline" onclick="editarFuncionario('${nif}')">Editar</button>
          <button class="btn btn-sm btn-danger" onclick="removerFuncionario('${nif}')">Remover</button>
        </td>
      </tr>`;
    }).join('') : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum funcionário encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar funcionários.</td></tr>';
  }
}

async function loadEspecialidades() {
  const body = document.getElementById('bodyEspecialidades');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/especialidade');
    body.innerHTML = (items || []).length
      ? items.map((item, i) => `<tr>
          <td>${item.especialidadeId || item.Id || i + 1}</td>
          <td>${item.especialidadeNome || item.Nome || item.NomeEspecialidade || '—'}</td>
          <td>${item.especialidadeDescricao || item.Descricao || '—'}</td>
          <td>
            <button class="btn btn-sm btn-outline" onclick="editarEspecialidade(${item.especialidadeId || item.Id})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="removerEspecialidade(${item.especialidadeId || item.Id})">Remover</button>
          </td>
        </tr>`).join('')
      : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhuma especialidade encontrada.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar especialidades.</td></tr>';
  }
}

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/servicos');
    body.innerHTML = (items || []).length
      ? items.map((item, i) => `<tr>
          <td>${item.servicoId || item.ServicoId || item.Id || i + 1}</td>
          <td>${item.servicoNome || item.ServicoNome || item.nome || '—'}</td>
          <td>${item.nomeEspecialidade || item.NomeEspecialidade || item.especialidade || item.Especialidade || '—'}</td>
          <td>${item.servicoPreco ?? item.Preco ?? '—'}</td>
          <td>
            <button class="btn btn-sm btn-outline" onclick="editarServico(${item.servicoId || item.ServicoId || item.Id})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="removerServico(${item.servicoId || item.ServicoId || item.Id})">Remover</button>
          </td>
        </tr>`).join('')
      : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviço encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar serviços.</td></tr>';
  }
}

async function loadPagamentos() {
  const body = document.getElementById('bodyPagamentos');
  if (!body) return;
  try {
    const [pagamentos, pagamentosConsulta] = await Promise.all([
      fetchJson('/Admin/pagamento'),
      fetchJson('/Admin/pagamentoconsulta')
    ]);
    const pagamentoValorMap = new Map((pagamentosConsulta || []).map((pc) => [pc.idPagamento || pc.IdPagamento, pc.valorServico ?? pc.ValorServico ?? '—']));
    const items = pagamentos || [];
    body.innerHTML = items.length
      ? items.map((item) => {
          const pagamentoId = item.id || item.Id;
          const precoServico = pagamentoValorMap.get(pagamentoId) ?? '—';
          return `<tr>
            <td>${pagamentoId || '—'}</td>
            <td>${item.cliente || item.Cliente || '—'}</td>
            <td>${item.secretaria || item.Secretaria || '—'}</td>
            <td>${precoServico}</td>
            <td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td>
            <td>${formatDate(item.dataEnvio || item.DataEnvio || item.Data)}</td>
          </tr>`;
        }).join('')
      : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pagamento encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos.</td></tr>';
  }
}

async function loadPagamentoConsulta() {
  const body = document.getElementById('bodyPagamentoConsulta');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/pagamentoconsulta');
    body.innerHTML = (items || []).length
      ? items.map((item) => `<tr>
          <td>${item.id || item.Id || '—'}</td>
          <td>${item.idPagamento || item.IdPagamento || '—'}</td>
          <td>${item.idConsulta || item.IdConsulta || '—'}</td>
          <td>${formatDate(item.dataConsulta || item.DataConsulta)}</td>
          <td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td>
        </tr>`).join('')
      : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum registo encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos de consulta.</td></tr>';
  }
}

async function loadSMS() {
  const body = document.getElementById('bodySMS');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/SMS');
    body.innerHTML = (items || []).length
      ? items.map((item) => `<tr>
          <td>${item.smsId || item.SmsId || '—'}</td>
          <td>${item.cliente?.clienteNome || item.cliente?.ClienteNome || '—'}</td>
          <td>${item.mensagem || item.Mensagem || '—'}</td>
          <td>${formatDate(item.dataEnvio || item.DataEnvio)}</td>
        </tr>`).join('')
      : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum SMS enviado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar SMS.</td></tr>';
  }
}

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Admin/pedidos');
    const items = res?.dados || [];
    body.innerHTML = items.length
      ? items.map((item) => {
          const id = item.id || item.Id || '—';
          const cliente = item.nomeCliente || item.NomeCliente || item.clienteNome || item.ClienteNome || '—';
          const servico = item.servico || item.Servico || item.servicoNome || item.ServicoNome || '—';
          const horario = item.horarioPreferencial || item.HorarioPreferencial || item.data_consulta || item.Data_consulta || item.DataConsulta;
          const estado = normalizePedidoEstado(item.estado || item.Estado || '—');
          const actions = renderPedidoActions(id, estado);
          return `<tr>
            <td>${id}</td>
            <td>${cliente}</td>
            <td>${servico}</td>
            <td>${formatDate(horario)}</td>
            <td>${badgeEstado(estado)}</td>
            <td style="display:flex;gap:6px;flex-wrap:wrap;">${actions || '<span style="color:var(--muted)">—</span>'}</td>
          </tr>`;
        }).join('')
      : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pedidos.</td></tr>';
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// POPULATE SELECTS
// ─────────────────────────────────────────────────────────────────────────────
async function populateFormSelects() {
  try {
    const [clientes, medicos, especialidades, pagamentos, consultas, estados, perfis, servicos, pacientes] = await Promise.all([
      fetchJson('/Admin/cliente'),
      fetchJson('/Admin/medicos'),
      fetchJson('/Admin/especialidade'),
      fetchJson('/Admin/pagamento'),
      fetchJson('/Admin/consulta'),
      fetchJson('/Admin/estados'),
      fetchJson('/Admin/perfil'),
      fetchJson('/Admin/servicos'),
      fetchJson('/Admin/paciente')
    ]);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', servicos || [], ['servicoId', 'ServicoId', 'id', 'Id'], ['servicoNome', 'ServicoNome', 'nome', 'Nome']);
    setSelectOptions('cPaciente', pacientes || [], ['id', 'Id', 'pacienteId', 'PacienteId'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('cEstado', estados || [], ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
    setSelectOptions('svcEsp', especialidades || [], ['especialidadeId', 'EspecialidadeId', 'id', 'Id'], ['especialidadeNome', 'EspecialidadeNome', 'nome', 'Nome']);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('fPerfil', perfis || [], ['perfilId', 'PerfilId'], ['perfilNome', 'PerfilNome']);
    setSelectOptions('fEspecialidade', especialidades || [], ['especialidadeId', 'EspecialidadeId'], ['especialidadeNome', 'EspecialidadeNome']);
    setSelectOptions('payCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('pcPagamento', pagamentos || [], ['id', 'Id'], ['comprovativo', 'Comprovativo']);
    setSelectOptions('pcConsulta', consultas || [], ['consultaId', 'ConsultaId', 'id', 'Id'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('smsCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    // Selects dos modais de edição
    setSelectOptions('ecMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('ecEstado', estados || [], ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
  } catch (error) {
    console.warn('Falha ao preencher selects', error);
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// SUBMITS — CRIAR
// ─────────────────────────────────────────────────────────────────────────────
async function submitConsulta(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/consulta', {
      method: 'POST',
      body: {
        Id_medico_especialiade: parseInt(document.getElementById('cMedico').value, 10),
        Id_servico: parseInt(document.getElementById('cServico').value, 10),
        Id_paciente: parseInt(document.getElementById('cPaciente').value, 10),
        Id_estado_consulta: parseInt(document.getElementById('cEstado').value, 10),
        Data_consulta: document.getElementById('cData').value
      }
    });
    showToast('Consulta criada com sucesso.', 'success');
    closeModal('modalConsulta');
    loadConsultas();
    loadTopLists();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar consulta.'), 'error'); }
}

async function submitPaciente(event) {
  event.preventDefault();
  try {
    const nifCliente = document.getElementById('pCliente').value;
    await fetchJson('/Admin/paciente', {
      method: 'POST',
      body: {
        PacienteNome: document.getElementById('pNome').value,
        PacienteData_nascimento: document.getElementById('pNasc').value,
        IdCliente_Paciente: parseInt(document.getElementById('pTipo')?.value || '1', 10) || 1,
        IdGenero: parseInt(document.getElementById('pGenero').value, 10),
        Nif_cliente: nifCliente
      }
    });
    showToast('Paciente criado com sucesso.', 'success');
    closeModal('modalPaciente');
    loadPacientes();
    loadTopLists();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar paciente.'), 'error'); }
}

async function submitCliente(event) {
  event.preventDefault();
  try {
    const payload = {
      ClienteNome: document.getElementById('clNome').value.trim(),
      ClienteNif: document.getElementById('clNif').value.trim(),
      Contactos: []
    };
    const email = document.getElementById('clEmail').value.trim();
    const tel = document.getElementById('clTel').value.trim();
    if (email) payload.Contactos.push({ TipoContacto: 2, Contacto: email });
    if (tel) payload.Contactos.push({ TipoContacto: 1, Contacto: tel });
    if (!payload.Contactos.length) return showToast('Informe pelo menos um contacto.', 'error');
    await fetchJson('/Admin/cliente', { method: 'POST', body: payload });
    showToast('Cliente criado com sucesso.', 'success');
    closeModal('modalCliente');
    loadClientes();
    populateFormSelects();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar cliente.'), 'error'); }
}

async function submitFuncionario(event) {
  event.preventDefault();
  try {
    const payload = {
      FuncionarioNome: document.getElementById('fNome').value,
      FuncionaioNif: document.getElementById('fNif').value,
      FuncionarioPerfil: parseInt(document.getElementById('fPerfil').value, 10),
      FuncionarioEstado: document.getElementById('fEstado').value === 'true',
      Contactos: [],
      Especialidades: []
    };
    const tel = document.getElementById('fTel').value;
    const esp = document.getElementById('fEspecialidade').value;
    if (tel) payload.Contactos.push({ TipoContacto: 1, Contacto: tel });
    if (esp) payload.Especialidades.push({ IdEspecialidade: parseInt(esp, 10) });
    await fetchJson('/Admin/funcionario', { method: 'POST', body: payload });
    showToast('Funcionário criado com sucesso.', 'success');
    closeModal('modalFuncionario');
    loadFuncionarios();
    loadTopLists();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar funcionário.'), 'error'); }
}

async function submitEspecialidade(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/especialidade', {
      method: 'POST',
      body: {
        EspecialidadeNome: document.getElementById('espNome').value,
        EspecialidadeDescricao: document.getElementById('espDesc').value,
        EspecialidadeEstado: document.getElementById('espEstado').value === 'true'
      }
    });
    showToast('Especialidade criada com sucesso.', 'success');
    closeModal('modalEspecialidade');
    loadEspecialidades();
    populateFormSelects();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar especialidade.'), 'error'); }
}

async function submitServico(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/servicos', {
      method: 'POST',
      body: {
        ServicoNome: document.getElementById('svcNome').value,
        IdEspecialidade: parseInt(document.getElementById('svcEsp').value, 10),
        ServicoDuracaoMinuto: parseInt(document.getElementById('svcDuracao').value, 10) || 30,
        ServicoPreco: parseFloat(document.getElementById('svcPreco').value) || 0,
        ServicoEstado: document.getElementById('svcEstado').value === 'true'
      }
    });
    showToast('Serviço criado com sucesso.', 'success');
    closeModal('modalServico');
    loadServicos();
    loadTopLists();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar serviço.'), 'error'); }
}

async function submitPagamento(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/pagamento', {
      method: 'POST',
      body: {
        IdCliente: document.getElementById('payCliente').value,
        IdSecretaria: localStorage.getItem('nif') || getUserId() || 1,
        Comprovativo: document.getElementById('payComprovativo').value,
        Data: document.getElementById('payData').value || new Date().toISOString()
      }
    });
    showToast('Pagamento registado com sucesso.', 'success');
    closeModal('modalPagamento');
    loadPagamentos();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar pagamento.'), 'error'); }
}

async function submitPagamentoConsulta(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/pagamentoconsulta', {
      method: 'POST',
      body: {
        IdPagamento: parseInt(document.getElementById('pcPagamento').value, 10),
        IdConsulta: parseInt(document.getElementById('pcConsulta').value, 10)
      }
    });
    showToast('Pagamento de consulta registado.', 'success');
    closeModal('modalPagamentoConsulta');
    loadPagamentoConsulta();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao registar pagamento de consulta.'), 'error'); }
}

async function submitSMS(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/SMS', {
      method: 'POST',
      body: {
        SMSMensagem: document.getElementById('smsMensagem').value,
        SMSNif_funcionario: localStorage.getItem('nif') || getUserId() || '',
        SMSNif_cliente: document.getElementById('smsCliente').value
      }
    });
    showToast('SMS enviado com sucesso.', 'success');
    closeModal('modalSMS');
    loadSMS();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao enviar SMS.'), 'error'); }
}

// ─────────────────────────────────────────────────────────────────────────────
// REMOVER
// ─────────────────────────────────────────────────────────────────────────────
async function removerConsulta(id) {
  openActionConfirmModal('Remover Consulta', 'Tem a certeza que deseja remover esta consulta?', async () => {
    try {
      await fetchJson(`/Admin/consulta/${id}`, { method: 'DELETE' });
      showToast('Consulta removida.', 'success');
      loadConsultas();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover consulta.'), 'error'); }
  });
}

async function removerPaciente(id) {
  openActionConfirmModal('Remover Paciente', 'Tem a certeza que deseja remover este paciente?', async () => {
    try {
      await fetchJson(`/Admin/paciente/${id}`, { method: 'DELETE' });
      showToast('Paciente removido.', 'success');
      loadPacientes();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover paciente.'), 'error'); }
  });
}

async function removerCliente(nif) {
  openActionConfirmModal('Remover Cliente', 'Tem a certeza que deseja remover este cliente?', async () => {
    try {
      await fetchJson(`/Admin/cliente/${nif}`, { method: 'DELETE' });
      showToast('Cliente removido.', 'success');
      loadClientes();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover cliente.'), 'error'); }
  });
}

async function removerFuncionario(nif) {
  openActionConfirmModal('Remover Funcionário', 'Tem a certeza que deseja remover este funcionário?', async () => {
    try {
      await fetchJson(`/Admin/funcionario/${nif}`, { method: 'DELETE' });
      showToast('Funcionário removido com sucesso.', 'success');
      loadFuncionarios();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover funcionário.'), 'error'); }
  });
}

async function removerEspecialidade(id) {
  openActionConfirmModal('Remover Especialidade', 'Tem a certeza que deseja remover esta especialidade?', async () => {
    try {
      await fetchJson(`/Admin/especialidade/${id}`, { method: 'DELETE' });
      showToast('Especialidade removida.', 'success');
      loadEspecialidades();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover especialidade.'), 'error'); }
  });
}

async function removerServico(id) {
  openActionConfirmModal('Remover Serviço', 'Tem a certeza que deseja remover este serviço?', async () => {
    try {
      await fetchJson(`/Admin/servicos/${id}`, { method: 'DELETE' });
      showToast('Serviço removido.', 'success');
      loadServicos();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover serviço.'), 'error'); }
  });
}

// ─────────────────────────────────────────────────────────────────────────────
// EDITAR CONSULTA — modal dedicado (igual à Secretaria)
// ─────────────────────────────────────────────────────────────────────────────
async function editarConsulta(id) {
  const item = adminState.consultas.get(id) || {};
  document.getElementById('ecConsultaId').value = id;
  document.getElementById('ecData').value = toDatetimeLocal(
    item.data_consulta || item.Data_consulta || item.DataConsulta || ''
  );
  document.getElementById('ecMedico').value = pickValue(item, [
    'id_medico_especialiade', 'Id_medico_especialiade',
    'idMedicoEspecialidade', 'IdMedicoEspecialidade',
    'idMedico', 'IdMedico'
  ]) || '';
  document.getElementById('ecEstado').value = pickValue(item, [
    'id_estado_consulta', 'Id_estado_consulta',
    'idEstadoConsulta', 'IdEstadoConsulta'
  ]) || '';
  openModal('modalEditConsulta');
}

async function submitEditConsulta(event) {
  event.preventDefault();
  const id       = parseInt(document.getElementById('ecConsultaId').value, 10);
  const medicoId = parseInt(document.getElementById('ecMedico').value, 10);
  const estadoId = parseInt(document.getElementById('ecEstado').value, 10);
  const data     = document.getElementById('ecData').value;
  try {
    await fetchJson(`/Admin/consulta/${id}`, {
      method: 'PUT',
      body: { IdConsulta: id, Id_medico_especialiade: medicoId, Id_estado_consulta: estadoId, Data_consulta: data }
    });
    showToast('Consulta atualizada com sucesso.', 'success');
    closeModal('modalEditConsulta');
    loadConsultas();
    loadTopLists();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar consulta.'), 'error');
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// EDITAR PACIENTE — modal dedicado (igual à Secretaria)
// ─────────────────────────────────────────────────────────────────────────────
async function editarPaciente(id) {
  const item = adminState.pacientes.get(id) || {};
  document.getElementById('epPacienteId').value = id;
  document.getElementById('epNome').value = pickValue(item, ['pacienteNome', 'PacienteNome', 'Nome']) || '';
  openModal('modalEditPaciente');
}

async function submitEditPaciente(event) {
  event.preventDefault();
  const id   = parseInt(document.getElementById('epPacienteId').value, 10);
  const nome = document.getElementById('epNome').value.trim();
  try {
    await fetchJson(`/Admin/paciente/${id}`, {
      method: 'PUT',
      body: { IdPaciente: id, PacienteNome: nome }
    });
    showToast('Paciente atualizado com sucesso.', 'success');
    closeModal('modalEditPaciente');
    loadPacientes();
    loadTopLists();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar paciente.'), 'error');
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// EDITAR CLIENTE — modal dedicado (igual à Secretaria)
// ─────────────────────────────────────────────────────────────────────────────
async function editarCliente(nif) {
  const item = adminState.clientes.get(nif) || {};
  const contactos = Array.isArray(item.contactos) ? item.contactos : [];
  const emailObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 2);
  const telObj   = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 1);
  document.getElementById('eclNif').value   = nif;
  document.getElementById('eclNome').value  = pickValue(item, ['clienteNome', 'ClienteNome', 'Nome']) || '';
  document.getElementById('eclEmail').value = (emailObj?.contacto || emailObj?.Contacto || '').trim();
  document.getElementById('eclTel').value   = (telObj?.contacto   || telObj?.Contacto   || '').trim();
  openModal('modalEditCliente');
}

async function submitEditCliente(event) {
  event.preventDefault();
  const nif   = document.getElementById('eclNif').value.trim();
  const nome  = document.getElementById('eclNome').value.trim();
  const email = document.getElementById('eclEmail').value.trim();
  const tel   = document.getElementById('eclTel').value.trim();
  const contactos = [];
  if (tel)   contactos.push({ TipoContacto: 1, Contacto: tel });
  if (email) contactos.push({ TipoContacto: 2, Contacto: email });
  if (!contactos.length) {
    showToast('Informe pelo menos um contacto (telefone ou email).', 'error');
    return;
  }
  try {
    await fetchJson(`/Admin/cliente/${nif}`, {
      method: 'PUT',
      body: { Nif_cliente: nif, Nome: nome, Contactos: contactos }
    });
    showToast('Cliente atualizado com sucesso.', 'success');
    closeModal('modalEditCliente');
    loadClientes();
    populateFormSelects();
    loadTopLists();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar cliente.'), 'error');
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// EDITAR — outros (mantém o modal genérico)
// ─────────────────────────────────────────────────────────────────────────────
async function editarFuncionario(nif) {
  openActionFormModal({
    title: 'Editar Funcionário',
    showDescricao: false,
    onSubmit: async ({ nome }) => {
      await fetchJson(`/Admin/funcionario/${nif}`, { method: 'PUT', body: { FuncionarioNif: nif, FuncionarioNome: nome } });
      showToast('Funcionário atualizado.', 'success');
      loadFuncionarios();
    }
  });
}

async function editarEspecialidade(id) {
  let atual = null;
  try { atual = await fetchJson(`/Admin/especialidade/id/${id}`); } catch { }
  openActionFormModal({
    title: 'Editar Especialidade',
    showDescricao: true,
    nome: atual?.especialidadeNome || atual?.EspecialidadeNome || '',
    descricao: atual?.especialidadeDescricao || atual?.EspecialidadeDescricao || '',
    onSubmit: async ({ nome, descricao }) => {
      await fetchJson(`/Admin/especialidade/${id}`, {
        method: 'PUT',
        body: {
          EspecialidadeId: id,
          EspecialidadeNome: nome,
          EspecialidadeDescricao: descricao,
          EspecialidadeEstado: (atual?.especialidadeEstado ?? atual?.EspecialidadeEstado ?? true) === true
        }
      });
      showToast('Especialidade atualizada.', 'success');
      loadEspecialidades();
    }
  });
}

async function editarServico(id) {
  openActionFormModal({
    title: 'Editar Serviço',
    showDescricao: false,
    onSubmit: async ({ nome }) => {
      await fetchJson(`/Admin/servicos/${id}`, { method: 'PUT', body: { ServicoId: id, ServicoNome: nome } });
      showToast('Serviço atualizado.', 'success');
      loadServicos();
    }
  });
}

// ─────────────────────────────────────────────────────────────────────────────
// PEDIDOS
// ─────────────────────────────────────────────────────────────────────────────
async function handlePedidoAction(id, action) {
  try {
    const result = await fetchJson(`/Admin/${id}/${action}`, { method: 'PUT' });
    showToast(result.mensagem || 'Operação concluída.', 'success');
    loadPedidos();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Falha na operação.', 'error');
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// COMPROVATIVOS
// ─────────────────────────────────────────────────────────────────────────────
async function openComprovativo(id) {
  try {
    const response = await fetch(`${API}/Admin/${id}/comprovativo`, {
      method: 'GET',
      headers: authHeaders()
    });
    if (!response.ok) {
      let mensagem = 'Não foi possível abrir o comprovativo.';
      try { const data = await response.json(); mensagem = data?.mensagem || mensagem; } catch { }
      showToast(mensagem, 'error');
      return;
    }
    const blob = await response.blob();
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
    setTimeout(() => URL.revokeObjectURL(url), 60000);
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao abrir comprovativo.'), 'error');
  }
}

function openComprovativoFromPath(path) {
  try {
    const raw = String(path || '').trim();
    if (!raw) return showToast('Comprovativo indisponível.', 'error');
    if (/^https?:\/\//i.test(raw)) { window.open(raw, '_blank'); return; }
    const relative = raw.replace(/^\/+/, '');
    window.open(`${API}/${relative}`, '_blank');
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao abrir comprovativo.'), 'error');
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// EVENT LISTENERS GLOBAIS
// ─────────────────────────────────────────────────────────────────────────────
document.getElementById('actionForm')?.addEventListener('submit', async (event) => {
  event.preventDefault();
  if (!actionFormHandler) return;
  const nome     = document.getElementById('actionFieldNome').value.trim();
  const descricao = document.getElementById('actionFieldDescricao').value.trim();
  const data     = document.getElementById('actionFieldData').value;
  try {
    await actionFormHandler({ nome, descricao, data });
    closeModal('modalActionForm');
  } catch (e) {
    showToast(getErrorMessage(e, 'Erro ao guardar alterações.'), 'error');
  }
});

document.getElementById('actionConfirmBtn')?.addEventListener('click', async () => {
  if (!actionConfirmHandler) return;
  try {
    await actionConfirmHandler();
    closeModal('modalActionConfirm');
  } catch (e) {
    showToast(getErrorMessage(e, 'Erro ao executar ação.'), 'error');
  }
});

window.addEventListener('load', initAdmin);