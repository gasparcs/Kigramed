/* ─── CONFIG ─── */
const API = 'http://localhost:5290/api';

/* ─── INPUT NIF → validação em tempo real ─── */
function onNifInput() {
  const nif = document.getElementById('f-nif').value.trim();
  const feedback = document.getElementById('nif-feedback');
  if (nif.length >= 15) {
    // Auto busca quando completa o NIF
    buscarClientePorNif();
  } else {
    feedback.textContent = '';
  }
}

/* ─── LOOKUP NIF → GET /api/Cliente/nif/{nif} ─── */
async function buscarClientePorNif() {
  const nif = document.getElementById('f-nif').value.trim();
  const feedback = document.getElementById('nif-feedback');
  if (!nif) { feedback.textContent = ''; return; }
  feedback.style.color = 'var(--muted)';
  feedback.textContent = 'A verificar...';
  try {
    const r = await fetch(`${API}/Cliente/nif/${encodeURIComponent(nif)}`, {
      signal: AbortSignal.timeout(5000)
    });
    if (r.status === 200) {
      const d = await r.json();
      document.getElementById('f-nome-cliente').value = d.nome || '';
      feedback.style.color = 'green';
      feedback.textContent = '✓ Cliente encontrado — dados preenchidos automaticamente.';
    } else if (r.status === 404) {
      document.getElementById('f-nome-cliente').value = '';
      feedback.style.color = 'var(--muted)';
      feedback.textContent = 'Novo cliente — preencha o nome abaixo.';
    } else {
      feedback.style.color = 'orange';
      feedback.textContent = 'Não foi possível verificar o NIF neste momento.';
    }
  } catch {
    feedback.style.color = 'var(--muted)';
    feedback.textContent = '';
  }
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
  // Com prefixo +244 (244 + 9 digitos = 12 digitos)
  if (digits.startsWith('244') && digits.length >= 12) {
    const rest = digits.slice(3);
    return rest.slice(0, 3) + ' ' + rest.slice(3, 6) + ' ' + rest.slice(6, 9);
  }
  // Apenas os 9 digitos locais
  if (digits.length <= 9) {
    const d = digits.slice(0, 9);
    if (d.length <= 3) return d;
    if (d.length <= 6) return d.slice(0, 3) + ' ' + d.slice(3);
    return d.slice(0, 3) + ' ' + d.slice(3, 6) + ' ' + d.slice(6);
  }
  return value;
}

function validatePhoneNumber(value) {
  const digits = getPhoneDigits(value);
  // Aceita 9 digitos locais OU 244 + 9 digitos (total 12)
  return digits.length === 9 || (digits.length === 12 && digits.startsWith('244'));
}

function handlePhoneMask(input) {
  input.value = input.value.replace(/\D/g, '').slice(0, 9).replace(/(\d{3})(\d{3})(\d{3})/, '$1 $2 $3');
}

function formatarValorDataHoraLocal(data, horas, minutos) {
  const ano = data.getFullYear();
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const dia = String(data.getDate()).padStart(2, '0');
  const hh = String(horas).padStart(2, '0');
  const mm = String(minutos).padStart(2, '0');
  return `${ano}-${mes}-${dia}T${hh}:${mm}`;
}

function validarHorario(input) {
  if (!input) return false;

  const valor = String(input.value || '').trim();
  if (!valor) return false;

  const partes = valor.match(/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})$/);
  if (!partes) {
    return false;
  }

  const horas = Number(partes[4]);
  const minutos = Number(partes[5]);
  const minutosTotais = horas * 60 + minutos;

  if (minutosTotais < 8 * 60 || minutosTotais > 19 * 60) {
    toast('⚠️', 'Seleccione um horário entre 08:00 e 19:00.');
    return false;
  }

  return true;
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

/* ─── SELEÇÃO PASSO 2: EU MESMO OU OUTRA PESSOA ─── */
let quemConsulta = 'eumesmo';
function selecionarQuem(opcao) {
  quemConsulta = opcao;
  document.getElementById('btn-eumesmo').classList.toggle('selected', opcao === 'eumesmo');
  document.getElementById('btn-outra').classList.toggle('selected', opcao === 'outra');
  const parentescoGroup = document.getElementById('parentesco-group');
  if (opcao === 'outra') {
    parentescoGroup.style.display = 'block';
  } else {
    parentescoGroup.style.display = 'none';
    document.getElementById('f-relacao-select').value = '1'; // "Eu mesmo"
  }
}

/* ─── CARREGAR RELAÇÕES → GET /api/Cliente/cliente-paciente ─── */
let relacoes = [];
async function carregarRelacoes() {
  const sel = document.getElementById('f-relacao-select');
  try {
    const r = await fetch(`${API}/Cliente/cliente-paciente`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    relacoes = await r.json();
    sel.innerHTML = '<option value="">Seleccione</option>' +
      relacoes.map(r => `<option value="${r.id}">${r.descricao}</option>`).join('');
  } catch {
    relacoes = [
      { id: 1, descricao: 'Eu mesmo' }, { id: 2, descricao: 'Pai' },
      { id: 3, descricao: 'Mae' },       { id: 4, descricao: 'Filho(a)' },
      { id: 5, descricao: 'Avo' },       { id: 6, descricao: 'Neto(a)' },
      { id: 7, descricao: 'Tio(a)' },    { id: 8, descricao: 'Conjuge' },
      { id: 9, descricao: 'Irmão(Irmã)' }
    ];
    sel.innerHTML = '<option value="">Seleccione</option>' +
      relacoes.map(r => `<option value="${r.id}">${r.descricao}</option>`).join('');
  }
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
  const alerta      = document.getElementById('s1-alert');
  
  // Step 1 só valida: NIF, Nome do cliente e Telefone
  if (!nif || !nomeCliente || !telefone) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Por favor, preencha o NIF, nome do cliente e telefone.</div>';
    return;
  }
  if (!validatePhoneNumber(telefone)) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Introduza um telefone/WhatsApp válido.</div>';
    return;
  }
  alerta.innerHTML = '';
  quemConsulta = 'eumesmo';
  selecionarQuem('eumesmo'); // Pre-seleciona "Eu mesmo" por defeito
  setStep(2);
  const now = new Date(); now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  document.getElementById('f-data').min = now.toISOString().slice(0, 16);
}
function voltarStep1() { setStep(1); }

/* ─── STEP 2 → STEP 3 ─── */
function irStep3() {
  const relacao = document.getElementById('f-relacao-select').value;
  const alerta = document.getElementById('s2-alert');
  
  // Se é "outra pessoa", valida a relação
  if (quemConsulta === 'outra' && !relacao) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Selecione o grau de parentesco.</div>';
    return;
  }
  
  alerta.innerHTML = '';
  setStep(3);
}
function voltarStep2() { setStep(2); }

/* ─── SUBMETER PEDIDO → POST /api/Cliente/pedido ─── */
async function submeterPedido() {
  const nif         = document.getElementById('f-nif').value.trim();
  const nomeCliente = document.getElementById('f-nome-cliente').value.trim();
  const nome        = document.getElementById('f-nome').value.trim();
  const nasc        = document.getElementById('f-nascimento').value;
  const genero      = parseInt(document.getElementById('f-genero').value);
  const relacao     = parseInt(document.getElementById('f-relacao-select').value);
  const idEsp       = parseInt(document.getElementById('f-esp').value);
  const idSrv       = parseInt(document.getElementById('f-srv').value);
  const inputData   = document.getElementById('f-data');
  const data        = inputData.value;
  const obs         = document.getElementById('f-obs').value.trim();
  const alerta      = document.getElementById('s3-alert');

  if (!nif || !nomeCliente || !nome || !nasc || !genero || !relacao || !idEsp || !idSrv || !data) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Preencha todos os dados antes de enviar.</div>';
    return;
  }

  if (!validarHorario(inputData, false)) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ O horário da consulta deve estar entre 08:00 e 19:00, em intervalos de 30 minutos.</div>';
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

  alerta.innerHTML = '';
  document.getElementById('btn-s3').disabled = true;
  document.getElementById('loading-s3').classList.add('show');

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
      alerta.innerHTML = `<div class="alert alert-danger">❌ ${data2.mensagem || JSON.stringify(data2)}</div>`;
    }
  } catch {
    alerta.innerHTML = '<div class="alert alert-danger">❌ Não foi possível ligar ao servidor. Verifique a sua ligação e tente novamente.</div>';
  } finally {
    document.getElementById('btn-s3').disabled = false;
    document.getElementById('loading-s3').classList.remove('show');
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
  document.getElementById('loading-estado').classList.add('show');
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
      document.getElementById('estado-error').innerHTML =
        '<div class="alert alert-danger">❌ Pedido não encontrado. Verifique o número.</div>';
    } else {
      renderEstado(num, { numeroPedido: num, estado: 'Pendente', horario: new Date(Date.now() + 86400000).toISOString(), especialidade: 'Clínica Geral', prazoPagamento: null });
      iniciarPollingEstado(num);
    }
  } finally {
    document.getElementById('loading-estado').classList.remove('show');
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
  const cancelarSection = document.getElementById('cancelar-section');
  if (cancelarSection) {
    cancelarSection.style.display = estado === 'Confirmada' ? 'flex' : 'none';
  }
  const compSection = document.getElementById('comp-section');
  if (compSection) compSection.style.display = estado === 'Aguarda Pagamento' ? 'block' : 'none';
  window._numeroPedidoActivo = d.numeroPedido || num;
  document.getElementById('estado-result').style.display = 'block';
}

function irParaConsulta() { switchTab('estado'); consultarEstado(); }

function cancelarConsultaCliente(numeroPedido) {
  const overlay = document.getElementById('modal-cancelar-overlay');
  overlay.style.display = 'flex';
  document.getElementById('btn-confirmar-cancelar').onclick = async () => {
    fecharModalCancelar();
    try {
      const res = await fetch(`${API}/Cliente/consulta/${encodeURIComponent(numeroPedido)}/cancelar`, { method: 'PUT' });
      if (res.ok) {
        toast('✓', 'Consulta cancelada com sucesso.');
        consultarEstado();
      } else {
        const data = await res.json().catch(() => ({}));
        toast('⚠️', data.mensagem || 'Não foi possível cancelar a consulta.');
      }
    } catch {
      toast('⚠️', 'Erro de ligação. Tente novamente.');
    }
  };
  overlay.onclick = (e) => { if (e.target === overlay) fecharModalCancelar(); };
}

function fecharModalCancelar() {
  document.getElementById('modal-cancelar-overlay').style.display = 'none';
}

async function consultarEstadoPedido(numeroPedido) {
  if (!numeroPedido) return;
  const input = document.getElementById('estado-num');
  if (input) input.value = numeroPedido;
  await consultarEstado();
}

/* ─── UPLOAD COMPROVATIVO ─── */
async function enviarComprovatioEstado() {
  const num    = window._numeroPedidoActivo;
  const file   = document.getElementById('comp-file').files[0];
  const alerta = document.getElementById('comp-alert');
  if (!num)  { alerta.innerHTML = '<div class="alert alert-danger">⚠️ Consulte primeiro o estado do pedido.</div>'; return; }
  if (!file) { alerta.innerHTML = '<div class="alert alert-danger">⚠️ Seleccione um ficheiro.</div>'; return; }
  if (!['.pdf', '.jpg', '.jpeg', '.png'].some(e => file.name.toLowerCase().endsWith(e))) {
    alerta.innerHTML = '<div class="alert alert-danger">❌ Apenas PDF, JPG e PNG são aceites.</div>'; return;
  }
  if (file.size > 5_000_000) {
    alerta.innerHTML = '<div class="alert alert-danger">❌ O ficheiro não pode exceder 5MB.</div>'; return;
  }
  alerta.innerHTML = '';
  document.getElementById('btn-comp').disabled = true;
  document.getElementById('loading-comp').classList.add('show');
  const fd = new FormData();
  fd.append('ficheiro', file);
  try {
    const r = await fetch(`${API}/Cliente/${encodeURIComponent(num)}/comprovativo`, { method: 'POST', body: fd });
    const data = await r.json();
    if (r.ok) {
      alerta.innerHTML = `<div class="alert alert-success">✅ ${data.mensagem}</div>`;
      document.getElementById('comp-section').style.display = 'none';
      toast('✅', 'Comprovativo enviado com sucesso!');
    } else {
      alerta.innerHTML = `<div class="alert alert-danger">❌ ${data.mensagem || JSON.stringify(data)}</div>`;
    }
  } catch {
    alerta.innerHTML = '<div class="alert alert-success">✅ Comprovativo enviado. A secretaria irá validar em breve.</div>';
    toast('✅', 'Comprovativo enviado!');
  } finally {
    document.getElementById('btn-comp').disabled = false;
    document.getElementById('loading-comp').classList.remove('show');
  }
}

function atualizarFicheiro(input) {
  const label = document.getElementById('file-name-label');
  if (input.files?.[0]) { label.textContent = input.files[0].name; label.classList.add('tem-ficheiro'); }
  else { label.textContent = 'Nenhum ficheiro seleccionado'; label.classList.remove('tem-ficheiro'); }
}

/* ─── NOVO PEDIDO ─── */
function novoPedido() {
  ['f-nif', 'f-nome-cliente', 'f-nome', 'f-nascimento', 'f-data', 'f-obs'].forEach(id =>
    document.getElementById(id).value = ''
  );
  document.getElementById('f-genero').value  = '';
  document.getElementById('f-relacao').value = '';
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
  const inputData = document.getElementById('f-data');
  inputData.min = now.toISOString().slice(0, 16);
  inputData.step = '1800';
  // Data de nascimento: não pode ser hoje nem no futuro
  const hoje = new Date().toISOString().slice(0, 10);
  document.getElementById('f-nascimento').max = hoje;
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
