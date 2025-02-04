// Program.cs - Configuração inicial do projeto
using ApiDocumentation.Application.Services.Interfaces;
using ApiDocumentation.Application.Services;
using ApiDocumentation.Infrastructure.Services.Interface;
using ApiDocumentation.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


builder.Services.AddScoped<IDocumentationService, DocumentationService>();
builder.Services.AddScoped<IConfluenceService, ConfluenceService>();
builder.Services.AddScoped<ICodeAnalyzerService, CodeAnalyzerService>();
builder.Services.AddScoped<IConfluenceService, ConfluenceService>();
builder.Services.AddScoped<IPublishToConfluenceService, PublishToConfluenceService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Executar geração de documentação ao iniciar em produção
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var docService = scope.ServiceProvider.GetRequiredService<IDocumentationService>();

    var documentationExists = docService.CheckIfDocumentationExists().Result;
    if (documentationExists)
    {
        docService.UpdateDocumentation().Wait();
    }
    else
    {
        await docService.GenerateDocumentationFromCode();
    }
}

app.Run();