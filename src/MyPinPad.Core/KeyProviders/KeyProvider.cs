using MyPinPad.Core.KeyProviders.Models;

namespace MyPinPad.Core.KeyProviders
{
    public class LocalKeyProvider : IKeyProvider
    {
        public SymetricKey GetSymetricKey(string keyId)
        {
            var base64Key = Environment.GetEnvironmentVariable(keyId);

            if (base64Key == null)
                throw new ArgumentNullException(nameof(keyId));

            return new SymetricKey(keyId, Convert.FromBase64String(base64Key));
        }

        public AsymetricKey GetAsymetricKey(string keyId)
        {
            var publicKey = $"{keyId}_PUBLIC_KEY";
            var privateKey = $"{keyId}_PRIVATE_KEY";

            var base64PublicKey = Environment.GetEnvironmentVariable(publicKey);
            var base64PrivateKey = Environment.GetEnvironmentVariable(privateKey);

            if (base64PublicKey == null)
                throw new ArgumentNullException(nameof(publicKey));

            if (base64PrivateKey == null)
                throw new ArgumentNullException(nameof(privateKey));

            return new AsymetricKey(keyId, Convert.FromBase64String(base64PublicKey), Convert.FromBase64String(base64PrivateKey));
        }
    }
}