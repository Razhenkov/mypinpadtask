namespace MyPinPad.Core.Extensions
{
    public static class ByteExtensions
    {
        public static string BytesToHex(this byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", "");
    }
}
