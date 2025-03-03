namespace SlipIntelligence.Application.Models;

public class AnalyzeResultResponse {
    public string ApiVersion { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, SlipFieldDto>? Fields { get; set; }
}
