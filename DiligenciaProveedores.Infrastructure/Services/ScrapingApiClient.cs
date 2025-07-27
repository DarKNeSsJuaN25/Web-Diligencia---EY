using DiligenciaProveedores.Application.Dtos.Scraping;
using DiligenciaProveedores.Application.Interfaces; 
using DiligenciaProveedores.Domain.Dtos.Screening;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DiligenciaProveedores.Infrastructure.Services
{
    public class ScrapingApiClient : IScrapingApiClient 
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScrapingApiClient> _logger;

        public ScrapingApiClient(HttpClient httpClient, ILogger<ScrapingApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ScrapingLoginResponseDto?> LoginScraperApiAsync(string username, string password, string tenantId)
        {
            try
            {
                var loginRequest = new
                {
                    username = username,
                    password = password,
                    tenant_id = tenantId
                };

                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("Attempting login to scraping API for user: {Username}", username);
                var response = await _httpClient.PostAsync("login", content);

                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();
                var loginResponse = await JsonSerializer.DeserializeAsync<ScrapingLoginResponseDto>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Login successful to scraping API for user: {Username}", username);
                return loginResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during scraping API login. Status: {StatusCode}", ex.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during scraping API login.");
                return null;
            }
        }

        public async Task<ScrapingResponseDto> ScrapeCompanyAsync(string companyName, string authToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var requestUri = $"scrape?nombre={Uri.EscapeDataString(companyName)}";
                _logger.LogInformation("Attempting scrape for '{CompanyName}' with token. URL: {RequestUri}", companyName, requestUri);

                var response = await _httpClient.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();
                var scrapingResults = await JsonSerializer.DeserializeAsync<ScrapingResponseDto>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Scraping successful for '{CompanyName}'.", companyName);
                return scrapingResults;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during scraping data retrieval for '{CompanyName}'. Status: {StatusCode}", companyName, ex.StatusCode);
                throw new InvalidOperationException($"Error communicating with the scraping API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during scraping for '{CompanyName}'.", companyName);
                throw new InvalidOperationException($"Unexpected error during scraping: {ex.Message}");
            }
        }
    }
}