using FluentAssertions;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;
using Moq;

namespace FairWorkly.UnitTests.Unit;

/// <summary>
/// Unit tests for ValidatePayrollHandler focusing on Pre-Validation logic
/// Tests TC-PREVAL-001~005 as per TEST_PLAN.md
/// </summary>
public class ValidatePayrollHandlerTests
{
    private readonly Mock<ICsvParserService> _mockCsvParser;
    private readonly Mock<IEmployeeSyncService> _mockEmployeeSync;
    private readonly Mock<IPayrollValidationRepository> _mockValidationRepo;
    private readonly Mock<IPayslipRepository> _mockPayslipRepo;
    private readonly Mock<IPayrollIssueRepository> _mockIssueRepo;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly ValidatePayrollHandler _handler;

    private readonly DateTimeOffset _testDateTime = new(2025, 12, 28, 10, 0, 0, TimeSpan.Zero);

    public ValidatePayrollHandlerTests()
    {
        _mockCsvParser = new Mock<ICsvParserService>();
        _mockEmployeeSync = new Mock<IEmployeeSyncService>();
        _mockValidationRepo = new Mock<IPayrollValidationRepository>();
        _mockPayslipRepo = new Mock<IPayslipRepository>();
        _mockIssueRepo = new Mock<IPayrollIssueRepository>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        _mockFileStorage = new Mock<IFileStorageService>();

        _mockDateTimeProvider.Setup(x => x.Now).Returns(_testDateTime);
        _mockFileStorage.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploads/test.csv");

        // Default repository setups - capture saved entities
        _mockValidationRepo.Setup(x => x.AddAsync(It.IsAny<PayrollValidation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayrollValidation v, CancellationToken _) => v);
        _mockValidationRepo.Setup(x => x.UpdateAsync(It.IsAny<PayrollValidation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockValidationRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockPayslipRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Payslip>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Payslip>, CancellationToken>((payslips, _) =>
            {
                // Simulate EF Core assigning unique IDs on save
                foreach (var payslip in payslips)
                {
                    if (payslip.Id == Guid.Empty)
                        payslip.Id = Guid.NewGuid();
                }
            })
            .Returns(Task.CompletedTask);
        _mockPayslipRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockIssueRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<PayrollIssue>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockIssueRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // No compliance rules for pre-validation tests
        _handler = new ValidatePayrollHandler(
            _mockCsvParser.Object,
            _mockEmployeeSync.Object,
            _mockValidationRepo.Object,
            _mockPayslipRepo.Object,
            _mockIssueRepo.Object,
            Enumerable.Empty<IComplianceRule>(),
            _mockDateTimeProvider.Object,
            _mockFileStorage.Object);
    }

    [Fact]
    public async Task Handle_WhenClassificationMissing_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL001", classification: "")
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Severity.Should().Be((int)IssueSeverity.Warning);
        issue.Warning.Should().Contain("Classification");
        issue.Description.Should().BeNull();
        issue.ImpactAmount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenHourlyRateZero_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL002", hourlyRate: 0)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Severity.Should().Be((int)IssueSeverity.Warning);
        issue.Warning.Should().Contain("Hourly Rate");
        issue.ImpactAmount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenHourlyRateNegative_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL003", hourlyRate: -10.00m)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Hourly Rate");
    }

    [Fact]
    public async Task Handle_WhenOrdinaryHoursNegative_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL004", ordinaryHours: -5.00m)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Ordinary Hours");
    }

    [Fact]
    public async Task Handle_WhenOrdinaryPayNegative_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL005", ordinaryPay: -100.00m)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Ordinary Pay");
    }

    [Fact]
    public async Task Handle_WhenGrossPayNegative_ShouldReturnPreValidationWarning()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL006", grossPay: -500.00m)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Gross Pay");
    }

    [Fact]
    public async Task Handle_WhenMultipleFieldsInvalid_ShouldReturnSingleWarningWithAllFields()
    {
        // Arrange: Missing classification AND zero hourly rate
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL007", classification: "", hourlyRate: 0)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Should combine multiple missing fields into one warning
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Classification");
        issue.Warning.Should().Contain("Hourly Rate");
    }

    [Fact]
    public async Task Handle_WhenAllFieldsValid_ShouldPassPreValidation()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL008")
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: No pre-validation issues (and no rule issues since no rules)
        result.Issues.Should().BeEmpty();
        result.Status.Should().Be("Passed");
        result.Summary.PassedCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenPreValidationFails_ShouldSkipRuleEvaluation()
    {
        // Arrange: Create handler with a real compliance rule
        var baseRateRule = new BaseRateRule();
        var handlerWithRules = new ValidatePayrollHandler(
            _mockCsvParser.Object,
            _mockEmployeeSync.Object,
            _mockValidationRepo.Object,
            _mockPayslipRepo.Object,
            _mockIssueRepo.Object,
            new[] { baseRateRule },
            _mockDateTimeProvider.Object,
            _mockFileStorage.Object);

        // This employee has missing classification (pre-validation fail)
        // AND would fail base rate check if it wasn't skipped
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("PREVAL009", classification: "", hourlyRate: 20.00m)
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await handlerWithRules.Handle(command, CancellationToken.None);

        // Assert: Only pre-validation issue, base rate rule should not run
        result.Issues.Should().HaveCount(1);
        var issue = result.Issues[0];
        issue.CategoryType.Should().Be("PreValidation");
        issue.Warning.Should().Contain("Classification");
    }

    [Fact]
    public async Task Handle_WhenCsvParseFails_ShouldReturnFailedResult()
    {
        // Arrange
        var parseErrors = new List<string> { "Row 1: Employee ID is required", "Row 2: Invalid date format" };
        _mockCsvParser.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PayrollCsvRow>(), parseErrors));

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Failed");
        result.Summary.TotalIssues.Should().Be(2);
        result.Issues.Should().HaveCount(2);
        result.Issues.Should().AllSatisfy(i =>
        {
            i.CategoryType.Should().Be("PreValidation");
            i.Warning.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectValidationIdFormat()
    {
        // Arrange
        var rows = new List<PayrollCsvRow> { CreateRow("TEST001") };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: ValidationId should start with "VAL-"
        result.ValidationId.Should().StartWith("VAL-");
        result.ValidationId.Length.Should().Be(12); // "VAL-" + 8 hex chars
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectTimestamp()
    {
        // Arrange
        var rows = new List<PayrollCsvRow> { CreateRow("TEST001") };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Timestamp.Should().Be(_testDateTime);
    }

    [Fact]
    public async Task Handle_WhenMixedValidAndInvalidRows_ShouldProcessCorrectly()
    {
        // Arrange
        var rows = new List<PayrollCsvRow>
        {
            CreateRow("VALID001"),  // Valid
            CreateRow("INVALID001", classification: ""),  // Pre-validation fail
            CreateRow("VALID002")   // Valid
        };
        SetupMocks(rows);

        var command = CreateCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Summary.PassedCount.Should().Be(2);
        result.Summary.AffectedEmployees.Should().Be(1);
        result.Issues.Should().HaveCount(1);
    }

    private void SetupMocks(List<PayrollCsvRow> rows)
    {
        _mockCsvParser.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((rows, new List<string>()));

        var employeeMapping = rows.ToDictionary(r => r.EmployeeId, _ => Guid.NewGuid());
        _mockEmployeeSync.Setup(x => x.SyncEmployeesAsync(rows, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeMapping);
    }

    private static PayrollCsvRow CreateRow(
        string employeeId,
        string classification = "Level 1",
        decimal hourlyRate = 26.55m,
        decimal ordinaryHours = 38.00m,
        decimal ordinaryPay = 1008.90m,
        decimal grossPay = 1008.90m,
        decimal superannuationPaid = 121.07m)
    {
        return new PayrollCsvRow
        {
            EmployeeId = employeeId,
            EmployeeName = $"Test {employeeId}",
            PayPeriodStart = new DateOnly(2025, 12, 15),
            PayPeriodEnd = new DateOnly(2025, 12, 21),
            AwardType = "Retail",
            Classification = classification,
            EmploymentType = "FullTime",
            HourlyRate = hourlyRate,
            OrdinaryHours = ordinaryHours,
            OrdinaryPay = ordinaryPay,
            SaturdayHours = 0,
            SaturdayPay = 0,
            SundayHours = 0,
            SundayPay = 0,
            PublicHolidayHours = 0,
            PublicHolidayPay = 0,
            GrossPay = grossPay,
            SuperannuationPaid = superannuationPaid
        };
    }

    private static ValidatePayrollCommand CreateCommand()
    {
        return new ValidatePayrollCommand
        {
            FileStream = new MemoryStream(),
            FileName = "test.csv",
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
