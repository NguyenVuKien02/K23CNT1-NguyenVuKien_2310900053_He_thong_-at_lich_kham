# PhongKham: clinic appointment booking and patient record system (graduation project)

Read this file first. Business spec: `docs/SPEC.md`. Database script: `docs/PhongKhamDB.sql`.

## 1. Non-negotiable rules

1. The SQL Server database `PhongKhamDB` ALREADY EXISTS and is the single source of truth. NEVER create, alter or drop tables, indexes or constraints. NEVER run `dotnet ef migrations ...` or `dotnet ef database update`. In the terminal, only run SELECT queries to inspect data. If the code seems to need a schema change, STOP and tell me.
2. Do not invent features, tables, columns or packages that are not in `docs/SPEC.md`. If something is ambiguous, ask one short question instead of guessing.
3. Work in small steps (one task = one feature). After each task: `dotnet build` must pass with 0 errors. Then report (a) files added/changed, (b) how to test manually, (c) anything left undone. Do NOT run `git commit` or `git push`; I commit myself.
4. All user-facing text is Vietnamese WITH diacritics. Code identifiers follow section 6.
5. Never hard-code secrets. Connection string and seed credentials come from user-secrets: `ConnectionStrings:DefaultConnection`, `Seed:AdminEmail`, `Seed:AdminPassword`.
6. Do not add NuGet/npm packages beyond section 2 without asking.
7. Never delete or overwrite files outside the solution folder.

## 2. Stack

- .NET 10 (LTS), ASP.NET Core MVC with Areas, Razor views. No SPA, no Blazor, no Web API controllers unless the spec asks for JSON endpoints (slot lists, notifications).
- EF Core 10 with `Microsoft.EntityFrameworkCore.SqlServer` (+ `.Design` for scaffolding only). Database First: scaffold ONCE, then hand-maintain.
- Bootstrap 5.3 and Bootstrap Icons via CDN, Chart.js 4 via CDN (dashboard only), font Be Vietnam Pro (Google Fonts). Plain JavaScript; jQuery only for client validation.
- Authentication: cookie authentication + `PasswordHasher<User>` (namespace `Microsoft.AspNetCore.Identity`). Do NOT use ASP.NET Core Identity tables or `IdentityDbContext`; the database has a custom `Users` table.
  - Role comes from `Users.RoleId` -> `Roles.Name` (Admin, Receptionist, Doctor, Patient). Look roles up by Name, never hard-code Ids.
  - Claims: NameIdentifier = Users.Id, Name, Email, Role. Use `[Authorize(Roles = "...")]`.
  - If `PasswordHasher<T>` is not available in the shared framework, ask before adding `Microsoft.Extensions.Identity.Core`.
- Tests (later task only): xUnit with mocked repositories for service-level business rules. Do not use InMemory/SQLite providers to test the double-booking index (it only exists in SQL Server).

## 3. Solution structure and dependency rule

```
PhongKham.sln
  PhongKham.Common     enums, constants, Result/Result<T>, helpers     (no references)
  PhongKham.Data       Entities/, Context/, Repositories/             -> Common
  PhongKham.Business   Services/ (+Interfaces), DTOs/                 -> Data, Common
  PhongKham.Web        Areas/{Admin,LeTan,BacSi,BenhNhan}, Controllers/, ViewModels/, Views/, wwwroot/  -> Business, Common
```

- Dependencies only go downward: Web -> Business -> Data. Common is shared by all. NEVER reference Data from Web; Business exposes `AddBusinessServices(IServiceCollection, IConfiguration)` which registers DbContext, repositories, unit of work and services.
- Controllers are thin: bind ViewModel, call a service, map the `Result` to the view/redirect. No business rules and no DbContext in controllers.
- Services hold all business rules and talk to the database only through repository interfaces and `IUnitOfWork`.
- Entities never reach Views. Views use ViewModels (Web) and services return DTOs (Business). Map manually (no AutoMapper).
- Namespaces mirror folders. Interfaces start with `I`. Async methods end with `Async`.

## 4. Database facts (22 tables, see docs/PhongKhamDB.sql)

Roles, Users, Patients, Doctors, Staffs, Specialties, CaLamViec, LichLamViec, LichHen, LichSuLichHen, HoSoBenhAn, PhieuKham, DonThuoc, ChiTietDonThuoc, MedicineCategories, Medicines, DichVu, ChiDinhDichVu, HoaDon, ChiTietHoaDon, DanhGia, ThongBao.

- `Users`: Id, RoleId, Email (unique), PasswordHash, IsActive, CreatedAt, LastLoginAt. One role per user. Patients.UserId is nullable (walk-in patients have no account). Doctors.UserId and Staffs.UserId are required and unique.
- Table and column names mix English and unaccented Vietnamese ON PURPOSE. Do not rename them. FK column = referenced table name + `Id` (DoctorId, LichHenId, PhieuKhamId).
- `LichHen.SoThuTu INT NULL` (queue number set at check-in) was added by me with a manual `ALTER TABLE` before scaffolding. If the scaffolded model lacks it, stop and tell me.
- DATE -> `DateOnly`, TIME -> `TimeOnly`, DATETIME2 -> `DateTime`, money DECIMAL(18,0) -> `decimal` (VND, no fractions).
- Computed persisted columns `HoaDon.ThanhTien` and `ChiTietHoaDon.ThanhTien` are read-only in EF (value generated on add or update). Never assign them.
- Filtered unique index on `LichHen(DoctorId, NgayHen, GioBatDau) WHERE TrangThai < 5` prevents double booking. On `SqlException` number 2601/2627 return a friendly Vietnamese error: "Khung giờ vừa được người khác đặt".
- Business codes (MaLichHen, MaPhieuKham, MaHoaDon, MaHoSo, PatientCode, MaDichVu) are NOT NULL UNIQUE. Generate them as: insert with a temporary unique value, SaveChanges to get the Id, set final code = prefix + Id padded to 6 digits (LH, PK, HD, HS, BN), SaveChanges, all inside one transaction.
- Scaffold ONCE with `dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --project PhongKham.Data --startup-project PhongKham.Web --output-dir Entities --context-dir Context --context ApplicationDbContext --no-onconfiguring`. After that the generated code is hand-maintained: never re-scaffold. Put extra mapping in a partial class `ApplicationDbContext.Custom.cs` (`OnModelCreatingPartial`).

## 5. Enums (in PhongKham.Common; map TINYINT columns with `HasConversion<byte>()` and change the entity property types)

- `TrangThaiLichHen`: ChoXacNhan=0, DaXacNhan=1, DaCheckIn=2, DangKham=3, HoanThanh=4, DaHuy=5, VangMat=6
- `HinhThucDat`: Online=0, TrucTiep=1
- `GioiTinh`: Nam=0, Nu=1, Khac=2
- `TrangThaiPhieuKham`: DangKham=0, HoanThanh=1
- `LoaiDichVu`: Kham=0, XetNghiem=1, ChanDoanHinhAnh=2, ThuThuat=3
- `TrangThaiChiDinh`: ChoThucHien=0, DaCoKetQua=1
- `TrangThaiHoaDon`: ChuaThanhToan=0, DaThanhToan=1, DaHuy=2
- `PhuongThucThanhToan`: TienMat=0, ChuyenKhoan=1, The=2, ViDienTu=3
- `LoaiMucHoaDon`: PhiKham=0, Thuoc=1, DichVu=2
- `LoaiThongBao`: HeThong=0, NhacLich=1, XacNhan=2, Huy=3
- Constants (class `QuyDinh`): `GioHuyToiThieu = 2` (hours before the appointment).

## 6. Coding conventions

- C# nullable enabled. File-scoped namespaces. One public type per file. No dead code, no TODO without a reason.
- Services return `Result` / `Result<T>` (`Success`, `Error`, `Data`); never throw for expected business failures.
- Wrap multi-table writes in one transaction through `IUnitOfWork` (`BeginTransactionAsync`, `CommitAsync`, `RollbackAsync`).
- Every POST action: `[HttpPost]` + `[ValidateAntiForgeryToken]` (or global `AutoValidateAntiforgeryToken`). Use tag helpers (`asp-action`, `asp-for`). Never `Html.Raw` on user data.
- Authorization at two levels: controller `[Authorize(Roles=...)]` AND ownership checks inside services (a patient sees only their own appointments, records and invoices; a doctor edits only their own PhieuKham).
- No hard delete of medical or financial data. Deactivate with `IsActive` / `ConHieuLuc` or a status.
- List pages: paging 10 rows per page, search box and filters as in the spec. Use a small reusable `PagedList<T>` helper.
- Formats: date `dd/MM/yyyy`, time `HH:mm`, money `N0` + " đ", culture `vi-VN`.
- Feedback: success/error messages through `TempData` rendered as a toast (top right); form errors under each field; destructive actions (hủy, từ chối, vô hiệu hóa) need a confirm modal.

## 7. UI standards

- Public and patient pages: top navbar + footer. Staff areas (Admin, LeTan, BacSi): left sidebar (dark #0F2A43) + top bar with bell icon and user name.
- Colors: primary #0A6EBD, secondary #20A4A4, background #F5F7FA, text #212529, success #198754, warning #FFC107, danger #DC3545. Radius 8px, buttons/inputs 44px tall, base font 16px.
- Button types: Primary (save/confirm), Secondary (back/close), Danger (cancel/reject/delete), Success (check-in/complete exam).
- Appointment status badges: ChoXacNhan yellow, DaXacNhan blue #0A6EBD, DaCheckIn teal #20A4A4, DangKham purple #6F42C1, HoanThanh green, DaHuy red, VangMat grey #6C757D (pill, 12px).
- Responsive: mobile first, test at 390px and 1440px. Time slots are touch-friendly buttons.

## 8. Definition of done for every task

- Builds with 0 errors and 0 new warnings; the feature works through the UI for the relevant role.
- Business rules from `docs/SPEC.md` are enforced in the service layer, not only in the UI.
- No secrets in the repo; `.gitignore` covers bin/obj/.vs/user files.
- The final message lists files changed, manual test steps and known gaps.
