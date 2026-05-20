// -----------------------------------------------------------------------------
// CONSULTAS
// -----------------------------------------------------------------------------

let estadoFiltroActivo = null;

const CONSULTA_ACTION_LABELS = {
  confirmar: { text: 'Confirmar', className: 'btn btn-sm btn-success' },
  cancelar: { text: 'Cancelar', className: 'btn btn-sm btn-outline' },
  validar: { text: 'Validar', className: 'btn btn-sm btn-primary' },
  verComprovativo: { text: 'Ver Comprovativo', className: 'btn btn-sm btn-outline' },
  editar: { text: 'Editar', className: 'btn btn-sm btn-outline' },
  remover: { text: 'Remover', className: 'btn btn-sm btn-danger' }
};

const CONSULTA_ACTIONS_BY_STATE = {
  'pendente': ['confirmar', 'cancelar'],
  'aguarda pagamento': [],
  'aguardar pagamento': [],
  'comprovativo enviado': ['validar', 'verComprovativo', 'remover'],
  'confirmado': ['editar', 'remover'],
  'confirmada': ['editar', 'remover']
};

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  const dashBody = document.getElementById('dashConsultas');
  if (!body && !dashBody) return;
  try {
    const url = estadoFiltroActivo ? `/Admin/consulta?estado=${encodeURIComponent(estadoFiltroActivo)}` : '/Admin/consulta';
    const items = await fetchJson(url);
    const list = items || [];

    adminState.consultas.clear();
    list.forEach((item, index) => {
      const id = item.consultaId || item.ConsultaId || index + 1;
      adminState.consultas.set(id, item);
    });

    if (body) {
      body.innerHTML = list.length
        ? list.map((item, i) => {
            const consultaId = item.consultaId || item.ConsultaId || i + 1;
            const estadoDesc = item.estadoDescricao || item.EstadoDescricao || '';
            return `<tr>
              <td>${consultaId}</td>
              <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
              <td>${item.medicoNome || item.MedicoNome || '—'}</td>
              <td>${item.servicoNome || item.ServicoNome || '—'}</td>
              <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
              <td>${badgeEstadoConsulta(estadoDesc || '—')}</td>
              <td style="display:flex;gap:6px;flex-wrap:wrap;">${renderConsultaActions(consultaId, estadoDesc)}</td>
            </tr>`;
          }).join('')
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
            <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao || '—')}</td>
          </tr>`).join('')
        : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
    }
  } catch {
    if (body) body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
    if (dashBody) dashBody.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

function filtrarConsultasPorEstado(estado) {
  estadoFiltroActivo = estado;
  loadConsultas();
}

function normalizeConsultaEstado(estado) {
  return String(estado || '').trim().toLowerCase();
}

function getConsultaActionsByEstado(estado) {
  const norm = normalizeConsultaEstado(estado);
  return CONSULTA_ACTIONS_BY_STATE[norm] ?? [];
}

function renderConsultaActions(id, estado) {
  const actions = getConsultaActionsByEstado(estado);
  if (!actions.length) return '<span style="color:var(--muted)">—</span>';
  return actions.map((action) => {
    const def = CONSULTA_ACTION_LABELS[action];
    if (!def) return '';
    return `<button class="${def.className}" onclick="handleConsultaAction(${id}, '${action}')">${def.text}</button>`;
  }).join('');
}

async function handleConsultaAction(id, action) {
  if (action === 'editar') {
    await editarConsulta(id);
    return;
  }
  if (action === 'remover') {
    await removerConsulta(id);
    return;
  }
  if (action === 'verComprovativo') {
    openComprovativoConsulta(id);
    return;
  }

  const mapping = { confirmar: 'confirmar', cancelar: 'cancelar', validar: 'validar' };
  const endpoint = mapping[action];
  if (!endpoint) return;

  try {
    const result = await fetchJson(`/Admin/${id}/${endpoint}`, { method: 'PUT' });
    showToast(result?.mensagem || 'Operação concluída.', 'success');
    await Promise.all([loadConsultas(), loadTopLists()]);
  } catch (e) {
    showToast(getErrorMessage(e), 'error');
  }
}

function openComprovativoConsulta(id) {
  window.open(`http://localhost:5290/api/Admin/${id}/comprovativo`, '_blank');
}

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

async function editarConsulta(id) {
  const item = adminState.consultas.get(id) || {};
  document.getElementById('ecConsultaId').value = id;
  document.getElementById('ecData').value = toDatetimeLocal(item.data_consulta || item.Data_consulta || item.DataConsulta);
  document.getElementById('ecMedico').value = item.idMedicoEspecialidade || '';
  document.getElementById('ecEstado').value = item.idEstadoConsulta || '';
  openModal('modalEditConsulta');
}

async function submitEditConsulta(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('ecConsultaId').value, 10);
  const payload = {
    IdConsulta: id,
    Id_medico_especialiade: parseInt(document.getElementById('ecMedico').value, 10),
    Id_estado_consulta: parseInt(document.getElementById('ecEstado').value, 10),
    Data_consulta: document.getElementById('ecData').value
  };
  try {
    await fetchJson(`/Admin/consulta/${id}`, { method: 'PUT', body: payload });
    showToast('Consulta atualizada com sucesso.', 'success');
    closeModal('modalEditConsulta');
    loadConsultas();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar consulta.'), 'error');
  }
}

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

function badgeEstadoConsulta(estado) {
  const texto = estado || '—';
  const norm = String(texto).trim().toLowerCase();
  const map = {
    'agendada':    'badge-amber',
    'confirmada':  'badge-amber',
    'em curso':    'badge-blue',
    'em andamento':'badge-blue',
    'finalizada':  'badge-green',
    'concluida':   'badge-green',
    'concluída':   'badge-green',
    'cancelada':   'badge-red',
    'cancelado':   'badge-red',
    'rejeitada':   'badge-red',
    'rejeitado':   'badge-red',
    'pendente':    'badge-amber',
    'aguarda pagamento': 'badge-blue',
    'comprovativo enviado': 'badge-blue',
    'confirmado': 'badge-green'
  };
  return `<span class="badge ${map[norm] || 'badge-gray'}">${texto}</span>`;
}
