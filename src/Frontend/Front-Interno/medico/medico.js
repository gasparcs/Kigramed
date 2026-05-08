function validateMedicoAccess() {
  if (!token || role !== 'Medico') redirectToLogin();
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

async function initMedico() {
  validateMedicoAccess();
  await Promise.all([loadConsultas(), loadTopStats()]);
}

async function loadTopStats() {
  try {
    const res = await fetchJson('/Medico/consultas');
    const consultas = res?.dados || [];
    document.getElementById('statConsultas').textContent = consultas.length;
    document.getElementById('statRecentes').textContent = consultas.slice(0, 3).length;
    renderDashboardConsultas(consultas);
  } catch {
    showToast('Não foi possível carregar o painel.', 'error');
  }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  if (!consultas.length) {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
    return;
  }
  body.innerHTML = consultas.slice(0, 5).map((item, index) => `
    <tr>
      <td>${item.consultaId || item.ConsultaId || index + 1}</td>
      <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
      <td>${item.servicoNome || item.ServicoNome || '—'}</td>
      <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
      <td><span class="badge badge-gray">${item.estadoDescricao || item.EstadoDescricao || '—'}</span></td>
    </tr>`).join('');
}

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const res = await fetchJson('/Medico/consultas');
    const consultas = res?.dados || [];
    if (!consultas.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta agendada.</td></tr>';
      return;
    }
    body.innerHTML = consultas.map((item, index) => `
      <tr>
        <td>${item.consultaId || item.ConsultaId || index + 1}</td>
        <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
        <td>${item.servicoNome || item.ServicoNome || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
        <td><span class="badge badge-gray">${item.estadoDescricao || item.EstadoDescricao || '—'}</span></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

window.addEventListener('load', initMedico);
