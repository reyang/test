using Microsoft.Extensions.Logging;

/*
In this scenario, strong-types are defined to represent "custom events", which can be considered as a self-contained log entry.

* It is common for large organization to have a "core/infra" team defining/owning the log schema.
* The developer has control over these "custom event" types.
* There might be inheritance among these "custom event" types.

FoodRecallLogRecord:
  * BrandName
  * ProductDescription
  * ProductType
  * ProductCode
  * RecallReasonDescription
  * CompanyName

BaseLogRecord:
  * EventId
  * LogLevel
  * Formatter (message string template)
  * Exception (callstack)
*/

[StructuredLogMessage(
    EventId = 100,
    LogLevel = LogLevel.Critical,
    Message = "A `{ProductType}` (#{ProductCode}) recall notice was published for `{BrandName} {ProductDescription}` produced by `{CompanyName}` ({RecallReasonDescription}).")]
public class FoodRecallLogRecord
{
    public string BrandName;
    public string ProductDescription;
    public string ProductType;
    public int ProductCode;
    public string RecallReasonDescription;
    public string CompanyName;
}

class Program
{
    static void Main()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<Program>();

        logger.Log(new FoodRecallLogRecord
        {
            BrandName = "Contoso",
            ProductDescription = "Salads",
            ProductType = "Food & Beverages",
            ProductCode = 123,
            RecallReasonDescription = "due to a possible health risk from Listeria monocytogenes",
            CompanyName = "Contoso Fresh Vegetables, Inc.",
        });
    }
}
