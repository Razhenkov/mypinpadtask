namespace MyPinPad.Core.Validators
{
    public interface ITransactionValidator
    {
        bool IsValid(string emvHex, string signature, Dictionary<string, byte[]> tags);
    }
}
