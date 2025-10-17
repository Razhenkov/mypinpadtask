using MyPinPad.Core.SensitiveDataSanitizers.Strategies;

namespace MyPinPad.UnitTests
{
    public class Track2DataSanitizerUnitTests
    {
        [Fact]
        public void Sanitize_ReturnMaskedTrack2Data()
        {
            // Arrange
            var track2Data = "ABC";
            var expectedTrack2Data = "****[MASKED_TRACK2DATA]****";
            var sanitizer = new Track2DataSanitizerStrategy();

            // Act
            var maskedTrack2Data = sanitizer.Sanitize(track2Data);
        
            // Assert
            Assert.Equal(expectedTrack2Data, maskedTrack2Data);
        }
    }
}
