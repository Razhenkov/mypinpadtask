namespace MyPinPad.Core.SensitiveDataSanitizers
{
    public interface ISensitiveDataSanitizer
    {
        bool SensitiveTlvTagCheck(string tlvTagName);

        string Sanitize(string tlvTagName, string tlvTagValue);
    }
}