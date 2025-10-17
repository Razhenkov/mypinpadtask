using MyPinPad.Core.Domains;
using System.Collections.Concurrent;

namespace MyPinPad.Core.Persistance
{
    public class TransactionStore: ITransactionStore
    {
        private readonly ConcurrentDictionary<Guid, Transaction> _transactions = new();

        public void Save(Transaction transaction)
        {
            _transactions[transaction.PublicId] = transaction;
        }

        public Transaction? Get(Guid transactionId)
        {
            _transactions.TryGetValue(transactionId, out Transaction? transaction);
            
            return transaction?.Clone() as Transaction;
        }

        public IEnumerable<Transaction> List()
        { 
            return _transactions.Values;
        }
    }
}