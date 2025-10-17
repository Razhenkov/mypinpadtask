namespace MyPinPad.Core.EncryptionAlgorithms
{
    public interface IDEKEncryptionService
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);

        string KeyId { get; }
    }
}
