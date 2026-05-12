// ─────────────────────────────────────────────────────────────────────────────
// CLIENTES
// ─────────────────────────────────────────────────────────────────────────────

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/cliente');

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

async function editarCliente(nif) {
  const item = adminState.clientes.get(nif) || {};
  const contactos = Array.isArray(item.contactos) ? item.contactos : [];
  const emailObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 2);
  const telObj = contactos.find(c => Number(c.tipoContacto || c.TipoContacto) === 1);

  document.getElementById('eclNif').value = nif;
  document.getElementById('eclNome').value = item.clienteNome || item.Nome || '';
  document.getElementById('eclEmail').value = (emailObj?.contacto || emailObj?.Contacto || '').trim();
  document.getElementById('eclTel').value = (telObj?.contacto || telObj?.Contacto || '').trim();
  openModal('modalEditCliente');
}

async function submitEditCliente(event) {
  event.preventDefault();
  const nif = document.getElementById('eclNif').value;
  const payload = {
    Nif_cliente: nif,
    Nome: document.getElementById('eclNome').value.trim(),
    Contactos: []
  };
  const email = document.getElementById('eclEmail').value.trim();
  const tel = document.getElementById('eclTel').value.trim();
  if (email) payload.Contactos.push({ TipoContacto: 2, Contacto: email });
  if (tel) payload.Contactos.push({ TipoContacto: 1, Contacto: tel });

  try {
    await fetchJson(`/Admin/cliente/${nif}`, { method: 'PUT', body: payload });
    showToast('Cliente atualizado com sucesso.', 'success');
    closeModal('modalEditCliente');
    loadClientes();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao atualizar cliente.'), 'error');
  }
}

async function removerCliente(nif) {
  openActionConfirmModal('Remover Cliente', 'Tem a certeza que deseja remover este cliente?', async () => {
    try {
      await fetchJson(`/Admin/cliente/${nif}`, { method: 'DELETE' });
      showToast('Cliente removida.', 'success');
      loadClientes();
      loadTopLists();
    } catch (e) { showToast(getErrorMessage(e, 'Erro ao remover cliente.'), 'error'); }
  });
}
