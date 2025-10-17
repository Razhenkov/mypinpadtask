namespace MyPinPad.UI.Models
{
    public record Transaction
    (
        Guid Id,
        string Status,
        DateTime ProcessedAt,
        Dictionary<string, string> TlvTags
    );
}
