using PublicApiGenerator;

namespace Altinn.FileAnalyzers.Tests;

public class PublicApiTests
{
    private static readonly string[] _excludedAttributes =
    [
        "System.Runtime.CompilerServices.RefSafetyRulesAttribute",
        "System.Diagnostics.DebuggerNonUserCodeAttribute",
        "System.ComponentModel.EditorBrowsableAttribute",
    ];

    [Fact]
    public async Task PublicApi_ShouldNotChange_Unintentionally()
    {
        // Arrange
        var assembly = typeof(Altinn.FileAnalyzers.MimeType.ServiceCollectionExtensions).Assembly;

        // Act
        var publicApi = assembly.GeneratePublicApi(
            new ApiGeneratorOptions { ExcludeAttributes = _excludedAttributes }
        );

        // Assert
        await Verify(publicApi);
    }
}
