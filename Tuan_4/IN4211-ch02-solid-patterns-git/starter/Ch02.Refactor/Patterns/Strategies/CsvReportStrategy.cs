using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class CsvReportStrategy : IReportStrategy
{
    public string Name => "CSV";

    public string Render(IReadOnlyList<ClubStat> stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ClubName,MemberCount,EventCount,TotalRegistrations,TotalCheckIns");
        foreach (var s in stats)
        {
            sb.AppendLine($"{s.ClubName},{s.MemberCount},{s.EventCount},{s.TotalRegistrations},{s.TotalCheckIns}");
        }
        return sb.ToString();
    }
}