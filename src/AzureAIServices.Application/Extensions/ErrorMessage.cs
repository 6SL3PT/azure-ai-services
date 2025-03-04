namespace AzureAIServices.Application.Extensions;

public class ErrorMessage {
    public string SubCode { get; set; }
    public string Description { set; get; }
    public Dictionary<string, object> MetaData { set; get; }
    public ErrorMessage(int code, string customMessage = "") {
        MetaData = new Dictionary<string, object>();
        SubCode = $"E{code}";
        Description = code switch {
            1101 => $"Missing required parameters.{customMessage}",
            1102 => $"Invalid parameters entered.{customMessage}",
            1103 => $"Empty string input not supported.{customMessage}",
            1104 => $"Requested entity record does not exist.{customMessage}",
            1105 => $"Unrecognized field name was entered - Please check spelling and/or refer to the API docs for correct name.{customMessage}",
            1111 => $"ข้อมูลซ้ำกับที่มีอยู่แล้ว ไม่สามารถ Insert หรือ Update ได้\r\nData entry duplicated with existing.{customMessage}",
            9102 => $"Missing required authorization credentials.{customMessage}",
            _ => string.Empty,
        };
    }
}
