using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Processors
{
    public class TransactionProcessor: ITransactionProcessor
    {
        private readonly ITransactionProcessorStrategy _transactionProcessorStrategy;

        public TransactionProcessor(ITransactionProcessorStrategy transactionProcessorStrategy)
        {
            _transactionProcessorStrategy = transactionProcessorStrategy;
        }

        public Decision ProcessTransaction(Dictionary<string, byte[]> tags)
        {
            var transactionStatus = _transactionProcessorStrategy.Process(tags);

            return transactionStatus;
        }
    }
}
