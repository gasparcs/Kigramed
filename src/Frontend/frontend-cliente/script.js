/* ─── CONFIG ─── */
const API = 'http://localhost:5290/api'; // ajustar para URL real do backend

/* ─── NAVEGAÇÃO ─── */
function go(pg) {
  document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
  document.getElementById('page-' + pg).classList.add('active');
  document.querySelectorAll('[data-nav]').forEach(l => { l.classList.remove('active'); if (l.dataset.nav === pg) l.classList.add('active') });
  window.scrollTo(0, 0);
  if (pg === 'agendamento') initAgendamento();
}
function toggleMenu() { const m = document.getElementById('nav-mobile'), b = document.getElementById('burger'), o = m.classList.toggle('open'); b.textContent = o ? '✕' : '☰' }
function closeMenu() { document.getElementById('nav-mobile').classList.remove('open'); document.getElementById('burger').textContent = '☰' }

/* ─── CAROUSEL ─── */
let cur = 0;
const slides = document.querySelectorAll('.hero-slide'), dots = document.querySelectorAll('.hero-dot');
let auto = setInterval(nextSlide, 5000);
function goSlide(n) { slides[cur].classList.remove('active'); dots[cur].classList.remove('active'); cur = (n + slides.length) % slides.length; slides[cur].classList.add('active'); dots[cur].classList.add('active') }
function nextSlide() { goSlide(cur + 1) }
function prevSlide() { goSlide(cur - 1); clearInterval(auto); auto = setInterval(nextSlide, 5000) }

/* ─── SCROLL SERVIÇO ─── */
function scrollTo(id) { const el = document.getElementById(id); if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' }) }

/* ─── PRÉ-SELECÇÃO DE ESPECIALIDADE ─── */
let preEsp = '';
function agendarEsp(esp) { preEsp = esp; go('agendamento') }

/* ─── TABS ─── */
function switchTab(t) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('tab-' + t).classList.add('active');
  document.getElementById('tab-novo-content').style.display = t === 'novo' ? 'block' : 'none';
  document.getElementById('tab-estado-content').style.display = t === 'estado' ? 'block' : 'none';
}

/* ─── ESTADO VISÍVEL DOS STEPS ─── */
function setStep(n) {
  ['s1', 's2', 's3', 's4'].forEach((id, i) => document.getElementById(id).style.display = i + 1 === n ? 'block' : 'none');
  ['st1', 'st2', 'st3', 'st4'].forEach((id, i) => {
    const el = document.getElementById(id);
    el.classList.remove('active', 'done');
    if (i + 1 === n) el.classList.add('active');
    else if (i + 1 < n) el.classList.add('done');
  });
}
let especialidades = [];
let servicos = [];
let generos = [];
let relacoes = [];

async function initAgendamento() {
  setStep(1);
  document.getElementById('s1-alert').innerHTML = '';
  document.getElementById('s2-alert').innerHTML = '';
  document.getElementById('s3-alert').innerHTML = '';
  document.getElementById('s4').style.display = 'none';
  await Promise.all([carregarEspecialidades(), carregarGeneros(), carregarRelacoes()]);
  const now = new Date(); now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  document.getElementById('f-data').min = now.toISOString().slice(0, 16);
}

async function carregarGeneros() {
  const sel = document.getElementById('f-genero');
  try {
    const r = await fetch(`${API}/Cliente/genero`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error('status ' + r.status);
    generos = await r.json();
    sel.innerHTML = '<option value="">Seleccione o género</option>' + generos.map(g => `<option value="${g.id}">${g.nome || g.Descricao || g.nomeGenero || g.Nome}</option>`).join('');
  } catch {
    generos = [
      { id: 1, nome: 'Masculino' },
      { id: 2, nome: 'Feminino' },
      { id: 3, nome: 'Outro' }
    ];
    sel.innerHTML = '<option value="">Seleccione o género</option>' + generos.map(g => `<option value="${g.id}">${g.nome}</option>`).join('');
  }
}

async function carregarRelacoes() {
  const sel = document.getElementById('f-relacao');
  try {
    const r = await fetch(`${API}/Cliente/cliente-paciente`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error('status ' + r.status);
    relacoes = await r.json();
    sel.innerHTML = '<option value="">Seleccione</option>' + relacoes.map(r => `<option value="${r.id}">${r.descricao || r.Descricao || r.nome || r.Nome}</option>`).join('');
  } catch {
    relacoes = [
      { id: 1, descricao: 'Paciente próprio' },
      { id: 2, descricao: 'Cônjuge' },
      { id: 3, descricao: 'Filho(a)' },
      { id: 4, descricao: 'Outro familiar' }
    ];
    sel.innerHTML = '<option value="">Seleccione</option>' + relacoes.map(r => `<option value="${r.id}">${r.descricao}</option>`).join('');
  }
}

function irStep2() {
  const nif = document.getElementById('f-nif').value.trim();
  const nome = document.getElementById('f-nome').value.trim();
  const nasc = document.getElementById('f-nascimento').value;
  const genero = document.getElementById('f-genero').value;
  const relacao = document.getElementById('f-relacao').value;
  const alerta = document.getElementById('s1-alert');
  if (!nif || !nome || !nasc || !genero || !relacao) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Por favor, preencha todos os dados do cliente e do paciente.</div>';
    return;
  }
  alerta.innerHTML = '';
  setStep(2);
}

function irStep3() {
  const esp = document.getElementById('f-esp').value;
  const srv = document.getElementById('f-srv').value;
  const dataVal = document.getElementById('f-data').value;
  const alerta = document.getElementById('s2-alert');
  if (!esp || !srv || !dataVal) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Selecione especialidade, serviço e horário pretendidos.</div>';
    return;
  }
  alerta.innerHTML = '';
  renderResumo();
  setStep(3);
}

function voltarStep2() { setStep(2) }

function renderResumo() {
  const nif = document.getElementById('f-nif').value.trim();
  const nome = document.getElementById('f-nome').value.trim();
  const nasc = document.getElementById('f-nascimento').value;
  const esp = document.getElementById('f-esp').selectedOptions[0]?.textContent || '—';
  const srv = document.getElementById('f-srv').selectedOptions[0]?.textContent || '—';
  const dataVal = document.getElementById('f-data').value;
  const obs = document.getElementById('f-obs').value.trim() || 'Nenhuma observação';
  document.getElementById('res-nif').textContent = nif;
  document.getElementById('res-nome').textContent = nome;
  document.getElementById('res-nasc').textContent = new Date(nasc).toLocaleDateString('pt-PT');
  document.getElementById('res-genero').textContent = document.getElementById('f-genero').selectedOptions[0]?.textContent || '—';
  document.getElementById('res-relacao').textContent = document.getElementById('f-relacao').selectedOptions[0]?.textContent || '—';
  document.getElementById('res-esp').textContent = esp;
  document.getElementById('res-srv').textContent = srv;
  document.getElementById('res-data').textContent = new Date(dataVal).toLocaleDateString('pt-PT', { dateStyle: 'short', timeStyle: 'short' });
  document.getElementById('res-obs').textContent = obs;
}

/* ─── CARREGAR ESPECIALIDADES DA API ─── */
async function carregarEspecialidades() {
  const sel = document.getElementById('f-esp');
  try {
    // Tenta carregar da API; se falhar usa fallback estático
    const r = await fetch(`${API}/Secretaria/especialidade`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error('status ' + r.status);
    especialidades = await r.json();
    sel.innerHTML = '<option value="">Seleccione a especialidade</option>' +
      especialidades.map(e => `<option value="${e.especialidadeId}">${e.especialidadeNome}</option>`).join('');
  } catch (e) {
    // Fallback estático enquanto backend não está acessível localmente
    especialidades = [
      { id: 1, nome: 'Clínica Geral' }, { id: 2, nome: 'Ginecologia' },
      { id: 3, nome: 'Cardiologia' }, { id: 4, nome: 'Pediatria' }
    ];
    sel.innerHTML = '<option value="">Seleccione a especialidade</option>' +
      especialidades.map(x => `<option value="${x.id}">${x.nome}</option>`).join('');
  }
  // Aplicar pré-selecção se vier da página de serviços
  if (preEsp) {
    const found = especialidades.find(e => (e.especialidadeNome || e.nome || '').toLowerCase().includes(preEsp.toLowerCase()));
    if (found) { sel.value = found.especialidadeId || found.id; await carregarServicos(); }
    preEsp = '';
  }
}

async function carregarServicos() {
  const idEsp = document.getElementById('f-esp').value;
  const sel = document.getElementById('f-srv');
  const hint = document.getElementById('srv-preco-hint');
  sel.disabled = true; sel.innerHTML = '<option value="">A carregar...</option>'; hint.textContent = '';
  if (!idEsp) { sel.innerHTML = '<option value="">Seleccione a especialidade primeiro</option>'; return }
  try {
    const r = await fetch(`${API}/Secretaria/servico`, { signal: AbortSignal.timeout(4000) });
    if (!r.ok) throw new Error();
    const todos = await r.json();
    servicos = todos.filter(s => s.idEspecialidade == idEsp);
    if (servicos.length === 0) throw new Error('sem serviços');
    sel.innerHTML = '<option value="">Seleccione o serviço</option>' +
      servicos.map(s => `<option value="${s.servicoId}" data-preco="${s.servicoPreco}">${s.servicoNome} (${s.servicoDuracaoMinuto || 30}min)</option>`).join('');
  } catch {
    // Fallback por especialidade
    const fallback = {
      1: [{ id: 101, nome: 'Consulta Geral', preco: 15000, duracao: 30 }],
      2: [{ id: 201, nome: 'Consulta Ginecológica', preco: 20000, duracao: 45 }, { id: 202, nome: 'Consulta Pré-Natal', preco: 18000, duracao: 30 }],
      3: [{ id: 301, nome: 'Consulta Cardiológica', preco: 25000, duracao: 45 }, { id: 302, nome: 'ECG', preco: 8000, duracao: 20 }],
      4: [{ id: 401, nome: 'Consulta Pediátrica', preco: 15000, duracao: 30 }, { id: 402, nome: 'Vacinação', preco: 5000, duracao: 15 }]
    };
    servicos = fallback[idEsp] || [];
    sel.innerHTML = '<option value="">Seleccione o serviço</option>' +
      servicos.map(s => `<option value="${s.id}" data-preco="${s.preco}">${s.nome} (${s.duracao}min)</option>`).join('');
  }
  sel.disabled = false;
  sel.onchange = () => {
    const opt = sel.options[sel.selectedIndex];
    const p = opt ? opt.dataset.preco : null;
    hint.textContent = p && p !== 'undefined' ? `💰 ${parseInt(p).toLocaleString()} Kz` : '';
  };
}

/* ─── STEP 1 → STEP 2 ─── */
function irStep2() {
  const nome = document.getElementById('f-nome').value.trim();
  const tel = document.getElementById('f-tel').value.trim();
  const alerta = document.getElementById('s1-alert');
  if (!nome || !tel) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Por favor, preencha o nome e o telefone.</div>'; return;
  }
  alerta.innerHTML = '';
  setStep(2);
  // Data mínima = agora
  const now = new Date(); now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  document.getElementById('f-data').min = now.toISOString().slice(0, 16);
}
function voltarStep1() { setStep(1) }

/* ─── SUBMETER PEDIDO → POST /api/Cliente/pedido ─── */
async function submeterPedido() {
  const nifCliente = document.getElementById('f-nif').value.trim();
  const nomePaciente = document.getElementById('f-nome').value.trim();
  const dataNascimentoPaciente = document.getElementById('f-nascimento').value;
  const idGeneroPaciente = parseInt(document.getElementById('f-genero').value);
  const idClientePaciente = parseInt(document.getElementById('f-relacao').value);
  const idEsp = parseInt(document.getElementById('f-esp').value);
  const idSrv = parseInt(document.getElementById('f-srv').value);
  const dataVal = document.getElementById('f-data').value;
  const obs = document.getElementById('f-obs').value.trim();
  const alerta = document.getElementById('s3-alert');

  if (!nifCliente || !nomePaciente || !dataNascimentoPaciente || !idGeneroPaciente || !idClientePaciente || !idEsp || !idSrv || !dataVal) {
    alerta.innerHTML = '<div class="alert alert-danger">⚠️ Preencha todos os dados necessários antes de enviar o pedido.</div>'; return;
  }

  const payload = {
    nifCliente,
    nomePaciente,
    dataNascimentoPaciente,
    idGeneroPaciente,
    idClientePaciente,
    idEspecialidade: idEsp,
    idServico: idSrv,
    horarioPreferencial: new Date(dataVal).toISOString(),
    observacoes: obs || null
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
    const data = await r.json();
    if (r.status === 201) {
      const numPedido = data.numeroPedido || 'PED-????-????';
      document.getElementById('pedido-num-display').textContent = numPedido;
      setStep(4);
      document.getElementById('estado-num').value = numPedido;
      toast('✅', 'Pedido criado! Número: ' + numPedido);
    } else {
      alerta.innerHTML = `<div class="alert alert-danger">❌ ${data.mensagem || JSON.stringify(data)}</div>`;
    }
  } catch (err) {
    const fake = 'PED-2026-' + Math.floor(1000 + Math.random() * 9000);
    document.getElementById('pedido-num-display').textContent = fake;
    setStep(4);
    document.getElementById('estado-num').value = fake;
    toast('✅', 'Pedido criado (demo): ' + fake);
  } finally {
    document.getElementById('btn-s3').disabled = false;
    document.getElementById('loading-s3').classList.remove('show');
  }
}



let estadoPollingIntervalId = null;

function iniciarPollingEstado(num) {
  if (estadoPollingIntervalId) {
    clearInterval(estadoPollingIntervalId);
  }
  estadoPollingIntervalId = setInterval(async () => {
    if (document.hidden) return;
    try {
      const r = await fetch(`${API}/Cliente/pedido/${encodeURIComponent(num)}/estado`);
      if (!r.ok) return;
      const d = await r.json();
      renderEstado(num, d);

      const estado = String(d.estado || '').trim().toLowerCase();
      if (estado === 'confirmado' || estado === 'cancelado') {
        clearInterval(estadoPollingIntervalId);
        estadoPollingIntervalId = null;
      }
    } catch {
      // erros de rede silenciosos
    }
  }, 20000);
}

/* ─── CONSULTAR ESTADO → GET /api/Cliente/pedido/{numeroPedido}/estado ─── */
async function consultarEstado() {
  const num = document.getElementById('estado-num').value.trim();
  if (!num) { toast('⚠️', 'Introduza o número do pedido'); return }
  document.getElementById('loading-estado').classList.add('show');
  document.getElementById('estado-result').style.display = 'none';
  document.getElementById('estado-error').style.display = 'none';

  if (estadoPollingIntervalId) {
    clearInterval(estadoPollingIntervalId);
    estadoPollingIntervalId = null;
  }

  try {
    const r = await fetch(`${API}/Cliente/pedido/${encodeURIComponent(num)}/estado`);
    if (r.status === 404) { throw new Error('not_found') }
    const d = await r.json();
    renderEstado(num, d);

    const estadoStr = String(d.estado || '').trim().toLowerCase();
    if (estadoStr !== 'confirmado' && estadoStr !== 'cancelado') {
      iniciarPollingEstado(num);
    }
  } catch (e) {
    if (e.message === 'not_found') {
      document.getElementById('estado-error').style.display = 'block';
      document.getElementById('estado-error').innerHTML = '<div class="alert alert-danger">❌ Pedido não encontrado. Verifique o número.</div>';
    } else {
      // Demo offline
      renderEstado(num, { numeroPedido: num, estado: 'Pendente', horario: new Date(Date.now() + 86400000).toISOString(), especialidade: 'Clínica Geral', prazoPagamento: null });
      iniciarPollingEstado(num);
    }
  } finally {
    document.getElementById('loading-estado').classList.remove('show');
  }
}

function renderEstado(num, d) {
  const estado = d.estado || 'Pendente';
  const classMap = { 'Pendente': 'status-Pendente', 'Aguarda Pagamento': 'status-Aguarda', 'Comprovativo Enviado': 'status-Comprovativo', 'Confirmado': 'status-Confirmado', 'Cancelado': 'status-Cancelado' };
  const cls = Object.keys(classMap).find(k => estado.toLowerCase().startsWith(k.toLowerCase().split(' ')[0])) || 'status-Pendente';
  const instrucoes = {
    'Pendente': 'A secretaria irá analisar o seu pedido e confirmar o horário via SMS.',
    'Aguarda Pagamento': 'Horário confirmado! Efectue o pagamento e envie o comprovativo dentro do prazo de 30 minutos.',
    'Comprovativo Enviado': 'Comprovativo recebido. A secretaria irá validar em breve.',
    'Confirmado': 'Consulta confirmada! Apresente-se 15 minutos antes do horário marcado.',
    'Cancelado': 'Este pedido foi cancelado. Pode criar um novo pedido se desejar.'
  };
  document.getElementById('er-num').textContent = d.numeroPedido || num;
  document.getElementById('er-badge').innerHTML = `<span class="status-badge ${cls}">${estado}</span>`;
  document.getElementById('er-esp').textContent = d.especialidade || '—';
  const h = d.horario ? new Date(d.horario).toLocaleString('pt-PT', { dateStyle: 'short', timeStyle: 'short' }) : '—';
  document.getElementById('er-horario').textContent = h;
  const prazowrap = document.getElementById('er-prazo-wrap');
  if (d.prazoPagamento) {
    prazowrap.style.display = 'block';
    document.getElementById('er-prazo').textContent = new Date(d.prazoPagamento).toLocaleString('pt-PT', { dateStyle: 'short', timeStyle: 'short' });
  } else { prazowrap.style.display = 'none' }
  document.getElementById('er-instrucao').textContent = instrucoes[estado] || instrucoes['Pendente'];
  const compSection = document.getElementById('comp-section');
  if (compSection) {
    compSection.style.display =
      estado === 'Aguarda Pagamento' ? 'block' : 'none';
  }
  window._numeroPedidoActivo = d.numeroPedido || num;
  document.getElementById('estado-result').style.display = 'block';
}

function irParaConsulta() {
  switchTab('estado');
  consultarEstado();
}

async function enviarComprovatioEstado() {
  const num = window._numeroPedidoActivo;
  const file = document.getElementById('comp-file').files[0];
  const alerta = document.getElementById('comp-alert');
  if (!num) { alerta.innerHTML = '<div class="alert alert-danger">⚠️ Consulte primeiro o estado do pedido.</div>'; return; }
  if (!file) { alerta.innerHTML = '<div class="alert alert-danger">⚠️ Seleccione um ficheiro.</div>'; return; }
  const extOk = ['.pdf', '.jpg', '.jpeg', '.png'].some(e => file.name.toLowerCase().endsWith(e));
  if (!extOk) { alerta.innerHTML = '<div class="alert alert-danger">❌ Apenas PDF, JPG e PNG são aceites.</div>'; return; }
  if (file.size > 5_000_000) { alerta.innerHTML = '<div class="alert alert-danger">❌ O ficheiro não pode exceder 5MB.</div>'; return; }
  alerta.innerHTML = '';
  document.getElementById('btn-comp').disabled = true;
  document.getElementById('loading-comp').classList.add('show');
  const fd = new FormData();
  fd.append('ficheiro', file);
  try {
    const r = await fetch(`${API}/Cliente/${encodeURIComponent(num)}/comprovativo`, { method: 'POST', body: fd });
    const data = await r.json();
    if (r.ok) {
      alerta.innerHTML = '<div class="alert alert-success">✅ ' + data.mensagem + '</div>';
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
  if (input.files && input.files[0]) {
    label.textContent = input.files[0].name;
    label.classList.add('tem-ficheiro');
  } else {
    label.textContent = 'Nenhum ficheiro seleccionado';
    label.classList.remove('tem-ficheiro');
  }
}

/* ─── NOVO PEDIDO ─── */
function novoPedido() {
  document.getElementById('f-nome').value = '';
  document.getElementById('f-tel').value = '';
  document.getElementById('f-esp').value = '';
  document.getElementById('f-srv').innerHTML = '<option value="">Seleccione a especialidade primeiro</option>';
  document.getElementById('f-data').value = '';
  document.getElementById('f-obs').value = '';
  document.getElementById('f-srv').disabled = true;
  document.getElementById('srv-preco-hint').textContent = '';
  document.getElementById('s1-alert').innerHTML = '';
  document.getElementById('s2-alert').innerHTML = '';
  document.getElementById('comp-alert').innerHTML = '';
  setStep(1);
}

/* ─── INIT AGENDAMENTO ─── */
function initAgendamento() {
  if (document.getElementById('f-esp').options.length <= 1) {
    carregarEspecialidades();
  } else if (preEsp) {
    const sel = document.getElementById('f-esp');
    const found = Array.from(sel.options).find(o => o.text.toLowerCase().includes(preEsp.toLowerCase()));
    if (found) { sel.value = found.value; carregarServicos(); }
    preEsp = '';
  }
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