using MyPinPad.Core.SensitiveDataSanitizers.Strategies;

namespace MyPinPad.UnitTests
{
    public class PANSanitizerUnitTests
    {
        [Fact]
        public void Sanitize_ReturnMaskedPAN()
        {
            // Arrange
            var pan = "4012888888881881";
            var expectedPan = "****1881";
            var sanitizer = new PANSanitizerStrategy();

            // Act
            var maskedPan = sanitizer.Sanitize(pan);
        
            // Assert
            Assert.Equal(expectedPan, maskedPan);
        }
    }
}
