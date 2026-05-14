// ─────────────────────────────────────────────
// ESTADO LOCAL
// ─────────────────────────────────────────────
const medicoState = {
  consultas: new Map()
};

const sectionLoaders = {
  'dashboard': () => loadTopStats(),
  'consultas': () => loadConsultas()
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

// ─────────────────────────────────────────────
// INICIALIZAÇÃO
// ─────────────────────────────────────────────
function validateMedicoAccess() {
  const roleNorm = String(role || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
  if (!token || roleNorm !== 'medico') redirectToLogin();
  document.getElementById('sidebarName').textContent = nome;
  document.getElementById('topbarName').textContent = nome;
  document.getElementById('sidebarAvatar').textContent = nome.slice(0, 2).toUpperCase();
}

function showSection(sectionId, button) {
  if (window.location.hash !== '#' + sectionId) window.location.hash = sectionId;
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

async function initMedico() {
  validateMedicoAccess();
  await Promise.all([loadConsultas(), loadTopStats(), loadEstadosConsulta()]);

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

// ─────────────────────────────────────────────
// ESTADOS DE CONSULTA (dinâmico)
// ─────────────────────────────────────────────
async function loadEstadosConsulta() {
  try {
    const estados = await fetchJson('/Medico/estados');
    const select = document.getElementById('ecEstado');
    if (!select) return;
    select.innerHTML = '<option value="">Seleccionar...</option>' +
      estados.map(e => {
        const id = e.id ?? e.Id ?? e.idEstadoConsulta ?? e.IdEstadoConsulta;
        const desc = e.descricao ?? e.Descricao ?? e.estadoDescricao ?? e.EstadoDescricao ?? id;
        return `<option value="${id}">${desc}</option>`;
      }).join('');
  } catch (e) {
    console.error('Erro ao carregar estados de consulta:', e);
    // fallback: mantém opções hardcoded se já existirem no HTML
  }
}

// ─────────────────────────────────────────────
// STATS & DASHBOARD
// ─────────────────────────────────────────────
async function loadTopStats() {
  try {
    const res = await fetchJson('/Medico/consultas');
    const consultas = res?.dados || [];

    const total = consultas.length;
    const agendadas = consultas.filter(c => {
      const e = String(c.estadoDescricao || c.EstadoDescricao || '').toLowerCase();
      return e === 'agendada' || e === 'confirmada';
    }).length;
    const finalizadas = consultas.filter(c => {
      const e = String(c.estadoDescricao || c.EstadoDescricao || '').toLowerCase();
      return e === 'finalizada' || e === 'concluída' || e === 'concluida';
    }).length;
    const canceladas = consultas.filter(c => {
      const e = String(c.estadoDescricao || c.EstadoDescricao || '').toLowerCase();
      return e === 'cancelada' || e === 'cancelado';
    }).length;

    document.getElementById('statTotal').textContent = total;
    document.getElementById('statAgendadas').textContent = agendadas;
    document.getElementById('statFinalizadas').textContent = finalizadas;
    document.getElementById('statCanceladas').textContent = canceladas;

    renderDashboardConsultas(consultas);
  } catch {
    showToast('Não foi possível carregar o painel.', 'error');
  }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  const recentes = consultas.slice(0, 6);
  if (!recentes.length) {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
    return;
  }
  body.innerHTML = recentes.map((item, index) => `
    <tr>
      <td>${item.consultaId || item.ConsultaId || index + 1}</td>
      <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
      <td>${item.servicoNome || item.ServicoNome || '—'}</td>
      <td>${item.especialidade || item.Especialidade || '—'}</td>
      <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
      <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao)}</td>
    </tr>`).join('');
}

// ─────────────────────────────────────────────
// CARREGAR CONSULTAS
// ─────────────────────────────────────────────
async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const res = await fetchJson('/Medico/consultas');
    const list = res?.dados || [];

    medicoState.consultas.clear();
    list.forEach((item, index) => {
      const id = item.consultaId || item.ConsultaId || index + 1;
      medicoState.consultas.set(Number(id), item);
    });

    if (!list.length) {
      body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta agendada.</td></tr>';
      return;
    }

    body.innerHTML = list.map((item, index) => {
      const id = item.consultaId || item.ConsultaId || index + 1;
      const estadoDesc = String(item.estadoDescricao || item.EstadoDescricao || '').trim().toLowerCase();
      const finalizada = estadoDesc === 'finalizada' || estadoDesc === 'concluída' || estadoDesc === 'concluida';
      return `<tr>
        <td>${id}</td>
        <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
        <td>${item.servicoNome || item.ServicoNome || '—'}</td>
        <td>${item.especialidade || item.Especialidade || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
        <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao)}</td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;">
          ${finalizada
            ? `<button class="btn btn-sm btn-outline" disabled title="Consulta finalizada" style="opacity:0.45;cursor:not-allowed;">Editar</button>`
            : `<button class="btn btn-sm btn-outline" onclick="editarConsulta(${id})">Editar</button>`
          }
        </td>
      </tr>`;
    }).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

// ─────────────────────────────────────────────
// EDITAR CONSULTA
// ─────────────────────────────────────────────
function editarConsulta(id) {
  const item = medicoState.consultas.get(Number(id)) || {};
  document.getElementById('ecConsultaId').value = id;
  document.getElementById('ecPaciente').value = item.pacienteNome || item.PacienteNome || '—';
  document.getElementById('ecData').value = toDatetimeLocal(item.data_consulta || item.Data_consulta || item.DataConsulta);

  // Aguarda o select estar populado antes de definir o valor
  const select = document.getElementById('ecEstado');
  const idEstado = item.idEstadoConsulta || item.IdEstadoConsulta || '';
  if (select.options.length <= 1) {
    // estados ainda não carregados, tenta carregar e depois define
    loadEstadosConsulta().then(() => {
      document.getElementById('ecEstado').value = idEstado;
    });
  } else {
    select.value = idEstado;
  }

  openModal('modalEditConsulta');
}

async function submitEditConsulta(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('ecConsultaId').value, 10);
  const idEstado = parseInt(document.getElementById('ecEstado').value, 10);

  if (!idEstado) {
    showToast('Por favor seleccione um estado.', 'error');
    return;
  }

  try {
    await fetchJson(`/Medico/consulta/${id}`, {
      method: 'PUT',
      body: {
        IdConsulta: id,
        Id_medico_especialiade: medicoState.consultas.get(Number(id))?.idMedicoEspecialidade
          || medicoState.consultas.get(Number(id))?.IdMedicoEspecialidade || 0,
        Id_estado_consulta: idEstado,
        Data_consulta: document.getElementById('ecData').value
      }
    });
    showToast('Consulta actualizada com sucesso.', 'success');
    closeModal('modalEditConsulta');
    await Promise.all([loadConsultas(), loadTopStats()]);
  } catch (e) {
    console.error('Erro em submitEditConsulta:', e);
    showToast(getErrorMessage(e, 'Erro ao actualizar consulta.'), 'error');
  }
}

// ─────────────────────────────────────────────
// BADGE DE ESTADO
// ─────────────────────────────────────────────
function badgeEstadoConsulta(estado) {
  const texto = estado || '—';
  const norm = String(texto).trim().toLowerCase();
  const map = {
    'agendada':    'badge-amber',
    'confirmada':  'badge-amber',
    'em curso':    'badge-blue',
    'finalizada':  'badge-green',
    'concluída':   'badge-green',
    'concluida':   'badge-green',
    'cancelada':   'badge-red',
    'cancelado':   'badge-red'
  };
  return `<span class="badge ${map[norm] || 'badge-gray'}">${texto}</span>`;
}

// ─────────────────────────────────────────────
// ARRANQUE
// ─────────────────────────────────────────────
window.addEventListener('load', initMedico);