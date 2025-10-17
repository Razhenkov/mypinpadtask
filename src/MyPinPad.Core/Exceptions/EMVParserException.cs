namespace MyPinPad.Core.Exceptions
{
    public class EMVParserException: MyPinPadExceptionBase
    {
        public EMVParserException(string emvHex, Exception innerException)
        : base($"An error occured while parsing EMV data {emvHex}", innerException)
        { }
    }
}
