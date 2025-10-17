namespace MyPinPad.Core.EMV.TLVTagEncryption
{
    public interface ITLVTagEncryptionService
    {
        void EncryptSensitiveTLVTags(Dictionary<string, byte[]> tlvTags, out byte[] key);

        void DecryptSensitiveTLVTags(Dictionary<string, byte[]> tlvTags, byte[] key);
    }
}
