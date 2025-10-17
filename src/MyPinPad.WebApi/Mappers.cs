using MyPinPad.Core.Domains;
using MyPinPad.Core.Dtos;
using MyPinPad.WebApi.Models.Responses;

namespace MyPinPad.WebApi
{
    public class Mappers
    {
        public static TransactionListItemResponse MapToTransactionListItem(TransactionListItemDto dto)
        {
            return new TransactionListItemResponse(dto.PublicId, MapTransactionStatus(dto.Decision), dto.ProcessedAt);
        }

        public static TransactionResponse MapToTransactionResponse(TransactionDto dto)
        {
            return new TransactionResponse(
                dto.PublicId,
                MapTransactionStatus(dto.Decision),
                dto.ProcessedAt,
                dto.TlvTags);
        }

        private static TransactionStatus MapTransactionStatus(Decision status)
        {
            return status switch
            {
                Decision.Approved => TransactionStatus.Approved,
                Decision.Declined => TransactionStatus.Declined,
                _ => throw new ArgumentException($"Unknown mapping between {nameof(Decision)} and {nameof(TransactionStatus)}")
            };
        }
    }
}
