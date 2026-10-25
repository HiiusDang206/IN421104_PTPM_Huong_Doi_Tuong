using EduHub.Ch02;
using EduHub.Ch02.Domain;

namespace Ch02.Refactor.Tests;

// Mẫu AAA: Arrange (chuẩn bị) - Act (thực hiện) - Assert (kiểm tra).
// Mỗi test chỉ kiểm tra MỘT hành vi. Đặt tên: TênMethod_TìnhHuống_KếtQuảMongĐợi.
public class DomainTests
{
    // Test mẫu đã viết sẵn: chạy được ngay trên ClubManager hiện tại.
    [Fact]
    public void CreateClub_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var manager = new ClubManager();

        // Act
        var act = () => manager.CreateClub("", "mô tả", "p@gmail.com");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    // TODO (Nhiệm vụ 4): bỏ Skip và viết thân test cho từng trường hợp dưới đây.
    // Cần ≥ 6 test AAA cho lớp Domain (Club, Event), trong đó có cả trường hợp lỗi.

    [Fact] //"TODO: viết test Club.Create với tên hợp lệ"
    public void Club_Create_WithValidName_ReturnsClub()
    {
        // Arrange
        var name = "Edu Hud";
        var description = "CLB Công nghệ";

        // Act
        var club = Club.Create(name, description);

        // Assert
        Assert.NotNull(club);
        Assert.Equal(name, club.Name);
        Assert.NotEqual(Guid.Empty, club.Id);
    }

    [Fact] //TODO: viết test Club.Create với tên rỗng hoặc dài hơn 100 ký tự
    public void Club_Create_WithInvalidName_ThrowsArgumentException()
    {
        // Arrange
        var emptyName = "";

        // Act
        var act = () => Club.Create(emptyName, "Mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]//TODO: viết test Club.HasRoom ở biên 49 và 50 thành viên
    public void Club_HasRoom_AtMaxMembers_ReturnsFalse()
    {
        // Arrange
        var club = Club.Create("EduHud", "Mô tả");
        // Act
        var roomAt49 = club.HasRoom(49);
        var roomAt50 = club.HasRoom(50);

        // Assert
        Assert.True(roomAt49);
        Assert.False(roomAt50);
    }

    [Fact] //TODO: viết test Event.Register thành viên mới
    public void Event_Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = new Event { Title = "Workshop C#", ClubId = Guid.NewGuid(), Date = DateTime.Now};
        var memberId = Guid.NewGuid();

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.True(result);
    }

    [Fact]//TODO: viết test Event.Register cùng một thành viên hai lần
    public void Event_Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = new Event { Title = "Workshop C#", ClubId = Guid.NewGuid(), Date = DateTime.Now };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.False(result);
    }

    [Fact] //TODO: viết test Event.CheckIn khi chưa đăng ký
    public void Event_CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = new Event {Title = "Workshop C#", ClubId = Guid.NewGuid(), Date = DateTime.Now };
        var unregisteredMemberId = Guid.NewGuid();

        // Act
        var act = () => ev.CheckIn(unregisteredMemberId);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}
