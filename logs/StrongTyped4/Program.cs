using Microsoft.Extensions.Logging;

public static partial class FoodSupplyLogs
{
    [LoggerMessage(
        EventId = 100,
        Message = "A `{productType}` (#{productCode}) recall notice was published for `{brandName} {productDescription}` produced by `{companyName}` ({recallReasonDescription}).")]
    public static partial void FoodRecallNotice(
        this ILogger logger,
        LogLevel logLevel,
        string brandName,
        string productDescription,
        string productType,
        int productCode,
        string recallReasonDescription,
        string companyName);
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

        logger.FoodRecallNotice(
            logLevel: LogLevel.Critical,
            brandName: "Contoso",
            productDescription: "Salads",
            productType: "Food & Beverages",
            productCode: 123,
            recallReasonDescription: "due to a possible health risk from Listeria monocytogenes",
            companyName: "Contoso Fresh Vegetables, Inc.");
    }
}
