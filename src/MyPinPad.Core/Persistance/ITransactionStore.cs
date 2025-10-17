using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Persistance
{
    public interface ITransactionStore
    {
        void Save(Transaction transaction);

        Transaction? Get(Guid transactionId);

        IEnumerable<Transaction> List();
    }
}
