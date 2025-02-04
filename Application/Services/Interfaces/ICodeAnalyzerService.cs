public interface ICodeAnalyzerService
{
    Task<string> GenerateDocumentationFromSourceCode();
}

