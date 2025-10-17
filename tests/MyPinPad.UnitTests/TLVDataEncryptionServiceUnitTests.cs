using MyPinPad.Core.EncryptionAlgorithms;
using MyPinPad.Core.Exceptions;
using System.Security.Cryptography;

namespace MyPinPad.UnitTests
{
    public class TLVDataEncryptionServiceUnitTests
    {
        [Fact]
        public void Encrypt_And_Decrypt_ShouldReturnOriginalData()
        {
            // Arrange
            var originalPan = "4242424242424242";
            var panBytes = System.Text.Encoding.UTF8.GetBytes(originalPan);

            var algorithm = new AesGcmEncryptionAlgorithm();
            algorithm.GenerateKey();
            var key = algorithm.GetKey();

            // Act
            var encryptedPan = algorithm.Encrypt(panBytes);
            var decryptedBytes = algorithm.Decrypt(encryptedPan, key);
            var decryptedPan = System.Text.Encoding.UTF8.GetString(decryptedBytes);

            // Assert
            Assert.Equal(originalPan, decryptedPan);
        }

        [Fact]
        public void GenerateKey_ShouldCreateKeyOfCorrectLength()
        {
            // Arrange
            var service = new AesGcmEncryptionAlgorithm();
            
            // Act
            service.GenerateKey();
            var key = service.GetKey();
            
            // Assert
            Assert.NotNull(key);
            Assert.Equal(32, key.Length);
        }


        [Fact]
        public void Decrypt_WithWrongKey_ShouldThrowException()
        {
            // Arrange
            var originalPan = "4242424242424242";
            var panBytes = System.Text.Encoding.UTF8.GetBytes(originalPan);

            var algorithm = new AesGcmEncryptionAlgorithm();
            algorithm.GenerateKey();
            var key = algorithm.GetKey();

            var encryptedPan = algorithm.Encrypt(panBytes);

            var wrongKey = new byte[32];
            Random.Shared.NextBytes(wrongKey);

            // Act & Assert
            Assert.ThrowsAny<CryptoException>(() =>
            {
                algorithm.Decrypt(encryptedPan, wrongKey);
            });
        }
    }
}
