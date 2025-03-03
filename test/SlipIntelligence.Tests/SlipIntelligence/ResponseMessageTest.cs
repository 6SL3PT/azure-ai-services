using SlipIntelligence.Application.Extensions;

namespace SlipIntelligence.Tests.SlipIntelligence;

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
}
