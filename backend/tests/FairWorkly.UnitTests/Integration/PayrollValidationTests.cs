using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.Infrastructure.Persistence.Repositories.Employees;
using FairWorkly.Infrastructure.Persistence.Repositories.Payroll;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FairWorkly.UnitTests.Integration;

/// <summary>
/// Integration tests for ISSUE_03: ValidatePayroll Handler
/// Tests TC-INT-001 ~ TC-INT-004 as per TEST_PLAN.md
/// Uses real PostgreSQL database for full workflow validation
/// </summary>
[Collection("IntegrationTests")]
public class PayrollValidationTests : IAsyncLifetime
{
    private readonly string _connectionString =
        "Host=localhost;Port=5433;Database=FairWorklyDb;Username=postgres;Password=fairworkly123";

    private FairWorklyDbContext _dbContext = null!;
    private ValidatePayrollHandler _handler = null!;
    private Guid _testOrganizationId;

    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    private readonly Mock<IFileStorageService> _mockFileStorage = new();
    private readonly DateTimeOffset _testDateTime = new(2025, 12, 28, 10, 0, 0, TimeSpan.Zero);

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<FairWorklyDbContext>()
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        _dbContext = new FairWorklyDbContext(options);

        _mockDateTimeProvider.Setup(x => x.Now).Returns(_testDateTime);
        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(_testDateTime);
        _mockFileStorage.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploads/test.csv");

        // Create test organization
        await CreateTestOrganizationAsync();

        // Create real services
        var employeeRepository = new EmployeeRepository(_dbContext);
        var csvParserService = new CsvParserService();
        var employeeSyncService = new EmployeeSyncService(employeeRepository, _mockDateTimeProvider.Object);
        var validationRepository = new PayrollValidationRepository(_dbContext);
        var payslipRepository = new PayslipRepository(_dbContext);
        var issueRepository = new PayrollIssueRepository(_dbContext);

        // Real compliance rules
        var complianceRules = new IComplianceRule[]
        {
            new BaseRateRule(),
            new PenaltyRateRule(),
            new CasualLoadingRule(),
            new SuperannuationRule()
        };

        _handler = new ValidatePayrollHandler(
            csvParserService,
            employeeSyncService,
            validationRepository,
            payslipRepository,
            issueRepository,
            complianceRules,
            _mockDateTimeProvider.Object,
            _mockFileStorage.Object);
    }

    public async Task DisposeAsync()
    {
        await CleanupTestDataAsync();
        await _dbContext.DisposeAsync();
    }

    private async Task CreateTestOrganizationAsync()
    {
        _testOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Check if MVP organization already exists
        var existing = await _dbContext.Set<Organization>()
            .FirstOrDefaultAsync(o => o.Id == _testOrganizationId);

        if (existing == null)
        {
            var random = new Random();
            var uniqueAbn = $"{random.Next(10000000, 99999999):D8}{random.Next(100, 999):D3}";

            var organization = new Organization
            {
                Id = _testOrganizationId,
                CompanyName = "MVP Test Company",
                ABN = uniqueAbn,
                IndustryType = "Retail",
                AddressLine1 = "123 Test Street",
                Suburb = "Melbourne",
                State = AustralianState.VIC,
                Postcode = "3000",
                ContactEmail = "mvptest@testcompany.com.au",
                SubscriptionTier = SubscriptionTier.Tier1,
                SubscriptionStartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                IsSubscriptionActive = true
            };

            _dbContext.Set<Organization>().Add(organization);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task CleanupTestDataAsync()
    {
        try
        {
            // Clean up test data in reverse dependency order
            await _dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM payroll_issues WHERE organization_id = @p0",
                _testOrganizationId);
            await _dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM payslips WHERE organization_id = @p0",
                _testOrganizationId);
            await _dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM payroll_validations WHERE organization_id = @p0",
                _testOrganizationId);
            await _dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM employees WHERE organization_id = @p0",
                _testOrganizationId);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Fact]
    public async Task TC_INT_001_AllCompliant_ShouldReturnPassed()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_13_AllCompliant.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_13_AllCompliant.csv");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Passed");
        result.Summary.PassedCount.Should().Be(10);
        result.Summary.TotalIssues.Should().Be(0);
        result.Summary.AffectedEmployees.Should().Be(0);
        result.Summary.TotalUnderpayment.Should().Be(0);
        result.Issues.Should().BeEmpty();
        result.ValidationId.Should().StartWith("VAL-");
    }

    [Fact]
    public async Task TC_INT_002_AllViolations_ShouldReturnFailed()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_14_AllViolations.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_14_AllViolations.csv");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Failed");
        result.Summary.AffectedEmployees.Should().Be(8);
        result.Summary.TotalIssues.Should().BeGreaterThan(0);
        result.Summary.TotalUnderpayment.Should().BeGreaterThan(0);

        // Should have issues in multiple categories
        var categories = result.Categories.Select(c => c.Key).ToList();
        categories.Should().Contain("BaseRate");
        categories.Should().Contain("PenaltyRate");
        categories.Should().Contain("CasualLoading");
        categories.Should().Contain("Superannuation");

        // Verify specific violations
        var baseRateIssues = result.Issues.Where(i => i.CategoryType == "BaseRate").ToList();
        baseRateIssues.Should().HaveCount(2); // FAIL001, FAIL002

        var penaltyIssues = result.Issues.Where(i => i.CategoryType == "PenaltyRate").ToList();
        penaltyIssues.Should().HaveCount(2); // FAIL003 (Saturday), FAIL004 (Sunday)

        var casualIssues = result.Issues.Where(i => i.CategoryType == "CasualLoading").ToList();
        casualIssues.Should().HaveCount(2); // FAIL005, FAIL006

        var superIssues = result.Issues.Where(i => i.CategoryType == "Superannuation").ToList();
        superIssues.Should().HaveCount(2); // FAIL007, FAIL008
    }

    [Fact]
    public async Task TC_INT_003_MixedScenarios_ShouldReturnCorrectCounts()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_15_MixedScenarios.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_15_MixedScenarios.csv");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Failed");
        result.Summary.PassedCount.Should().Be(6);  // MIX001-MIX006 pass
        result.Summary.AffectedEmployees.Should().Be(6);  // MIX007-MIX012 fail

        // 12 total employees - 6 pass, 6 fail
        (result.Summary.PassedCount + result.Summary.AffectedEmployees).Should().Be(12);

        // Verify underpayment is calculated
        result.Summary.TotalUnderpayment.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TC_INT_004_EdgeCases_ShouldHandleCorrectly()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_16_EdgeCases.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_16_EdgeCases.csv");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // EDGE001: Exact minimum rate - should pass
        var edge001Issues = result.Issues.Where(i => i.EmployeeId == "EDGE001").ToList();
        edge001Issues.Should().BeEmpty();

        // EDGE002: Just below minimum ($26.54 vs $26.55) - within tolerance, should pass
        var edge002Issues = result.Issues.Where(i => i.EmployeeId == "EDGE002").ToList();
        edge002Issues.Should().BeEmpty(); // Within $0.01 tolerance

        // EDGE003: Zero hours - should be skipped for BaseRate check
        var edge003Issues = result.Issues.Where(i => i.EmployeeId == "EDGE003").ToList();
        edge003Issues.Where(i => i.CategoryType == "BaseRate").Should().BeEmpty();

        // EDGE005: Zero gross pay - should skip Super check
        var edge005Issues = result.Issues.Where(i => i.EmployeeId == "EDGE005").ToList();
        edge005Issues.Where(i => i.CategoryType == "Superannuation").Should().BeEmpty();

        // EDGE006: Super within tolerance ($133.43 vs $133.47) - should pass
        var edge006Issues = result.Issues.Where(i => i.EmployeeId == "EDGE006").ToList();
        edge006Issues.Where(i => i.CategoryType == "Superannuation").Should().BeEmpty();

        // EDGE007: Penalty over tolerance - should fail
        var edge007Issues = result.Issues.Where(i => i.EmployeeId == "EDGE007").ToList();
        edge007Issues.Where(i => i.CategoryType == "PenaltyRate").Should().HaveCount(1);

        // EDGE008: Level 8 Casual at correct rate - should pass
        var edge008Issues = result.Issues.Where(i => i.EmployeeId == "EDGE008").ToList();
        edge008Issues.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenRulesDisabled_ShouldNotCreateIssues()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_14_AllViolations.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_14_AllViolations.csv");

        // Disable all rules
        command.EnableBaseRateCheck = false;
        command.EnablePenaltyCheck = false;
        command.EnableCasualLoadingCheck = false;
        command.EnableSuperCheck = false;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: No issues because all rules are disabled
        result.Issues.Should().BeEmpty();
        result.Status.Should().Be("Passed");
    }

    [Fact]
    public async Task Handle_WhenOnlyBaseRateEnabled_ShouldOnlyCheckBaseRate()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_14_AllViolations.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_14_AllViolations.csv");

        // Enable only base rate check
        command.EnableBaseRateCheck = true;
        command.EnablePenaltyCheck = false;
        command.EnableCasualLoadingCheck = false;
        command.EnableSuperCheck = false;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Only BaseRate issues
        result.Issues.Should().AllSatisfy(i => i.CategoryType.Should().Be("BaseRate"));
        result.Issues.Should().HaveCount(2); // FAIL001, FAIL002
    }

    [Fact]
    public async Task Handle_ShouldPersistDataToDatabase()
    {
        // Arrange
        var csvPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestData", "Csv", "Integration", "TEST_13_AllCompliant.csv");

        using var stream = File.OpenRead(csvPath);
        var command = CreateCommand(stream, "TEST_13_AllCompliant.csv");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Verify data persisted to database
        _dbContext.ChangeTracker.Clear();

        // Check PayrollValidation record
        var validationId = Guid.Parse(result.ValidationId.Replace("VAL-", "") + new string('0', 24));
        // Actually we need to query differently since we only have first 8 chars
        var validations = await _dbContext.Set<FairWorkly.Domain.Payroll.Entities.PayrollValidation>()
            .Where(v => v.OrganizationId == _testOrganizationId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        validations.Should().NotBeEmpty();
        var latestValidation = validations.First();
        latestValidation.TotalPayslips.Should().Be(10);
        latestValidation.Status.Should().Be(ValidationStatus.Passed);

        // Check Payslips
        var payslips = await _dbContext.Set<FairWorkly.Domain.Payroll.Entities.Payslip>()
            .Where(p => p.PayrollValidationId == latestValidation.Id)
            .ToListAsync();

        payslips.Should().HaveCount(10);

        // Check Employees were synced
        var employees = await _dbContext.Employees
            .Where(e => e.OrganizationId == _testOrganizationId)
            .ToListAsync();

        employees.Count.Should().BeGreaterThanOrEqualTo(10);
    }

    private static ValidatePayrollCommand CreateCommand(Stream fileStream, string fileName)
    {
        return new ValidatePayrollCommand
        {
            FileStream = fileStream,
            FileName = fileName,
            AwardType = "Retail",
            PayPeriod = "Weekly",
            WeekStarting = new DateOnly(2025, 12, 15),
            WeekEnding = new DateOnly(2025, 12, 21),
            State = "VIC",
            EnableBaseRateCheck = true,
            EnablePenaltyCheck = true,
            EnableCasualLoadingCheck = true,
            EnableSuperCheck = true
        };
    }
}
