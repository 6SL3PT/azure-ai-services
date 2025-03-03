namespace SlipIntelligence.Domain;

public class AnalyzeResultDto
{
    public string ApiVersion { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, SlipField>? Fields { get; set; }
}
