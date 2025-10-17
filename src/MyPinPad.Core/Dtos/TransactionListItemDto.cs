using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Dtos
{
    public record TransactionListItemDto
    (
        Guid PublicId,
        Decision Decision,
        DateTime ProcessedAt
    );
}
