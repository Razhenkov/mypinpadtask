using MyPinPad.Core.Exceptions;
using MyPinPad.Core.KeyProviders.Models;
using System.Security.Cryptography;

namespace MyPinPad.Core.EncryptionAlgorithms
{
    public class DEKEncryptionService: IDEKEncryptionService
    {
        private readonly RSA _kek;
        private readonly string _keyId;

        public DEKEncryptionService(AsymetricKey key)
        {
            _kek = RSA.Create();
            _kek.ImportSubjectPublicKeyInfo(key.PublicKey, out _);
            _kek.ImportPkcs8PrivateKey(key.PrivateKey, out _);

            _keyId = key.KeyId;
        }

        public string KeyId => _keyId;

        public byte[] Encrypt(byte[] data)
        {
            try
            {
                byte[] cipher = _kek.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

                return cipher;
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("Failed to encrypt the data key", ex);
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            try
            {
                byte[] decrypted = _kek.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA256);

                return decrypted;
            }
            catch (CryptographicException ex)
            {
                throw new CryptoException("Failed to decrypt the data key", ex);
            }
        }
    }
}
