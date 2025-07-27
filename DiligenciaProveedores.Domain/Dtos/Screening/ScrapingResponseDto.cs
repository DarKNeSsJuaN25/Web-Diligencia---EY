using System.Text.Json.Serialization;

namespace DiligenciaProveedores.Domain.Dtos.Screening
{
    public class ScrapingResponseDto
    {
        [JsonPropertyName("hits")]
        public int Hits { get; set; }

        [JsonPropertyName("resultados")]
        public List<ScrapingResultDto> Resultados { get; set; } = new List<ScrapingResultDto>();
    }

    public class ScrapingResultDto
    {
        [JsonPropertyName("Firm Name")]
        public string FirmName { get; set; } = string.Empty;

        [JsonPropertyName("Additional Info")]
        public string AdditionalInfo { get; set; } = string.Empty;

        [JsonPropertyName("Address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("Country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("From")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("To")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("Grounds")]
        public string Grounds { get; set; } = string.Empty;
    }
}