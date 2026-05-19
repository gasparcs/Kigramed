// ─────────────────────────────────────────────────────────────────────────────
// SECRETARIA CORE - GESTÃO DE ESTADO E INICIALIZAÇÃO
// ─────────────────────────────────────────────────────────────────────────────

const secretariaState = {
  consultas: new Map(),
  pacientes: new Map(),
  clientes: new Map()
};

const sectionLoaders = {
  'dashboard': () => loadTopStats(),
  'consultas': () => loadConsultas(),
  'pacientes': () => loadPacientes(),
  'clientes': () => loadClientes(),
  'especialidades': () => loadEspecialidades(),
  'servicos': () => loadServicos(),
  'medicos': () => loadMedicos(),
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
  await populateSecretariaSelects();

  const hash = window.location.hash.substring(1);
  if (hash && sectionLoaders[hash]) {
    showSection(hash, document.querySelector(`[onclick*="'${hash}'"]`));
  } else {
    showSection('dashboard', document.querySelector(`[onclick*="'dashboard'"]`));
  }

  startPolling();
}

window.addEventListener('hashchange', () => {
  const hash = window.location.hash.substring(1);
  if (hash && sectionLoaders[hash] && activeSectionId !== hash) {
    showSection(hash, document.querySelector(`[onclick*="'${hash}'"]`));
  }
});

function validateSecretariaAccess() {
  if (!token || role !== 'Secretaria') redirectToLogin();
  document.getElementById('sidebarName').textContent = nome;
  document.getElementById('topbarName').textContent = nome;
  document.getElementById('sidebarAvatar').textContent = nome.slice(0, 2).toUpperCase();
}

function showSection(sectionId, button) {
  if (window.location.hash !== '#' + sectionId) window.location.hash = sectionId;
  if (window.innerWidth <= 900) {
    document.getElementById('sidebar')?.classList.remove('open');
    document.getElementById('sidebarOverlay')?.classList.remove('open');
  }
  document.querySelectorAll('.section').forEach(el => el.classList.remove('active'));
  document.getElementById(`sec-${sectionId}`)?.classList.add('active');
  document.getElementById('topbarTitle').textContent = button?.textContent.trim() || sectionId;
  document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
  if (button) button.classList.add('active');
  if (window.innerWidth <= 900) sidebarToggle();

  activeSectionId = sectionId;
  if (sectionLoaders[activeSectionId]) {
    sectionLoaders[activeSectionId]();
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// DASHBOARD & STATS
// ─────────────────────────────────────────────────────────────────────────────

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
  } catch { }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  body.innerHTML = consultas.slice(0, 6).map((item, index) => `
    <tr>
      <td>${pickValue(item, ['consultaId', 'id']) || index + 1}</td>
      <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
      <td>${item.medicoNome || item.MedicoNome || '—'}</td>
      <td>${item.servicoNome || item.ServicoNome || '—'}</td>
      <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
      <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao)}</td>
    </tr>`).join('');
}

// ─────────────────────────────────────────────────────────────────────────────
// LOADERS
// ─────────────────────────────────────────────────────────────────────────────

async function loadEstados() {
  try {
    const estados = await fetchJson('/Secretaria/estado');
    setSelectOptions('cEstado', estados || [], ['consultaId', 'id'], ['descricao', 'Descricao']);
    setSelectOptions('ecEstado', estados || [], ['consultaId', 'id'], ['descricao', 'Descricao']);
  } catch { }
}

let estadoFiltroActivo = null;

async function filtrarConsultasPorEstado(estado, btn) {
  estadoFiltroActivo = estado;
  document.querySelectorAll('.estado-filter').forEach(b => b.classList.remove('active'));
  if (btn) btn.classList.add('active');
  await loadConsultas();
}

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const url = estadoFiltroActivo
      ? `/Secretaria/consulta?estado=${encodeURIComponent(estadoFiltroActivo)}`
      : '/Secretaria/consulta';
    const items = await fetchJson(url);
    const list = items || [];

    secretariaState.consultas.clear();
    list.forEach((item, index) => {
      const id = item.consultaId || item.id || index + 1;
      secretariaState.consultas.set(id, item);
    });

    body.innerHTML = list.length ? list.map((item, index) => {
      const id = item.consultaId || item.id || index + 1;
      const estadoDesc = (item.estadoDescricao || item.EstadoDescricao || '').trim().toLowerCase();
      const finalizada = estadoDesc === 'finalizada';
      return `<tr>
        <td>${id}</td>
        <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
        <td>${item.medicoNome || item.MedicoNome || '—'}</td>
        <td>${item.servicoNome || item.ServicoNome || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta)}</td>
        <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao)}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          ${finalizada
            ? `<button class="btn btn-sm btn-outline" disabled title="Consulta finalizada" style="opacity:0.45;cursor:not-allowed;">Editar</button>`
            : `<button class="btn btn-sm btn-outline" onclick="editarConsulta(${id})">Editar</button>`
          }
          <button class="btn btn-sm btn-danger" onclick="removerConsulta(${id})">Remover</button>
        </td>
      </tr>`;
    }).join('') : '<tr><td colspan="7" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/paciente');
    const list = items || [];

    secretariaState.pacientes.clear();
    list.forEach((item, index) => {
      const id = item.pacienteId || item.Id || index + 1;
      secretariaState.pacientes.set(id, item);
    });

    body.innerHTML = list.length ? list.map((item, index) => {
      const id = item.pacienteId || item.Id || index + 1;
      return `<tr>
        <td>${id}</td>
        <td>${item.pacienteNome || item.Nome || '—'}</td>
        <td>${formatDateShort(item.pacienteData_nascimento || item.Data_nascimento)}</td>
        <td>${item.cliente || item.Cliente || item.clienteNome || item.ClienteNome || item.nif_cliente || item.Nif_cliente || '—'}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          <button class="btn btn-sm btn-outline" onclick="editarPaciente(${id})">Editar</button>
          <button class="btn btn-sm btn-danger" onclick="removerPaciente(${id})">Remover</button>
        </td>
      </tr>`;
    }).join('') : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum paciente encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pacientes.</td></tr>';
  }
}

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/cliente');
    const list = items || [];

    secretariaState.clientes.clear();
    list.forEach((item) => {
      const nif = item.clienteNif || item.Nif_cliente || '';
      secretariaState.clientes.set(nif, item);
    });

    body.innerHTML = list.length ? list.map((item) => {
      const nif = item.clienteNif || item.Nif_cliente || '—';
      const contactos = Array.isArray(item.contactos) ? item.contactos.map(c => c.contacto || c.Contacto).join(', ') : '—';
      return `<tr>
        <td>${nif}</td>
        <td>${item.clienteNome || item.Nome || '—'}</td>
        <td>${contactos}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          <button class="btn btn-sm btn-outline" onclick="editarCliente('${nif}')">Editar</button>
          <button class="btn btn-sm btn-danger" onclick="removerCliente('${nif}')">Remover</button>
        </td>
      </tr>`;
    }).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum cliente encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar clientes.</td></tr>';
  }
}

async function loadEspecialidades() {
  const body = document.getElementById('bodyEspecialidades');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/especialidade');
    body.innerHTML = (items || []).length ? items.map((item, index) => {
      const eid = item.especialidadeId || item.id || item.Id || index + 1;
      return `<tr>
        <td>${eid}</td>
        <td>${item.especialidadeNome || item.Nome || item.NomeEspecialidade || '—'}</td>
        <td>${item.especialidadeDescricao || item.Descricao || '—'}</td>
        <td><button class="btn btn-sm btn-primary" onclick="abrirNovaConsultaCom(null, '${eid}')">Marcar consulta</button></td>
      </tr>`;
    }).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhuma especialidade encontrada.</td></tr>';
  } catch { }
}

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/servico');
    body.innerHTML = (items || []).length ? items.map((item, index) => {
      const sid = item.id || item.Id || item.servicoId || index + 1;
      return `<tr>
        <td>${sid}</td>
        <td>${item.servicoNome || item.nome || '—'}</td>
        <td>${item.especialidade || item.nomeEspecialidade || '—'}</td>
        <td>${item.servicoPreco || item.preco || '—'} Kz</td>
        <td><button class="btn btn-sm btn-primary" onclick="abrirNovaConsultaComServico('${sid}')">Marcar consulta</button></td>
      </tr>`;
    }).join('') : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviço encontrado.</td></tr>';
  } catch { }
}

async function loadMedicos() {
  const body = document.getElementById('bodyMedicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/medicos');
    body.innerHTML = (items || []).length ? items.map((item, index) => `
      <tr>
        <td>${item.id || index + 1}</td>
        <td>${item.nomefuncionario || item.NomeFuncionario || '—'}</td>
        <td>${item.nomeEspecialidade || item.Especialidade || '—'}</td>
      </tr>`).join('') : '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhum médico encontrado.</td></tr>';
  } catch { }
}

async function loadPagamentos() {
  const body = document.getElementById('bodyPagamentos');
  if (!body) return;
  try {
    const [pagamentos, pagamentosConsulta] = await Promise.all([
      fetchJson('/Secretaria/pagamento'),
      fetchJson('/Secretaria/pagamentoconsulta')
    ]);
    const map = new Map((pagamentosConsulta || []).map(pc => [pc.idPagamento || pc.IdPagamento, pc.valorServico ?? '—']));
    body.innerHTML = (pagamentos || []).length ? pagamentos.map((item) => {
      const id = item.id || item.Id;
      return `<tr><td>${id || '—'}</td><td>${item.cliente || '—'}</td><td>${item.secretaria || '—'}</td><td>${map.get(id) ?? '—'}</td><td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td><td>${formatDate(item.dataEnvio || item.Data)}</td></tr>`;
    }).join('') : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pagamento encontrado.</td></tr>';
  } catch { }
}

async function loadPagamentoConsulta() {
  const body = document.getElementById('bodyPagamentoConsulta');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/pagamentoconsulta');
    body.innerHTML = (items || []).length ? items.map((item) => `<tr><td>${item.id || '—'}</td><td>${item.idPagamento || '—'}</td><td>${item.idConsulta || '—'}</td><td>${formatDate(item.dataConsulta)}</td><td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td></tr>`).join('') : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum registo encontrado.</td></tr>';
  } catch { }
}

async function loadSMS() {
  const body = document.getElementById('bodySMS');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/SMS');
    body.innerHTML = (items || []).length ? items.map((item) => `<tr><td>${item.smsId || '—'}</td><td>${item.cliente?.clienteNome || '—'}</td><td>${item.mensagem || '—'}</td><td>${formatDate(item.dataEnvio)}</td></tr>`).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum SMS enviado.</td></tr>';
  } catch { }
}

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Secretaria/pedidos');
    const items = res?.dados || [];
    body.innerHTML = items.length
      ? items.map((item) => {
          const id            = item.id ?? '—';
          const numeroPedido  = item.numeroPedido || '—';
          const paciente      = item.nomePaciente || '—';
          const especialidade = item.especialidade || '—';
          const servico       = item.servico || '—';
          const horario       = item.dataConsulta;
          const prazo         = item.prazoPagamento;
          const estado        = item.estado || '—';
          const actions       = renderPedidoActions(id, estado, item.caminhoComprovativo);
          return `<tr>
            <td>${numeroPedido}</td>
            <td>${paciente}</td>
            <td>${especialidade}</td>
            <td>${servico}</td>
            <td>${formatDate(horario)}</td>
            <td>${prazo ? formatDate(prazo) : '—'}</td>
            <td>${badgeEstadoPedido(estado)}</td>
            <td style="display:flex;gap:6px;flex-wrap:wrap;">${actions || '<span style="color:var(--muted)">—</span>'}</td>
          </tr>`;
        }).join('')
      : '<tr><td colspan="8" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="8" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pedidos.</td></tr>';
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// ACTIONS & SUBMITS
// ─────────────────────────────────────────────────────────────────────────────

function abrirNovaConsultaComServico(servicoId) {
  openModal('modalConsulta');
  setTimeout(() => {
    const sel = document.getElementById('cServico');
    if (sel) sel.value = String(servicoId);
  }, 0);
}

function abrirNovaConsultaCom(servicoId, medicoEspId) {
  if (servicoId) {
    const sel = document.getElementById('cServico');
    if (sel) sel.value = String(servicoId);
  }
  if (medicoEspId) {
    const sel = document.getElementById('cMedico');
    if (sel) sel.value = String(medicoEspId);
  }
  openModal('modalConsulta');
}

async function populateSecretariaSelects() {
  try {
    const [clientes, medicos, especialidades, pacientes, pagamentos, consultas, servicos] = await Promise.all([
      fetchJson('/Secretaria/cliente'),
      fetchJson('/Secretaria/medicos'),
      fetchJson('/Secretaria/especialidade'),
      fetchJson('/Secretaria/paciente'),
      fetchJson('/Secretaria/pagamento'),
      fetchJson('/Secretaria/consulta'),
      fetchJson('/Secretaria/servico')
    ]);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente'], ['clienteNome', 'nome']);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'NomeFuncionario']);
    setSelectOptions('ecMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', servicos || [], ['id', 'Id'], ['servicoNome', 'nome']);
    setSelectOptions('cPaciente', pacientes || [], ['id', 'pacienteId'], ['pacienteNome', 'Nome']);
    setSelectOptions('payCliente', clientes || [], ['clienteNif', 'Nif_cliente'], ['clienteNome', 'nome']);
    setSelectOptions('pcPagamento', pagamentos || [], ['id', 'Id'], ['id', 'Id']);
    setSelectOptions('pcConsulta', consultas || [], ['consultaId', 'id'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('smsCliente', clientes || [], ['clienteNif', 'Nif_cliente'], ['clienteNome', 'nome']);
  } catch { }
}

async function submitConsulta(event) {
  event.preventDefault();
  try {
    await fetchJson('/Secretaria/consulta', {
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
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar consulta.'), 'error'); }
}

async function editarConsulta(id) {
  const item = secretariaState.consultas.get(id) || {};
  document.getElementById('ecConsultaId').value = id;
  document.getElementById('ecData').value = toDatetimeLocal(item.data_consulta || item.Data_consulta);
  document.getElementById('ecMedico').value = item.idMedicoEspecialidade || '';
  document.getElementById('ecEstado').value = item.idEstadoConsulta || '';
  openModal('modalEditConsulta');
}

async function submitEditConsulta(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('ecConsultaId').value, 10);
  try {
    await fetchJson(`/Secretaria/consulta/${id}`, {
      method: 'PUT',
      body: {
        IdConsulta: id,
        Id_medico_especialiade: parseInt(document.getElementById('ecMedico').value, 10),
        Id_estado_consulta: parseInt(document.getElementById('ecEstado').value, 10),
        Data_consulta: document.getElementById('ecData').value
      }
    });
    showToast('Consulta atualizada com sucesso.', 'success');
    closeModal('modalEditConsulta');
    loadConsultas();
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao atualizar consulta.'), 'error'); }
}

async function removerConsulta(id) {
  openActionConfirmModal('Remover Consulta', 'Tem certeza que deseja remover esta consulta?', async () => {
    await fetchJson(`/Secretaria/consulta/${id}`, { method: 'DELETE' });
    showToast('Consulta removida.', 'success');
    loadConsultas();
    loadTopStats();
  });
}

async function submitPaciente(event) {
  event.preventDefault();
  try {
    await fetchJson('/Secretaria/paciente', {
      method: 'POST',
      body: {
        PacienteNome: document.getElementById('pNome').value,
        PacienteData_nascimento: document.getElementById('pNasc').value,
        IdCliente_Paciente: parseInt(document.getElementById('pTipo')?.value || '1', 10) || 1,
        IdGenero: parseInt(document.getElementById('pGenero').value, 10),
        Nif_cliente: document.getElementById('pCliente').value
      }
    });
    showToast('Paciente criado com sucesso.', 'success');
    closeModal('modalPaciente');
    loadPacientes();
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar paciente.'), 'error'); }
}

async function editarPaciente(id) {
  const item = secretariaState.pacientes.get(id) || {};
  document.getElementById('epPacienteId').value = id;
  document.getElementById('epNome').value = item.pacienteNome || item.Nome || '';
  document.getElementById('epNasc').value = (item.pacienteData_nascimento || item.Data_nascimento || '').split('T')[0];
  document.getElementById('epGenero').value = item.idGenero || item.IdGenero || '';
  document.getElementById('epTipo').value = item.idClientePaciente || item.IdClientePaciente || '';
  openModal('modalEditPaciente');
}

async function submitEditPaciente(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('epPacienteId').value, 10);
  try {
    await fetchJson(`/Secretaria/paciente/${id}`, {
      method: 'PUT',
      body: {
        IdPaciente: id,
        PacienteNome: document.getElementById('epNome').value.trim(),
        DataNascimento: document.getElementById('epNasc').value,
        IdGenero: parseInt(document.getElementById('epGenero').value, 10),
        IdClientePaciente: parseInt(document.getElementById('epTipo').value, 10)
      }
    });
    showToast('Paciente atualizado.', 'success');
    closeModal('modalEditPaciente');
    loadPacientes();
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao atualizar paciente.'), 'error'); }
}

async function removerPaciente(id) {
  openActionConfirmModal('Remover Paciente', 'Tem certeza que deseja remover este paciente?', async () => {
    await fetchJson(`/Secretaria/paciente/${id}`, { method: 'DELETE' });
    showToast('Paciente removido.', 'success');
    loadPacientes();
    loadTopStats();
  });
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
    await fetchJson('/Secretaria/cliente', { method: 'POST', body: payload });
    showToast('Cliente criado com sucesso.', 'success');
    closeModal('modalCliente');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar cliente.'), 'error'); }
}

async function editarCliente(nif) {
  const item = secretariaState.clientes.get(nif) || {};
  const contactos = Array.isArray(item.contactos) ? item.contactos : [];

  const emailObj = contactos.find(c => {
    const desc = String(
      c.tipoContacto?.descricao || c.TipoContacto?.descricao ||
      c.tipoContacto?.Descricao || c.TipoContacto?.Descricao || ''
    ).trim().toLowerCase();
    return desc === 'email' || desc === 'e-mail';
  });

  const telObj = contactos.find(c => {
    const desc = String(
      c.tipoContacto?.descricao || c.TipoContacto?.descricao ||
      c.tipoContacto?.Descricao || c.TipoContacto?.Descricao || ''
    ).trim().toLowerCase();
    return desc === 'telefone' || desc === 'telemóvel' || desc === 'telemovel' || desc === 'phone';
  });

  document.getElementById('eclNif').value = nif;
  document.getElementById('eclNome').value = item.clienteNome || item.ClienteNome || item.Nome || '';
  document.getElementById('eclEmail').value = (emailObj?.contacto || emailObj?.Contacto || '').trim();
  document.getElementById('eclTel').value = (telObj?.contacto || telObj?.Contacto || '').trim();
  openModal('modalEditCliente');
}

async function submitEditCliente(event) {
  event.preventDefault();
  const nif = document.getElementById('eclNif').value;
  try {
    const payload = {
      Nif_cliente: nif,
      Nome: document.getElementById('eclNome').value.trim(),
      Contactos: []
    };
    const email = document.getElementById('eclEmail').value.trim();
    const tel = document.getElementById('eclTel').value.trim();
    if (email) payload.Contactos.push({ TipoContacto: 2, Contacto: email });
    if (tel) payload.Contactos.push({ TipoContacto: 1, Contacto: tel });

    await fetchJson(`/Secretaria/cliente/${nif}`, { method: 'PUT', body: payload });
    showToast('Cliente atualizado.', 'success');
    closeModal('modalEditCliente');
    loadClientes();
    populateSecretariaSelects();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao atualizar cliente.'), 'error'); }
}

async function removerCliente(nif) {
  openActionConfirmModal('Remover Cliente', 'Tem certeza que deseja remover este cliente?', async () => {
    await fetchJson(`/Secretaria/cliente/${nif}`, { method: 'DELETE' });
    showToast('Cliente removido.', 'success');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  });
}

// ─────────────────────────────────────────────────────────────────────────────
// PEDIDOS & UTILS
// ─────────────────────────────────────────────────────────────────────────────

async function handlePedidoAction(id, action) {
  const mapping = { confirmar: 'confirmar', cancelar: 'cancelar', validar: 'validar', rejeitar: 'rejeitar' };
  try {
    const result = await fetchJson(`/Secretaria/${id}/${mapping[action]}`, { method: 'PUT' });
    showToast(result.mensagem || 'Operação concluída.', 'success');
    loadPedidos();
    loadTopStats();
  } catch (e) { showToast(getErrorMessage(e), 'error'); }
}

function badgeEstadoPedido(estado) {
  const norm = String(estado || '').trim().toLowerCase();
  const map = {
    'pendente':              'badge-amber',
    'aguarda pagamento':     'badge-blue',
    'comprovativo enviado':  'badge-blue',
    'rejeitado':             'badge-red',
    'confirmada':            'badge-green',
    'cancelada':             'badge-red',
  };
  return `<span class="badge ${map[norm] || 'badge-gray'}">${estado}</span>`;
}

function renderPedidoActions(id, estado, caminhoComprovativo) {
  const norm = String(estado || '').trim().toLowerCase();
  if (norm === 'pendente') {
    return `
      <button class="btn btn-sm btn-success" onclick="handlePedidoAction(${id}, 'confirmar')">Confirmar</button>
      <button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${id}, 'cancelar')">Cancelar</button>`;
  }
  if (norm === 'aguarda pagamento') {
    return `
      <button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${id}, 'cancelar')">Cancelar</button>`;
  }
  if (norm === 'comprovativo enviado') {
    return `
      <button class="btn btn-sm btn-primary" onclick="handlePedidoAction(${id}, 'validar')">Validar</button>
      <button class="btn btn-sm btn-danger"  onclick="handlePedidoAction(${id}, 'rejeitar')">Rejeitar</button>
      <button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Ver Comprovativo</button>`;
  }
  if (norm === 'rejeitado') {
    return `
      <button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${id}, 'cancelar')">Cancelar</button>`;
  }
  if (norm === 'confirmada' || norm === 'cancelada') {
    return caminhoComprovativo
      ? `<button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Ver Comprovativo</button>`
      : '';
  }
  return '';
}

function badgeEstadoConsulta(estado) {
  const texto = estado || '—';
  const norm = String(texto).trim().toLowerCase();
  const map = {
    'agendada':             'badge-amber',
    'confirmada':           'badge-amber',
    'pendente':             'badge-amber',
    'pagamento enviado':    'badge-amber',
    'em curso':             'badge-blue',
    'em andamento':         'badge-blue',
    'finalizada':           'badge-green',
    'concluida':            'badge-green',
    'concluída':            'badge-green',
    'validado':             'badge-green',
    'confirmado':           'badge-green',
    'cancelada':            'badge-red',
    'cancelado':            'badge-red',
    'rejeitada':            'badge-red',
    'rejeitado':            'badge-red'
  };
  return `<span class="badge ${map[norm] || 'badge-gray'}">${texto}</span>`;
}

function renderComprovativoCell(value) {
  if (!value) return '—';
  const safe = String(value).replace(/'/g, "\\'");
  return `<button class="btn btn-sm btn-outline" onclick="openComprovativoFromPath('${safe}')">Ver comprovativo</button>`;
}

function openComprovativoFromPath(path) {
  if (!path) return;
  window.open(`http://localhost:5290/api/Secretaria/comprovativo?caminho=${encodeURIComponent(path)}`, '_blank');
}

function openComprovativo(id) {
  window.open(`http://localhost:5290/api/Admin/${id}/comprovativo`, '_blank');
}

window.addEventListener('load', initSecretaria);