using Altinn.App.Core.Features.FileAnalysis;
using Altinn.App.Core.Features.Validation;
using Altinn.App.Core.Models.Validation;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.FileAnalyzers.MimeType;

/// <summary>
/// Validates that the file is of the allowed content and uploaded file extension is the same as filename extension.
/// </summary>
public class MimeTypeValidator : IFileValidator
{
    /// <summary>
    /// The unique identifier for the validator to be used when enabling it from config.
    /// </summary>
    public string Id { get; private set; } = "mimeTypeValidator";

    /// <summary>
    /// Validates that the file is of the allowed content type and uploaded file extension is the same as filename extension.
    /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously. Suppressed because of the interface.
    public async Task<(bool Success, IEnumerable<ValidationIssue> Errors)> Validate(DataType dataType, IEnumerable<FileAnalysisResult> fileAnalysisResults)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        List<ValidationIssue> errors = new();

        var fileMimeTypeResult = fileAnalysisResults.FirstOrDefault(x => x.MimeType != null);
        if (fileMimeTypeResult == null) return (true, errors);

        // Verify that uploaded file extension is the same as filename extension

        foreach (var fileExtension in fileMimeTypeResult.Extensions)
        {
            if (fileMimeTypeResult.Filename != null && !fileMimeTypeResult.Filename.EndsWith(fileExtension))
            {
                ValidationIssue error = new()
                {
                    Source = "File",
                    Code = "Uploaded file extension is not the same as filename extension", // TODO - Add correct code
                    Severity = ValidationIssueSeverity.Error,
                    Description = $"The {fileMimeTypeResult.Filename} filename does not appear to have the same extension as uploaded file. File extension on uploaded file is .{fileExtension}"
                };

                errors.Add(error);
            }
        }

        // Verify that file mime type is an allowed content-type
        if (!dataType.AllowedContentTypes.Contains(fileMimeTypeResult?.MimeType, StringComparer.InvariantCultureIgnoreCase) && !dataType.AllowedContentTypes.Contains("application/octet-stream"))
        {
            ValidationIssue error = new()
            {
                Source = "File",
                Code = ValidationIssueCodes.DataElementCodes.ContentTypeNotAllowed,
                Severity = ValidationIssueSeverity.Error,
                Description = $"The {fileMimeTypeResult?.Filename + " "}file does not appear to be of the allowed content type according to the configuration for data type {dataType.Id}. Allowed content types are {string.Join(", ", dataType.AllowedContentTypes)}"
            };

            errors.Add(error);
        }

        return errors.Any() ? (false, errors) : (true, errors);
    }
}
