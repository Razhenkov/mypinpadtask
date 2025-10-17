using MyPinPad.Core.EMV;
using MyPinPad.Core.Exceptions;
using MyPinPad.Core.Extensions;

namespace MyPinPad.UnitTests
{
    public class EMVParserUnitTests
    {
        [Fact]
        public void ParseTLV_Success()
        {
            const string emvHex = "9C01009F02060000000001005A081234567890123456";

            var emvParser = new EMVParser();
            var actual = emvParser.Parse(emvHex);

            Assert.Equal(3, actual.Count);

            Assert.True(actual.ContainsKey("9C"));
            Assert.Equal("00", actual["9C"].BytesToHex());
            Assert.True(actual.ContainsKey("9F02"));
            Assert.Equal("000000000100", actual["9F02"].BytesToHex());
            Assert.True(actual.ContainsKey("5A"));
            Assert.Equal("1234567890123456", actual["5A"].BytesToHex());
        }

        [Fact]
        public void ParseTLV_InvalidTag_Throws()
        {
            // Tag: 9F, incomplete
            const string hex = "9F";
            var emvParser = new EMVParser();
            Assert.Throws<EMVParserException>(() => emvParser.Parse(hex));
        }

        [Fact]
        public void ParseTLV_InvalidLength_Throws()
        {
            // Tag: 9F33, Length: 03, Value: 11 (missing bytes)
            const string hex = "9F330311";
            var emvParser = new EMVParser();
            Assert.Throws<EMVParserException>(() => emvParser.Parse(hex));
        }
    }
}