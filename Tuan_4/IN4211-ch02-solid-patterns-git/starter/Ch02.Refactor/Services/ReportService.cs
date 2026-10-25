using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ReportService : IReportService
{
    private readonly IClubRepository _clubs;
    private readonly IMemberRepository _members;
    private readonly IEventRepository _events;

    public ReportService(IClubRepository clubs, IMemberRepository members, IEventRepository events)
    {
        _clubs = clubs;
        _members = members;
        _events = events;
    }

    public string Generate(IReportStrategy strategy)
    {
        // 1. Gom số liệu thành List<ClubStat>
        var stats = _clubs.GetAll().Select(c =>
        {
            var events = _events.GetByClub(c.Id);
            return new ClubStat(
                c.Name,
                _members.GetByClub(c.Id).Count,
                events.Count,
                events.Sum(e => e.RegisteredMemberIds.Count),
                events.Sum(e => e.CheckedInMemberIds.Count));
        }).ToList();

        // 2. Giao cho Strategy "vẽ" ra định dạng tương ứng (Console hoặc CSV)
        return strategy.Render(stats);
    }
}