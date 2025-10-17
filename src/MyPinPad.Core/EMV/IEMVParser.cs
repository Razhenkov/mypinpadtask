namespace MyPinPad.Core.EMV
{
    public interface IEMVParser
    {
        Dictionary<string, byte[]> Parse(string hexString);
    }
}
