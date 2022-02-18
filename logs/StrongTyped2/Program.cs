using Microsoft.Extensions.Logging;

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

        logger.Log(
            logLevel: LogLevel.Critical,
            eventId: 100,
            state: new FoodRecallLogRecord
            {
                BrandName = "Contoso",
                ProductDescription = "Salads",
                ProductType = "Food & Beverages",
                ProductCode = 123,
                RecallReasonDescription = "due to a possible health risk from Listeria monocytogenes",
                CompanyName = "Contoso Fresh Vegetables, Inc.",
            },
            exception: null,
            formatter: (state, ex) =>
            {
                var record = state as FoodRecallLogRecord;
                return $"A `{record.ProductType}` (#{record.ProductCode}) recall notice was published for `{record.BrandName} {record.ProductDescription}` produced by `{record.CompanyName}` ({record.RecallReasonDescription}).";
            }
        );
    }
}
