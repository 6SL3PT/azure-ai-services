namespace SlipIntelligence.Application.Extensions {
    public class ResponseMessage<T> {
        public ResponseMessageStatus Status { set; get; }
        public object Data { get; }
        public ResponseMessage(object value) {
            Status = new ResponseMessageStatus();
            if(value is ErrorMessage errorMessage) {
                if(errorMessage.MetaData != null && errorMessage.MetaData.Count > 1) {
                    Status.Code = 4002;
                } else {
                    Status.Code = 4001;
                }
            }
            Data = value;
        }
    }

    public class ResponseMessageStatus {
        public int Code { get; set; } = 1000;
        public string CustomMessage { set; private get; } = string.Empty;
        public string Description {
            get {
                switch(Code) {
                    case 1000:
                        return $"Success.{CustomMessage}";
                    case 4001:
                        return $"Business error with  1 errors.{CustomMessage}";
                    case 4002:
                        return $"Business error with more than 1 errors.{CustomMessage}";
                    default:
                        return $"{CustomMessage}";
                }
            }
        }
    }

    public class ErrorMessage {
        public string SubCode { get; set; }
        public string Description { set; get; }
        public Dictionary<string, object> MetaData { set; get; }
        public ErrorMessage(int code, string customMessage = "") {
            MetaData = new Dictionary<string, object>();
            SubCode = $"E{code}";
            Description = code switch {
                1101 => $"Missing required parameters.{customMessage}",
                1102 => $"Invalid parameters enterd.{customMessage}",
                1103 => $"Empty string input not supported.{customMessage}",
                1104 => $"Requested entity record does not exist.{customMessage}",
                1105 => $"Unrecognized field name was entered - Please check spelling and/or refer to the API docs for correct name.{customMessage}",
                1111 => $"ข้อมูลซ้ำกับที่มีอยู่แล้ว ไม่สามารถ Insert หรือ Update ได้\r\nData entry duplicated with existing.{customMessage}",
                9102 => $"Missing required authorization credentials.{customMessage}",
                _ => string.Empty,
            };
        }
    }
}
