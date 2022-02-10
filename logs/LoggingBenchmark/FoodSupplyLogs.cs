using Microsoft.Extensions.Logging;

public static partial class FoodSupplyLogs
{
    [LoggerMessage(
        EventId = 100,
        Message = "A `{productType}` recall notice was published for `{brandName} {productDescription}` produced by `{companyName}` ({recallReasonDescription}).")]
    public static partial void FoodRecallNotice(
        this ILogger logger,
        LogLevel logLevel,
        string brandName,
        string productDescription,
        string productType,
        string recallReasonDescription,
        string companyName);
}
