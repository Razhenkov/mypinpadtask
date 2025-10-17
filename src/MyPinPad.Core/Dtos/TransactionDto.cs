using MyPinPad.Core.Domains;
namespace MyPinPad.Core.Dtos
{
    public record TransactionDto
    (
        Guid PublicId,
        Decision Decision,
        DateTime ProcessedAt,
        Dictionary<string, string> TlvTags
    );
}
