using System.Text.Json.Serialization;

namespace MyPinPad.WebApi.Models.Responses
{
    public record TransactionListItemResponse(
        Guid Id,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] TransactionStatus Status, 
        DateTime ProcessedAt);
}
