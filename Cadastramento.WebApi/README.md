# API de Usuários Cadastramento

Backend desenvolvido em .NET 8+ com Clean Architecture, Dapper, autenticação via Redis integrada ao ASP Classic.

## 💡 Autenticação
- A API exige o cookie `SessionEntrevistaId` válido (gerado pelo ASP Classic) em todas as requisições.
- Cada requisição consulta a chave `sessionEntrevista:{SESSION_ID}` no Redis remoto.
- Se inválido ou expirado a API retorna HTTP 401.

## 🔐 Configuração Redis
- Redis host: `10.255.200.7:47846`
- A chave/ticket de sessão deve estar nesse servidor, exatamente como gerado pelo ASP Classic.

## 📚 Endpoints `api/user`

### 1. Listar usuários (paginação)
```
GET /api/user?page=1&pageSize=10
Cookie: SessionEntrevistaId=4766AFA9-D050-442E-839C-CE0497EBD556
```
- **Retorna:**
```json
{
  "data": [
    {
      "id": 1,
      "username": "admin",
      "status": true,
      "perfis": ["Administrador"]
    }
  ],
  "total": 15
}
```

### 2. Obter usuário por ID
```
GET /api/user/1
Cookie: SessionEntrevistaId=4766AFA9-D050-442E-839C-CE0497EBD556
```
- **Retorna:**
```json
{
  "id": 1,
  "username": "admin",
  "status": true,
  "perfis": ["Administrador"]
}
```
Ou `404` se não encontrado.

### 3. Criar usuário
```
POST /api/user
Cookie: SessionEntrevistaId=...
Content-Type: application/json
{
  "username": "novoUser",
  "status": true,
  "perfis": ["Administrador"]
}
```
- **Retorna:**
`201 Created` no sucesso. Location no header.

### 4. Atualizar usuário
```
PUT /api/user/1
Cookie: SessionEntrevistaId=...
Content-Type: application/json
{
  "id": 1,
  "username": "adminAtualizado",
  "status": false,
  "perfis": ["Leitura"]
}
```
- **Retorna:**
`204 No Content` no sucesso.

## ℹ️ Observações
- Todos os endpoints exigem autenticação Redis (não serão acessíveis sem um SessionEntrevistaId válido no cookie).
- Transações SQL são garantidas para escrita/atualização.
- Logging via Serilog, documentação dinâmica via Swagger (`/swagger`, se habilitado).

---
Dúvidas? Chame! 😉
