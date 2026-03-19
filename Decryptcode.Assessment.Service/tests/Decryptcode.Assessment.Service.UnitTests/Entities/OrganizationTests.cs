using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the Organization aggregate root entity.
/// Tests domain logic, validation, and state changes.
/// </summary>
public class OrganizationTests
{
    [Fact]
    public void Create_WithValidData_ReturnsOrganization()
    {
        // Arrange
        var id = "org-001";
        var name = "Test Organization";
        var slug = "test-org";
        var industry = "Technology";
        var tier = "enterprise";
        var email = "contact@test.com";
        var description = "A test organization";
        var settings = new Settings("America/New_York", "USD", true, "en-US");
        var metadata = new Metadata("migration", 1001, DateTime.UtcNow);

        // Act
        var organization = Organization.Create(
            id, name, slug, industry, tier, email, description, settings, metadata);

        // Assert
        Assert.NotNull(organization);
        Assert.Equal(id, organization.Id);
        Assert.Equal(name, organization.Name);
        Assert.Equal(slug, organization.Slug);
        Assert.Equal(industry, organization.Industry);
        Assert.Equal(tier, organization.Tier);
        Assert.Equal(email, organization.ContactEmail);
        Assert.Equal(description, organization.Description);
    }

    [Fact]
    public void ChangeName_WithValidName_UpdatesName()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newName = "Updated Organization";

        // Act
        organization.ChangeName(newName);

        // Assert
        Assert.Equal(newName, organization.Name);
    }

    [Fact]
    public void ChangeSlug_WithValidSlug_UpdatesSlug()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newSlug = "updated-slug";

        // Act
        organization.ChangeSlug(newSlug);

        // Assert
        Assert.Equal(newSlug, organization.Slug);
    }

    [Fact]
    public void ChangeIndustry_WithValidIndustry_UpdatesIndustry()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newIndustry = "Healthcare";

        // Act
        organization.ChangeIndustry(newIndustry);

        // Assert
        Assert.Equal(newIndustry, organization.Industry);
    }

    [Fact]
    public void ChangeTier_WithValidTier_UpdatesTier()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newTier = "professional";

        // Act
        organization.ChangeTier(newTier);

        // Assert
        Assert.Equal(newTier, organization.Tier);
    }

    [Fact]
    public void ChangeContactEmail_WithValidEmail_UpdatesEmail()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newEmail = "newemail@test.com";

        // Act
        organization.ChangeContactEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, organization.ContactEmail);
    }

    [Fact]
    public void ChangeSettings_WithValidSettings_UpdatesSettings()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newSettings = new Settings("Europe/London", "GBP", false, "en-GB");

        // Act
        organization.ChangeSettings(newSettings);

        // Assert
        Assert.Equal("Europe/London", organization.Settings.Timezone);
        Assert.Equal("GBP", organization.Settings.Currency);
        Assert.False(organization.Settings.AllowOvertime);
        Assert.Equal("en-GB", organization.Settings.DefaultLocale);
    }

    [Fact]
    public void ChangeMetadata_WithValidMetadata_UpdatesMetadata()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newMetadata = new Metadata("api", 2001, DateTime.UtcNow);

        // Act
        organization.ChangeMetadata(newMetadata);

        // Assert
        Assert.Equal("api", organization.Metadata.Source);
        Assert.Equal(2001, organization.Metadata.LegacyId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeName_WithEmptyOrWhitespaceString_ThrowsException(string invalidName)
    {
        // Arrange
        var organization = CreateTestOrganization();

        // Act & Assert
        var exception = Record.Exception(() => organization.ChangeName(invalidName));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeSlug_WithEmptyOrWhitespaceString_ThrowsException(string invalidSlug)
    {
        // Arrange
        var organization = CreateTestOrganization();

        // Act & Assert
        var exception = Record.Exception(() => organization.ChangeSlug(invalidSlug));
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeContactEmail_WithEmptyOrWhitespaceString_ThrowsException(string invalidEmail)
    {
        // Arrange
        var organization = CreateTestOrganization();

        // Act & Assert
        var exception = Record.Exception(() => organization.ChangeContactEmail(invalidEmail));
        Assert.NotNull(exception);
    }

    [Fact]
    public void ChangeDescription_WithValidDescription_UpdatesDescription()
    {
        // Arrange
        var organization = CreateTestOrganization();
        var newDescription = "Updated description";

        // Act
        organization.ChangeDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, organization.Description);
    }

    [Fact]
    public void Organization_WithValidSettings_SettingsAreNotNull()
    {
        // Arrange & Act
        var organization = CreateTestOrganization();

        // Assert
        Assert.NotNull(organization.Settings);
        Assert.NotNull(organization.Metadata);
    }

    [Fact]
    public void Organization_NavigationCollectionsAreInitialized()
    {
        // Arrange & Act
        var organization = CreateTestOrganization();

        // Assert
        Assert.NotNull(organization.Users);
        Assert.NotNull(organization.Projects);
        Assert.NotNull(organization.Invoices);
        Assert.Empty(organization.Users);
        Assert.Empty(organization.Projects);
        Assert.Empty(organization.Invoices);
    }

    // Helper method
    private Organization CreateTestOrganization() =>
        Organization.Create(
            "org-001",
            "Test Organization",
            "test-org",
            "Technology",
            "enterprise",
            "contact@test.com",
            "A test organization",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));
}
