using Microsoft.Extensions.Logging;

/*
In this scenario, strong-types are coming from business model which is not tight to logs.

* It is common for developers to log something that involves complex business types.
* The developer could have no control over these business types - e.g. these types could come from a 3rd party library.
* A log entry might contain multiple business types.
*/

// this type is not owned by the developer (e.g. coming from a 3rd party library)
public class Food
{
    public string BrandName;
    public string ProductDescription;
    public string ProductType;
    public string CompanyName;
}

// this type is owned by the developer
public class FoodRecallLogRecord
{
    public Food Food;
    public int ProductCode;
    public string RecallReasonDescription;
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
                Food = new Food {
                    BrandName = "Contoso",
                    ProductDescription = "Salads",
                    ProductType = "Food & Beverages",
                    CompanyName = "Contoso Fresh Vegetables, Inc.",
                },
                ProductCode = 123,
                RecallReasonDescription = "due to a possible health risk from Listeria monocytogenes",
            },
            exception: null,
            formatter: (state, ex) =>
            {
                var record = state as FoodRecallLogRecord;
                return $"A `{record.Food.ProductType}` (#{record.ProductCode}) recall notice was published for `{record.Food.BrandName} {record.Food.ProductDescription}` produced by `{record.Food.CompanyName}` ({record.RecallReasonDescription}).";
            }
        );
    }
}
