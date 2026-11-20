# SPEC: business specification for PhongKham

Companion to `AGENTS.md`. Read only the sections a task refers to.

## 1. Actors, areas and routes

| Actor | Area / entry | Main routes |
|---|---|---|
| Khách (guest) | public | `/`, `/chuyen-khoa`, `/bac-si`, `/bac-si/{id}`, `/bang-gia`, `/Account/Login`, `/Account/Register`, `/Account/AccessDenied` |
| Bệnh nhân (Patient) | `BenhNhan` | `LichHen` (Index, DatLich, ChiTiet, Huy), `HoSo`, `HoaDon`, `ThongBao`, `TaiKhoan` |
| Lễ tân (Receptionist) | `LeTan` | `LichHen`, `TiepDon`, `BenhNhan`, `ThanhToan`, `ThongBao` |
| Bác sĩ (Doctor) | `BacSi` | `LichLamViec`, `DanhSachCho`, `PhieuKham`, `BenhNhan` (history), `ThongBao` |
| Quản trị (Admin) | `Admin` | `TaiKhoan`, `ChuyenKhoa`, `BacSi`, `NhanVien`, `CaLamViec`, `LichLamViec`, `Thuoc`, `NhomThuoc`, `DichVu`, `DanhGia`, `ThongKe` |

After login redirect by role: Patient -> `/`, Receptionist -> `/LeTan/LichHen`, Doctor -> `/BacSi/DanhSachCho`, Admin -> `/Admin/ThongKe`.

## 2. Appointment lifecycle (LichHen.TrangThai)

```
ChoXacNhan -> DaXacNhan -> DaCheckIn -> DangKham -> HoanThanh
   |              |            |
   +--------------+------------+--> DaHuy
                  +--> VangMat
```

Allowed transitions only:
- ChoXacNhan -> DaXacNhan | DaHuy
- DaXacNhan -> DaCheckIn | DaHuy | VangMat
- DaCheckIn -> DangKham | DaHuy
- DangKham -> HoanThanh
- HoanThanh, DaHuy, VangMat are final.

Every transition: update LichHen, insert `LichSuLichHen` (TrangThaiCu, TrangThaiMoi, NguoiThayDoiId, GhiChu), create `ThongBao` for the affected users, in ONE transaction. Method `LichHen.ChuyenTrangThai(moi)` validates the transition.

## 3. Business rules

### Slots and booking (UC01)
- Slot generation for (doctorId, date): take active `LichLamViec` (ConHieuLuc = 1, NgayLam = date) of the doctor; for each, split `CaLamViec.GioBatDau..GioKetThuc` into slots of `ThoiLuongKhungGio` minutes.
- Remove slots that already have a LichHen with TrangThai < 5 for the same doctor/date; remove past slots when the date is today; stop offering slots when the shift already has `SoBenhNhanToiDa` active appointments.
- Only logged-in patients book. Booking for a date in the past is rejected. A patient cannot hold two active appointments at the same date and start time.
- Validation order: slot belongs to an active LichLamViec -> slot free -> patient has no clash -> insert (inside a transaction). A unique-index violation means somebody booked first: return "Khung giờ vừa được người khác đặt".
- New LichHen: TrangThai = ChoXacNhan, HinhThucDat = Online, NguoiTaoId = current user, MaLichHen = LH + Id (see AGENTS.md section 4). Notify the patient and all receptionists.

### Cancel / reschedule (UC02)
- Patient may cancel only own appointments in ChoXacNhan or DaXacNhan and only if `NgayHen + GioBatDau - now >= QuyDinh.GioHuyToiThieu` hours. LyDoHuy required. Result: DaHuy (slot becomes free again). Notify receptionists.
- Reschedule = cancel the old appointment and book the new one in one transaction; if the new slot is taken, keep the old appointment.

### Receptionist (UC03, UC04)
- Confirm: ChoXacNhan -> DaXacNhan. Reject: ChoXacNhan -> DaHuy with required reason. If the appointment was already handled, show "Lịch hẹn đã được xử lý" and reload.
- Check-in: only DaXacNhan and NgayHen = today. Set TrangThai = DaCheckIn and `SoThuTu` = (max SoThuTu of the same doctor and date) + 1. Show the queue number.
- Mark absent (VangMat) allowed for DaXacNhan appointments whose start time has passed.
- Walk-in: search patient by name, phone, PatientCode or IdentityNumber; if none, create Patient (UserId = null) + HoSoBenhAn in one transaction; create LichHen with HinhThucDat = TrucTiep and TrangThai = DaXacNhan; may go straight to check-in.

### Examination (UC05, UC06)
- Doctor sees only own appointments (DoctorId) with TrangThai = DaCheckIn, ordered by SoThuTu.
- Start exam: LichHen -> DangKham; create PhieuKham (HoSoBenhAnId of the patient, LichHenId, DoctorId, TrangThai = DangKham, MaPhieuKham = PK + Id). If a PhieuKham already exists for the LichHen, reopen it instead of creating another.
- Doctor sees the patient's HoSoBenhAn (NhomMau, DiUng, TienSuBenh, BenhManTinh) and previous PhieuKham list.
- Save draft: vital signs ranges: NhietDo 30-45, Mach 20-250, HuyetApTamThu 40-300, HuyetApTamTruong 20-200, CanNang 0.5-500, ChieuCao 20-250; all optional.
- Prescription: one DonThuoc per PhieuKham (create on first save, then edit lines). Each line: Medicine, SoLuong > 0, LieuDung, CachDung, SoNgayDung, DonGia = Medicine.Price at that moment. Reject duplicates of the same medicine; reject SoLuong > Medicines.StockQuantity with a warning that names the medicine and the remaining stock.
- Service orders: `ChiDinhDichVu` lines with DonGia = DichVu.DonGia; the doctor can enter KetQua text and an optional result file path; status ChoThucHien -> DaCoKetQua and NgayCoKetQua set.
- Complete exam (single transaction): require ChanDoan not empty; re-check stock; decrement `Medicines.StockQuantity`; PhieuKham -> HoanThanh (locked afterwards: no more edits); LichHen -> HoanThanh; create HoaDon (ChuaThanhToan) with lines: PhiKham = Doctors.ConsultationFee, one Thuoc line per prescription line (SoLuong x DonGia), one DichVu line per service order; TongTien = sum of line amounts, GiamGia = 0; MaHoaDon = HD + Id; notify the patient.

### Billing (UC07)
- Receptionist lists HoaDon ChuaThanhToan, opens details, may set GiamGia (0 <= GiamGia <= TongTien) and PhuongThucThanhToan (required), confirms payment: TrangThai = DaThanhToan, NgayThanhToan = now, NguoiLapId = current Staff.Id. A paid or cancelled invoice cannot be paid again. Cancelling an unpaid invoice (confirm modal) sets DaHuy; the table has no reason column, so do not add one. Provide a printable invoice page (print stylesheet).

### Patient data, reviews, notifications
- Patient pages show only own data: appointments, HoSoBenhAn (read-only), PhieuKham history with prescription and service results, invoices.
- DanhGia: only for own LichHen in HoanThanh, once per LichHen (unique), SoSao 1-5, NhanXet optional. Doctor page shows average stars and count (HienThi = 1 only). Admin can hide (HienThi = 0).
- ThongBao: created on booking, confirm, reject, cancel, check-in, exam completed, service result available. Bell icon shows the unread count; list page marks as read. Email sending is OUT of scope.

### Accounts
- Register (public): create Users (role Patient) + Patients + HoSoBenhAn in one transaction. Email unique. Password min 8 chars with letter and digit. Hash with `PasswordHasher<User>`.
- Admin creates Doctors and Staffs: create Users (role Doctor/Receptionist) + profile row in one transaction; can lock/unlock (IsActive) and reset password. Locked users cannot log in. Update `LastLoginAt` on login. Lockout: after 5 failed logins in 10 minutes show a temporary message (in-memory counter is enough).

### Admin schedule (UC08)
- Create LichLamViec (DoctorId, CaLamViecId, NgayLam >= today, SoBenhNhanToiDa). Unique (DoctorId, NgayLam, CaLamViecId): show a friendly message on duplicates. Support bulk creation for a week.
- Deactivating a LichLamViec (ConHieuLuc = 0) that has active appointments (ChoXacNhan, DaXacNhan or DaCheckIn) first shows the affected appointments and needs confirmation; after confirming, notify receptionists so they can handle them.

### Statistics (Admin dashboard)
- Cards: visits today, revenue this month (paid invoices), pending appointments, cancel rate (DaHuy / all in period).
- Charts: visits per day (line), revenue per month (bar), share by specialty (doughnut). Table: top 5 doctors by visits and average rating. Date range filter (default last 30 days). Export to CSV is optional.

## 4. Service contracts (from the thesis design; keep these names)

- `ILichHenService`: `LayKhungGioTrongAsync(doctorId, ngay)`, `DatLichAsync(dto, userId)`, `HuyLichAsync(lichHenId, userId, lyDo)`, `DoiLichAsync(...)`, `XacNhanAsync(lichHenId, userId)`, `TuChoiAsync(lichHenId, userId, lyDo)`, `CheckInAsync(lichHenId, userId)` (returns queue number), `DanhDauVangMatAsync(...)`, `LayLichCuaBenhNhanAsync(patientId)`, `TimKiemAsync(filter, page)`.
- `IPhieuKhamService`: `LayLichDaCheckInAsync(doctorId, ngay)`, `BatDauKhamAsync(lichHenId, doctorId)`, `CapNhatPhieuKhamAsync(phieuKhamId, dto)`, `HoanTatPhieuKhamAsync(phieuKhamId, doctorId)`, plus prescription and service-order methods.
- `IHoaDonService`: `TaoHoaDonTuPhieuKhamAsync(phieuKhamId)`, `LayChiTietAsync(hoaDonId)`, `LayDanhSachChuaThanhToanAsync()`, `ApDungGiamGiaAsync(hoaDonId, giamGia)`, `ThanhToanAsync(hoaDonId, phuongThuc, staffId)`, `HuyHoaDonAsync(hoaDonId, lyDo)`.
- `IThongBaoService`: `TaoThongBaoAsync(userId, tieuDe, noiDung, loai, lichHenId?)`, `LayThongBaoAsync(userId)`, `DemChuaDocAsync(userId)`, `DanhDauDaDocAsync(thongBaoId, userId)`.
- Also: `IAuthService` (register, validate login, change password), `IDoctorService`, `IPatientService`, `IDanhMucService` (specialties, medicines, services, shifts), `ILichLamViecService`, `IThongKeService`, `IDanhGiaService`.
- Repositories: `IRepository<T>` (GetByIdAsync, GetAllAsync, AddAsync, Update, Remove, Query) + `ILichHenRepository` (ExistsAsync, CoTrungGioAsync, GetByDoctorAndNgayAsync, GetByPatientAsync, GetWithChiTietAsync), `IPhieuKhamRepository` (GetByLichHenIdAsync, GetLichSuKhamAsync, GetWithChiTietAsync), `IHoaDonRepository` (GetChuaThanhToanAsync, GetWithChiTietAsync, GetByPhieuKhamIdAsync). `IUnitOfWork` exposes them plus `SaveChangesAsync`, `BeginTransactionAsync`, `CommitAsync`, `RollbackAsync`.

## 5. Screens per area (Vietnamese titles)

- Public: Trang chủ (banner, quick search, specialties, featured doctors, 3-step guide), Chuyên khoa, Bác sĩ (filter by specialty, average rating), Chi tiết bác sĩ (+ reviews), Bảng giá, Đăng nhập, Đăng ký.
- BenhNhan: Đặt lịch khám (3 steps: specialty -> doctor + date + slots -> confirm with reason), Lịch hẹn của tôi (filter by status; cancel with reason), Chi tiết lịch hẹn, Hồ sơ sức khỏe (history list, prescription, service results), Hóa đơn, Thông báo, Thông tin cá nhân + đổi mật khẩu, Đánh giá bác sĩ.
- LeTan: Quản lý lịch hẹn (filters date/doctor/status/search; confirm/reject/check-in/absent; "Tạo lịch trực tiếp"), Tiếp đón (today's list), Bệnh nhân (search, create, edit), Thanh toán (unpaid list, detail, pay, print).
- BacSi: Lịch làm việc, Danh sách chờ hôm nay, Phiếu khám (tabs: Thông tin khám / Kê đơn thuốc / Chỉ định dịch vụ; right panel = patient summary + previous visits), Tra cứu bệnh nhân đã khám.
- Admin: CRUD pages for TaiKhoan, ChuyenKhoa, BacSi, NhanVien, CaLamViec, LichLamViec (week view + bulk create), NhomThuoc, Thuoc (stock), DichVu, DanhGia moderation, ThongKe dashboard.

## 6. Seed data (development only, in a `DbSeeder` run at startup when Users is empty)

- Admin user from user-secrets (`Seed:AdminEmail`, `Seed:AdminPassword`). Roles, CaLamViec, Specialties, MedicineCategories and DichVu are already seeded by the SQL script: do not insert them again.
- Demo data (only when `Seed:Demo = true`): 5 doctors (one per specialty), 2 receptionists, 15 patients with HoSoBenhAn, 20 medicines, schedules for the next 14 days, about 60 appointments in mixed statuses, some completed visits with prescriptions and invoices. All demo passwords come from user-secrets (`Seed:DemoPassword`).

## 7. Manual test checklist (per area)

- Guest: browse pages, register, login, wrong password message, locked account.
- Patient: book -> appears as ChoXacNhan; booking the same slot from another account fails; cancel inside the 2-hour window fails; cancel outside works; cannot open another patient's appointment by changing the id in the URL.
- Receptionist: confirm, reject with reason, check-in (queue numbers increase per doctor/day), walk-in registration, payment with discount, cannot pay twice.
- Doctor: sees only own queue; cannot open another doctor's PhieuKham; prescription rejects too-large quantity; completing without diagnosis fails; after completion the form is read-only; invoice is created.
- Admin: create doctor account and log in with it; duplicate schedule rejected; deactivating a schedule with appointments shows the affected list; dashboard numbers match the data.
