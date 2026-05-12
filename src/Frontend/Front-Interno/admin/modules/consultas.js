// ─────────────────────────────────────────────────────────────────────────────
// CONSULTAS
// ─────────────────────────────────────────────────────────────────────────────

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  const dashBody = document.getElementById('dashConsultas');
  if (!body && !dashBody) return;
  try {
    const items = await fetchJson('/Admin/consulta');
    const list = items || [];

    adminState.consultas.clear();
    list.forEach((item, index) => {
      const id = item.consultaId || item.ConsultaId || index + 1;
      adminState.consultas.set(id, item);
    });

    if (body) {
      body.innerHTML = list.length
        ? list.map((item, i) => {
            const estadoDesc = (item.estadoDescricao || item.EstadoDescricao || '').trim().toLowerCase();
            const finalizada = estadoDesc === 'finalizada';
            const consultaId = item.consultaId || item.ConsultaId || i + 1;
            return `<tr>
              <td>${consultaId}</td>
              <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
              <td>${item.medicoNome || item.MedicoNome || '—'}</td>
              <td>${item.servicoNome || item.ServicoNome || '—'}</td>
              <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
              <td>${badgeEstadoConsulta(item.estadoDescricao || item.EstadoDescricao || '—')}</td>
              <td>
                ${finalizada
                  ? `<button class="btn btn-sm btn-outline" disabled title="Consulta finalizada — não é possível editar" style="opacity:0.45;cursor:not-allowed;">Editar</button>`
                  : `<button class="btn btn-sm btn-outline" onclick="editarConsulta(${consultaId})">Editar</button>`
                }
                <button class="btn btn-sm btn-danger" onclick="removerConsulta(${consultaId})">Remover</button>
              </td>
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
  document.getElementById('ecMedico').value = pickValue(item, ['id_medico_especialiade', 'Id_medico_especialiade', 'idMedicoEspecialidade', 'IdMedicoEspecialidade', 'idMedico', 'IdMedico']) || '';
  document.getElementById('ecEstado').value = pickValue(item, ['id_estado_consulta', 'Id_estado_consulta', 'idEstadoConsulta', 'IdEstadoConsulta']) || '';
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
    'pendente':    'badge-amber'
  };
  return `<span class="badge ${map[norm] || 'badge-gray'}">${texto}</span>`;
}
