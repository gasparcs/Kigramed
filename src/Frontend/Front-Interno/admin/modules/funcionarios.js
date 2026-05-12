// ─────────────────────────────────────────────────────────────────────────────
// FUNCIONÁRIOS
// ─────────────────────────────────────────────────────────────────────────────

async function loadFuncionarios() {
  const body = document.getElementById('bodyFuncionarios');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/funcionario');
    const list = items || [];

    adminState.funcionarios.clear();
    list.forEach(item => {
      const nif = item.funcionarioNif || item.FuncionarioNif || item.Nif || item.Nif_funcionario || '';
      adminState.funcionarios.set(nif, item);
    });

    body.innerHTML = list.length ? list.map((item) => {
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

async function editarFuncionario(nif) {
  const item = adminState.funcionarios.get(nif) || {};
  const contactos = Array.isArray(item.contactos) ? item.contactos : [];
  const telObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 1);
  const especialidades = Array.isArray(item.medicoEspecialidades) ? item.medicoEspecialidades : [];

  document.getElementById('efNif').value = nif;
  document.getElementById('efNome').value = item.funcionarioNome || item.Nome || '';
  document.getElementById('efPerfil').value = item.id_Perfil || item.Id_Perfil || '';
  document.getElementById('efEstado').value = String(item.funcionaroEstado ?? item.FuncionaroEstado ?? item.funcionarioEstado ?? true);
  document.getElementById('efTel').value = (telObj?.contacto || telObj?.Contacto || '').trim();
  document.getElementById('efEspecialidade').value = especialidades[0]?.id_especialidade || especialidades[0]?.Id_especialidade || '';
  
  openModal('modalEditFuncionario');
}

async function submitEditFuncionario(event) {
  event.preventDefault();
  const nif = document.getElementById('efNif').value;
  const payload = {
    FuncionarioNif: nif,
    FuncionarioNome: document.getElementById('efNome').value.trim(),
    FuncionarioPerfil: parseInt(document.getElementById('efPerfil').value, 10),
    FuncionarioEstado: document.getElementById('efEstado').value === 'true',
    Contactos: [],
    Especialidades: []
  };

  const tel = document.getElementById('efTel').value.trim();
  if (tel) payload.Contactos.push({ TipoContacto: 1, Contacto: tel });

  const esp = document.getElementById('efEspecialidade').value;
  if (esp) payload.Especialidades.push({ IdEspecialidade: parseInt(esp, 10) });

  try {
    await fetchJson(`/Admin/funcionario/${nif}`, { method: 'PUT', body: payload });
    showToast('Funcionário atualizado com sucesso.', 'success');
    closeModal('modalEditFuncionario');
    loadFuncionarios();
    loadTopLists();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar funcionário.'), 'error');
  }
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
