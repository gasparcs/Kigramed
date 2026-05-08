function parseJwt(token) {
  if (!token) return null;
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
  } catch {
    return null;
  }
}

function getUserId() {
  const payload = parseJwt(token);
  return payload?.nameid || payload?.sub || null;
}

function filterTable(input, tableId) {
  const query = input.value.toLowerCase();
  const rows = document.querySelectorAll(`#${tableId} tbody tr`);
  rows.forEach(row => {
    const text = row.textContent.toLowerCase();
    row.style.display = text.includes(query) ? '' : 'none';
  });
}

function validateAccess() {
  if (!token || role !== 'Admin') redirectToLogin();
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

async function initAdmin() {
  validateAccess();
  await Promise.all([
    loadEstados(),
    loadTopLists(),
    loadConsultas(),
    loadPacientes(),
    loadClientes(),
    loadFuncionarios(),
    loadEspecialidades(),
    loadServicos(),
    loadPagamentos(),
    loadPagamentoConsulta(),
    loadSMS(),
    loadPedidos(),
  ]);
  populateFormSelects();
}

async function loadTopLists() {
  try {
    const [consultas, pacientes, funcionarios, pagamentos] = await Promise.all([
      fetchJson('/Admin/consulta'),
      fetchJson('/Admin/paciente'),
      fetchJson('/Admin/funcionario'),
      fetchJson('/Admin/pagamento')
    ]);
    document.getElementById('statConsultas').textContent = consultas?.length ?? 0;
    document.getElementById('statPacientes').textContent = pacientes?.length ?? 0;
    document.getElementById('statFuncionarios').textContent = funcionarios?.length ?? 0;
    document.getElementById('statPagamentos').textContent = pagamentos?.length ?? 0;
    renderDashboardConsultas(consultas || []);
  } catch (error) {
    showToast('Não foi possível carregar os indicadores.', 'error');
  }
}

function renderDashboardConsultas(consultas) {
  const body = document.getElementById('dashConsultas');
  if (!body) return;
  body.innerHTML = consultas.slice(0, 6).map((consulta, index) => `
    <tr>
      <td>${consulta.consultaId || consulta.ConsultaId || index + 1}</td>
      <td>${consulta.pacienteNome || consulta.PacienteNome || consulta.ClienteNome || '—'}</td>
      <td>${consulta.medicoNome || consulta.MedicoNome || '—'}</td>
      <td>${consulta.servicoNome || consulta.ServicoNome || '—'}</td>
      <td>${formatDate(consulta.data_consulta || consulta.Data_consulta || consulta.DataConsulta)}</td>
      <td><span class="badge badge-gray">${consulta.estadoDescricao || consulta.EstadoDescricao || '—'}</span></td>
    </tr>`).join('');
}

async function loadEstados() {
  try {
    const estados = await fetchJson('/Admin/estados');
    const target = document.getElementById('cEstado');
    if (!target) return;
    target.innerHTML = '<option value="">Seleccionar...</option>' + estados.map(est => `<option value="${est.consultaId || est.id || est.ConsultaId}">${est.descricao || est.Descricao || est.EstadoDescricao || 'Estado'}</option>`).join('');
  } catch (error) {
    console.warn('Não foi possível carregar estados', error);
  }
}

async function loadConsultas() {
  const body = document.getElementById('bodyConsultas');
  if (!body) return;
  try {
    const consultas = await fetchJson('/Admin/consulta');
    if (!consultas.length) {
      body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--muted);padding:24px">Nenhuma consulta encontrada.</td></tr>';
      return;
    }
    body.innerHTML = consultas.map((item, index) => `
      <tr>
        <td>${item.consultaId || item.ConsultaId || index + 1}</td>
        <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
        <td>${item.medicoNome || item.MedicoNome || '—'}</td>
        <td>${item.servicoNome || item.ServicoNome || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
        <td><span class="badge badge-gray">${item.estadoDescricao || item.EstadoDescricao || '—'}</span></td>
        <td><button class="btn btn-sm btn-outline" onclick="viewConsulta(${item.consultaId || item.ConsultaId})">Ver</button></td>
      </tr>`).join('');
  } catch (error) {
    body.innerHTML = '<tr><td colspan="7" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar consultas.</td></tr>';
  }
}

async function viewConsulta(id) {
  try {
    const consulta = await fetchJson(`/Admin/consulta/id/${id}`);
    alert(`Consulta #${id}\nPaciente: ${consulta.pacienteNome || consulta.PacienteNome}\nMédico: ${consulta.medicoNome || consulta.MedicoNome}\nServiço: ${consulta.servicoNome || consulta.ServicoNome}\nData: ${formatDate(consulta.data_consulta || consulta.Data_consulta)}\nEstado: ${consulta.estadoDescricao || consulta.EstadoDescricao}`);
  } catch {
    showToast('Não foi possível recuperar a consulta.', 'error');
  }
}

async function loadPacientes() {
  const body = document.getElementById('bodyPacientes');
  if (!body) return;
  try {
    const pacientes = await fetchJson('/Admin/paciente');
    if (!pacientes.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum paciente encontrado.</td></tr>';
      return;
    }
    body.innerHTML = pacientes.map((item, index) => `
      <tr>
        <td>${item.pacienteId || item.Id || index + 1}</td>
        <td>${item.pacienteNome || item.PacienteNome || '—'}</td>
        <td>${formatDateShort(item.pacienteData_nascimento || item.Data_nascimento || item.DataNascimento)}</td>
        <td>${item.clienteNome || item.ClienteNome || item.nif_cliente || item.Nif_cliente || '—'}</td>
        <td><button class="btn btn-sm btn-outline" onclick="alert('ID: ${item.pacienteId || item.Id}')">Detalhes</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pacientes.</td></tr>';
  }
}

async function loadClientes() {
  const body = document.getElementById('bodyClientes');
  if (!body) return;
  try {
    const clientes = await fetchJson('/Admin/cliente');
    if (!clientes.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum cliente encontrado.</td></tr>';
      return;
    }
    body.innerHTML = clientes.map((item) => `
      <tr>
        <td>${item.clienteNif || item.Nif_cliente || '—'}</td>
        <td>${item.clienteNome || item.Nome || '—'}</td>
        <td>${Array.isArray(item.contactos) ? item.contactos.map(c => c.contacto || c.Contacto).join(', ') : '—'}</td>
        <td><button class="btn btn-sm btn-outline" onclick="alert('NIF: ${item.clienteNif || item.Nif_cliente}')">Detalhes</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar clientes.</td></tr>';
  }
}

async function loadFuncionarios() {
  const body = document.getElementById('bodyFuncionarios');
  if (!body) return;
  try {
    const funcionarios = await fetchJson('/Admin/funcionario');
    if (!funcionarios.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum funcionário encontrado.</td></tr>';
      return;
    }
    body.innerHTML = funcionarios.map((item) => `
      <tr>
        <td>${item.funcionarioNif || item.Nif || item.Nif_funcionario || '—'}</td>
        <td>${item.funcionarioNome || item.Nome || '—'}</td>
        <td>${item.perfil || item.FuncionarioPerfil || '—'}</td>
        <td>${String(item.estado || item.FuncionarioEstado || '—')}</td>
        <td><button class="btn btn-sm btn-outline" onclick="alert('NIF: ${item.funcionarioNif || item.Nif || item.Nif_funcionario}')">Detalhes</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar funcionários.</td></tr>';
  }
}

async function loadEspecialidades() {
  const body = document.getElementById('bodyEspecialidades');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/especialidade');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhuma especialidade encontrada.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${item.especialidadeId || item.Id || index + 1}</td>
        <td>${item.especialidadeNome || item.Nome || item.NomeEspecialidade || '—'}</td>
        <td>${item.especialidadeDescricao || item.Descricao || '—'}</td>
        <td><button class="btn btn-sm btn-outline" onclick="alert('ID: ${item.especialidadeId || item.Id}')">Detalhes</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar especialidades.</td></tr>';
  }
}

async function loadServicos() {
  const body = document.getElementById('bodyServicos');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/servicos');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum serviço encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item, index) => `
      <tr>
        <td>${item.servicoId || item.Id || index + 1}</td>
        <td>${item.servicoNome || item.nome || '—'}</td>
        <td>${item.especialidade || item.nomeEspecialidade || item.Especialidade || '—'}</td>
        <td>${item.servicoPreco ?? item.Preco ?? '—'}</td>
        <td><button class="btn btn-sm btn-outline" onclick="alert('ID: ${item.servicoId || item.Id}')">Detalhes</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar serviços.</td></tr>';
  }
}

async function loadPagamentos() {
  const body = document.getElementById('bodyPagamentos');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/pagamento');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pagamento encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => `
      <tr>
        <td>${item.id || item.Id || '—'}</td>
        <td>${item.cliente || item.Cliente || '—'}</td>
        <td>${item.secretaria || item.Secretaria || '—'}</td>
        <td>${item.comprovativo || item.Comprovativo || '—'}</td>
        <td>${formatDate(item.dataEnvio || item.DataEnvio || item.Data)}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos.</td></tr>';
  }
}

async function loadPagamentoConsulta() {
  const body = document.getElementById('bodyPagamentoConsulta');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/pagamentoconsulta');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum registo encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => `
      <tr>
        <td>${item.id || item.Id || '—'}</td>
        <td>${item.idPagamento || item.IdPagamento || '—'}</td>
        <td>${item.idConsulta || item.IdConsulta || '—'}</td>
        <td>${formatDate(item.dataConsulta || item.DataConsulta)}</td>
        <td>${item.comprovativo || item.Comprovativo || '—'}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos de consulta.</td></tr>';
  }
}

async function loadSMS() {
  const body = document.getElementById('bodySMS');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/SMS');
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum SMS enviado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => `
      <tr>
        <td>${item.smsId || item.SmsId || '—'}</td>
        <td>${item.cliente?.clienteNome || item.cliente?.ClienteNome || '—'}</td>
        <td>${item.mensagem || item.Mensagem || '—'}</td>
        <td>${formatDate(item.dataEnvio || item.DataEnvio)}</td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar SMS.</td></tr>';
  }
}

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Admin/pedidos');
    const items = res?.dados || [];
    if (!items.length) {
      body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
      return;
    }
    body.innerHTML = items.map((item) => `
      <tr>
        <td>${item.id || item.Id || '—'}</td>
        <td>${item.clienteNome || item.ClienteNome || '—'}</td>
        <td>${item.servicoNome || item.ServicoNome || '—'}</td>
        <td>${formatDate(item.data_consulta || item.Data_consulta || item.DataConsulta)}</td>
        <td><span class="badge badge-gray">${item.estado || item.Estado || '—'}</span></td>
        <td style="display:flex;gap:6px;flex-wrap:wrap;"><button class="btn btn-sm btn-success" onclick="handlePedidoAction(${item.id || item.Id}, 'confirmar')">Confirmar</button><button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${item.id || item.Id}, 'cancelar')">Cancelar</button><button class="btn btn-sm btn-primary" onclick="handlePedidoAction(${item.id || item.Id}, 'validar')">Validar</button><button class="btn btn-sm btn-danger" onclick="handlePedidoAction(${item.id || item.Id}, 'rejeitar')">Rejeitar</button><button class="btn btn-sm btn-outline" onclick="openComprovativo(${item.id || item.Id})">Comprovativo</button></td>
      </tr>`).join('');
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pedidos.</td></tr>';
  }
}

async function handlePedidoAction(id, action) {
  const mapping = { confirmar: 'confirmar', cancelar: 'cancelar', validar: 'validar', rejeitar: 'rejeitar' };
  try {
    const result = await fetchJson(`/Admin/${id}/${mapping[action]}`, { method: 'PUT' });
    showToast(result.mensagem || 'Operação concluída.', 'success');
    loadPedidos();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Falha na operação.', 'error');
  }
}

function openComprovativo(id) {
  window.open(`http://localhost:5290/api/Admin/${id}/comprovativo`, '_blank');
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

async function populateFormSelects() {
  try {
    const [clientes, medicos, especialidades, pagos, consultas, estados, perfis] = await Promise.all([
      fetchJson('/Admin/cliente'),
      fetchJson('/Admin/medicos'),
      fetchJson('/Admin/especialidade'),
      fetchJson('/Admin/pagamento'),
      fetchJson('/Admin/consulta'),
      fetchJson('/Admin/estados'),
      fetchJson('/Admin/perfil')
    ]);
    setSelectOptions('cMedico', medicos || [], ['id', 'Id'], ['nomefuncionario', 'Nomefuncionario', 'nomeFuncionario', 'NomeFuncionario']);
    setSelectOptions('cServico', await fetchJson('/Admin/servicos') || [], ['id', 'Id'], ['servicoNome', 'ServicoNome', 'nome', 'Nome']);
    setSelectOptions('cPaciente', await fetchJson('/Admin/paciente') || [], ['id', 'Id', 'pacienteId', 'PacienteId'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('pCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('fPerfil', perfis || [], ['perfilId', 'PerfilId'], ['perfilNome', 'PerfilNome']);
    setSelectOptions('fEspecialidade', especialidades || [], ['especialidadeId', 'EspecialidadeId'], ['especialidadeNome', 'EspecialidadeNome']);
    setSelectOptions('payCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    setSelectOptions('pcPagamento', pagamentos || [], ['id', 'Id'], ['comprovativo', 'Comprovativo']);
    setSelectOptions('pcConsulta', consultas || [], ['consultaId', 'ConsultaId', 'id', 'Id'], ['pacienteNome', 'PacienteNome']);
    setSelectOptions('smsCliente', clientes || [], ['clienteNif', 'Nif_cliente', 'ClienteNif'], ['clienteNome', 'ClienteNome', 'nome']);
    const estadoItems = estados || [];
    const cEstado = document.getElementById('cEstado');
    if (cEstado) cEstado.innerHTML = '<option value="">Seleccionar...</option>' + estadoItems.map(item => createOption(item.consultaId || item.id || item.ConsultaId, item.descricao || item.Descricao || item.EstadoDescricao)).join('');
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
    await fetchJson('/Admin/consulta', { method: 'POST', body: payload });
    showToast('Consulta criada com sucesso.', 'success');
    closeModal('modalConsulta');
    loadConsultas();
    loadTopLists();
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
    await fetchJson('/Admin/paciente', { method: 'POST', body: payload });
    showToast('Paciente criado com sucesso.', 'success');
    closeModal('modalPaciente');
    loadPacientes();
    loadTopLists();
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
    await fetchJson('/Admin/cliente', { method: 'POST', body: payload });
    showToast('Cliente criado com sucesso.', 'success');
    closeModal('modalCliente');
    loadClientes();
    populateFormSelects();
  } catch (error) {
    showToast(getErrorMessage(error, 'Erro ao criar cliente.'), 'error');
  }
}

async function submitFuncionario(event) {
  event.preventDefault();
  try {
    const payload = {
      FuncionarioNome: document.getElementById('fNome').value,
      FuncionarioNif: document.getElementById('fNif').value,
      FuncionarioPerfil: parseInt(document.getElementById('fPerfil').value, 10),
      FuncionarioSenha: document.getElementById('fSenha').value,
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
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar funcionário.', 'error');
  }
}

async function submitEspecialidade(event) {
  event.preventDefault();
  try {
    const payload = {
      EspecialidadeNome: document.getElementById('espNome').value,
      EspecialidadeDescricao: document.getElementById('espDesc').value,
      EspecialidadeEstado: document.getElementById('espEstado').value === 'true'
    };
    await fetchJson('/Admin/especialidade', { method: 'POST', body: payload });
    showToast('Especialidade criada com sucesso.', 'success');
    closeModal('modalEspecialidade');
    loadEspecialidades();
    populateFormSelects();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar especialidade.', 'error');
  }
}

async function submitServico(event) {
  event.preventDefault();
  try {
    const payload = {
      ServicoNome: document.getElementById('svcNome').value,
      IdEspecialidade: parseInt(document.getElementById('svcEsp').value, 10),
      ServicoDuracao: parseInt(document.getElementById('svcDuracao').value, 10) || 30,
      ServicoPreco: parseFloat(document.getElementById('svcPreco').value) || 0,
      ServicoEstado: document.getElementById('svcEstado').value === 'true'
    };
    await fetchJson('/Admin/servicos', { method: 'POST', body: payload });
    showToast('Serviço criado com sucesso.', 'success');
    closeModal('modalServico');
    loadServicos();
    loadTopLists();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar serviço.', 'error');
  }
}

async function submitPagamento(event) {
  event.preventDefault();
  try {
    const payload = {
      IdCliente: document.getElementById('payCliente').value,
      IdSecretaria: localStorage.getItem('nif') || getUserId() || 1,
      Comprovativo: document.getElementById('payComprovativo').value,
      Data: document.getElementById('payData').value || new Date().toISOString()
    };
    await fetchJson('/Admin/pagamento', { method: 'POST', body: payload });
    showToast('Pagamento registado com sucesso.', 'success');
    closeModal('modalPagamento');
    loadPagamentos();
    loadTopLists();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao criar pagamento.', 'error');
  }
}

async function submitPagamentoConsulta(event) {
  event.preventDefault();
  try {
    const payload = {
      IdPagamento: parseInt(document.getElementById('pcPagamento').value, 10),
      IdConsulta: parseInt(document.getElementById('pcConsulta').value, 10)
    };
    await fetchJson('/Admin/pagamentoconsulta', { method: 'POST', body: payload });
    showToast('Pagamento de consulta registado.', 'success');
    closeModal('modalPagamentoConsulta');
    loadPagamentoConsulta();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao registar pagamento de consulta.', 'error');
  }
}

async function submitSMS(event) {
  event.preventDefault();
  try {
    const payload = {
      SMSMensagem: document.getElementById('smsMensagem').value,
      SMSNif_funcionario: localStorage.getItem('nif') || getUserId() || '',
      SMSNif_cliente: document.getElementById('smsCliente').value
    };
    await fetchJson('/Admin/SMS', { method: 'POST', body: payload });
    showToast('SMS enviado com sucesso.', 'success');
    closeModal('modalSMS');
    loadSMS();
  } catch (error) {
    showToast(error?.data?.mensagem || 'Erro ao enviar SMS.', 'error');
  }
}

window.addEventListener('load', initAdmin);
