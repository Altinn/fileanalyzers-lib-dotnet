using Altinn.App.Core.Features.FileAnalysis;
using Altinn.App.Core.Models.Validation;
using Altinn.FileAnalyzers.MimeType;
using Altinn.Platform.Storage.Interface.Models;
using FluentAssertions;

namespace Altinn.FileAnalyzers.Tests.MimeType;

public class MimeTypeValidatorTests
{
    [Fact]
    public async Task Validate_MimeTypeCorrect_ShouldValidateOk()
    {   // Simulate config in applicationMetadata.json
        var dataType = new DataType()
        {
            EnabledFileValidators = new List<string>() { "mimeTypeValidator" },
            AllowedContentTypes = new List<string>() { "application/pdf" }
        };

        // Simulate file analysis result
        IEnumerable<FileAnalysisResult> fileAnalysisResults = new List<FileAnalysisResult>()
        {
            new FileAnalysisResult("mimeTypeValidator")
            {
                MimeType = "application/pdf",
                Filename = "test.pdf",
                Extensions = new List<string>() { "pdf" }
            }
        };

        var validator = new MimeTypeValidator();
        (bool success, IEnumerable<ValidationIssue> errors) = await validator.Validate(dataType, fileAnalysisResults);

        success.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_MimeTypeCorrect_ShouldReturnWithError()
    {   // Simulate config in applicationMetadata.json
        var dataType = new DataType()
        {
            EnabledFileValidators = new List<string>() { "mimeTypeValidator" },
            AllowedContentTypes = new List<string>() { "application/pdf" }
        };

        // Simulate file analysis result
        IEnumerable<FileAnalysisResult> fileAnalysisResults = new List<FileAnalysisResult>()
        {
            new FileAnalysisResult("mimeTypeValidator")
            {
                MimeType = "application/pdfx",
                Filename = "test.png",
                Extensions = new List<string>() { "png" }
            }
        };

        var validator = new MimeTypeValidator();
        (bool success, IEnumerable<ValidationIssue> errors) = await validator.Validate(dataType, fileAnalysisResults);

        success.Should().BeFalse();
        errors.FirstOrDefault()?.Code.Should().Be(ValidationIssueCodes.DataElementCodes.ContentTypeNotAllowed);
    }
}