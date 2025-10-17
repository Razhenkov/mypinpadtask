namespace MyPinPad.Core.SensitiveDataSanitizers.Strategies
{
    public class Track2DataSanitizerStrategy : ISensitiveDataSanitizerStrategy
    {
        public string Sanitize(string source)
        {
            return "****[MASKED_TRACK2DATA]****";
        }
    }
}