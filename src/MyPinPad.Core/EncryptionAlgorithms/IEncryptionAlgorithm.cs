namespace MyPinPad.Core.EncryptionAlgorithms
{
    public interface IEncryptionAlgorithm
    {
        byte[] GetKey();

        void GenerateKey();

        byte[] Encrypt(byte[] tlvBytes);

        byte[] Decrypt(byte[] decryptedTlvBytes, byte[] key);
    }
}