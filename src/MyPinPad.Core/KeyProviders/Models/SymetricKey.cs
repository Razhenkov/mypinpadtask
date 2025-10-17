namespace MyPinPad.Core.KeyProviders.Models
{
    public class SymetricKey : KeyBase
    {
        public SymetricKey(string id, byte[] sharedKey)
            : base(id)
        {
            SharedKey = sharedKey;
        }

        public byte[] SharedKey { get; }
    }
}