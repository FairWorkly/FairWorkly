using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace FairWorkly.UnitTests.Unit;

/// <summary>
/// Unit tests for EmployeeSyncService
/// Tests employee synchronization (Upsert) functionality
/// </summary>
public class EmployeeSyncServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly EmployeeSyncService _employeeSyncService;
    private readonly Guid _testOrganizationId = Guid.NewGuid();
    private readonly DateTimeOffset _testDateTime = new DateTimeOffset(2025, 12, 28, 0, 0, 0, TimeSpan.Zero);

    public EmployeeSyncServiceTests()
    {
        _mockEmployeeRepository = new Mock<IEmployeeRepository>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(_testDateTime);

        _employeeSyncService = new EmployeeSyncService(
            _mockEmployeeRepository.Object,
            _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task SyncEmployeesAsync_NewEmployees_CreatesEmployees()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            new PayrollCsvRow
            {
                EmployeeId = "NEW001",
                EmployeeName = "Alice Johnson",
                AwardType = "Retail",
                Classification = "Level 1",
                EmploymentType = "FullTime",
                HourlyRate = 26.55m,
                OrdinaryHours = 38.00m,
                OrdinaryPay = 1008.90m,
                GrossPay = 1008.90m,
                SuperannuationPaid = 121.07m
            },
            new PayrollCsvRow
            {
                EmployeeId = "NEW002",
                EmployeeName = "Bob Williams",
                AwardType = "Retail",
                Classification = "Level 2",
                EmploymentType = "PartTime",
                HourlyRate = 27.16m,
                OrdinaryHours = 20.00m,
                OrdinaryPay = 543.20m,
                GrossPay = 543.20m,
                SuperannuationPaid = 65.18m
            }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(
                _testOrganizationId,
                It.IsAny<List<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>()); // No existing employees

        var createdEmployees = new List<Employee>();
        _mockEmployeeRepository
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((emp, ct) =>
            {
                emp.Id = Guid.NewGuid(); // Simulate DB generating Id
                createdEmployees.Add(emp);
            })
            .ReturnsAsync((Employee emp, CancellationToken ct) => emp);

        // Act
        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainKey("NEW001");
        result.Should().ContainKey("NEW002");

        _mockEmployeeRepository.Verify(
            x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        // Verify employee data
        var alice = createdEmployees.First(e => e.EmployeeNumber == "NEW001");
        alice.FirstName.Should().Be("Alice");
        alice.LastName.Should().Be("Johnson");
        alice.AwardType.Should().Be(AwardType.GeneralRetailIndustryAward2020);
        alice.AwardLevelNumber.Should().Be(1);
        alice.EmploymentType.Should().Be(EmploymentType.FullTime);
        alice.OrganizationId.Should().Be(_testOrganizationId);
        alice.IsActive.Should().BeTrue();

        var bob = createdEmployees.First(e => e.EmployeeNumber == "NEW002");
        bob.FirstName.Should().Be("Bob");
        bob.LastName.Should().Be("Williams");
        bob.AwardLevelNumber.Should().Be(2);
        bob.EmploymentType.Should().Be(EmploymentType.PartTime);
    }

    [Fact]
    public async Task SyncEmployeesAsync_ExistingEmployees_UpdatesEmployees()
    {
        // Arrange
        var existingEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrganizationId,
            EmployeeNumber = "EMP001",
            FirstName = "Old",
            LastName = "Name",
            Email = "emp001@placeholder.local",
            JobTitle = "Employee",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.UtcNow,
            IsActive = true
        };

        var rows = new List<PayrollCsvRow>
        {
            new PayrollCsvRow
            {
                EmployeeId = "EMP001",
                EmployeeName = "Updated Name",
                AwardType = "Retail",
                Classification = "Level 2",
                EmploymentType = "Casual",
                HourlyRate = 34.48m,
                OrdinaryHours = 25.00m,
                OrdinaryPay = 862.00m,
                GrossPay = 862.00m,
                SuperannuationPaid = 103.44m
            }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(
                _testOrganizationId,
                It.IsAny<List<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { existingEmployee });

        Employee? updatedEmployee = null;
        _mockEmployeeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((emp, ct) => updatedEmployee = emp)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainKey("EMP001");
        result["EMP001"].Should().Be(existingEmployee.Id);

        _mockEmployeeRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockEmployeeRepository.Verify(
            x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);

        // Verify updated fields
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("Updated");
        updatedEmployee.LastName.Should().Be("Name");
        updatedEmployee.AwardLevelNumber.Should().Be(2);
        updatedEmployee.EmploymentType.Should().Be(EmploymentType.Casual);
    }

    [Fact]
    public async Task SyncEmployeesAsync_MixedScenario_CreatesAndUpdates()
    {
        // Arrange
        var existingEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrganizationId,
            EmployeeNumber = "MIX001",
            FirstName = "Existing",
            LastName = "Employee",
            Email = "mix001@placeholder.local",
            JobTitle = "Employee",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.UtcNow,
            IsActive = true
        };

        var rows = new List<PayrollCsvRow>
        {
            new PayrollCsvRow
            {
                EmployeeId = "MIX001", // Existing
                EmployeeName = "Existing Employee Updated",
                AwardType = "Retail",
                Classification = "Level 2",
                EmploymentType = "FullTime"
            },
            new PayrollCsvRow
            {
                EmployeeId = "MIX002", // New
                EmployeeName = "New Employee",
                AwardType = "Retail",
                Classification = "Level 1",
                EmploymentType = "Casual"
            }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(
                _testOrganizationId,
                It.IsAny<List<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { existingEmployee });

        _mockEmployeeRepository
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee emp, CancellationToken ct) =>
            {
                emp.Id = Guid.NewGuid();
                return emp;
            });

        _mockEmployeeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainKey("MIX001");
        result.Should().ContainKey("MIX002");

        _mockEmployeeRepository.Verify(
            x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockEmployeeRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SyncEmployeesAsync_ParsesEmploymentTypeCorrectly()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            new() { EmployeeId = "E1", EmployeeName = "Test 1", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FullTime" },
            new() { EmployeeId = "E2", EmployeeName = "Test 2", AwardType = "Retail", Classification = "Level 1", EmploymentType = "PartTime" },
            new() { EmployeeId = "E3", EmployeeName = "Test 3", AwardType = "Retail", Classification = "Level 1", EmploymentType = "Casual" },
            new() { EmployeeId = "E4", EmployeeName = "Test 4", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FixedTerm" }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        var createdEmployees = new List<Employee>();
        _mockEmployeeRepository
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((emp, ct) =>
            {
                emp.Id = Guid.NewGuid();
                createdEmployees.Add(emp);
            })
            .ReturnsAsync((Employee emp, CancellationToken ct) => emp);

        // Act
        await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        createdEmployees.Should().HaveCount(4);
        createdEmployees[0].EmploymentType.Should().Be(EmploymentType.FullTime);
        createdEmployees[1].EmploymentType.Should().Be(EmploymentType.PartTime);
        createdEmployees[2].EmploymentType.Should().Be(EmploymentType.Casual);
        createdEmployees[3].EmploymentType.Should().Be(EmploymentType.FixedTerm);
    }

    [Fact]
    public async Task SyncEmployeesAsync_ParsesAwardLevelCorrectly()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            new() { EmployeeId = "E1", EmployeeName = "Test 1", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FullTime" },
            new() { EmployeeId = "E2", EmployeeName = "Test 2", AwardType = "Retail", Classification = "Level 5", EmploymentType = "FullTime" },
            new() { EmployeeId = "E3", EmployeeName = "Test 3", AwardType = "Retail", Classification = "Level 8", EmploymentType = "FullTime" }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        var createdEmployees = new List<Employee>();
        _mockEmployeeRepository
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((emp, ct) =>
            {
                emp.Id = Guid.NewGuid();
                createdEmployees.Add(emp);
            })
            .ReturnsAsync((Employee emp, CancellationToken ct) => emp);

        // Act
        await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        createdEmployees.Should().HaveCount(3);
        createdEmployees[0].AwardLevelNumber.Should().Be(1);
        createdEmployees[1].AwardLevelNumber.Should().Be(5);
        createdEmployees[2].AwardLevelNumber.Should().Be(8);
    }

    [Fact]
    public async Task SyncEmployeesAsync_ParsesNameCorrectly()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            new() { EmployeeId = "E1", EmployeeName = "Alice Johnson", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FullTime" },
            new() { EmployeeId = "E2", EmployeeName = "Bob", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FullTime" },
            new() { EmployeeId = "E3", EmployeeName = "Carol Anne Smith", AwardType = "Retail", Classification = "Level 1", EmploymentType = "FullTime" }
        };

        _mockEmployeeRepository
            .Setup(x => x.GetByEmployeeNumbersAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        var createdEmployees = new List<Employee>();
        _mockEmployeeRepository
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((emp, ct) =>
            {
                emp.Id = Guid.NewGuid();
                createdEmployees.Add(emp);
            })
            .ReturnsAsync((Employee emp, CancellationToken ct) => emp);

        // Act
        await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        createdEmployees.Should().HaveCount(3);

        createdEmployees[0].FirstName.Should().Be("Alice");
        createdEmployees[0].LastName.Should().Be("Johnson");

        createdEmployees[1].FirstName.Should().Be("Bob");
        createdEmployees[1].LastName.Should().Be("");

        createdEmployees[2].FirstName.Should().Be("Carol");
        createdEmployees[2].LastName.Should().Be("Anne Smith");
    }
}
