namespace MyPinPad.Core.SensitiveDataSanitizers
{
    public interface ISensitiveDataSanitizerStrategy
    {
        string Sanitize(string source);
    }
}