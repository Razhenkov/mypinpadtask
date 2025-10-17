namespace MyPinPad.UI.Models
{
    public record TransactionListItem
    (
        Guid Id,
        string Status,
        DateTime ProcessedAt
    );
}
