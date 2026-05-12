// ─────────────────────────────────────────────────────────────────────────────
// ESPECIALIDADES
// ─────────────────────────────────────────────────────────────────────────────

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

async function editarEspecialidade(id) {
  // Use generic modal or specific one? The user wants pre-fill.
  // Currently specialities use generic modal. I'll stick to it but pre-fill correctly.
  const items = await fetchJson('/Admin/especialidade');
  const item = items.find(i => (i.especialidadeId || i.Id) === id) || {};

  openActionFormModal({
    title: 'Editar Especialidade',
    nome: item.especialidadeNome || item.Nome || '',
    descricao: item.especialidadeDescricao || item.Descricao || '',
    showDescricao: true,
    onSubmit: async ({ nome, descricao }) => {
      await fetchJson(`/Admin/especialidade/${id}`, { 
        method: 'PUT', 
        body: { EspecialidadeId: id, EspecialidadeNome: nome, EspecialidadeDescricao: descricao } 
      });
      showToast('Especialidade atualizada.', 'success');
      loadEspecialidades();
      populateFormSelects();
    }
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
