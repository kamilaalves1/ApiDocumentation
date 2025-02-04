using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class PublishToConfluenceService : IPublishToConfluenceService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public PublishToConfluenceService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> PublishToConfluence(string documentation)
    {
        var confluenceUrl = $"{_configuration["Confluence:BaseUrl"]}/rest/api/content";
        var spaceKey = _configuration["Confluence:SpaceKey"];
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuration["Confluence:Email"]}:{_configuration["Confluence:ApiKey"]}"));

        var confluencePayload = new
        {
            type = "page",
            title = "Documentação Automática da API",
            space = new { key = spaceKey },
            body = new
            {
                storage = new
                {
                    value = $"<h1>Documentação da API</h1><p>{documentation}</p>",
                    representation = "storage"
                }
            }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, confluenceUrl)
        {
            Headers = { { "Authorization", $"Basic {authHeader}" } },
            Content = new StringContent(JsonConvert.SerializeObject(confluencePayload), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(requestMessage);
        return response.IsSuccessStatusCode;
    }
}
