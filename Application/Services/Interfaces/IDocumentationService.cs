namespace ApiDocumentation.Application.Services.Interfaces
{
    public interface IDocumentationService
    {
        Task<bool> GenerateOrUpdateDocumentation();
        Task<bool> GenerateDocumentationFromCode(); // Alterado para Task<bool>
        Task<bool> CheckIfDocumentationExists();
        Task<bool> UpdateDocumentation();
    }
}