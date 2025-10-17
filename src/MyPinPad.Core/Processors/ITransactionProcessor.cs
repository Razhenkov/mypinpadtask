using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Processors
{
    public interface ITransactionProcessor
    {
        Decision ProcessTransaction(Dictionary<string, byte[]> tags);
    }
}
