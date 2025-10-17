using MyPinPad.Core.EncryptionAlgorithms;

namespace MyPinPad.Core.EMV.TLVTagEncryption
{
    public class TLVTagEncryptionService : ITLVTagEncryptionService
    {
        private readonly List<string> _sensitiveTlvTags;
        private readonly IEncryptionAlgorithm _encryptionAlgorithm;

        private TLVTagEncryptionService() { }

        public TLVTagEncryptionService(
            List<string> sensitiveTlvTags,
            IEncryptionAlgorithm encryptionAlgorithm)
        {
            _sensitiveTlvTags = sensitiveTlvTags;
            _encryptionAlgorithm = encryptionAlgorithm;
        }

        public void EncryptSensitiveTLVTags(Dictionary<string, byte[]> tlvTags, out byte[] key)
        {
            var sensitiveTLVs = tlvTags.Where(x => _sensitiveTlvTags.Contains(x.Key));

            if (!sensitiveTLVs.Any())
            {
                key = Array.Empty<byte>();
                return;
            }

            _encryptionAlgorithm.GenerateKey();

            foreach (var tlv in sensitiveTLVs)
            {
                tlvTags[tlv.Key] = _encryptionAlgorithm.Encrypt(tlv.Value);
            }

            key = _encryptionAlgorithm.GetKey();
        }

        public void DecryptSensitiveTLVTags(Dictionary<string, byte[]> rawData, byte[] key)
        {
            foreach (var tlvTag in rawData)
            {
                if (_sensitiveTlvTags.Contains(tlvTag.Key))
                {
                    rawData[tlvTag.Key] = _encryptionAlgorithm.Decrypt(tlvTag.Value, key);
                }
            }
        }
    }
}
