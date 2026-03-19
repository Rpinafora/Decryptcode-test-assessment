using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for the Settings value object.
/// Tests immutability, equality, and validation.
/// </summary>
public class SettingsTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSettings()
    {
        // Arrange & Act
        var settings = new Settings("America/New_York", "USD", true, "en-US");

        // Assert
        Assert.Equal("America/New_York", settings.Timezone);
        Assert.Equal("USD", settings.Currency);
        Assert.True(settings.AllowOvertime);
        Assert.Equal("en-US", settings.DefaultLocale);
    }

    [Fact]
    public void Create_WithDifferentValues_ReturnsSettingsWithCorrectValues()
    {
        // Arrange & Act
        var settings = new Settings("Europe/London", "GBP", false, "en-GB");

        // Assert
        Assert.Equal("Europe/London", settings.Timezone);
        Assert.Equal("GBP", settings.Currency);
        Assert.False(settings.AllowOvertime);
        Assert.Equal("en-GB", settings.DefaultLocale);
    }

    [Theory]
    [InlineData(null, "USD", true, "en-US")]
    [InlineData("America/New_York", null, true, "en-US")]
    [InlineData("America/New_York", "USD", true, null)]
    public void Create_WithNullValues_ThrowsArgumentNullException(
        string timezone, string currency, bool allowOvertime, string locale)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Settings(timezone, currency, allowOvertime, locale));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTimezone_ThrowsArgumentNullException(string emptyTimezone)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Settings(emptyTimezone, "USD", true, "en-US"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCurrency_ThrowsArgumentNullException(string emptyCurrency)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Settings("America/New_York", emptyCurrency, true, "en-US"));
    }

    [Fact]
    public void ValueEquality_WithSameValues_AreEqual()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("America/New_York", "USD", true, "en-US");

        // Act & Assert
        Assert.Equal(settings1, settings2);
    }

    [Fact]
    public void ValueEquality_WithDifferentTimezone_AreNotEqual()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("Europe/London", "USD", true, "en-US");

        // Act & Assert
        Assert.NotEqual(settings1, settings2);
    }

    [Fact]
    public void ValueEquality_WithDifferentCurrency_AreNotEqual()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("America/New_York", "EUR", true, "en-US");

        // Act & Assert
        Assert.NotEqual(settings1, settings2);
    }

    [Fact]
    public void ValueEquality_WithDifferentAllowOvertime_AreNotEqual()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("America/New_York", "USD", false, "en-US");

        // Act & Assert
        Assert.NotEqual(settings1, settings2);
    }

    [Fact]
    public void ValueEquality_WithDifferentLocale_AreNotEqual()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("America/New_York", "USD", true, "en-GB");

        // Act & Assert
        Assert.NotEqual(settings1, settings2);
    }

    [Fact]
    public void Settings_IsRecord_SupportsCopyWithExpression()
    {
        // Arrange
        var originalSettings = new Settings("America/New_York", "USD", true, "en-US");

        // Act
        var updatedSettings = originalSettings with { Timezone = "Europe/London" };

        // Assert
        Assert.Equal("Europe/London", updatedSettings.Timezone);
        Assert.Equal("USD", updatedSettings.Currency);
        Assert.True(updatedSettings.AllowOvertime);
        Assert.Equal("en-US", updatedSettings.DefaultLocale);
    }

    [Fact]
    public void Settings_GetHashCode_ConsistentForEqualObjects()
    {
        // Arrange
        var settings1 = new Settings("America/New_York", "USD", true, "en-US");
        var settings2 = new Settings("America/New_York", "USD", true, "en-US");

        // Act & Assert
        Assert.Equal(settings1.GetHashCode(), settings2.GetHashCode());
    }
}

/// <summary>
/// Unit tests for the Metadata value object.
/// Tests immutability, equality, and nullable properties.
/// </summary>
public class MetadataTests
{
    [Fact]
    public void Create_WithAllValues_ReturnsMetadata()
    {
        // Arrange
        var migratedAt = DateTime.UtcNow;

        // Act
        var metadata = new Metadata("migration", 1001, migratedAt);

        // Assert
        Assert.Equal("migration", metadata.Source);
        Assert.Equal(1001, metadata.LegacyId);
        Assert.Equal(migratedAt, metadata.MigratedAt);
    }

    [Fact]
    public void Create_WithNullableLegacyId_AllowsNull()
    {
        // Arrange & Act
        var metadata = new Metadata("api", null, null);

        // Assert
        Assert.Equal("api", metadata.Source);
        Assert.Null(metadata.LegacyId);
        Assert.Null(metadata.MigratedAt);
    }

    [Fact]
    public void Create_WithNullSource_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Metadata(null, null, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySource_ThrowsArgumentNullException(string emptySource)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Metadata(emptySource, null, null));
    }

    [Fact]
    public void ValueEquality_WithSameValues_AreEqual()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;
        var metadata1 = new Metadata("migration", 1001, dateTime);
        var metadata2 = new Metadata("migration", 1001, dateTime);

        // Act & Assert
        Assert.Equal(metadata1, metadata2);
    }

    [Fact]
    public void ValueEquality_WithDifferentSource_AreNotEqual()
    {
        // Arrange
        var metadata1 = new Metadata("migration", 1001, null);
        var metadata2 = new Metadata("api", 1001, null);

        // Act & Assert
        Assert.NotEqual(metadata1, metadata2);
    }

    [Fact]
    public void ValueEquality_WithDifferentLegacyId_AreNotEqual()
    {
        // Arrange
        var metadata1 = new Metadata("migration", 1001, null);
        var metadata2 = new Metadata("migration", 1002, null);

        // Act & Assert
        Assert.NotEqual(metadata1, metadata2);
    }

    [Fact]
    public void ValueEquality_WithNullVsValue_AreNotEqual()
    {
        // Arrange
        var metadata1 = new Metadata("api", null, null);
        var metadata2 = new Metadata("api", 1001, null);

        // Act & Assert
        Assert.NotEqual(metadata1, metadata2);
    }

    [Fact]
    public void Metadata_IsRecord_SupportsCopyWithExpression()
    {
        // Arrange
        var originalMetadata = new Metadata("migration", 1001, DateTime.UtcNow);

        // Act
        var updatedMetadata = originalMetadata with { Source = "api" };

        // Assert
        Assert.Equal("api", updatedMetadata.Source);
        Assert.Equal(1001, updatedMetadata.LegacyId);
        Assert.Equal(originalMetadata.MigratedAt, updatedMetadata.MigratedAt);
    }
}
