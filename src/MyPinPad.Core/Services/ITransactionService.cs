using MyPinPad.Core.Dtos;

namespace MyPinPad.Core.Services
{
    public interface ITransactionService
    {
        TransactionDto ProcessTransaction(string emvHex);

        IEnumerable<TransactionListItemDto> ListTransactions();

        TransactionDto? GetTransactionDetails(Guid id);
    }
}
