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
  window.location.href = 'index.html';
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
        try {
          json = JSON.parse(text);
        } catch {
          json = text;
        }
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
