namespace MyPinPad.Core.KeyProviders.Models
{
    public class AsymetricKey : KeyBase
    {
        public AsymetricKey(string keyId, byte[] publicKey, byte[] privateKey)
            : base(keyId)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public byte[] PublicKey { get; }

        public byte[] PrivateKey { get; }
    }
}
