using AzureAIServices.Application.Extensions;

namespace AzureAIServices.Tests.AzureAIServices;

public class ResponseMessageTests
{
    [Fact]
    public void ResponseMessage_WithErrorMessageAndSingleMetadata_SetsStatusCodeTo4001()
    {
        // Arrange
        var errorMessage = new ErrorMessage(1101)
        {
            MetaData = new Dictionary<string, object> { { "key", "value" } }
        };

        // Act
        var responseMessage = new ResponseMessage<ErrorMessage>(errorMessage);

        // Assert
        Assert.Equal(4001, responseMessage.Status.Code);
    }

    [Fact]
    public void ResponseMessage_WithErrorMessageAndMultipleMetadata_SetsStatusCodeTo4002()
    {
        // Arrange
        var errorMessage = new ErrorMessage(1101)
        {
            MetaData = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            }
        };

        // Act
        var responseMessage = new ResponseMessage<ErrorMessage>(errorMessage);

        // Assert
        Assert.Equal(4002, responseMessage.Status.Code);
    }

    [Fact]
    public void ResponseMessage_WithErrorMessageAndNoMetadata_SetsStatusCodeTo4001()
    {
        // Arrange
        var errorMessage = new ErrorMessage(1101); // No metadata

        // Act
        var responseMessage = new ResponseMessage<ErrorMessage>(errorMessage);

        // Assert
        Assert.Equal(4001, responseMessage.Status.Code);
    }

    [Fact]
    public void ResponseMessage_WithNonErrorMessage_SetsStatusCodeTo1000()
    {
        // Arrange
        var successValue = new { Message = "Success" };

        // Act
        var responseMessage = new ResponseMessage<object>(successValue);

        // Assert
        Assert.Equal(1000, responseMessage.Status.Code);
    }

    [Fact]
    public void ResponseMessageStatus_Description_WithStatusCode1000_ReturnsCorrectDescription()
    {
        // Arrange
        var status = new ResponseMessageStatus
        {
            Code = 1000,
            CustomMessage = "Custom success message."
        };

        // Act
        var description = status.Description;

        // Assert
        Assert.Equal("Success.Custom success message.", description);
    }

    [Fact]
    public void ResponseMessageStatus_Description_WithStatusCode4001_ReturnsCorrectDescription()
    {
        // Arrange
        var status = new ResponseMessageStatus
        {
            Code = 4001,
            CustomMessage = "Business error with 1 error."
        };

        // Act
        var description = status.Description;

        // Assert
        Assert.Equal("Business error with 1 errors.Business error with 1 error.", description);
    }

    [Fact]
    public void ResponseMessageStatus_Description_WithStatusCode4002_ReturnsCorrectDescription()
    {
        // Arrange
        var status = new ResponseMessageStatus
        {
            Code = 4002,
            CustomMessage = "Business error with multiple errors."
        };

        // Act
        var description = status.Description;

        // Assert
        Assert.Equal("Business error with more than 1 errors.Business error with multiple errors.", description);
    }

    [Theory]
    [InlineData(1101, "Missing required parameters.")]
    [InlineData(1102, "Invalid parameters entered.")]
    [InlineData(1103, "Empty string input not supported.")]
    [InlineData(1104, "Requested entity record does not exist.")]
    [InlineData(1105, "Unrecognized field name was entered - Please check spelling and/or refer to the API docs for correct name.")]
    [InlineData(1111, "ข้อมูลซ้ำกับที่มีอยู่แล้ว ไม่สามารถ Insert หรือ Update ได้\r\nData entry duplicated with existing.")]
    [InlineData(9102, "Missing required authorization credentials.")]
    public void Test_ValidErrorCodes_SetCorrectDescription(int code, string expectedDescription)
    {
        // Arrange & Act
        var errorMessage = new ErrorMessage(code);

        // Assert
        Assert.Equal(expectedDescription, errorMessage.Description);
    }

    [Fact]
    public void Test_UnrecognizedCode_ShouldSetDescriptionToEmpty()
    {
        // Arrange
        int unrecognizedCode = 9999;

        // Act
        var errorMessage = new ErrorMessage(unrecognizedCode);

        // Assert
        Assert.Equal(string.Empty, errorMessage.Description);
    }

    [Theory]
    [InlineData(1101, "Custom message", "Missing required parameters.Custom message")]
    [InlineData(1102, "Please check parameters", "Invalid parameters entered.Please check parameters")]
    [InlineData(9102, "Missing credentials", "Missing required authorization credentials.Missing credentials")]
    public void Test_WithCustomMessage_ShouldAppendCorrectly(int code, string customMessage, string expectedDescription)
    {
        // Arrange & Act
        var errorMessage = new ErrorMessage(code, customMessage);

        // Assert
        Assert.Equal(expectedDescription, errorMessage.Description);
    }
}
