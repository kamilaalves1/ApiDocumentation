namespace ApiDocumentation.Infrastructure.Services.Interface
{
    public interface IConfluenceService
    {
        Task<bool> GenerateDocumentation();
        Task<bool> CheckIfDocumentationExists();
        Task<bool> UpdateDocumentation();
        Task<bool> PublishDocumentation(string documentationContent);
    }
}