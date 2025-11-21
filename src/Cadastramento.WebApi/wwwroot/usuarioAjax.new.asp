<% ' usuarioAjax.new.asp
'  This is a ready-to-copy script file to replace/update your ASP Classic AJAX calls.
'  It defines window.API_BASE dynamically (from the ASP page) and provides
'  fetch-based functions to call the new .NET WebAPI endpoints.
'  Copy this file to your server (e.g. C:\inetpub\wwwroot\TesteEntrevista\v2\usuario\ajax\usuarioAjax.asp)
'  or include its <script> contents in your usuario.asp page.
%>
<%
' Generate API base URL dynamically. Adjust apiPort if your API runs on another port.
Dim apiHost, apiPort, apiBase
apiHost = Request.ServerVariables("SERVER_NAME")
apiPort = "7034"  ' change if needed
apiBase = "http://" & apiHost & ":" & apiPort
%>
<script type="text/javascript">
// Base URL provided by ASP server
window.API_BASE = "<%= apiBase %>";
if (!window.API_BASE) window.API_BASE = "http://localhost:7034";

// Compatibility helper for legacy scripts that call parseJson
function parseJson(text) {
  try {
    return JSON.parse(text);
  } catch (e) {
    return text;
  }
}

// --- API helper functions (fetch) ---
async function listarUsuarios(page = 1, pageSize = 10) {
  const base = window.API_BASE;
  const url = `${base}/api/user?page=${page}&pageSize=${pageSize}`;
  const res = await fetch(url, { method: 'GET', credentials: 'include' });
  if (!res.ok) throw new Error('HTTP ' + res.status + ' - ' + (await res.text()));
  return await res.json();
}

async function criarUsuario(username, status, perfis) {
  const base = window.API_BASE;
  const res = await fetch(`${base}/api/user`, {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, status, perfis })
  });
  if (!res.ok) {
    const txt = await res.text();
    throw new Error(res.status + ' ' + txt);
  }
  return res;
}

async function atualizarUsuario(id, username, status, perfis) {
  const base = window.API_BASE;
  const res = await fetch(`${base}/api/user/${id}`, {
    method: 'PUT',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ id, username, status, perfis })
  });
  if (!res.ok) {
    const txt = await res.text();
    throw new Error(res.status + ' ' + txt);
  }
  return res;
}

// Utility: render rows into your existing table (implement according to your DOM)
function renderizarTabelaUsuarios(rows) {
  // Example: assumes table body with id="usuarios-tbody"
  const tbody = document.getElementById('usuarios-tbody');
  if (!tbody) return;
  tbody.innerHTML = '';
  rows.forEach(u => {
    const tr = document.createElement('tr');
    const tdId = document.createElement('td'); tdId.textContent = u.id;
    const tdNome = document.createElement('td'); tdNome.textContent = u.username || u.nome || '';
    const tdStatus = document.createElement('td'); tdStatus.textContent = (u.status ? 'ATIVO' : 'INATIVO');
    tr.appendChild(tdId); tr.appendChild(tdNome); tr.appendChild(tdStatus);
    tbody.appendChild(tr);
  });
}

// Example: wire searching button to call listarUsuarios
document.addEventListener('DOMContentLoaded', function () {
  const btnBuscar = document.getElementById('btn-buscar-usuarios');
  if (btnBuscar) {
    btnBuscar.addEventListener('click', async function (e) {
      e.preventDefault();
      try {
        const resp = await listarUsuarios(1, 10);
        renderizarTabelaUsuarios(resp.data || []);
      } catch (err) {
        console.error(err);
        alert('Erro ao listar usuários: ' + err.message);
      }
    });
  }
});
</script>


