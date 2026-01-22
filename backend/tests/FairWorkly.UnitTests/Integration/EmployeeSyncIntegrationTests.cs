using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.Infrastructure.Persistence.Repositories.Employees;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FairWorkly.UnitTests.Integration;

/// <summary>
/// Integration tests for ISSUE_01: CSV Parser + Employee Sync
/// Tests full workflow with real PostgreSQL database
/// </summary>
[Collection("IntegrationTests")]
public class EmployeeSyncIntegrationTests : IAsyncLifetime
{
    private readonly string _connectionString =
        "Host=localhost;Port=5433;Database=FairWorklyDb;Username=postgres;Password=fairworkly123";

    private FairWorklyDbContext _dbContext = null!;
    private EmployeeRepository _employeeRepository = null!;
    private EmployeeSyncService _employeeSyncService = null!;
    private CsvParserService _csvParserService = null!;
    private Guid _testOrganizationId;

    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    private readonly DateTimeOffset _testDateTime = new(2025, 12, 28, 0, 0, 0, TimeSpan.Zero);

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<FairWorklyDbContext>()
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        _dbContext = new FairWorklyDbContext(options);

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(_testDateTime);

        _employeeRepository = new EmployeeRepository(_dbContext);
        _employeeSyncService = new EmployeeSyncService(_employeeRepository, _mockDateTimeProvider.Object);
        _csvParserService = new CsvParserService();

        // Create test organization
        await CreateTestOrganizationAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up test data
        await CleanupTestDataAsync();
        await _dbContext.DisposeAsync();
    }

    private async Task CreateTestOrganizationAsync()
    {
        _testOrganizationId = Guid.NewGuid();

        // Generate unique ABN for each test run to avoid conflicts
        var random = new Random();
        var uniqueAbn = $"{random.Next(10000000, 99999999):D8}{random.Next(100, 999):D3}";

        var organization = new Organization
        {
            Id = _testOrganizationId,
            CompanyName = "Test Company",
            ABN = uniqueAbn,
            IndustryType = "Retail",
            AddressLine1 = "123 Test Street",
            Suburb = "Melbourne",
            State = AustralianState.VIC,
            Postcode = "3000",
            ContactEmail = $"test{_testOrganizationId:N}@testcompany.com.au",
            SubscriptionTier = SubscriptionTier.Tier1,
            SubscriptionStartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            IsSubscriptionActive = true
        };

        _dbContext.Set<Organization>().Add(organization);
        await _dbContext.SaveChangesAsync();
    }

    private async Task CleanupTestDataAsync()
    {
        try
        {
            // Use ExecuteDeleteAsync to avoid entity tracking issues
            await _dbContext.Employees
                .Where(e => e.OrganizationId == _testOrganizationId)
                .ExecuteDeleteAsync();

            await _dbContext.Set<Organization>()
                .Where(o => o.Id == _testOrganizationId)
                .ExecuteDeleteAsync();
        }
        catch
        {
            // Ignore cleanup errors - test data isolation is handled by unique IDs
        }
    }

    [Fact]
    public async Task TC_SYNC_001_NewEmployees_CreatesInDatabase()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "EmployeeSync", "TEST_01_NewEmployees.csv");

        using var stream = File.OpenRead(csvPath);

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);
        errors.Should().BeEmpty();
        rows.Should().HaveCount(5);

        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert - verify returned mapping
        result.Should().HaveCount(5);
        result.Should().ContainKey("NEW001");
        result.Should().ContainKey("NEW002");
        result.Should().ContainKey("NEW003");
        result.Should().ContainKey("NEW004");
        result.Should().ContainKey("NEW005");

        // Assert - verify database records
        var dbEmployees = await _dbContext.Employees
            .Where(e => e.OrganizationId == _testOrganizationId)
            .OrderBy(e => e.EmployeeNumber)
            .ToListAsync();

        dbEmployees.Should().HaveCount(5);

        var alice = dbEmployees.First(e => e.EmployeeNumber == "NEW001");
        alice.FirstName.Should().Be("Alice");
        alice.LastName.Should().Be("Johnson");
        alice.AwardType.Should().Be(AwardType.GeneralRetailIndustryAward2020);
        alice.AwardLevelNumber.Should().Be(1);
        alice.EmploymentType.Should().Be(EmploymentType.FullTime);
        alice.OrganizationId.Should().Be(_testOrganizationId);
        alice.IsActive.Should().BeTrue();

        var carol = dbEmployees.First(e => e.EmployeeNumber == "NEW003");
        carol.FirstName.Should().Be("Carol");
        carol.LastName.Should().Be("Davis");
        carol.EmploymentType.Should().Be(EmploymentType.Casual);
        carol.AwardLevelNumber.Should().Be(3);
    }

    [Fact]
    public async Task TC_SYNC_002_UpdateEmployees_UpdatesInDatabase()
    {
        // Arrange - pre-insert employees that will be updated
        var existingEmployee1 = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrganizationId,
            EmployeeNumber = "EMP001",
            FirstName = "Original",
            LastName = "Name",
            Email = "emp001@placeholder.local",
            JobTitle = "Employee",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            IsActive = true
        };

        var existingEmployee2 = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrganizationId,
            EmployeeNumber = "EMP002",
            FirstName = "Original",
            LastName = "Jane",
            Email = "emp002@placeholder.local",
            JobTitle = "Employee",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            IsActive = true
        };

        _dbContext.Employees.AddRange(existingEmployee1, existingEmployee2);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "EmployeeSync", "TEST_02_UpdateEmployees.csv");

        using var stream = File.OpenRead(csvPath);

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);
        errors.Should().BeEmpty();

        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        result.Should().ContainKey("EMP001");
        result["EMP001"].Should().Be(existingEmployee1.Id);

        // Verify database update
        _dbContext.ChangeTracker.Clear();
        var updatedEmployee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.EmployeeNumber == "EMP001" && e.OrganizationId == _testOrganizationId);

        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("John");
        updatedEmployee.LastName.Should().Be("Updated");
        updatedEmployee.AwardLevelNumber.Should().Be(2); // Updated from Level 1 to Level 2
    }

    [Fact]
    public async Task TC_SYNC_003_MixedScenario_CreatesAndUpdates()
    {
        // Arrange - pre-insert one employee
        var existingEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrganizationId,
            EmployeeNumber = "MIX001",
            FirstName = "Existing",
            LastName = "Person",
            Email = "mix001@placeholder.local",
            JobTitle = "Employee",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            IsActive = true
        };

        _dbContext.Employees.Add(existingEmployee);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "EmployeeSync", "TEST_03_MixedEmployees.csv");

        using var stream = File.OpenRead(csvPath);

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);
        errors.Should().BeEmpty();

        var result = await _employeeSyncService.SyncEmployeesAsync(rows, _testOrganizationId);

        // Assert
        result.Should().NotBeEmpty();

        // Verify existing employee was updated (not duplicated)
        _dbContext.ChangeTracker.Clear();
        var mix001Count = await _dbContext.Employees
            .CountAsync(e => e.EmployeeNumber == "MIX001" && e.OrganizationId == _testOrganizationId);
        mix001Count.Should().Be(1);

        // Verify new employees were created
        var totalEmployees = await _dbContext.Employees
            .CountAsync(e => e.OrganizationId == _testOrganizationId);
        totalEmployees.Should().BeGreaterThan(1);
    }
}
