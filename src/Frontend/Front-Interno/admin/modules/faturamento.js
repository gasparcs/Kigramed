// ─────────────────────────────────────────────────────────────────────────────
// FATURAMENTO (PAGAMENTOS & SMS)
// ─────────────────────────────────────────────────────────────────────────────

async function loadPagamentos() {
  const body = document.getElementById('bodyPagamentos');
  if (!body) return;
  try {
    const [pagamentos, pagamentosConsulta] = await Promise.all([
      fetchJson('/Admin/pagamento'),
      fetchJson('/Admin/pagamentoconsulta')
    ]);
    const pagamentoValorMap = new Map((pagamentosConsulta || []).map((pc) => [pc.idPagamento || pc.IdPagamento, pc.valorServico ?? pc.ValorServico ?? '—']));
    const items = pagamentos || [];
    body.innerHTML = items.length
      ? items.map((item) => {
          const pagamentoId = item.id || item.Id;
          const precoServico = pagamentoValorMap.get(pagamentoId) ?? '—';
          return `<tr>
            <td>${pagamentoId || '—'}</td>
            <td>${item.cliente || item.Cliente || '—'}</td>
            <td>${item.secretaria || item.Secretaria || '—'}</td>
            <td>${precoServico}</td>
            <td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td>
            <td>${formatDate(item.dataEnvio || item.DataEnvio || item.Data)}</td>
          </tr>`;
        }).join('')
      : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pagamento encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="6" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos.</td></tr>';
  }
}

async function loadPagamentoConsulta() {
  const body = document.getElementById('bodyPagamentoConsulta');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/pagamentoconsulta');
    body.innerHTML = (items || []).length
      ? items.map((item) => `<tr>
          <td>${item.id || item.Id || '—'}</td>
          <td>${item.idPagamento || item.IdPagamento || '—'}</td>
          <td>${item.idConsulta || item.IdConsulta || '—'}</td>
          <td>${formatDate(item.dataConsulta || item.DataConsulta)}</td>
          <td>${renderComprovativoCell(item.comprovativo || item.Comprovativo || item.caminhoComprovativo || item.CaminhoComprovativo)}</td>
        </tr>`).join('')
      : '<tr><td colspan="5" style="text-align:center;color:var(--muted);padding:24px">Nenhum registo encontrado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar pagamentos de consulta.</td></tr>';
  }
}

async function loadSMS() {
  const body = document.getElementById('bodySMS');
  if (!body) return;
  try {
    const items = await fetchJson('/Admin/SMS');
    body.innerHTML = (items || []).length
      ? items.map((item) => `<tr>
          <td>${item.smsId || item.SmsId || '—'}</td>
          <td>${item.cliente?.clienteNome || item.cliente?.ClienteNome || '—'}</td>
          <td>${item.mensagem || item.Mensagem || '—'}</td>
          <td>${formatDate(item.dataEnvio || item.DataEnvio)}</td>
        </tr>`).join('')
      : '<tr><td colspan="4" style="text-align:center;color:var(--muted);padding:24px">Nenhum SMS enviado.</td></tr>';
  } catch {
    body.innerHTML = '<tr><td colspan="4" style="text-align:center;color:var(--danger);padding:24px">Erro ao carregar SMS.</td></tr>';
  }
}

async function submitPagamento(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/pagamento', {
      method: 'POST',
      body: {
        IdCliente: document.getElementById('payCliente').value,
        IdSecretaria: localStorage.getItem('nif') || getUserId() || 1,
        Comprovativo: document.getElementById('payComprovativo').value,
        Data: document.getElementById('payData').value || new Date().toISOString()
      }
    });
    showToast('Pagamento registado com sucesso.', 'success');
    closeModal('modalPagamento');
    loadPagamentos();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao criar pagamento.'), 'error'); }
}

async function submitPagamentoConsulta(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/pagamentoconsulta', {
      method: 'POST',
      body: {
        IdPagamento: parseInt(document.getElementById('pcPagamento').value, 10),
        IdConsulta: parseInt(document.getElementById('pcConsulta').value, 10)
      }
    });
    showToast('Pagamento de consulta registado.', 'success');
    closeModal('modalPagamentoConsulta');
    loadPagamentoConsulta();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao registar pagamento de consulta.'), 'error'); }
}

async function submitSMS(event) {
  event.preventDefault();
  try {
    await fetchJson('/Admin/SMS', {
      method: 'POST',
      body: {
        SMSMensagem: document.getElementById('smsMensagem').value,
        SMSNif_funcionario: localStorage.getItem('nif') || getUserId() || '',
        SMSNif_cliente: document.getElementById('smsCliente').value
      }
    });
    showToast('SMS enviado com sucesso.', 'success');
    closeModal('modalSMS');
    loadSMS();
  } catch (e) { showToast(getErrorMessage(e, 'Erro ao enviar SMS.'), 'error'); }
}

function renderComprovativoCell(value) {
  if (!value) return '—';
  const safe = String(value).replace(/'/g, "\\'");
  return `<button class="btn btn-sm btn-outline" onclick="openComprovativoFromPath('${safe}')">Ver comprovativo</button>`;
}
