function validateSecretariaAccess() {
  if (!token || role !== 'Secretaria') redirectToLogin();
  document.getElementById('sidebarName').textContent = nome;
  document.getElementById('topbarName').textContent = nome;
  document.getElementById('sidebarAvatar').textContent = nome.slice(0, 2).toUpperCase();
}

function showSection(sectionId, button) {
  document.querySelectorAll('.section').forEach(el => el.classList.remove('active'));
  document.getElementById(`sec-${sectionId}`).classList.add('active');
  document.getElementById('topbarTitle').textContent = button?.textContent.trim() || sectionId;
  document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
  if (button) button.classList.add('active');
  if (window.innerWidth <= 900) sidebarToggle();
}

function openModal(id) {
  document.getElementById(id)?.classList.add('open');
}

function closeModal(id) {
  document.getElementById(id)?.classList.remove('open');
}
let actionFormHandler = null;
let actionConfirmHandler = null;
const secretariaState = {
  consultas: new Map(),
  pacientes: new Map(),
  clientes: new Map()
};

function openActionFormModal(config) {
  document.getElementById('actionFormTitle').textContent = config.title || 'Editar Registo';
  document.getElementById('actionLabelNome').textContent = config.labelNome || 'Nome';
  document.getElementById('actionLabelDescricao').textContent = config.labelDescricao || 'Descrição';
  document.getElementById('actionLabelData').textContent = config.labelData || 'Data e Hora';
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

function normalizePedidoEstado(rawEstado) {
  const estado = String(rawEstado || '').trim().toLowerCase();
  if (estado === 'pendente') return 'Pendente';
  if (estado === 'comprovativo enviado' || estado === 'pagamento enviado') return 'Pagamento Enviado';
  if (estado === 'confirmado' || estado === 'validado') return 'Validado';
  if (estado === 'cancelado') return 'Cancelado';
  if (estado === 'rejeitado') return 'Rejeitado';
  return rawEstado || '—';
}

function badgeEstado(estado) {
  const texto = estado || '-';
  const map = {
    'Pendente': 'badge-amber',
    'Pagamento Enviado': 'badge-amber',
    'Validado': 'badge-green',
    'Cancelado': 'badge-red',
    'Rejeitado': 'badge-red'
  };
  return `<span class="badge ${map[texto] || 'badge-gray'}">${texto}</span>`;
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

async function initSecretaria() {
  validateSecretariaAccess();
  await Promise.all([
    loadTopStats(),
    loadConsultas(),
    loadPacientes(),
    loadClientes(),
    loadEspecialidades(),
    loadServicos(),
    loadMedicos(),
    loadPagamentos(),
    loadPagamentoConsulta(),
    loadSMS(),
    loadPedidos(),
    loadEstados()
  ]);
  populateSecretariaSelects();
}

async function loadTopStats() {
  try {
    const [consultas, pacientes, clientes, pedidos] = await Promise.all([
      fetchJson('/Secretaria/consulta'),
      fetchJson('/Secretaria/paciente'),
      fetchJson('/Secretaria/cliente'),
      fetchJson('/Secretaria/pedidos')
    ]);
    document.getElementById('statConsultas').textContent = consultas?.length ?? 0;
    document.getElementById('statPacientes').textContent = pacientes?.length ?? 0;
    document.getElementById('statClientes').textContent = clientes?.length ?? 0;
    document.getElementById('statPedidos').textContent = pedidos?.dados?.length ?? 0;
    renderDashboardConsultas(consultas || []);
  } catch {
    showToast('NÃ£o foi possÃ­vel carregar mÃ©tricas.', 'error');
  }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  body.innerHTML = consultas.slice(0, 6).map((item, index) => `
    <tr>
      <td>${pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1}</td>
      <td>${pickValue(item, ['pacienteNome', 'PacienteNome', 'Paciente', 'paciente', 'PacienteNome']) || 'â€”'}</td>
      <td>${pickValue(item, ['medicoNome', 'MedicoNome', 'nomeMedico', 'NomeMedico']) || 'â€”'}</td>
      <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || 'â€”'}</td>
      <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta || item.Data_Consulta)}</td>
      <td><span class="badge badge-gray">${pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || 'â€”'}</span></td>
    </tr>`).join('');
}

async function loadEstados() {
  try {
    const estados = await fetchJson('/Secretaria/estado');
    const target = document.getElementById('cEstado');
    if (!target) return;
    target.innerHTML = '<option value="">Seleccionar...</option>' + estados.map(est => `<option value="${pickValue(est, ['consultaId', 'id', 'ConsultaId', 'Id'])}">${pickValue(est, ['descricao', 'Descricao', 'EstadoDescricao', 'Nome', 'descricao']) || 'Estado'}</option>`).join('');
  } catch {
    console.warn('NÃ£o foi possÃ­vel carregar os estados');
  }
}

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const consultas = await fetchJson('/Secretaria/consulta');
    if (!consultas.length) {
      body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
      return;
    }
    secretariaState.consultas.clear();
    consultas.forEach((item, index) => {
      const id = pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1;
      secretariaState.consultas.set(id, item);
    });
    body.innerHTML = consultas.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1}</td>
        <td>${pickValue(item, ['pacienteNome', 'PacienteNome', 'Paciente', 'paciente']) || 'â€”'}</td>
        <td>${pickValue(item, ['medicoNome', 'MedicoNome', 'nomeMedico', 'NomeMedico']) || 'â€”'}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || 'â€”'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta || item.Data_Consulta)}</td>
        <td><span class="badge badge-gray">${pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || 'â€”'}</span></td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          ${(()=>{
            const estadoDesc = (pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || '').trim().toLowerCase();
            const finalizada = estadoDesc === 'finalizada';
            const consultaId = pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1;
            return finalizada
              ? `<button class="btn btn-sm btn-outline" disabled title="Consulta finalizada — não é possível editar" style="opacity:0.45;cursor:not-allowed;">Editar</button>`
              : `<button class="btn btn-sm btn-outline" onclick="editarConsulta(${consultaId})">Editar</button>`;
          })()}
          <button class="btn btn-sm btn-danger" onclick="removerConsulta(${pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1})">Remover</button>
        </td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const pacientes = await fetchJson('/Secretaria/paciente');
    if (!pacientes.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum paciente encontrado.</td></tr>';
      return;
    }
    secretariaState.pacientes.clear();
    pacientes.forEach((item, index) => {
      const id = pickValue(item, ['pacienteId', 'PacienteId', 'Id']) || index + 1;
      secretariaState.pacientes.set(id, item);
    });
    body.innerHTML = pacientes.map((item, index) => {
      const id = pickValue(item, ['pacienteId', 'PacienteId', 'Id']) || index + 1;
      const nome = pickValue(item, ['pacienteNome', 'PacienteNome', 'Nome', 'nome']) || '';
      const cliente = pickValue(item, ['cliente', 'Cliente', 'Cliente_Paciente', 'clienteNome', 'ClienteNome', 'nif_cliente', 'Nif_cliente']) || 'â€”';
      const nomeSafe = String(nome).replace(/'/g, "\\'");
      return `
      <tr>
        <td>${id}</td>
        <td>${nome || 'â€”'}</td>
        <td>${formatDateShort(item.pacienteData_nascimento || item.Data_nascimento || item.DataNascimento || item.Data_Consulta)}</td>
        <td>${cliente}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          <button class="btn btn-sm btn-outline" onclick="editarPaciente(${id}, '${nomeSafe}')">Editar</button>
          <button class="btn btn-sm btn-danger" onclick="removerPaciente(${id})">Remover</button>
        </td>
      </tr>`;
    }).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pacientes.</td></tr>';
  }
}

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const clientes = await fetchJson('/Secretaria/cliente');
    if (!clientes.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum cliente encontrado.</td></tr>';
      return;
    }
    secretariaState.clientes.clear();
    clientes.forEach((item) => {
      const nif = pickValue(item, ['clienteNif', 'ClienteNif', 'Nif_cliente', 'nif_cliente']) || '';
      secretariaState.clientes.set(nif, item);
    });
    body.innerHTML = clientes.map((item) => {
      const nif = pickValue(item, ['clienteNif', 'ClienteNif', 'Nif_cliente', 'nif_cliente']) || '';
      const nome = pickValue(item, ['clienteNome', 'ClienteNome', 'Nome', 'nome']) || '';
      const contactos = Array.isArray(item.contactos) ? item.contactos.map(c => pickValue(c, ['contacto', 'Contacto']) || 'â€”').join(', ') : 'â€”';
      const nifSafe = String(nif).replace(/'/g, "\\'");
      const nomeSafe = String(nome).replace(/'/g, "\\'");
      return `
      <tr>
        <td>${nif || 'â€”'}</td>
        <td>${nome || 'â€”'}</td>
        <td>${contactos}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          <button class="btn btn-sm btn-outline" onclick="editarCliente('${nifSafe}', '${nomeSafe}')">Editar</button>
          <button class="btn btn-sm btn-danger" onclick="removerCliente('${nifSafe}')">Remover</button>
        </td>
      </tr>`;
    }).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar clientes.</td></tr>';
  }
}

async function loadEspecialidades() {
  const body = document.getElementById('bodyEspecialidades');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/especialidade');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhuma especialidade encontrada.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['especialidadeId', 'EspecialidadeId', 'id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['especialidadeNome', 'EspecialidadeNome', 'Nome', 'nome', 'NomeEspecialidade']) || 'â€”'}</td>
        <td>${pickValue(item, ['especialidadeDescricao', 'EspecialidadeDescricao', 'Descricao', 'descricao']) || 'â€”'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar especialidades.</td></tr>';
  }
}

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/servico');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviÃ§o encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['servicoId', 'ServicoId', 'id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Nome', 'nome']) || 'â€”'}</td>
        <td>${pickValue(item, ['especialidade', 'nomeEspecialidade', 'Especialidade', 'NomeEspecialidade']) || 'â€”'}</td>
        <td>${pickValue(item, ['servicoPreco', 'ServicoPreco', 'Preco', 'preco']) || 'â€”'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar serviÃ§os.</td></tr>';
  }
}

async function loadMedicos() {
  const body = document.getElementById('bodyMedicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/medicos');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhum mÃ©dico encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']) || 'â€”'}</td>
        <td>${pickValue(item, ['nomeEspecialidade', 'NomeEspecialidade', 'especialidade', 'Especialidade']) || 'â€”'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar mÃ©dicos.</td></tr>';
  }
}

function renderComprovativoValue(value) {
  return value ? 'Comprovativo' : '—';
}

async function loadPagamentos() {
  const body = document.getElementById('bodyPagamentos');
  if (!body) return;
  try {
    const [pagamentos, pagamentosConsulta] = await Promise.all([
      fetchJson('/Secretaria/pagamento'),
      fetchJson('/Secretaria/pagamentoconsulta')
    ]);
    const pagamentoValorMap = new Map((pagamentosConsulta || []).map((pc) => [pc.idPagamento || pc.IdPagamento, pc.valorServico ?? pc.ValorServico ?? '—']));
    body.innerHTML = (pagamentos || []).length ? pagamentos.map((item) => {
      const id = item.id || item.Id;
      return `<tr><td>${id || '—'}</td><td>${item.cliente || item.Cliente || '—'}</td><td>${item.secretaria || item.Secretaria || '—'}</td><td>${pagamentoValorMap.get(id) ?? '—'}</td><td>${renderComprovativoValue(item.comprovativo || item.Comprovativo)}</td><td>${formatDate(item.dataEnvio || item.DataEnvio || item.Data)}</td></tr>`;
    }).join('') : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pagamento encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos.</td></tr>';
  }
}

async function loadPagamentoConsulta() {
  const body = document.getElementById('bodyPagamentoConsulta');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/pagamentoconsulta');
    body.innerHTML = (items || []).length ? items.map((item) => `<tr><td>${item.id || item.Id || '—'}</td><td>${item.idPagamento || item.IdPagamento || '—'}</td><td>${item.idConsulta || item.IdConsulta || '—'}</td><td>${formatDate(item.dataConsulta || item.DataConsulta)}</td><td>${renderComprovativoValue(item.comprovativo || item.Comprovativo)}</td></tr>`).join('') : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum registo encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos de consulta.</td></tr>';
  }
}

async function loadSMS() {
  const body = document.getElementById('bodySMS');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/SMS');
    body.innerHTML = (items || []).length ? items.map((item) => `<tr><td>${item.smsId || item.SmsId || '—'}</td><td>${item.cliente?.clienteNome || item.cliente?.ClienteNome || '—'}</td><td>${item.mensagem || item.Mensagem || '—'}</td><td>${formatDate(item.dataEnvio || item.DataEnvio)}</td></tr>`).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum SMS enviado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar SMS.</td></tr>';
  }
}

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Secretaria/pedidos');
    const items = res?.dados || [];
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => {
      const id = pickValue(item, ['id', 'Id']) || 'â€”';
      const estado = normalizePedidoEstado(pickValue(item, ['estado', 'Estado', 'estadoDescricao', 'EstadoDescricao']) || 'â€”');
      const actions = renderPedidoActions(id, estado);
      return `
      <tr>
        <td>${id}</td>
        <td>${pickValue(item, ['clienteNome', 'ClienteNome', 'NomeCliente', 'nomeCliente', 'Cliente', 'cliente']) || 'â€”'}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || 'â€”'}</td>
        <td>${formatDate(item.horarioPreferencial || item.HorarioPreferencial || item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
        <td>${badgeEstado(estado)}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">${actions || '<span style="color:var(--muted)">—</span>'}</td>
      </tr>`;
    }).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pedidos.</td></tr>';
  }
}

async function handlePedidoAction(id, action) {
  const mapping = { confirmar: 'confirmar', cancelar: 'cancelar', validar: 'validar', rejeitar: 'rejeitar' };
  try {
    const result = await fetchJson(`/Secretaria/${id}/${mapping[action]}`, { method: 'PUT' });
    showToast(result.mensagem || 'OperaÃ§Ã£o concluÃ­da.', 'success');
    loadPedidos();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Falha na operaÃ§Ã£o.', 'error');
  }
}

function openComprovativo(id) {
  window.open(`http://localhost:5290/api/Secretaria/${id}/comprovativo`, '_blank');
}

function pickValue(item, keys) {
  for (const key of keys) {
    if (item && item[key] !== undefined && item[key] !== null) {
      return item[key];
    }
  }
  return '';
}

function createOption(value, label) {
  return `<option value="${value}">${label}</option>`;
}

function setSelectOptions(selectId, items, valueKeys, labelKeys) {
  const select = document.getElementById(selectId);
  if (!select) return;
  const valueArray = Array.isArray(valueKeys) ? valueKeys : [valueKeys];
  const labelArray = Array.isArray(labelKeys) ? labelKeys : [labelKeys];
  select.innerHTML = '<option value="">Seleccionar...</option>' + items.map(item => {
    const value = pickValue(item, valueArray);
    const label = pickValue(item, labelArray) || value;
    return createOption(value, label);
  }).join('');
}

async function populateSecretariaSelects() {
  try {
    const [clientes, medicos, especialidades, pacientes, pagamentos, consultas] = await Promise.all([
      fetchJson('/Secretaria/cliente'),
      fetchJson('/Secretaria/medicos'),
      fetchJson('/Secretaria/especialidade'),
      fetchJson('/Secretaria/paciente'),
      fetchJson('/Secretaria/pagamento'),
      fetchJson('/Secretaria/consulta')
    ]);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', await fetchJson('/Secretaria/servico') || [], ['id', 'Id'], ['servicoNome', 'ServicoNome', 'nome', 'Nome']);
    setSelectOptions('cPaciente', pacientes || [], ['id', 'Id', 'pacienteId', 'PacienteId'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('payCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('pcPagamento', pagamentos || [], ['id', 'Id'], ['comprovativo', 'Comprovativo']);
    setSelectOptions('pcConsulta', consultas || [], ['consultaId', 'ConsultaId', 'id', 'Id'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('smsCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('ecMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('ecEstado', estadosFromDom(), ['consultaId', 'id', 'ConsultaId'], ['descricao', 'Descricao', 'EstadoDescricao']);
  } catch (error) {
    console.warn('Falha ao preencher selects', error);
  }
}

function estadosFromDom() {
  const opts = Array.from(document.querySelectorAll('#cEstado option')).filter(o => o.value);
  return opts.map(o => ({ id: o.value, descricao: o.textContent }));
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

async function submitConsulta(event) {
  event.preventDefault();
  try {
    const payload = {
      Id_medico_especialiade: parseInt(document.getElementById('cMedico').value, 10),
      Id_servico: parseInt(document.getElementById('cServico').value, 10),
      Id_paciente: parseInt(document.getElementById('cPaciente').value, 10),
      Id_estado_consulta: parseInt(document.getElementById('cEstado').value, 10),
      Data_consulta: document.getElementById('cData').value
    };
    await fetchJson('/Secretaria/consulta', { method: 'POST', body: payload });
    showToast('Consulta criada com sucesso.', 'success');
    closeModal('modalConsulta');
    loadConsultas();
    loadTopStats();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar consulta.', 'error');
  }
}

async function submitPaciente(event) {
  event.preventDefault();
  try {
    const payload = {
      PacienteNome: document.getElementById('pNome').value,
      PacienteData_nascimento: document.getElementById('pNasc').value,
      IdCliente_Paciente: parseInt(document.getElementById('pCliente').value, 10),
      IdGenero: parseInt(document.getElementById('pGenero').value, 10),
      Nif_cliente: document.getElementById('pCliente').value
    };
    await fetchJson('/Secretaria/paciente', { method: 'POST', body: payload });
    showToast('Paciente criado com sucesso.', 'success');
    closeModal('modalPaciente');
    loadPacientes();
    loadTopStats();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar paciente.', 'error');
  }
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
    if (!payload.Contactos.length) {
      showToast('Informe pelo menos um contacto: email ou telefone.', 'error');
      return;
    }
    await fetchJson('/Secretaria/cliente', { method: 'POST', body: payload });
    showToast('Cliente criado com sucesso.', 'success');
    closeModal('modalCliente');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao criar cliente.'), 'error');
  }
}

async function submitPagamento(event) {
  event.preventDefault();
  try {
    await fetchJson('/Secretaria/pagamento', { method: 'POST', body: { IdCliente: document.getElementById('payCliente').value, IdSecretaria: localStorage.getItem('nif') || 1, Comprovativo: document.getElementById('payComprovativo').value, Data: document.getElementById('payData').value || new Date().toISOString() } });
    showToast('Pagamento registado com sucesso.', 'success');
    closeModal('modalPagamento');
    loadPagamentos();
    populateSecretariaSelects();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao criar pagamento.'), 'error');
  }
}

async function submitPagamentoConsulta(event) {
  event.preventDefault();
  try {
    await fetchJson('/Secretaria/pagamentoconsulta', { method: 'POST', body: { IdPagamento: parseInt(document.getElementById('pcPagamento').value, 10), IdConsulta: parseInt(document.getElementById('pcConsulta').value, 10) } });
    showToast('Pagamento de consulta registado.', 'success');
    closeModal('modalPagamentoConsulta');
    loadPagamentoConsulta();
    loadPagamentos();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao registar pagamento de consulta.'), 'error');
  }
}

async function submitSMS(event) {
  event.preventDefault();
  try {
    await fetchJson('/Secretaria/SMS', { method: 'POST', body: { SMSMensagem: document.getElementById('smsMensagem').value, SMSNif_funcionario: localStorage.getItem('nif') || '', SMSNif_cliente: document.getElementById('smsCliente').value } });
    showToast('SMS enviado com sucesso.', 'success');
    closeModal('modalSMS');
    loadSMS();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao enviar SMS.'), 'error');
  }
}

async function editarConsulta(id) {
  const item = secretariaState.consultas.get(id) || {};
  const estadoDesc = (pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || '').trim().toLowerCase();
  if (estadoDesc === 'finalizada') {
    showToast('Esta consulta já foi finalizada e não pode ser editada.', 'error');
    return;
  }
  document.getElementById('ecConsultaId').value = id;
  document.getElementById('ecData').value = toDatetimeLocal(item.data_consulta || item.Data_consulta || item.DataConsulta);
  document.getElementById('ecMedico').value = pickValue(item, ['id_medico_especialiade', 'Id_medico_especialiade', 'idMedicoEspecialidade', 'IdMedicoEspecialidade', 'idMedico', 'IdMedico']) || '';
  document.getElementById('ecEstado').value = pickValue(item, ['id_estado_consulta', 'Id_estado_consulta', 'idEstadoConsulta', 'IdEstadoConsulta']) || '';
  openModal('modalEditConsulta');
}

async function removerConsulta(id) {
  openActionConfirmModal('Remover Consulta', 'Tem certeza que deseja remover esta consulta?', async () => {
    await fetchJson(`/Secretaria/consulta/${id}`, { method: 'DELETE' });
    showToast('Consulta removida com sucesso.', 'success');
    loadConsultas();
    loadTopStats();
  });
}


async function editarPaciente(id, nomeAtual = '') {
  document.getElementById('epPacienteId').value = id;
  document.getElementById('epNome').value = nomeAtual || pickValue(secretariaState.pacientes.get(id), ['pacienteNome', 'PacienteNome', 'Nome']) || '';
  openModal('modalEditPaciente');
}

async function removerPaciente(id) {
  openActionConfirmModal('Remover Paciente', 'Tem certeza que deseja remover este paciente?', async () => {
    await fetchJson(`/Secretaria/paciente/${id}`, { method: 'DELETE' });
    showToast('Paciente removido com sucesso.', 'success');
    loadPacientes();
    loadTopStats();
  });
}

async function editarCliente(nif, nomeAtual = '') {
  const item = secretariaState.clientes.get(nif) || {};
  const contactos = Array.isArray(item.contactos) ? item.contactos : [];
  const emailObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 2);
  const telObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 1);
  document.getElementById('eclNif').value = nif;
  document.getElementById('eclNome').value = nomeAtual || pickValue(item, ['clienteNome', 'ClienteNome', 'Nome']) || '';
  document.getElementById('eclEmail').value = (emailObj?.contacto || emailObj?.Contacto || '').trim();
  document.getElementById('eclTel').value = (telObj?.contacto || telObj?.Contacto || '').trim();
  openModal('modalEditCliente');
}

async function removerCliente(nif) {
  openActionConfirmModal('Remover Cliente', 'Tem certeza que deseja remover este cliente?', async () => {
    await fetchJson(`/Secretaria/cliente/${nif}`, { method: 'DELETE' });
    showToast('Cliente removido com sucesso.', 'success');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  });
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

document.getElementById('actionConfirmBtn')?.addEventListener('click', async () => {
  if (!actionConfirmHandler) return;
  try {
    await actionConfirmHandler();
    closeModal('modalActionConfirm');
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao executar ação.'), 'error');
  }
});

async function submitEditPaciente(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('epPacienteId').value, 10);
  const nome = document.getElementById('epNome').value.trim();
  try {
    await fetchJson(`/Secretaria/paciente/${id}`, { method: 'PUT', body: { IdPaciente: id, PacienteNome: nome } });
    showToast('Paciente atualizado com sucesso.', 'success');
    closeModal('modalEditPaciente');
    loadPacientes();
    loadTopStats();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar paciente.'), 'error');
  }
}

async function submitEditCliente(event) {
  event.preventDefault();
  const nif = document.getElementById('eclNif').value.trim();
  const nome = document.getElementById('eclNome').value.trim();
  const email = document.getElementById('eclEmail').value.trim();
  const tel = document.getElementById('eclTel').value.trim();
  const contactos = [];
  if (tel) contactos.push({ TipoContacto: 1, Contacto: tel });
  if (email) contactos.push({ TipoContacto: 2, Contacto: email });
  if (!contactos.length) {
    showToast('Informe pelo menos um contacto (telefone ou email).', 'error');
    return;
  }
  try {
    await fetchJson(`/Secretaria/cliente/${nif}`, { method: 'PUT', body: { Nif_cliente: nif, Nome: nome, Contactos: contactos } });
    showToast('Cliente atualizado com sucesso.', 'success');
    closeModal('modalEditCliente');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar cliente.'), 'error');
  }
}

async function submitEditConsulta(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('ecConsultaId').value, 10);
  const medicoId = parseInt(document.getElementById('ecMedico').value, 10);
  const estadoId = parseInt(document.getElementById('ecEstado').value, 10);
  const data = document.getElementById('ecData').value;
  try {
    await fetchJson(`/Secretaria/consulta/${id}`, {
      method: 'PUT',
      body: { IdConsulta: id, Id_medico_especialiade: medicoId, Id_estado_consulta: estadoId, Data_consulta: data }
    });
    showToast('Consulta atualizada com sucesso.', 'success');
    closeModal('modalEditConsulta');
    loadConsultas();
    loadTopStats();
  } catch (error) {
    const msg = error?.status === 409
      ? 'Esta consulta já foi finalizada e não pode ser editada.'
      : getErrorMessage(error, 'Erro ao atualizar consulta.');
    showToast(msg, 'error');
  }
}

window.addEventListener('load', initSecretaria);