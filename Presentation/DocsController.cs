using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

[ApiController]
[Route("api/docs")]
   public class DocsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IPublishToConfluenceService _confluenceService;

        public DocsController(HttpClient httpClient, IConfiguration configuration, IPublishToConfluenceService confluenceService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _confluenceService = confluenceService; 
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDocs()
        {
            // prompt detalhado
            var objetivo = _configuration["ApiDocumentation:Objetivo"] ?? "Não definido.";
            var repositorio = _configuration["ApiDocumentation:Repositorio"] ?? "Não definido.";

            var prompt = $@"
            Analise o documento e gere a documentação completa desta API seguindo o formato abaixo: 
            1. **Sumário**: Crie um sumário da API.
            2. **Objetivo**:  {objetivo}  
            3. **Arquitetura**: Descreva a arquitetura do projeto, incluindo um diagrama de classes e explique a responsabilidade de cada camada dentro da api. Explique também o que cada classe faz,
            3.1.  **Tecnologia**  Descreva também a tecnologia que está sendo utilizada para construção da api
            3.2 . **Fluxo de Trabalho**: Gere um diagrama de sequência baseado na ordem das chamadas
            3.3.  **Requisitos  mínimos**: Detalhe os requisitos mínimos de para executar a api
            4. **Mensagens de Erro**:  Liste os padrões e exemplos de erro que a Api reotrna
            5. **Segurança**:  Descreva o tipo de segurança da api
            6. **Como executar a Aplicação**:  Descreva a forma de executar a api
            7. **Endereço do Repositório**:  {repositorio}";

            // enviar os dados pro ChatGPT
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("Erro: Chave da OpenAI não foi configurada corretamente.");
            }

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "Você é um arquiteto de soluções que gera documentação de API a partir de leitura de código." },
                new { role = "user", content = prompt }
            }
            };
            
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            //enviar a documentação no confluence usando o serviço
            bool published = await _confluenceService.PublishToConfluence(responseContent);

            if (published)
                return Ok("Documentação gerada e publicada no Confluence!");
            else
                return StatusCode(500, "Erro ao publicar no Confluence.");
        }
    }

