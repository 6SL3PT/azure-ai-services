namespace SlipIntelligence.Application.Extensions;

public class ResponseMessageStatus {
    public int Code { get; set; } = 1000;
    public string CustomMessage { set; private get; } = string.Empty;
    public string Description {
        get => Code switch {
            1000 => $"Success.{CustomMessage}",
            4001 => $"Business error with 1 errors.{CustomMessage}",
            4002 => $"Business error with more than 1 errors.{CustomMessage}",
            _ => $"{CustomMessage}",
        };
    }
}
