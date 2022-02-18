using Microsoft.Extensions.Logging;

[StructuredLogMessage(
    EventId = 100,
    LogLevel = LogLevel.Critical,
    Message = "A `{ProductType}` (#{ProductCode}) recall notice was published for `{BrandName} {ProductDescription}` produced by `{CompanyName}` ({RecallReasonDescription}).")]
public class FoodSupplyLogs
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

        logger.Log(new FoodSupplyLogs
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
