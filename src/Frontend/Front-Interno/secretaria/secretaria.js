function validateSecretariaAccess() {
  if (!token || role !== 'Secretaria') redirectToLogin();
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

function openModal(id) {
  document.getElementById(id)?.classList.add('open');
}

function closeModal(id) {
  document.getElementById(id)?.classList.remove('open');
}

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
    loadPedidos(),
    loadEstados()
  ]);
  populateSecretariaSelects();
}

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
  } catch {
    showToast('Não foi possível carregar métricas.', 'error');
  }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  body.innerHTML = consultas.slice(0, 6).map((item, index) => `
    <tr>
      <td>${pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1}</td>
      <td>${pickValue(item, ['pacienteNome', 'PacienteNome', 'Paciente', 'paciente', 'PacienteNome']) || '—'}</td>
      <td>${pickValue(item, ['medicoNome', 'MedicoNome', 'nomeMedico', 'NomeMedico']) || '—'}</td>
      <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || '—'}</td>
      <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta || item.Data_Consulta)}</td>
      <td><span class="badge badge-gray">${pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || '—'}</span></td>
    </tr>`).join('');
}

async function loadEstados() {
  try {
    const estados = await fetchJson('/Secretaria/estado');
    const target = document.getElementById('cEstado');
    if (!target) return;
    target.innerHTML = '<option value="">Seleccionar...</option>' + estados.map(est => `<option value="${pickValue(est, ['consultaId', 'id', 'ConsultaId', 'Id'])}">${pickValue(est, ['descricao', 'Descricao', 'EstadoDescricao', 'Nome', 'descricao']) || 'Estado'}</option>`).join('');
  } catch {
    console.warn('Não foi possível carregar os estados');
  }
}

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const consultas = await fetchJson('/Secretaria/consulta');
    if (!consultas.length) {
      body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
      return;
    }
    body.innerHTML = consultas.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['consultaId', 'ConsultaId', 'id']) || index + 1}</td>
        <td>${pickValue(item, ['pacienteNome', 'PacienteNome', 'Paciente', 'paciente']) || '—'}</td>
        <td>${pickValue(item, ['medicoNome', 'MedicoNome', 'nomeMedico', 'NomeMedico']) || '—'}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta || item.Data_Consulta)}</td>
        <td><span class="badge badge-gray">${pickValue(item, ['estadoDescricao', 'EstadoDescricao', 'estado', 'Estado']) || '—'}</span></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const pacientes = await fetchJson('/Secretaria/paciente');
    if (!pacientes.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum paciente encontrado.</td></tr>';
      return;
    }
    body.innerHTML = pacientes.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['pacienteId', 'PacienteId', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['pacienteNome', 'PacienteNome', 'Nome', 'nome']) || '—'}</td>
        <td>${formatDateShort(item.pacienteData_nascimento || item.Data_nascimento || item.DataNascimento || item.Data_Consulta)}</td>
        <td>${pickValue(item, ['cliente', 'Cliente', 'Cliente_Paciente', 'clienteNome', 'ClienteNome', 'nif_cliente', 'Nif_cliente']) || '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pacientes.</td></tr>';
  }
}

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const clientes = await fetchJson('/Secretaria/cliente');
    if (!clientes.length) {
      body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhum cliente encontrado.</td></tr>';
      return;
    }
    body.innerHTML = clientes.map((item) => `
      <tr>
        <td>${pickValue(item, ['clienteNif', 'ClienteNif', 'Nif_cliente', 'nif_cliente']) || '—'}</td>
        <td>${pickValue(item, ['clienteNome', 'ClienteNome', 'Nome', 'nome']) || '—'}</td>
        <td>${Array.isArray(item.contactos) ? item.contactos.map(c => pickValue(c, ['contacto', 'Contacto']) || '—').join(', ') : '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar clientes.</td></tr>';
  }
}

async function loadEspecialidades() {
  const body = document.getElementById('bodyEspecialidades');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/especialidade');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhuma especialidade encontrada.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['especialidadeId', 'EspecialidadeId', 'id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['especialidadeNome', 'EspecialidadeNome', 'Nome', 'nome', 'NomeEspecialidade']) || '—'}</td>
        <td>${pickValue(item, ['especialidadeDescricao', 'EspecialidadeDescricao', 'Descricao', 'descricao']) || '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar especialidades.</td></tr>';
  }
}

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/servico');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviço encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['servicoId', 'ServicoId', 'id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Nome', 'nome']) || '—'}</td>
        <td>${pickValue(item, ['especialidade', 'nomeEspecialidade', 'Especialidade', 'NomeEspecialidade']) || '—'}</td>
        <td>${pickValue(item, ['servicoPreco', 'ServicoPreco', 'Preco', 'preco']) || '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar serviços.</td></tr>';
  }
}

async function loadMedicos() {
  const body = document.getElementById('bodyMedicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Secretaria/medicos');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--muted);padding:24px">Nenhum médico encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${pickValue(item, ['id', 'Id']) || index + 1}</td>
        <td>${pickValue(item, ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']) || '—'}</td>
        <td>${pickValue(item, ['nomeEspecialidade', 'NomeEspecialidade', 'especialidade', 'Especialidade']) || '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="3" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar médicos.</td></tr>';
  }
}

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Secretaria/pedidos');
    const items = res?.dados || [];
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => `
      <tr>
        <td>${pickValue(item, ['id', 'Id']) || '—'}</td>
        <td>${pickValue(item, ['clienteNome', 'ClienteNome', 'NomeCliente', 'nomeCliente', 'Cliente', 'cliente']) || '—'}</td>
        <td>${pickValue(item, ['servicoNome', 'ServicoNome', 'Servico', 'servico']) || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta || item.HorarioPreferencial || item.HorarioReservado)}</td>
        <td><span class="badge badge-gray">${pickValue(item, ['estado', 'Estado', 'estadoDescricao', 'EstadoDescricao']) || '—'}</span></td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;"><button class="btn btn-sm btn-success" onclick="handlePedidoAction(${pickValue(item, ['id', 'Id'])}, 'confirmar')">Confirmar</button><button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${pickValue(item, ['id', 'Id'])}, 'cancelar')">Cancelar</button><button class="btn btn-sm btn-primary" onclick="handlePedidoAction(${pickValue(item, ['id', 'Id'])}, 'validar')">Validar</button><button class="btn btn-sm btn-danger" onclick="handlePedidoAction(${pickValue(item, ['id', 'Id'])}, 'rejeitar')">Rejeitar</button><button class="btn btn-sm btn-outline" onclick="openComprovativo(${pickValue(item, ['id', 'Id'])})">Comprovativo</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pedidos.</td></tr>';
  }
}

async function handlePedidoAction(id, action) {
  const mapping = { confirmar: 'confirmar', cancelar: 'cancelar', validar: 'validar', rejeitar: 'rejeitar' };
  try {
    const result = await fetchJson(`/Secretaria/${id}/${mapping[action]}`, { method: 'PUT' });
    showToast(result.mensagem || 'Operação concluída.', 'success');
    loadPedidos();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Falha na operação.', 'error');
  }
}

function openComprovativo(id) {
  window.open(`http://localhost:5290/api/Secretaria/${id}/comprovativo`, '_blank');
}

function pickValue(item, keys) {
  for (const key of keys) {
    if (item && item[key] !== undefined && item[key] !== null) {
      return item[key];
    }
  }
  return '';
}

function createOption(value, label) {
  return `<option value="${value}">${label}</option>`;
}

function setSelectOptions(selectId, items, valueKeys, labelKeys) {
  const select = document.getElementById(selectId);
  if (!select) return;
  const valueArray = Array.isArray(valueKeys) ? valueKeys : [valueKeys];
  const labelArray = Array.isArray(labelKeys) ? labelKeys : [labelKeys];
  select.innerHTML = '<option value="">Seleccionar...</option>' + items.map(item => {
    const value = pickValue(item, valueArray);
    const label = pickValue(item, labelArray) || value;
    return createOption(value, label);
  }).join('');
}

async function populateSecretariaSelects() {
  try {
    const [clientes, medicos, especialidades, pacientes] = await Promise.all([
      fetchJson('/Secretaria/cliente'),
      fetchJson('/Secretaria/medicos'),
      fetchJson('/Secretaria/especialidade'),
      fetchJson('/Secretaria/paciente')
    ]);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', await fetchJson('/Secretaria/servico') || [], ['id', 'Id'], ['servicoNome', 'ServicoNome', 'nome', 'Nome']);
    setSelectOptions('cPaciente', pacientes || [], ['id', 'Id', 'pacienteId', 'PacienteId'], ['pacienteNome', 'PacienteNome']);
  } catch (error) {
    console.warn('Falha ao preencher selects', error);
  }
}

async function submitConsulta(event) {
  event.preventDefault();
  try {
    const payload = {
      Id_medico_especialiade: parseInt(document.getElementById('cMedico').value, 10),
      Id_servico: parseInt(document.getElementById('cServico').value, 10),
      Id_paciente: parseInt(document.getElementById('cPaciente').value, 10),
      Id_estado_consulta: parseInt(document.getElementById('cEstado').value, 10),
      Data_consulta: document.getElementById('cData').value
    };
    await fetchJson('/Secretaria/consulta', { method: 'POST', body: payload });
    showToast('Consulta criada com sucesso.', 'success');
    closeModal('modalConsulta');
    loadConsultas();
    loadTopStats();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar consulta.', 'error');
  }
}

async function submitPaciente(event) {
  event.preventDefault();
  try {
    const payload = {
      PacienteNome: document.getElementById('pNome').value,
      PacienteData_nascimento: document.getElementById('pNasc').value,
      IdCliente_Paciente: parseInt(document.getElementById('pCliente').value, 10),
      IdGenero: parseInt(document.getElementById('pGenero').value, 10),
      Nif_cliente: document.getElementById('pCliente').value
    };
    await fetchJson('/Secretaria/paciente', { method: 'POST', body: payload });
    showToast('Paciente criado com sucesso.', 'success');
    closeModal('modalPaciente');
    loadPacientes();
    loadTopStats();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar paciente.', 'error');
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
    if (!payload.Contactos.length) {
      showToast('Informe pelo menos um contacto: email ou telefone.', 'error');
      return;
    }
    await fetchJson('/Secretaria/cliente', { method: 'POST', body: payload });
    showToast('Cliente criado com sucesso.', 'success');
    closeModal('modalCliente');
    loadClientes();
    populateSecretariaSelects();
    loadTopStats();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao criar cliente.'), 'error');
  }
}

window.addEventListener('load', initSecretaria);
