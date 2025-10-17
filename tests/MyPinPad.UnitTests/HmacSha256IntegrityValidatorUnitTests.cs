using MyPinPad.Core.Validators.IntegrityValidators;

namespace MyPinPad.UnitTests
{
    public class HmacSha256IntegrityValidatorUnitTests
    {
        [Fact]
        public void Verify_WhenValidSignature_ReturnTrue()
        {
            // Arrange
            const string emvHex = "9C01009F02060000000001005A081234567890123456";
            var signatureValidator = new HmacSha256IntegrityValidator(Convert.FromBase64String("YUU4JXUxM3pSOSRw"));
            
            // Act
            var signature = signatureValidator.Compute(emvHex);
            var isValid = signatureValidator.Verify(emvHex, signature);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Verify_WhenInvalidSignature_ReturnFalse()
        {
            // Arrange
            const string emvHex = "5A0854133300890200129F24032212315F2402";
            var signatureValidator = new HmacSha256IntegrityValidator(Convert.FromBase64String("YUU4JXUxM3pSOSRw"));
            
            // Act
            var signature = "InvalidSignature";
            var isValid = signatureValidator.Verify(emvHex, signature);

            // Assert
            Assert.False(isValid);
        }
    }
}