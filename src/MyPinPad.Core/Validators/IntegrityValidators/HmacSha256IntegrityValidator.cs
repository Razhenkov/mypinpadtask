using MyPinPad.Core.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace MyPinPad.Core.Validators.IntegrityValidators
{
    public class HmacSha256IntegrityValidator : IIntegrityValidator
    {
        private readonly byte[] _key;

        public HmacSha256IntegrityValidator(byte[] key)
        {
            _key = key;
        }

        public string Compute(string data)
        {
            try
            {
                using var hmac = new HMACSHA256(_key);

                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("An error occurred while computing integrity", ex);
            }
        }

        public bool Verify(string data, string signature)
        {
            try
            {
                var computed = Compute(data);
                return computed.Equals(signature, StringComparison.OrdinalIgnoreCase);
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("An error occurred while verifying integrity", ex);
            }
        }
    }
}