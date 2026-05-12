// ─────────────────────────────────────────────────────────────────────────────
// PACIENTES
// ─────────────────────────────────────────────────────────────────────────────

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/paciente');

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

async function editarPaciente(id) {
  const item = adminState.pacientes.get(id) || {};
  document.getElementById('epPacienteId').value = id;
  document.getElementById('epNome').value = pickValue(item, ['pacienteNome', 'PacienteNome', 'Nome']) || '';
  document.getElementById('epNasc').value = (item.pacienteData_nascimento || item.Data_nascimento || '').split('T')[0];
  document.getElementById('epGenero').value = item.idGenero || item.IdGenero || item.id_genero || item.Id_genero || '';
  document.getElementById('epTipo').value = item.idClientePaciente || item.IdClientePaciente || item.id_cliente_paciente || item.Id_cliente_paciente || '';
  openModal('modalEditPaciente');
}

async function submitEditPaciente(event) {
  event.preventDefault();
  const id   = parseInt(document.getElementById('epPacienteId').value, 10);
  const payload = {
    IdPaciente: id,
    PacienteNome: document.getElementById('epNome').value.trim(),
    DataNascimento: document.getElementById('epNasc').value,
    IdGenero: parseInt(document.getElementById('epGenero').value, 10),
    IdClientePaciente: parseInt(document.getElementById('epTipo').value, 10)
  };
  try {
    await fetchJson(`/Admin/paciente/${id}`, { method: 'PUT', body: payload });
    showToast('Paciente atualizado com sucesso.', 'success');
    closeModal('modalEditPaciente');
    loadPacientes();
    loadTopLists();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar paciente.'), 'error');
  }
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
