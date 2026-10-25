using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

// ═══════════════════════════════════════════════════════════════
//  EduHub Ch02 — Refactoring với SOLID và Design Patterns
//  ClubManager.cs chứa code vi phạm SOLID (đọc các TODO ở đầu file đó).
// ═══════════════════════════════════════════════════════════════

// Demo code cũ (vi phạm SOLID) — chạy được nhưng thiết kế rất xấu:

IAppLogger logger = new ConsoleLogger();

INotificationServiceFactory notificationFactory = new NotificationServiceFactory();
INotificationService notifier = notificationFactory.Create(NotificationChannel.Email);

IClubRepository clubRepo = new LoggingClubRepository(new InMemoryClubRepository(), logger);
IMemberRepository memberRepo = new InMemoryMemberRepository();
IEventRepository eventRepo = new InMemoryEventRepository();

IClubService clubService = new ClubService(clubRepo, memberRepo, notifier, logger);
IEventService eventService = new EventService(clubRepo, memberRepo, eventRepo);
IReportService reportService = new ReportService(clubRepo, memberRepo, eventRepo);

Console.WriteLine("== 1. Tạo CLB và thêm thành viên ==");
var club = clubService.CreateClub("Tech Club", "Câu lạc bộ công nghệ", "president@gmail.com");
var a = clubService.AddMember(club.Id, "Nguyễn Văn A", "a@student.dthu.edu.vn");
var b = clubService.AddMember(club.Id, "Trần Thị B", "b@student.dthu.edu.vn");

Console.WriteLine("\n== 2. Observer: tạo sự kiện -> 2 observer được báo ==");
eventService.Subscribe(new LogNotifier(logger));
eventService.Subscribe(new EmailNotifier(memberRepo, notifier));
var ev = eventService.CreateEvent(club.Id, "Workshop SOLID", new DateTime(2026, 11, 20));
eventService.Register(ev.Id, a.Id);
eventService.Register(ev.Id, b.Id);
eventService.CheckIn(ev.Id, a.Id);

Console.WriteLine("\n== 3. Strategy: báo cáo Console ==");
IReportStrategy console = new ConsoleReportStrategy();
Console.Write(reportService.Generate(console));
Console.WriteLine("== 3. Strategy: báo cáo CSV ==");
IReportStrategy csv = new CsvReportStrategy();
Console.Write(reportService.Generate(csv));

Console.WriteLine("\n== 4. Factory: tạo kênh thông báo theo yêu cầu ==");
foreach (var channel in Enum.GetValues<NotificationChannel>())
{
    notificationFactory.Create(channel).Send("a@student.dthu.edu.vn", $"Thử kênh {channel}");
}

Console.WriteLine("\n== 5. Kiểm tra validate ==");
try
{
    clubService.CreateClub("", "x", "p@gmail.com");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  Lỗi đúng dự kiến: {ex.Message}");
}