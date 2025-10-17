namespace MyPinPad.Core.KeyProviders.Models
{
    public abstract class KeyBase
    {
        public KeyBase(string keyId)
        {
            KeyId = keyId;
        }

        public string KeyId { get; }
    }
}