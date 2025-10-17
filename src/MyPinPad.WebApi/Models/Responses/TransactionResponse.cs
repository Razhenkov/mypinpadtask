using System.Text.Json.Serialization;

namespace MyPinPad.WebApi.Models.Responses
{
    public record TransactionResponse(
        Guid Id,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] TransactionStatus Status,
        DateTime ProcessedAt,
        Dictionary<string, string> TlvTags);
}
