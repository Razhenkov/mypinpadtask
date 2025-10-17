using MyPinPad.Core.Exceptions;
using System.Security.Cryptography;

namespace MyPinPad.Core.EncryptionAlgorithms
{
    public sealed class AesGcmEncryptionAlgorithm : IEncryptionAlgorithm
    {
        const int AES_KEY_LENGTH = 32;
        const int NONCE_LENGTH = 12;
        const int TAG_SIZE_BYTES = 16;

        private byte[] _aesKey;

        public AesGcmEncryptionAlgorithm()
        {
        }

        public byte[] GetKey() => _aesKey;

        public void GenerateKey()
        { 
            _aesKey = new byte[AES_KEY_LENGTH];
            RandomNumberGenerator.Fill(_aesKey);
        }

        public byte[] Encrypt(byte[] tlvBytes)
        {
            if (tlvBytes == null)
                throw new ArgumentNullException(nameof(tlvBytes));

            try
            {
                byte[] nonce = RandomNumberGenerator.GetBytes(NONCE_LENGTH);
                byte[] cipherTextBytes = new byte[tlvBytes.Length];
                byte[] tag = new byte[TAG_SIZE_BYTES];

                using (var aes = new AesGcm(_aesKey, TAG_SIZE_BYTES))
                {
                    aes.Encrypt(nonce, tlvBytes, cipherTextBytes, tag);
                }

                byte[] combined = new byte[nonce.Length + tag.Length + cipherTextBytes.Length];

                Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
                Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
                Buffer.BlockCopy(cipherTextBytes, 0, combined, nonce.Length + tag.Length, cipherTextBytes.Length);

                return combined;
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("Failed to encrypt data", ex);
            }
        }

        public byte[] Decrypt(byte[] decryptedTlvBytes, byte[] key)
        {
            if (decryptedTlvBytes == null)
                throw new ArgumentNullException(nameof(decryptedTlvBytes));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            try
            {
                byte[] nonce = new byte[NONCE_LENGTH];
                byte[] tag = new byte[TAG_SIZE_BYTES];
                byte[] ciphertext = new byte[decryptedTlvBytes.Length - nonce.Length - tag.Length];

                Buffer.BlockCopy(decryptedTlvBytes, 0, nonce, 0, nonce.Length);
                Buffer.BlockCopy(decryptedTlvBytes, nonce.Length, tag, 0, tag.Length);
                Buffer.BlockCopy(decryptedTlvBytes, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

                byte[] plaintext = new byte[ciphertext.Length];

                using (var aes = new AesGcm(key, TAG_SIZE_BYTES))
                {
                    aes.Decrypt(nonce, ciphertext, tag, plaintext);
                }

                return plaintext;
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("Failed to decrypt data", ex);
            }
        }
    }
}