using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class ConsoleReportStrategy : IReportStrategy
{
    public string Name => "Console";

    public string Render(IReadOnlyList<ClubStat> stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== CLUB REPORT (CONSOLE) ===");
        foreach (var s in stats)
        {
            sb.AppendLine($"CLB: {s.ClubName,-15} | Thành viên: {s.MemberCount,2} | Sự kiện: {s.EventCount,2} | Đăng ký: {s.TotalRegistrations,2} | Điểm danh: {s.TotalCheckIns,2}");
        }
        return sb.ToString();
    }
}