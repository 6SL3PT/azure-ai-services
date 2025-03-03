namespace SlipIntelligence.Application.Extensions;
public class ResponseMessage<T> {
    public ResponseMessageStatus Status { set; get; }
    public object Data { get; }
    public ResponseMessage(object value) {
        Status = new ResponseMessageStatus();
        if(value is ErrorMessage errorMessage) {
            _ = errorMessage.MetaData?.Count > 1
                ? Status.Code = 4002
                : Status.Code = 4001;
        }
        Data = value;
    }
}
