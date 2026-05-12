const API = 'http://localhost:5290/api';
const token = localStorage.getItem('token');
const role = localStorage.getItem('role');
const nome = localStorage.getItem('nome') || '';

function authHeaders() {
  const headers = { 'Content-Type': 'application/json' };
  if (token) headers.Authorization = 'Bearer ' + token;
  return headers;
}

function showToast(message, type = 'success') {
  const toast = document.getElementById('toast');
  if (!toast) return;
  const icon = type === 'success'
    ? '<svg viewBox="0 0 24 24"><polyline points="20 6 9 17 4 12"/></svg>'
    : '<svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>';
  toast.className = `toast ${type}`;
  toast.innerHTML = icon + message;
  toast.classList.add('show');
  setTimeout(() => toast.classList.remove('show'), 3200);
}

function formatDate(date) {
  if (!date) return '—';
  const d = new Date(date);
  if (Number.isNaN(d.valueOf())) return String(date);
  return d.toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
}

function formatDateShort(date) {
  if (!date) return '—';
  const d = new Date(date);
  if (Number.isNaN(d.valueOf())) return String(date);
  return d.toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function redirectToLogin() {
  localStorage.clear();
  const inAdminLikeFolder = /\/(admin|secretaria|medico)\//i.test(window.location.pathname);
  window.location.href = inAdminLikeFolder ? '../index.html' : 'index.html';
}

function logout() {
  redirectToLogin();
}

function fetchJson(path, options = {}) {
  const headers = { ...authHeaders(), ...(options.headers || {}) };
  const config = { ...options, headers };
  if (options.body && typeof options.body !== 'string') {
    config.body = JSON.stringify(options.body);
  }
  return fetch(API + path, config)
    .then(async (res) => {
      const text = await res.text();
      let json = null;
      if (text) {
        try { json = JSON.parse(text); } catch { json = text; }
      }
      if (!res.ok) throw { status: res.status, data: json };
      return json;
    });
}

function fetchJsonNoBody(path, method = 'GET') {
  return fetchJson(path, { method });
}

function getErrorMessage(error, fallback = 'Erro inesperado.') {
  if (!error) return fallback;
  if (error.data) {
    if (typeof error.data === 'string') return error.data;
    if (typeof error.data === 'object') {
      if (typeof error.data.mensagem === 'string') return error.data.mensagem;
      const errors = [];
      const flatten = (value) => {
        if (value == null) return;
        if (typeof value === 'string') {
          errors.push(value);
        } else if (Array.isArray(value)) {
          value.forEach(flatten);
        } else if (typeof value === 'object') {
          Object.values(value).forEach(flatten);
        }
      };
      flatten(error.data);
      if (errors.length) return errors.join(' ');
    }
  }
  if (typeof error === 'string') return error;
  return error.message || fallback;
}

function safeField(item, ...keys) {
  for (const key of keys) {
    if (item && key in item && item[key] !== null && item[key] !== undefined) {
      return item[key];
    }
  }
  return '—';
}

function openModal(id) {
  document.getElementById(id)?.classList.add('open');
}

function closeModal(id) {
  document.getElementById(id)?.classList.remove('open');
}

function pickValue(item, keys) {
  for (const key of keys) {
    if (item && item[key] !== undefined && item[key] !== null) {
      return item[key];
    }
  }
  return '';
}

function setSelectOptions(selectId, items, valueKeys, labelKeys) {
  const select = document.getElementById(selectId);
  if (!select) return;
  const valArr = Array.isArray(valueKeys) ? valueKeys : [valueKeys];
  const labArr = Array.isArray(labelKeys) ? labelKeys : [labelKeys];
  select.innerHTML = '<option value="">Seleccionar...</option>' + items.map(item => {
    const value = pickValue(item, valArr);
    const label = pickValue(item, labArr) || value;
    return `<option value="${value}">${label}</option>`;
  }).join('');
}

function toDatetimeLocal(value) {
  if (!value) return '';
  const d = new Date(value);
  if (Number.isNaN(d.valueOf())) return '';
  const pad = n => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function badgeEstado(estado) {
  const t = String(estado || '').trim();
  const map = {
    'Activo': 'badge-green', 'Inactivo': 'badge-red',
    'Pendente': 'badge-amber', 'Validado': 'badge-green',
    'Cancelado': 'badge-red', 'Rejeitado': 'badge-red',
    'Finalizada': 'badge-blue', 'Agendada': 'badge-amber'
  };
  return `<span class="badge ${map[t] || 'badge-gray'}">${t}</span>`;
}

let actionConfirmHandler = null;
function openActionConfirmModal(title, message, onConfirm) {
  const t = document.getElementById('actionConfirmTitle');
  const m = document.getElementById('actionConfirmMessage');
  if (t) t.textContent = title || 'Confirmar Ação';
  if (m) m.textContent = message || 'Tem a certeza?';
  actionConfirmHandler = onConfirm;
  openModal('modalActionConfirm');
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('actionConfirmBtn')?.addEventListener('click', async () => {
    if (!actionConfirmHandler) return;
    try {
      await actionConfirmHandler();
      closeModal('modalActionConfirm');
    } catch (error) {
      showToast(getErrorMessage(error), 'error');
    }
  });
});

const Validators = {
  nif: (val) => /^\d{9}$/.test(val),
  bi: (val) => /^\d{9}[A-Z]{2}\d{3}$/.test(val)
};

function filterTable(input, tableId) {
  const query = input.value.toLowerCase();
  document.querySelectorAll(`#${tableId} tbody tr`).forEach(row => {
    row.style.display = row.textContent.toLowerCase().includes(query) ? '' : 'none';
  });
}

function sidebarToggle() {
  const sidebar = document.getElementById('sidebar');
  const overlay = document.getElementById('sidebarOverlay');
  sidebar?.classList.toggle('open');
  overlay?.classList.toggle('open');
}
