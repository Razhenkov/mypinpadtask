namespace MyPinPad.Core.SensitiveDataSanitizers
{
    public sealed class SensitiveDataSanitizer: ISensitiveDataSanitizer
    {
        private readonly Dictionary<string, ISensitiveDataSanitizerStrategy> _sanitizers = new();

        private SensitiveDataSanitizer() 
        { }

        public SensitiveDataSanitizer(Dictionary<string, ISensitiveDataSanitizerStrategy> sanitizers)
        {
            _sanitizers = sanitizers;
        }

        public bool SensitiveTlvTagCheck(string tlvTagName) => _sanitizers.ContainsKey(tlvTagName);

        public string Sanitize(string tlvTagName, string tlvTagValue)
        {
            return _sanitizers[tlvTagName].Sanitize(tlvTagValue);
        }
    }
}