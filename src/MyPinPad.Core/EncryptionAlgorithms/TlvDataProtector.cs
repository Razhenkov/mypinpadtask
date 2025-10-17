using MyPinPad.Core.EMVDataProtection;
using MyPinPad.Core.TlvDataProtector;

namespace MyPinPad.Core.SensitiveDataProtector
{
    

    public class EmvDataProtector: IEMVDataProtector
    {
        private readonly List<string> _sensitiveTlvTags;
        private readonly ITlvDataEncryptionService _tlvDataEncryptionService;

        private EmvDataProtector() { }

        //public byte[] Secret => _tlvDataEncryptionService.GetSecret();

        public EmvDataProtector(List<string> sensitiveTlvTags, ITlvDataEncryptionService tlvDataEncryptionService)
        {
            _sensitiveTlvTags = sensitiveTlvTags;
            _tlvDataEncryptionService = tlvDataEncryptionService;
        }

        public void Protect(Dictionary<string, byte[]> rawData, out byte[] key)
        {
            _tlvDataEncryptionService.GenerateKey();

            foreach (var tlvTag in rawData)
            {
                if (_sensitiveTlvTags.Contains(tlvTag.Key))
                { 
                    rawData[tlvTag.Key] = _tlvDataEncryptionService.Encrypt(tlvTag.Value);
                }
            }

            key = _tlvDataEncryptionService.GetKey();
        }

        public void Unprotect(Dictionary<string, byte[]> rawData, byte[] key)
        {
            foreach (var tlvTag in rawData)
            {
                if (_sensitiveTlvTags.Contains(tlvTag.Key))
                {
                    rawData[tlvTag.Key] = _tlvDataEncryptionService.Decrypt(tlvTag.Value, key);
                }
            }
        }
    }
}
