# Customer Support Ticket Management System

Sistem manajemen tiket dukungan pelanggan berbasis .NET 10, menggantikan proses komplain manual via email menjadi sistem tiket terpusat dengan role-based access (Support Agent & Manager).

> Dokumen pendukung: `product.md`, `tech.md`, `structure.md` (steering docs), serta `Requirements.md`, `Design.md`, `tasks.md` (spec docs) tersedia di root repository.

---

## 🏗️ Arsitektur

Proyek ini menggunakan **8-Layer Clean Architecture**. Detail lengkap ada di `structure.md`, ringkasannya:

| Layer | Isi |
|---|---|
| `01.Base` | Primitives, exceptions |
| `02.Domain` | Entities, enums, repository interfaces |
| `03.Shared` | DTOs, PagedResult, constants |
| `04.Application` | Business services |
| `05.Infrastructure` | EF Core, repositories, migrations |
| `06.WebApi` | REST API (Controllers, JWT, Swagger) |
| `07.Client` | Typed HTTP Client SDK |
| `08.Bsui` | ASP.NET Core Razor Pages + jQuery/AJAX (UI) |

---

## 🔧 Prasyarat

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB / Express / Developer Edition)
- `dotnet-ef` CLI tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 🚀 Setup & Menjalankan Proyek

### 1. Clone & Restore

```bash
git clone <repository-url>
cd TicketManagementSystem
dotnet restore
```

### 2. Konfigurasi Connection String

Edit `src/06.WebApi/appsettings.json` dan `src/08.Bsui/appsettings.json` sesuai environment SQL Server Anda:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### 3. Jalankan Migration (Membuat Database + Seed Data)

```bash
dotnet ef database update \
  --project src/05.Infrastructure \
  --startup-project src/06.WebApi
```

Ini akan otomatis membuat seluruh tabel (`Users`, `Tickets`, `TicketHistories`) dan seed 2 user default:

| Role | Email | Password |
|---|---|---|
| Manager | `manager@ticket.com` | `Manager123!` |
| Support Agent | `agent1@ticket.com` | `Agent123!` |

> Alternatif: jalankan `schema.sql` (lampiran terpisah, hasil `dotnet ef migrations script --idempotent`) langsung di SQL Server Management Studio / Azure Data Studio jika tidak ingin pakai CLI.

### 4. Jalankan Web API

```bash
dotnet run --project src/06.WebApi
```

Catat port yang muncul di terminal (`Now listening on: https://localhost:XXXX`), lalu buka:
```
https://localhost:XXXX/swagger
```

### 5. Update Base URL di Bsui

Edit `src/08.Bsui/appsettings.json`, samakan port dengan Web API pada langkah 4:
```json
{
  "WebApiBaseUrl": "https://localhost:XXXX/"
}
```

### 6. Jalankan Web UI (Razor Pages)

Di terminal terpisah (biarkan Web API tetap berjalan):
```bash
dotnet run --project src/08.Bsui
```

Buka URL yang muncul di terminal, lalu login dengan salah satu akun di atas.

---

## 📋 Contoh API Calls (via Swagger atau cURL)

### Login
```bash
curl -X POST https://localhost:XXXX/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"manager@ticket.com","password":"Manager123!"}'
```
Response:
```json
{
  "token": "eyJhbGciOi...",
  "name": "Default Manager",
  "role": "Manager",
  "expiresAt": "2026-07-29T08:00:00Z"
}
```

### Buat Tiket Baru
```bash
curl -X POST https://localhost:XXXX/api/tickets \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "customerName": "Budi Santoso",
    "customerEmail": "budi@example.com",
    "title": "Gagal login aplikasi",
    "description": "Tidak bisa login sejak kemarin malam"
  }'
```

### Assign Tiket ke Agent (Manager Only)
```bash
curl -X PUT https://localhost:XXXX/api/tickets/1/assign \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <manager-token>" \
  -d '{"assignedToUserId": 2}'
```

### Manager Report dengan Filter
```bash
curl -X GET "https://localhost:XXXX/api/reports/manager?status=Open&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer <manager-token>"
```

### Dashboard Summary
```bash
curl -X GET https://localhost:XXXX/api/dashboard/summary \
  -H "Authorization: Bearer <manager-token>"
```

---

## 🔐 Business Rules Penting

- Tiket baru selalu berstatus `Open` (REQ-2.3).
- Tiket berstatus `Closed` **tidak dapat diubah** — baik update maupun assign (REQ-2.5).
- Tiket hanya bisa di-assign ke user dengan role `SupportAgent` (REQ-2.6).
- **Tidak ada hard-delete tiket** — sengaja tidak diimplementasikan untuk menjaga integritas audit trail (`TicketHistory`). Lihat REQ-2.9.
- Setiap perubahan status/assignment dicatat otomatis di tabel `TicketHistories` (REQ-2.8).

---

## ⚠️ Known Limitations / Keputusan Desain Sadar

- **Unit/Integration Test tidak disertakan** dalam deliverable final. Pengujian dilakukan secara manual melalui Swagger UI dan Razor Pages UI langsung, mengingat keterbatasan waktu pengerjaan (1 hari). Business rules kritikal (validasi status Closed, validasi assignee role, auto-generate ticket number) sudah diverifikasi manual dan berfungsi sesuai spesifikasi.
- **Anti-forgery token** diterapkan pada seluruh AJAX call POST/PUT di `08.Bsui` demi keamanan CSRF (lihat implementasi di `tickets.js` dan `manager-report.js`).
- **JWT Secret** di `appsettings.json` untuk keperluan assessment ini disimpan plaintext demi kemudahan setup. Untuk production, sebaiknya dipindah ke User Secrets / Environment Variable / Azure Key Vault.
- **Frontend UI** menggunakan ASP.NET Core Razor Pages + jQuery/AJAX (bukan Blazor atau SPA framework), sesuai keputusan yang disepakati di `tech.md`.

---

## 📁 Struktur Deliverables

```
TicketManagementSystem/
├── src/                    # Source code (8-layer architecture)
├── tests/                  # (kosong — lihat Known Limitations)
├── schema.sql               # SQL script lengkap (schema + seed data, idempotent)
├── seed-data.sql            # Seed data terpisah (referensi, lihat catatan di file)
├── product.md                # Steering: product overview
├── tech.md                   # Steering: tech stack
├── structure.md              # Steering: architecture & conventions
├── Requirements.md            # Spec: functional & non-functional requirements
├── Design.md                  # Spec: database design, API design, flow
├── tasks.md                   # Spec: task breakdown & progress checklist
└── README.md                  # Dokumen ini
```
