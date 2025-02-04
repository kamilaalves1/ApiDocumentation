using ApiDocumentation.Application.Services.Interfaces;
using ApiDocumentation.Infrastructure.Services.Interface;
using static ApiDocumentation.Application.Services.DocumentationService;

namespace ApiDocumentation.Application.Services
{
    // DocumentationService.cs - Implementação do serviço na camada Application
    public class DocumentationService : IDocumentationService
    {
        private readonly IConfluenceService _confluenceService;
        private readonly ICodeAnalyzerService _codeAnalyzerService;

        public DocumentationService(IConfluenceService confluenceService, ICodeAnalyzerService codeAnalyzerService)
        {
            _confluenceService = confluenceService;
            _codeAnalyzerService = codeAnalyzerService;
        }

        public async Task<bool> GenerateOrUpdateDocumentation()
        {
            var exists = await CheckIfDocumentationExists();
            if (exists)
            {
                return await UpdateDocumentation();
            }
            return await _confluenceService.GenerateDocumentation();
        }

        public async Task<bool> CheckIfDocumentationExists()
        {
            return await _confluenceService.CheckIfDocumentationExists();
        }

        public async Task<bool> UpdateDocumentation()
        {
            return await _confluenceService.UpdateDocumentation();
        }

        public async Task<string> GenerateDocumentationFromSourceCode()
        {
            return await Task.FromResult("Documentação gerada automaticamente a partir do código.");
        }

        public async Task<bool> GenerateDocumentationFromCode()
        {
            var documentationContent = await _codeAnalyzerService.GenerateDocumentationFromSourceCode();

            if (!string.IsNullOrEmpty(documentationContent))
            {
                return await _confluenceService.PublishDocumentation(documentationContent);
            }

            return false;
        }

    }
}