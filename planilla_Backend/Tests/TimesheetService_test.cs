using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Services;
using Planilla_Backend.CleanArchitecture.Application.UseCases;

namespace Tests
{
  public class TimesheetService_tests
  {
    private TimesheetService _sut;
    private Mock<ITimesheetRepository> _repoMock;

    [SetUp]
    public void Setup()
    {
      _repoMock = new Mock<ITimesheetRepository>(MockBehavior.Strict);
      _sut = new TimesheetService(_repoMock.Object);
    }

    [Test]
    public void InsertWeekAsync_WhenEmployeeIdIsInvalid_Throws()
    {
      Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        await _sut.InsertWeekAsync(0, DateTime.Today, new List<DayEntryDto> { new DayEntryDto { Date = DateTime.Today, Hours = 1 } }));
    }

    [Test]
    public void InsertWeekAsync_WhenEntriesNull_Throws()
    {
      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, DateTime.Today, null!));
    }

    [Test]
    public void InsertWeekAsync_WhenEntriesEmpty_Throws()
    {
      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, DateTime.Today, Array.Empty<DayEntryDto>()));
    }

    [Test]
    public void InsertWeekAsync_WhenEntryIsNull_Throws()
    {
      // Arrange
      var today = new DateTime(2025, 10, 29);
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      var list = new List<DayEntryDto?> { null };

      // Act + Assert
      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, today, list!));
    }

    [Test]
    public void InsertWeekAsync_WhenDateOutOfWeek_Throws()
    {
      var wednesday = new DateTime(2025, 10, 29); // miércoles
      var monday = new DateTime(2025, 10, 27);    // lunes
      var friday = monday.AddDays(4);             // viernes

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      var saturday = new DateTime(2025, 11, 1);   // fuera de la semana

      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, wednesday, new List<DayEntryDto> {new DayEntryDto { Date = saturday, Hours = 2 }}));
    }

    [Test]
    public void InsertWeekAsync_WhenDateIsFuture_Throws()
    {
      var today = DateTime.Today;
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      var futureDay = today.AddDays(1); // mañana

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, today, new List<DayEntryDto> {new DayEntryDto { Date = futureDay, Hours = 2 }}));
    }

    [Test]
    public void InsertWeekAsync_WhenExistingDateInDb_Throws()
    {
      var today = new DateTime(2025, 10, 30); // jueves
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      var existing = new List<DayEntryDto>
      {
        new DayEntryDto { Date = monday.AddDays(2), Hours = 4 } // miércoles
      };

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(existing);

      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, today, new List<DayEntryDto> {new DayEntryDto { Date = monday.AddDays(2), Hours = 2 }}));
    }

    [Test]
    public void InsertWeekAsync_WhenDuplicateDatesInPayload_Throws()
    {
      var today = new DateTime(2025, 10, 30);
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      var d = monday.AddDays(1); // martes

      Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.InsertWeekAsync(1, today, new List<DayEntryDto> {new DayEntryDto { Date = d, Hours = 2 }, new DayEntryDto { Date = d, Hours = 3 }})); //duplicado
    }

    [Test]
    public void InsertWeekAsync_WhenHoursOutOfRange_Throws()
    {
      var today = DateTime.Today;
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        await _sut.InsertWeekAsync(1, today, new List<DayEntryDto>{new DayEntryDto { Date = monday, Hours = 10 }}));
    }

    [Test]
    public async Task InsertWeekAsync_WhenValid_CallsSaveWithNormalizedEntries()
    {
      var today = new DateTime(2025, 10, 29); // miércoles
      var monday = WeekStartMonday(today);
      var friday = monday.AddDays(4);

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(1, monday, friday, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<DayEntryDto>());

      IEnumerable<DayEntryDto> capturedEntries = null!;
      CancellationToken capturedCt = default;

      _repoMock
        .Setup(r => r.SaveWeekAsync(1, monday, It.IsAny<IEnumerable<DayEntryDto>>(), It.IsAny<CancellationToken>()))
        .Callback<int, DateTime, IEnumerable<DayEntryDto>, CancellationToken>((emp, start, entries, ct) =>
        {
          capturedEntries = entries;
          capturedCt = ct;
        })
        .Returns(Task.CompletedTask);

      var payload = new List<DayEntryDto>
      {
        new DayEntryDto { Date = monday.AddDays(3).AddHours(5), Hours = 3, Description = "  hola  " },
        new DayEntryDto { Date = monday.AddDays(1), Hours = 2, Description = null }
      };

      await _sut.InsertWeekAsync(1, today, payload);

      _repoMock.VerifyAll();
      Assert.NotNull(capturedEntries);

      var list = capturedEntries.ToList();
      // deben venir ordenados
      Assert.That(list[0].Date, Is.EqualTo(monday.AddDays(1)));
      Assert.That(list[1].Date, Is.EqualTo(monday.AddDays(3)));
      // descripción trim
      Assert.That(list[1].Description, Is.EqualTo("hola"));
    }

    [Test]
    public async Task GetWeekHoursAsync_ReturnsQueryWithRepoData()
    {
      var start = new DateTime(2025, 10, 27);
      var end = start.AddDays(4);

      var repoData = (IReadOnlyList<DayEntryDto>)new List<DayEntryDto> {new DayEntryDto { Date = start, Hours = 2 }, new DayEntryDto { Date = start.AddDays(1), Hours = 3 }};

      _repoMock
        .Setup(r => r.GetWeekHoursAsync(5, start, end, It.IsAny<CancellationToken>()))
        .ReturnsAsync(repoData);

      var result = await _sut.GetWeekHoursAsync(5, start, end);

      Assert.That(result.WeekStart, Is.EqualTo(start));
      Assert.That(result.WeekEnd, Is.EqualTo(end));
      Assert.That(result.Entries.Count, Is.EqualTo(2));
      Assert.That(result.Entries[0].Hours, Is.EqualTo(2));
      Assert.That(result.Entries[1].Hours, Is.EqualTo(3));

      _repoMock.VerifyAll();
    }

    private static DateTime WeekStartMonday(DateTime d)
    {
      int delta = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
      return d.AddDays(-delta).Date;
    }
  }
}
