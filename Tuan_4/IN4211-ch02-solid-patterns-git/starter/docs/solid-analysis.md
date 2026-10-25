# Phân tích vi phạm SOLID — ClubManager.cs

## Vi phạm 1: SRP (Single Responsibility Principle)

**Vị trí:** Class `ClubManager` (dòng 28–117) — toàn bộ class.

**Mô tả:** `ClubManager` là "God class", đảm nhận quá nhiều trách nhiệm trong cùng một class:
- Quản lý CLB (tạo CLB, lấy danh sách)
- Quản lý thành viên (thêm thành viên)
- Quản lý sự kiện (tạo sự kiện)
- Ghi log (gọi `File.AppendAllText`)
- Gửi thông báo (in ra console)
- In báo cáo (`PrintClubReport`)

**Ví dụ cụ thể:** Method `CreateClub()` (dòng 35–64) làm 3 việc cùng lúc: validate tên CLB, lưu CLB, ghi log ra file, và gửi thông báo email.

**Hậu quả:**
- Khi cần thay đổi cách gửi thông báo → phải sửa `ClubManager` → có thể ảnh hưởng đến logic tạo CLB.
- Không thể tái sử dụng riêng phần gửi thông báo hay ghi log ở chỗ khác.
- Class ngày càng phình to, khó đọc, khó bảo trì.

---

## Vi phạm 2: OCP (Open/Closed Principle)

**Vị trí:** Method `CreateClub()`, dòng 52–63 (chuỗi if/else gửi thông báo).

**Mô tả:** Việc gửi thông báo sử dụng chuỗi `if/else` kiểm tra `@gmail.com`, `@yahoo.com` để quyết định cách gửi. Muốn thêm kênh thông báo mới (SMS, Slack, Outlook...) thì **phải mở method này ra sửa**, thêm nhánh `else if`.

**Ví dụ cụ thể:**
```csharp
// Dòng 52–63: muốn hỗ trợ @outlook.com phải thêm else if ở đây
if (presidentEmail.Contains("@gmail.com"))
    Console.WriteLine($"Sending Gmail notification to {presidentEmail}");
else if (presidentEmail.Contains("@yahoo.com"))
    Console.WriteLine($"Sending Yahoo notification to {presidentEmail}");
else
    Console.WriteLine($"Sending generic email to {presidentEmail}");
```

Tương tự, `AddMemberToClub()` (dòng 82–83) gửi cố định email + Slack, muốn bỏ Slack hay thêm kênh mới cũng phải sửa method.

**Hậu quả:**
- Vi phạm nguyên tắc "mở để mở rộng, đóng để sửa đổi".
- Mỗi lần thêm kênh thông báo mới phải sửa nhiều method, dễ gây lỗi regression.

---

## Vi phạm 3: DIP (Dependency Inversion Principle)

**Vị trí:** Dòng 48, 79, 94, 111 — tất cả các lời gọi `File.AppendAllText(...)`.

**Mô tả:** Class phụ thuộc trực tiếp vào `System.IO.File` (concrete class) để ghi log, không đi qua bất kỳ abstraction (interface) nào. Module cấp cao (`ClubManager` — nghiệp vụ) phụ thuộc trực tiếp vào module cấp thấp (`File` — hạ tầng I/O).

**Ví dụ cụ thể:**
```csharp
// Dòng 48: ghi file trực tiếp trong logic nghiệp vụ
File.AppendAllText(_logFilePath, $"[{DateTime.Now}] Club created: {name}\n");

// Dòng 79: lặp lại ở AddMemberToClub
File.AppendAllText(_logFilePath, $"[{DateTime.Now}] Member {memberName} joined {club.Name}\n");
```

**Hậu quả:**
- Không thể thay đổi cách ghi log (ví dụ: log ra database, log ra cloud) mà không sửa code nghiệp vụ.
- Không thể viết unit test cho `ClubManager` vì mỗi lần test đều tạo file thật trên đĩa.
- Ghép chặt (tight coupling) giữa tầng nghiệp vụ và tầng hạ tầng.

---

## Vi phạm 4: SRP + OCP — Method `CreateEvent()` tự gửi thông báo

**Vị trí:** Method `CreateEvent()`, dòng 88–98.

**Mô tả:** Method này vừa tạo sự kiện, vừa tự duyệt danh sách thành viên và gửi thông báo email cho từng người. Đây là 2 trách nhiệm khác nhau (SRP), và nếu muốn thêm cách thông báo khác (SMS, push notification) phải sửa method này (OCP).

**Ví dụ cụ thể:**
```csharp
// Dòng 96–97: CreateEvent không nên biết cách thông báo cho thành viên
foreach (var m in _members.Where(m => m.ClubId == clubId))
    Console.WriteLine($"Email to {m.Email}: new event '{title}' at {club.Name}");
```

**Hậu quả:**
- `CreateEvent` bị buộc chặt với cơ chế thông báo cụ thể.
- Không thể thêm/bớt kênh thông báo linh hoạt (ví dụ: thêm LogNotifier mà không sửa CreateEvent).
- Khó test riêng logic tạo sự kiện vì lẫn với logic gửi thông báo.

---

## Vi phạm 5: ISP (Interface Segregation Principle) + DIP

**Vị trí:** Dòng 30–31 — `List<Club> _clubs` và `List<Member> _members` nằm ngay trong class.

**Mô tả:** `ClubManager` tự quản lý dữ liệu bằng `List<>` nội bộ thay vì sử dụng các repository interface nhỏ gọn (`IClubRepository`, `IMemberRepository`). Mọi method truy cập dữ liệu trực tiếp qua `_clubs` và `_members`, không qua abstraction.

**Ví dụ cụ thể:**
```csharp
// Dòng 30–31: dữ liệu giữ ngay trong class
private readonly List<Club> _clubs = new();
private readonly List<Member> _members = new();

// Dòng 69–70: truy vấn trực tiếp trên List
var club = _clubs.FirstOrDefault(c => c.Id == clubId)
    ?? throw new InvalidOperationException("Club not found");
```

**Hậu quả:**
- Không thể thay đổi cách lưu trữ (ví dụ: chuyển sang database, file JSON) mà không sửa toàn bộ class.
- Không thể bọc thêm hành vi (ví dụ: Decorator ghi log khi truy cập repository).
- Một class biết quá nhiều chi tiết lưu trữ, vi phạm ISP vì client buộc phải phụ thuộc vào cả class lớn thay vì interface nhỏ chuyên biệt.

---

## Vi phạm 6: SRP + OCP — Method `PrintClubReport()`

**Vị trí:** Method `PrintClubReport()`, dòng 103–113.

**Mô tả:** Method vừa thống kê dữ liệu, vừa định dạng output (Console), vừa ghi log. Nếu muốn xuất báo cáo dạng CSV hoặc PDF, phải sửa trực tiếp method này hoặc viết thêm method mới trùng logic.

**Ví dụ cụ thể:**
```csharp
// Dòng 108: format cố định, muốn CSV phải sửa
Console.WriteLine($"Club: {club.Name} | Members: {memberCount}");

// Dòng 111: ghi log lẫn trong báo cáo
File.AppendAllText(_logFilePath, $"Report generated for {club.Name}\n");
```

**Hậu quả:**
- Không thể mở rộng thêm định dạng báo cáo mới mà không sửa code cũ.
- Logic thống kê bị trộn với logic hiển thị, không tái sử dụng được.