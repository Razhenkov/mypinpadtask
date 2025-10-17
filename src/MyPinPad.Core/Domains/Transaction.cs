using MyPinPad.Core.Helpers;
using System.Text.Json;

namespace MyPinPad.Core.Domains
{
    public class Transaction: ICloneable
    {
        private Transaction(
            long id, Guid publicId, DateTime processedAt, Decision decision, 
            Dictionary<string, byte[]> tlvTags, string encryptedDEK, string keyId) 
        {
            Id = id;
            PublicId = publicId;
            ProcessedAt = processedAt;
            Decision = decision;
            TLVTags = tlvTags;
            EncryptedDEK = encryptedDEK;
            KeyId = keyId;
        }

        public Transaction(Decision decision, Dictionary<string, byte[]> tlvTags)
        {
            Id = AutoIncrementIdGenerator.GetNextId();
            PublicId = Guid.NewGuid();
            ProcessedAt = DateTime.UtcNow;
            Decision = decision;
            TLVTags = tlvTags;
        }

        public long Id { get; }
        
        public Guid PublicId { get; }

        public Decision Decision { get; set; }
        
        public DateTime ProcessedAt { get; }

        public Dictionary<string, byte[]> TLVTags { get; }

        public string EncryptedDEK { get; set; }

        public string KeyId { get; set; }

        public void AddMetadata(string encryptedDEK, string keyId)
        { 
            EncryptedDEK = encryptedDEK;
            KeyId = keyId;
        }

        public object Clone()
        {
            return new Transaction(
                this.Id, this.PublicId, this.ProcessedAt, this.Decision,
                new Dictionary<string, byte[]>(this.TLVTags), this.EncryptedDEK, this.KeyId);
        }
    }
}
