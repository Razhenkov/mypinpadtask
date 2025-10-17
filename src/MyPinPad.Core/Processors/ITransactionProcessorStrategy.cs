using MyPinPad.Core.Domains;

namespace MyPinPad.Core.Processors
{   
    public interface ITransactionProcessorStrategy
    {
        Decision Process(Dictionary<string, byte[]> tags);
    }
}
