using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Dtos
{
    public record TransactionListItemDto
    (
        Guid Id,
        string TransactionStatus,
        DateTime ProcessedAt
    );
}
