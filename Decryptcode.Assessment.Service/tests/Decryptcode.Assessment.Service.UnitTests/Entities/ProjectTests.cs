using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Enums;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the Project entity.
/// Tests business logic for project status transitions and property changes.
/// </summary>
public class ProjectTests
{
    [Fact]
    public void Create_WithValidData_ReturnsProject()
    {
        // Arrange & Act
        var project = Project.Create(
            "proj-001",
            "org-001",
            "Platform Modernization",
            ProjectStatus.Active,
            800,
            DateTime.Parse("2024-01-01"),
            DateTime.Parse("2024-06-30"),
            "Full migration project");

        // Assert
        Assert.NotNull(project);
        Assert.Equal("Platform Modernization", project.Name);
        Assert.Equal(ProjectStatus.Active, project.Status);
        Assert.Equal(800, project.BudgetHours);
        Assert.NotNull(project.StartDate);
        Assert.NotNull(project.EndDate);
    }

    [Fact]
    public void Create_WithNullDates_SetsNullDates()
    {
        // Arrange & Act
        var project = Project.Create(
            "proj-001",
            "org-001",
            "Future Project",
            ProjectStatus.Planned,
            0,
            null,
            null,
            "Planned project");

        // Assert
        Assert.Null(project.StartDate);
        Assert.Null(project.EndDate);
    }

    [Fact]
    public void Create_WithMixedNulls_SetsPartialDates()
    {
        // Arrange
        var startDate = DateTime.Parse("2024-01-01");

        // Act
        var project = Project.Create(
            "proj-001",
            "org-001",
            "Open-ended Project",
            ProjectStatus.Active,
            100,
            startDate,
            null,
            "Project without end date");

        // Assert
        Assert.NotNull(project.StartDate);
        Assert.Null(project.EndDate);
        Assert.Equal(startDate, project.StartDate);
    }

    [Fact]
    public void Activate_SetsStatusToActive()
    {
        // Arrange
        var project = CreatePlannedProject();

        // Act
        project.Activate();

        // Assert
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public void Complete_SetsStatusToCompleted()
    {
        // Arrange
        var project = CreateActiveProject();

        // Act
        project.Complete();

        // Assert
        Assert.Equal(ProjectStatus.Completed, project.Status);
    }

    [Fact]
    public void Plan_SetsStatusToPlanned()
    {
        // Arrange
        var project = CreateActiveProject();

        // Act
        project.Plan();

        // Assert
        Assert.Equal(ProjectStatus.Planned, project.Status);
    }

    [Fact]
    public void ChangeName_WithValidName_UpdatesName()
    {
        // Arrange
        var project = CreateActiveProject();
        var newName = "Renamed Project";

        // Act
        project.ChangeName(newName);

        // Assert
        Assert.Equal(newName, project.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeName_WithEmptyOrWhitespace_ThrowsException(string invalidName)
    {
        // Arrange
        var project = CreateActiveProject();

        // Act & Assert
        var exception = Record.Exception(() => project.ChangeName(invalidName));
        Assert.NotNull(exception);
    }

    [Fact]
    public void ChangeBudgetHours_WithValidValue_UpdatesBudgetHours()
    {
        // Arrange
        var project = CreateActiveProject();
        int newBudget = 1200;

        // Act
        project.ChangeBudgetHours(newBudget);

        // Assert
        Assert.Equal(newBudget, project.BudgetHours);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void ChangeBudgetHours_WithNegativeValue_ThrowsException(int invalidHours)
    {
        // Arrange
        var project = CreateActiveProject();

        // Act & Assert
        var exception = Record.Exception(() => project.ChangeBudgetHours(invalidHours));
        Assert.NotNull(exception);
    }

    [Fact]
    public void ChangeBudgetHours_WithZero_Succeeds()
    {
        // Arrange
        var project = CreateActiveProject();

        // Act
        project.ChangeBudgetHours(0);

        // Assert
        Assert.Equal(0, project.BudgetHours);
    }

    [Fact]
    public void ChangeStartDate_WithValidDate_UpdatesStartDate()
    {
        // Arrange
        var project = CreateActiveProject();
        var newDate = DateTime.Parse("2024-02-15");

        // Act
        project.ChangeStartDate(newDate);

        // Assert
        Assert.Equal(newDate, project.StartDate);
    }

    [Fact]
    public void ChangeEndDate_WithValidDate_UpdatesEndDate()
    {
        // Arrange
        var project = CreateActiveProject();
        var newDate = DateTime.Parse("2024-12-31");

        // Act
        project.ChangeEndDate(newDate);

        // Assert
        Assert.Equal(newDate, project.EndDate);
    }

    [Fact]
    public void ChangeDescription_WithValidDescription_UpdatesDescription()
    {
        // Arrange
        var project = CreateActiveProject();
        var newDescription = "Updated project description";

        // Act
        project.ChangeDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, project.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeDescription_WithEmptyOrWhitespace_ThrowsException(string invalidDescription)
    {
        // Arrange
        var project = CreateActiveProject();

        // Act & Assert
        var exception = Record.Exception(() => project.ChangeDescription(invalidDescription));
        Assert.NotNull(exception);
    }

    [Fact]
    public void Project_HasEmptyInvoicesCollection()
    {
        // Arrange & Act
        var project = CreateActiveProject();

        // Assert
        Assert.NotNull(project.Invoices);
        Assert.Empty(project.Invoices);
    }

    [Fact]
    public void Project_HasEmptyTimeEntriesCollection()
    {
        // Arrange & Act
        var project = CreateActiveProject();

        // Assert
        Assert.NotNull(project.TimeEntries);
        Assert.Empty(project.TimeEntries);
    }

    [Fact]
    public void Project_WithValidOrgId_HasCorrectOrgId()
    {
        // Arrange
        var orgId = "org-test-123";

        // Act
        var project = Project.Create(
            "proj-001", orgId, "Test", ProjectStatus.Active, 100,
            null, null, "Description");

        // Assert
        Assert.Equal(orgId, project.OrgId);
    }

    [Theory]
    [InlineData(ProjectStatus.Planned)]
    [InlineData(ProjectStatus.Active)]
    [InlineData(ProjectStatus.Completed)]
    public void Create_WithVariousStatuses_SucceedsForAllValidStatuses(ProjectStatus status)
    {
        // Arrange & Act
        var project = Project.Create(
            "proj-001", "org-001", "Test", status, 100,
            null, null, "Description");

        // Assert
        Assert.Equal(status, project.Status);
    }

    // Helper methods
    private Project CreateActiveProject() =>
        Project.Create(
            "proj-001",
            "org-001",
            "Test Project",
            ProjectStatus.Active,
            100,
            DateTime.Parse("2024-01-01"),
            DateTime.Parse("2024-06-30"),
            "Test project description");

    private Project CreatePlannedProject() =>
        Project.Create(
            "proj-001",
            "org-001",
            "Planned Project",
            ProjectStatus.Planned,
            100,
            DateTime.Parse("2024-04-01"),
            DateTime.Parse("2024-09-30"),
            "Planned project description");
}
