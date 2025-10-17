namespace MyPinPad.Core.Exceptions
{
    public class CryptoException: MyPinPadExceptionBase
    {
        public CryptoException(string message, Exception innerException)
        : base(message, innerException)
        { }
    }
}
