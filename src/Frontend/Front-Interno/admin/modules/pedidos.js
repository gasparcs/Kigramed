// ─────────────────────────────────────────────────────────────────────────────
// PEDIDOS (ONLINE)
// ─────────────────────────────────────────────────────────────────────────────

async function loadPedidos() {
  const body = document.getElementById('bodyPedidos');
  if (!body) return;
  try {
    const res = await fetchJson('/Admin/pedidos');
    const items = res?.dados || [];
    body.innerHTML = items.length
      ? items.map((item) => {
          const id = item.id || item.Id || '—';
          const cliente = item.nomeCliente || item.NomeCliente || item.clienteNome || item.ClienteNome || '—';
          const servico = item.servico || item.Servico || item.servicoNome || item.ServicoNome || '—';
          const horario = item.horarioPreferencial || item.HorarioPreferencial || item.data_consulta || item.Data_consulta || item.DataConsulta;
          const estado = normalizePedidoEstado(item.estado || item.Estado || '—');
          const actions = renderPedidoActions(id, estado);
          return `<tr>
            <td>${id}</td>
            <td>${cliente}</td>
            <td>${servico}</td>
            <td>${formatDate(horario)}</td>
            <td>${badgeEstado(estado)}</td>
            <td style="display:flex;gap:6px;flex-wrap:wrap;">${actions || '<span style="color:var(--muted)">—</span>'}</td>
          </tr>`;
        }).join('')
      : '<tr><td colspan="6" style="text-align:center;color:var(--muted);padding:24px">Nenhum pedido encontrado.</td></tr>';
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

function openComprovativoFromPath(path) {
  if (!path) return;
  window.open(`http://localhost:5290/api/Secretaria/comprovativo?caminho=${encodeURIComponent(path)}`, '_blank');
}

function openComprovativo(id) {
  window.open(`http://localhost:5290/api/Secretaria/${id}/comprovativo`, '_blank');
}

function normalizePedidoEstado(rawEstado) {
  const estado = String(rawEstado || '').trim().toLowerCase();
  if (estado === 'pendente') return 'Pendente';
  if (estado === 'comprovativo enviado' || estado === 'pagamento enviado') return 'Pagamento Enviado';
  if (estado === 'confirmado' || estado === 'validado') return 'Validado';
  if (estado === 'cancelado') return 'Cancelado';
  if (estado === 'rejeitado') return 'Rejeitado';
  return rawEstado || '—';
}

function renderPedidoActions(id, estadoNormalizado) {
  if (estadoNormalizado === 'Pendente') {
    return `<button class="btn btn-sm btn-success" onclick="handlePedidoAction(${id}, 'confirmar')">Confirmar</button><button class="btn btn-sm btn-outline" onclick="handlePedidoAction(${id}, 'cancelar')">Cancelar</button>`;
  }
  if (estadoNormalizado === 'Pagamento Enviado') {
    return `<button class="btn btn-sm btn-primary" onclick="handlePedidoAction(${id}, 'validar')">Validar</button><button class="btn btn-sm btn-danger" onclick="handlePedidoAction(${id}, 'rejeitar')">Rejeitar</button><button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Comprovativo</button>`;
  }
  if (estadoNormalizado === 'Validado') {
    return `<button class="btn btn-sm btn-outline" onclick="openComprovativo(${id})">Comprovativo</button>`;
  }
  return '';
}
