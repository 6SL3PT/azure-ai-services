namespace SlipIntelligence.Domain;

public class SlipField
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float? Confidence { get; set; }
}