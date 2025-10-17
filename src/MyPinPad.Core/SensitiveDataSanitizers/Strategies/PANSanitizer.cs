namespace MyPinPad.Core.SensitiveDataSanitizers.Strategies
{
    public class PANSanitizerStrategy : ISensitiveDataSanitizerStrategy
    {
        public string Sanitize(string source)
        {
            return $"****{source[^4..]}";
        }
    }
}