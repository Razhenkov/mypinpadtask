using MyPinPad.Core.Domains;
using MyPinPad.Core.EMV;
using MyPinPad.Core.Extensions;

namespace MyPinPad.Core.Processors
{
    public sealed class OnlyPurchaseTransactionProcessorStrategy : ITransactionProcessorStrategy
    {
        const string TRANSACTION_TYPE_PURCHASE_CODE = "00";

        public Decision Process(Dictionary<string, byte[]> tags)
        {
            if (!tags.ContainsKey(TLVDictionary.TransactionType))
                return Decision.Declined;

            var transactionType = tags[TLVDictionary.TransactionType].BytesToHex();

            return transactionType == TRANSACTION_TYPE_PURCHASE_CODE ?
                Decision.Approved : Decision.Declined;
        }
    }
}
