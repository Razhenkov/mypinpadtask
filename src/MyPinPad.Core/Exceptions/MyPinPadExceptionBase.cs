namespace MyPinPad.Core.Exceptions
{
    public abstract class MyPinPadExceptionBase: Exception
    {
        protected MyPinPadExceptionBase(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}
