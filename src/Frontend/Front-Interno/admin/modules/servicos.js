// ─────────────────────────────────────────────────────────────────────────────
// SERVIÇOS
// ─────────────────────────────────────────────────────────────────────────────

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/servicos');
    const list = items || [];
    
    // Cache in state if needed (optional but good for consistency)
    if (!adminState.servicos) adminState.servicos = new Map();
    adminState.servicos.clear();
    list.forEach(item => adminState.servicos.set(item.servicoId || item.Id, item));

    body.innerHTML = list.length
      ? list.map((item, i) => `<tr>
          <td>${item.servicoId || item.Id || i + 1}</td>
          <td>${item.servicoNome || item.nome || '—'}</td>
          <td>${item.nomeEspecialidade || item.especialidade || '—'}</td>
          <td>${item.servicoPreco ?? item.preco ?? '—'} Kz</td>
          <td>${item.servicoEstado ?? item.estado ? badgeEstado('Activo') : badgeEstado('Inactivo')}</td>
          <td>
            <button class="btn btn-sm btn-outline" onclick="editarServico(${item.servicoId || item.Id})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="removerServico(${item.servicoId || item.Id})">Remover</button>
          </td>
        </tr>`).join('')
      : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviço encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar serviços.</td></tr>';
  }
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

async function editarServico(id) {
  const item = adminState.servicos?.get(id) || {};
  document.getElementById('esId').value = id;
  document.getElementById('esNome').value = item.servicoNome || item.nome || '';
  document.getElementById('esEsp').value = item.idEspecialidade || item.id_especialidade || '';
  document.getElementById('esDuracao').value = item.servicoDuracaoMinuto || item.duracao_minuto || 30;
  document.getElementById('esPreco').value = item.servicoPreco || item.preco || 0;
  document.getElementById('esEstado').value = String(item.servicoEstado ?? item.estado ?? true);
  
  openModal('modalEditServico');
}

async function submitEditServico(event) {
  event.preventDefault();
  const id = parseInt(document.getElementById('esId').value, 10);
  const payload = {
    ServicoId: id,
    ServicoNome: document.getElementById('esNome').value.trim(),
    IdEspecialidade: parseInt(document.getElementById('esEsp').value, 10),
    ServicoDuracaoMinuto: parseInt(document.getElementById('esDuracao').value, 10),
    ServicoPreco: parseFloat(document.getElementById('esPreco').value),
    ServicoEstado: document.getElementById('esEstado').value === 'true'
  };

  try {
    await fetchJson(`/Admin/servicos/${id}`, { method: 'PUT', body: payload });
    showToast('Serviço atualizado com sucesso.', 'success');
    closeModal('modalEditServico');
    loadServicos();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar serviço.'), 'error');
  }
}

async function removerServico(id) {
  openActionConfirmModal('Remover Serviço', 'Tem a certeza que deseja remover este serviço?', async () => {
    try {
      await fetchJson(`/Admin/servicos/${id}`, { method: 'DELETE' });
      showToast('Serviço removido.', 'success');
      loadServicos();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover serviço.'), 'error'); }
  });
}
