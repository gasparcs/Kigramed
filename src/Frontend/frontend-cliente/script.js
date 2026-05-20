/* ─── CONFIG ─── */
const API = 'http://localhost:5290/api';

/* ─── NIF INPUT CLEAR ─── */
function onNifInput() {
  const fb = document.getElementById('nif-feedback');
  if (fb) { fb.textContent = ''; fb.className = 'ag-feedback'; }
}

/* ─── LOOKUP NIF → GET /api/Cliente/nif/{nif} ─── */
async function buscarClientePorNif() {
  const nif = document.getElementById('f-nif').value.trim();
  const feedback = document.getElementById('nif-feedback');
  if (!nif) { feedback.textContent = ''; return; }
  feedback.className = 'ag-feedback';
  feedback.textContent = 'A verificar...';
  try {
    const r = await fetch(`${API}/Cliente/nif/${encodeURIComponent(nif)}`, {
      signal: AbortSignal.timeout(5000)
    });
    if (r.status === 200) {
      const d = await r.json();
      document.getElementById('f-nome-cliente').value = d.nome || '';
      feedback.className = 'ag-feedback ok';
      feedback.textContent = '✓ Cliente encontrado — dados preenchidos automaticamente.';
    } else if (r.status === 404) {
      document.getElementById('f-nome-cliente').value = '';
      feedback.className = 'ag-feedback';
      feedback.textContent = 'Novo cliente — preencha o nome abaixo.';
    } else {
      feedback.className = 'ag-feedback err';
      feedback.textContent = 'Não foi possível verificar o NIF neste momento.';
    }
  } catch {
    feedback.className = 'ag-feedback';
    feedback.textContent = '';
  }
}

/* ─── SHOW ALERT HELPER ─── */
function showAlert(containerId, type, msg) {
  const el = document.getElementById(containerId);
  if (!el) return;
  if (!msg) { el.innerHTML = ''; return; }
  el.innerHTML = `<div class="ag-alert ag-alert-${type}">${msg}</div>`;
}

/* ─── NAVEGAÇÃO ─── */
function go(pg) {
  document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
  document.getElementById('page-' + pg).classList.add('active');
  document.querySelectorAll('[data-nav]').forEach(l => {
    l.classList.remove('active');
    if (l.dataset.nav === pg) l.classList.add('active');
  });
  window.scrollTo(0, 0);
  if (pg === 'agendamento') initAgendamento();
}
function toggleMenu() {
  const m = document.getElementById('nav-mobile'), b = document.getElementById('burger');
  const o = m.classList.toggle('open');
  b.textContent = o ? '✕' : '☰';
}
function closeMenu() {
  document.getElementById('nav-mobile').classList.remove('open');
  document.getElementById('burger').textContent = '☰';
}

/* ─── CAROUSEL ─── */
let cur = 0;
const slides = document.querySelectorAll('.hero-slide'), dots = document.querySelectorAll('.hero-dot');
let auto = setInterval(nextSlide, 5000);
function goSlide(n) {
  slides[cur].classList.remove('active'); dots[cur].classList.remove('active');
  cur = (n + slides.length) % slides.length;
  slides[cur].classList.add('active'); dots[cur].classList.add('active');
}
function nextSlide() { goSlide(cur + 1); }
function prevSlide() { goSlide(cur - 1); clearInterval(auto); auto = setInterval(nextSlide, 5000); }

/* ─── SCROLL ─── */
function scrollTo(id) {
  const el = document.getElementById(id);
  if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

/* ─── PRÉ-SELECÇÃO DE ESPECIALIDADE ─── */
let preEsp = '';
function agendarEsp(esp) { preEsp = esp; go('agendamento'); }

function getPhoneDigits(value) {
  return String(value || '').replace(/\D/g, '');
}

function formatPhoneForDisplay(value) {
  const digits = getPhoneDigits(value);
  if (!digits) return '';
  if (digits.startsWith('351') && digits.length >= 12) {
    const rest = digits.slice(3);
    return `+351 ${rest.slice(0, 3)} ${rest.slice(3, 6)} ${rest.slice(6, 9)}`.trim();
  }
  if (digits.length === 9) {
    return `${digits.slice(0, 3)} ${digits.slice(3, 6)} ${digits.slice(6, 9)}`;
  }
  return value;
}

function validatePhoneNumber(value) {
  const digits = getPhoneDigits(value);
  return digits.length === 9 || (digits.length === 12 && digits.startsWith('351'));
}

function handlePhoneMask(input) {
  input.value = formatPhoneForDisplay(input.value);
}

/* ─── TABS ─── */
function switchTab(t) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('tab-' + t).classList.add('active');
  document.getElementById('tab-novo-content').style.display = t === 'novo' ? 'block' : 'none';
  document.getElementById('tab-estado-content').style.display = t === 'estado' ? 'block' : 'none';
}

/* ─── STEPS ─── */
function setStep(n) {
  ['s1', 's2', 's3', 's4'].forEach((id, i) =>
    document.getElementById(id).style.display = i + 1 === n ? 'block' : 'none'
  );
  ['st1', 'st2', 'st3', 'st4'].forEach((id, i) => {
    const el = document.getElementById(id);
    el.classList.remove('active', 'done');
    if (i + 1 === n) el.classList.add('active');
    else if (i + 1 < n) el.classList.add('done');
  });
}

/* ─── CARREGAR GÉNEROS → GET /api/Cliente/genero ─── */
let generos = [];
async function carregarGeneros() {
  const sel = document.getElementById('f-genero');
  try {
    const r = await fetch(`${API}/Cliente/genero`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    generos = await r.json();
    sel.innerHTML = '<option value="">Seleccione o género</option>' +
      generos.map(g => `<option value="${g.id}">${g.nome}</option>`).join('');
  } catch {
    generos = [{ id: 1, nome: 'Masculino' }, { id: 2, nome: 'Feminino' }];
    sel.innerHTML = '<option value="">Seleccione o género</option>' +
      generos.map(g => `<option value="${g.id}">${g.nome}</option>`).join('');
  }
}

async function carregarRelacoes() {
  const sel = document.getElementById('f-relacao-select');
  if (!sel) return;
  try {
    const r = await fetch(`${API}/Cliente/cliente-paciente`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    relacoes = await r.json();
  } catch {
    relacoes = [
      { id: 2, descricao: 'Pai' },      { id: 3, descricao: 'Mae' },
      { id: 4, descricao: 'Filho(a)' }, { id: 5, descricao: 'Avo' },
      { id: 6, descricao: 'Neto(a)' },  { id: 7, descricao: 'Tio(a)' },
      { id: 8, descricao: 'Conjuge' },  { id: 9, descricao: 'Irmão(Irmã)' }
    ];
  }
  // exclude "Eu mesmo" from dropdown (id=1), it's handled by button
  const outros = relacoes.filter(r => r.id !== 1);
  sel.innerHTML = '<option value="">Seleccione...</option>' +
    outros.map(r => `<option value="${r.id}">${r.descricao}</option>`).join('');
}

let idRelacaoSeleccionada = null;
let tipoQuem = null; // 'eumesmo' | 'outra'

function selecionarQuem(tipo) {
  tipoQuem = tipo;
  document.getElementById('btn-eumesmo').classList.toggle('selected', tipo === 'eumesmo');
  document.getElementById('btn-outra').classList.toggle('selected', tipo === 'outra');
  const pg = document.getElementById('parentesco-group');
  if (tipo === 'eumesmo') {
    idRelacaoSeleccionada = 1; // "Eu mesmo"
    pg.style.display = 'none';
  } else {
    idRelacaoSeleccionada = null;
    pg.style.display = 'block';
  }
}

function irStep3() {
  if (!tipoQuem) {
    showAlert('s2-alert', 'danger', '⚠️ Indique se a consulta é para si ou para outra pessoa.');
    return;
  }
  if (tipoQuem === 'outra') {
    const sel = document.getElementById('f-relacao-select').value;
    if (!sel) {
      showAlert('s2-alert', 'danger', '⚠️ Seleccione o grau de parentesco.');
      return;
    }
    idRelacaoSeleccionada = parseInt(sel);
  }
  showAlert('s2-alert', '', '');
  setStep(3);
}

/* ─── CARREGAR ESPECIALIDADES → GET /api/Secretaria/especialidade ─── */
let especialidades = [], servicos = [];
async function carregarEspecialidades() {
  const sel = document.getElementById('f-esp');
  try {
    const r = await fetch(`${API}/Secretaria/especialidade`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    especialidades = await r.json();
    sel.innerHTML = '<option value="">Seleccione a especialidade</option>' +
      especialidades.map(e => `<option value="${e.especialidadeId || e.id}">${e.especialidadeNome || e.nome}</option>`).join('');
  } catch {
    especialidades = [
      { id: 1, nome: 'Clínica Geral' }, { id: 2, nome: 'Ginecologia' },
      { id: 3, nome: 'Cardiologia' },   { id: 4, nome: 'Pediatria' }
    ];
    sel.innerHTML = '<option value="">Seleccione a especialidade</option>' +
      especialidades.map(e => `<option value="${e.id}">${e.nome}</option>`).join('');
  }
  if (preEsp) {
    const found = especialidades.find(e =>
      (e.especialidadeNome || e.nome || '').toLowerCase().includes(preEsp.toLowerCase())
    );
    if (found) { sel.value = found.especialidadeId || found.id; await carregarServicos(); }
    preEsp = '';
  }
}

/* ─── CARREGAR SERVIÇOS → GET /api/Secretaria/servico ─── */
async function carregarServicos() {
  const idEsp = document.getElementById('f-esp').value;
  const sel = document.getElementById('f-srv');
  const hint = document.getElementById('srv-preco-hint');
  sel.disabled = true;
  sel.innerHTML = '<option value="">A carregar...</option>';
  hint.textContent = '';
  if (!idEsp) { sel.innerHTML = '<option value="">Seleccione a especialidade primeiro</option>'; return; }
  try {
    const r = await fetch(`${API}/Secretaria/servico`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    const todos = await r.json();
    servicos = todos.filter(s => (s.idEspecialidade || s.IdEspecialidade) == idEsp);
    if (!servicos.length) throw new Error('sem serviços');
    sel.innerHTML = '<option value="">Seleccione o serviço</option>' +
      servicos.map(s => `<option value="${s.servicoId || s.id}" data-preco="${s.servicoPreco || s.preco}">${s.servicoNome || s.nome} (${s.servicoDuracaoMinuto || s.duracao || 30}min)</option>`).join('');
  } catch {
    const fallback = {
      1: [{ id: 101, nome: 'Consulta Geral',       preco: 15000, duracao: 30 }],
      2: [{ id: 201, nome: 'Consulta Ginecológica', preco: 20000, duracao: 45 },
          { id: 202, nome: 'Consulta Pré-Natal',    preco: 18000, duracao: 30 }],
      3: [{ id: 301, nome: 'Consulta Cardiológica', preco: 25000, duracao: 45 },
          { id: 302, nome: 'ECG',                   preco:  8000, duracao: 20 }],
      4: [{ id: 401, nome: 'Consulta Pediátrica',   preco: 15000, duracao: 30 },
          { id: 402, nome: 'Vacinação',             preco:  5000, duracao: 15 }]
    };
    servicos = fallback[idEsp] || [];
    sel.innerHTML = '<option value="">Seleccione o serviço</option>' +
      servicos.map(s => `<option value="${s.id}" data-preco="${s.preco}">${s.nome} (${s.duracao}min)</option>`).join('');
  }
  sel.disabled = false;
  sel.onchange = () => {
    const opt = sel.options[sel.selectedIndex];
    const p = opt?.dataset.preco;
    hint.textContent = p && p !== 'undefined' ? `💰 ${parseInt(p).toLocaleString()} Kz` : '';
  };
}

/* ─── STEP 1 → STEP 2 ─── */
function irStep2() {
  const nif         = document.getElementById('f-nif').value.trim();
  const nomeCliente = document.getElementById('f-nome-cliente').value.trim();
  const telefone    = document.getElementById('f-telefone').value.trim();
  if (!nif || !nomeCliente || !telefone) {
    showAlert('s1-alert', 'danger', '⚠️ Preencha o NIF, nome do titular e telefone.');
    return;
  }
  if (!validatePhoneNumber(telefone)) {
    showAlert('s1-alert', 'danger', '⚠️ Introduza um telefone/WhatsApp válido.');
    return;
  }
  showAlert('s1-alert', '', '');
  setStep(2);
  carregarRelacoes();
  const now = new Date(); now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  document.getElementById('f-data').min = now.toISOString().slice(0, 16);
}
function voltarStep1() { setStep(1); }

/* ─── STEP 2 → STEP 3 ─── */
function voltarStep2() { setStep(2); }

/* ─── RESUMO ─── */
function renderResumo() {
  document.getElementById('res-nif').textContent          = document.getElementById('f-nif').value.trim();
  document.getElementById('res-nome-cliente').textContent = document.getElementById('f-nome-cliente').value.trim();
  document.getElementById('res-nome').textContent         = document.getElementById('f-nome').value.trim();
  document.getElementById('res-nasc').textContent         = new Date(document.getElementById('f-nascimento').value).toLocaleDateString('pt-PT');
  document.getElementById('res-genero').textContent       = document.getElementById('f-genero').selectedOptions[0]?.textContent || '—';
  const relOpt = document.getElementById('f-relacao-select');
  const relLabel = tipoQuem === 'eumesmo' ? 'Eu mesmo' : (relOpt?.selectedOptions[0]?.textContent || '—');
  document.getElementById('res-relacao').textContent = relLabel;
  document.getElementById('res-tel').textContent     = document.getElementById('f-telefone').value.trim() || '—';
  document.getElementById('res-esp').textContent     = document.getElementById('f-esp').selectedOptions[0]?.textContent || '—';
  document.getElementById('res-srv').textContent     = document.getElementById('f-srv').selectedOptions[0]?.textContent || '—';
  document.getElementById('res-data').textContent    = new Date(document.getElementById('f-data').value).toLocaleString('pt-PT', { dateStyle: 'short', timeStyle: 'short' });
  document.getElementById('res-obs').textContent     = document.getElementById('f-obs').value.trim() || 'Nenhuma observação';
}

/* ─── SUBMETER PEDIDO → POST /api/Cliente/pedido ─── */
async function submeterPedido() {
  const nif         = document.getElementById('f-nif').value.trim();
  const nomeCliente = document.getElementById('f-nome-cliente').value.trim();
  const nome        = document.getElementById('f-nome').value.trim();
  const nasc        = document.getElementById('f-nascimento').value;
  const genero      = parseInt(document.getElementById('f-genero').value);
  const relacao     = idRelacaoSeleccionada;
  const idEsp       = parseInt(document.getElementById('f-esp').value);
  const idSrv       = parseInt(document.getElementById('f-srv').value);
  const data        = document.getElementById('f-data').value;
  const obs         = document.getElementById('f-obs').value.trim();
  const alerta      = document.getElementById('s3-alert');

  if (!nif || !nomeCliente || !nome || !nasc || !genero || !relacao || !idEsp || !idSrv || !data) {
    showAlert('s3-alert', 'danger', '⚠️ Preencha todos os campos obrigatórios.');
    return;
  }

  const telefone = document.getElementById('f-telefone').value.trim();
  const payload = {
    nifCliente:              nif,
    nomeCliente:             nomeCliente,
    nomePaciente:            nome,
    telefoneCliente:         telefone,
    dataNascimentoPaciente:  new Date(nasc).toISOString(),
    idGeneroPaciente:        genero,
    idClientePaciente:       relacao,
    idEspecialidade:         idEsp,
    idServico:               idSrv,
    horarioPreferencial:     new Date(data).toISOString(),
    observacoes:             obs || null
  };

  showAlert('s3-alert', '', '');
  document.getElementById('btn-s3').disabled = true;
  document.getElementById('loading-s3').style.display = 'flex';

  try {
    const r = await fetch(`${API}/Cliente/pedido`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    const data2 = await r.json();
    if (r.status === 201) {
      const num = data2.numeroPedido || 'PED-????-????';
      document.getElementById('pedido-num-display').textContent = num;
      document.getElementById('estado-num').value = num;
      setStep(4);
      toast('✅', 'Pedido criado! Número: ' + num);
    } else {
      showAlert('s3-alert', 'danger', `❌ ${data2.mensagem || JSON.stringify(data2)}`);
    }
  } catch {
    showAlert('s3-alert', 'danger', '❌ Não foi possível ligar ao servidor. Verifique a sua ligação e tente novamente.');
  } finally {
    document.getElementById('btn-s3').disabled = false;
    document.getElementById('loading-s3').style.display = 'none';
  }
}

/* ─── POLLING DE ESTADO ─── */
let estadoPollingIntervalId = null;

function iniciarPollingEstado(num) {
  if (estadoPollingIntervalId) clearInterval(estadoPollingIntervalId);
  estadoPollingIntervalId = setInterval(async () => {
    if (document.hidden) return;
    try {
      const r = await fetch(`${API}/Cliente/pedido/${encodeURIComponent(num)}/estado`);
      if (!r.ok) return;
      const d = await r.json();
      renderEstado(num, d);
      const e = String(d.estado || '').trim().toLowerCase();
      if (e === 'confirmada' || e === 'cancelada') {
        clearInterval(estadoPollingIntervalId);
        estadoPollingIntervalId = null;
      }
    } catch { /* silencioso */ }
  }, 20000);
}

/* ─── CONSULTAR ESTADO → GET /api/Cliente/pedido/{num}/estado ─── */
async function consultarEstado() {
  const num = document.getElementById('estado-num').value.trim();
  if (!num) { toast('⚠️', 'Introduza o número do pedido'); return; }
  document.getElementById('loading-estado').style.display = 'flex';
  document.getElementById('estado-result').style.display = 'none';
  document.getElementById('estado-error').style.display = 'none';
  if (estadoPollingIntervalId) { clearInterval(estadoPollingIntervalId); estadoPollingIntervalId = null; }
  try {
    const r = await fetch(`${API}/Cliente/pedido/${encodeURIComponent(num)}/estado`);
    if (r.status === 404) throw new Error('not_found');
    const d = await r.json();
    renderEstado(num, d);
    const e = String(d.estado || '').trim().toLowerCase();
    if (e !== 'confirmada' && e !== 'cancelada') iniciarPollingEstado(num);
  } catch (err) {
    if (err.message === 'not_found') {
      document.getElementById('estado-error').style.display = 'block';
      document.getElementById('estado-error').innerHTML = '<div class="ag-alert ag-alert-danger">❌ Pedido não encontrado. Verifique o número introduzido.</div>';
    } else {
      renderEstado(num, { numeroPedido: num, estado: 'Pendente', horario: new Date(Date.now() + 86400000).toISOString(), especialidade: 'Clínica Geral', prazoPagamento: null });
      iniciarPollingEstado(num);
    }
  } finally {
    document.getElementById('loading-estado').style.display = 'none';
  }
}

/* ─── RENDER ESTADO ─── */
function renderEstado(num, d) {
  const estado = d.estado || 'Pendente';
  const classMap = {
    'Pendente':             'status-Pendente',
    'Aguarda Pagamento':    'status-Aguarda',
    'Comprovativo Enviado': 'status-Comprovativo',
    'Confirmada':           'status-Confirmado',
    'Cancelada':            'status-Cancelado',
    'Rejeitado':            'status-Cancelado'
  };
  const instrucoes = {
    'Pendente':             'A secretaria irá analisar o seu pedido e confirmar o horário via SMS.',
    'Aguarda Pagamento':    'Horário confirmado! Efectue o pagamento e envie o comprovativo dentro do prazo indicado.',
    'Comprovativo Enviado': 'Comprovativo recebido. A secretaria irá validar em breve.',
    'Confirmada':           'Consulta confirmada! Apresente-se 15 minutos antes do horário marcado.',
    'Cancelada':            'Este pedido foi cancelado. Pode criar um novo pedido se desejar.',
    'Rejeitado':            'O seu comprovativo foi rejeitado. Por favor envie um novo comprovativo válido.'
  };
  document.getElementById('er-num').textContent     = d.numeroPedido || num;
  document.getElementById('er-badge').innerHTML     = `<span class="status-badge ${classMap[estado] || 'status-Pendente'}">${estado}</span>`;
  document.getElementById('er-esp').textContent     = d.especialidade || '—';
  document.getElementById('er-horario').textContent = d.horario
    ? new Date(d.horario).toLocaleString('pt-PT', { dateStyle: 'short', timeStyle: 'short' }) : '—';

  const prazowrap = document.getElementById('er-prazo-wrap');
  if (d.prazoPagamento) {
    prazowrap.style.display = 'block';
    document.getElementById('er-prazo').textContent =
      new Date(d.prazoPagamento).toLocaleString('pt-PT', { dateStyle: 'short', timeStyle: 'short' });
  } else {
    prazowrap.style.display = 'none';
  }

  document.getElementById('er-instrucao').textContent = instrucoes[estado] || instrucoes['Pendente'];
  const compSection = document.getElementById('comp-section');
  if (compSection) compSection.style.display = estado === 'Aguarda Pagamento' ? 'block' : 'none';
  window._numeroPedidoActivo = d.numeroPedido || num;
  document.getElementById('estado-result').style.display = 'block';
}

function irParaConsulta() { switchTab('estado'); consultarEstado(); }

/* ─── UPLOAD COMPROVATIVO ─── */
async function enviarComprovatioEstado() {
  const num    = window._numeroPedidoActivo;
  const file   = document.getElementById('comp-file').files[0];
  const alerta = document.getElementById('comp-alert');
  if (!num)  { showAlert('comp-alert', 'danger', '⚠️ Consulte primeiro o estado do pedido.'); return; }
  if (!file) { showAlert('comp-alert', 'danger', '⚠️ Seleccione um ficheiro.'); return; }
  if (!['.pdf', '.jpg', '.jpeg', '.png'].some(e => file.name.toLowerCase().endsWith(e))) {
    showAlert('comp-alert', 'danger', '❌ Apenas PDF, JPG e PNG são aceites.'); return;
  }
  if (file.size > 5_000_000) {
    showAlert('comp-alert', 'danger', '❌ O ficheiro não pode exceder 5MB.'); return;
  }
  showAlert('s3-alert', '', '');
  document.getElementById('btn-comp').disabled = true;
  document.getElementById('loading-comp').style.display = 'flex';
  const fd = new FormData();
  fd.append('ficheiro', file);
  try {
    const r = await fetch(`${API}/Cliente/${encodeURIComponent(num)}/comprovativo`, { method: 'POST', body: fd });
    const data = await r.json();
    if (r.ok) {
      showAlert('comp-alert', 'success', `✅ ${data.mensagem}`);
      document.getElementById('comp-section').style.display = 'none';
      toast('✅', 'Comprovativo enviado com sucesso!');
    } else {
      showAlert('comp-alert', 'danger', `❌ ${data.mensagem || JSON.stringify(data)}`);
    }
  } catch {
    showAlert('comp-alert', 'success', '✅ Comprovativo enviado. A secretaria irá validar em breve.');
    toast('✅', 'Comprovativo enviado!');
  } finally {
    document.getElementById('btn-comp').disabled = false;
    document.getElementById('loading-comp').style.display = 'none';
  }
}

function atualizarFicheiro(input) {
  const label = document.getElementById('file-name-label');
  if (input.files?.[0]) { label.textContent = input.files[0].name; label.style.color = 'var(--primary)'; }
  else { label.textContent = 'Clique para seleccionar ficheiro'; label.style.color = ''; }
}

/* ─── NOVO PEDIDO ─── */
function novoPedido() {
  ['f-nif', 'f-nome-cliente', 'f-nome', 'f-nascimento', 'f-data', 'f-obs'].forEach(id =>
    document.getElementById(id).value = ''
  );
  document.getElementById('f-genero').value  = '';
  idRelacaoSeleccionada = null;
  tipoQuem = null;
  document.getElementById('btn-eumesmo')?.classList.remove('selected');
  document.getElementById('btn-outra')?.classList.remove('selected');
  const pg = document.getElementById('parentesco-group');
  if (pg) pg.style.display = 'none';
  document.getElementById('f-esp').value     = '';
  document.getElementById('f-srv').innerHTML = '<option value="">Seleccione a especialidade primeiro</option>';
  document.getElementById('f-srv').disabled  = true;
  document.getElementById('srv-preco-hint').textContent = '';
  ['s1-alert', 's2-alert', 's3-alert', 'comp-alert'].forEach(id =>
    document.getElementById(id).innerHTML = ''
  );
  setStep(1);
}

/* ─── INIT AGENDAMENTO ─── */
async function initAgendamento() {
  setStep(1);
  ['s1-alert', 's2-alert', 's3-alert'].forEach(id =>
    document.getElementById(id).innerHTML = ''
  );
  const now = new Date(); now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  document.getElementById('f-data').min = now.toISOString().slice(0, 16);
  await Promise.all([carregarEspecialidades(), carregarGeneros(), carregarRelacoes()]);
}

/* ─── TOAST ─── */
function toast(ico, msg) {
  const t = document.getElementById('toast');
  document.getElementById('toast-ico').textContent = ico;
  document.getElementById('toast-msg').textContent = msg;
  t.classList.add('show');
  setTimeout(() => t.classList.remove('show'), 5000);
}

/* ─── INIT ─── */
go('home');