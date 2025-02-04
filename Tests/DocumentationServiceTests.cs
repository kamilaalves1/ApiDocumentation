using System.Threading.Tasks;
using ApiDocumentation.Application.Services;
using ApiDocumentation.Infrastructure.Services.Interface;
using Moq;
using Xunit;

public class DocumentationServiceTests
{
    private readonly Mock<IConfluenceService> _confluenceServiceMock;
    private readonly Mock<ICodeAnalyzerService> _codeAnalyzerServiceMock;
    private readonly DocumentationService _documentationService;

    public DocumentationServiceTests()
    {
        _confluenceServiceMock = new Mock<IConfluenceService>();
        _codeAnalyzerServiceMock = new Mock<ICodeAnalyzerService>();
        _documentationService = new DocumentationService(_confluenceServiceMock.Object, _codeAnalyzerServiceMock.Object);
    }

    [Fact]
    public async Task GenerateOrUpdateDocumentation_ShouldGenerate_WhenDocumentationDoesNotExist()
    {
        _confluenceServiceMock.Setup(x => x.CheckIfDocumentationExists()).ReturnsAsync(false);
        _confluenceServiceMock.Setup(x => x.GenerateDocumentation()).ReturnsAsync(true);

        var result = await _documentationService.GenerateOrUpdateDocumentation();

        Assert.True(result);
        _confluenceServiceMock.Verify(x => x.GenerateDocumentation(), Times.Once);
    }

    [Fact]
    public async Task GenerateOrUpdateDocumentation_ShouldUpdate_WhenDocumentationExists()
    {
        _confluenceServiceMock.Setup(x => x.CheckIfDocumentationExists()).ReturnsAsync(true);
        _confluenceServiceMock.Setup(x => x.UpdateDocumentation()).ReturnsAsync(true);

        var result = await _documentationService.GenerateOrUpdateDocumentation();

        Assert.True(result);
        _confluenceServiceMock.Verify(x => x.UpdateDocumentation(), Times.Once);
    }

    [Fact]
    public async Task GenerateDocumentationFromCode_ShouldPublish_WhenValidContent()
    {
        _codeAnalyzerServiceMock.Setup(x => x.GenerateDocumentationFromSourceCode()).ReturnsAsync("Generated Docs");
        _confluenceServiceMock.Setup(x => x.PublishDocumentation(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _documentationService.GenerateDocumentationFromCode();

        Assert.True(result);
        _confluenceServiceMock.Verify(x => x.PublishDocumentation(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GenerateDocumentationFromCode_ShouldFail_WhenEmptyContent()
    {
        _codeAnalyzerServiceMock.Setup(x => x.GenerateDocumentationFromSourceCode()).ReturnsAsync(string.Empty);

        var result = await _documentationService.GenerateDocumentationFromCode();

        Assert.False(result);
        _confluenceServiceMock.Verify(x => x.PublishDocumentation(It.IsAny<string>()), Times.Never);
    }
}
