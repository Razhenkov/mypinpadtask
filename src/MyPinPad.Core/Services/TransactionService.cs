using MyPinPad.Core.Domains;
using MyPinPad.Core.Dtos;
using MyPinPad.Core.EMV;
using MyPinPad.Core.EMV.TLVTagEncryption;
using MyPinPad.Core.EncryptionAlgorithms;
using MyPinPad.Core.Extensions;
using MyPinPad.Core.Persistance;
using MyPinPad.Core.Processors;
using MyPinPad.Core.SensitiveDataSanitizers;

namespace MyPinPad.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IEMVParser _emvParser;
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly ITransactionStore _transactionStore;
        private readonly ISensitiveDataSanitizer _sensitiveDataSanitizer;
        private readonly ITLVTagEncryptionService _tlvTagEncryptionService;
        private readonly IDEKEncryptionService _dekEncryptionService;

        public TransactionService(
            IEMVParser emvParser,
            ITransactionProcessor transactionProcessor,
            ITransactionStore transactionStore,
            ISensitiveDataSanitizer sensitiveDataSanitizer,
            ITLVTagEncryptionService tlvTagEncryptionService,
            IDEKEncryptionService dekEncryptionService)
        {   
            _emvParser = emvParser;
            _transactionProcessor = transactionProcessor;
            _transactionStore = transactionStore;
            _sensitiveDataSanitizer = sensitiveDataSanitizer;
            _tlvTagEncryptionService = tlvTagEncryptionService;
            _dekEncryptionService = dekEncryptionService;
        }

        public TransactionDto ProcessTransaction(string emvHex)
        {
            var tlvTags = _emvParser.Parse(emvHex);
            var readableTLVTags = tlvTags.Select(x => (x.Key, x.Value.BytesToHex())).ToDictionary();

            var status = _transactionProcessor.ProcessTransaction(tlvTags);

            _tlvTagEncryptionService.EncryptSensitiveTLVTags(tlvTags, out byte[] dataEncrytionKey);

            var transaction = new Transaction(status, tlvTags);
            if (dataEncrytionKey != null && dataEncrytionKey.Length > 0)
            {
                var encryptedDEKBytes = _dekEncryptionService.Encrypt(dataEncrytionKey);
                transaction.AddMetadata(Convert.ToBase64String(encryptedDEKBytes), _dekEncryptionService.KeyId);
            }

            _transactionStore.Save(transaction);
            
            Sanitize(readableTLVTags);

            var dto = new TransactionDto(
                transaction.PublicId,
                transaction.Decision,
                transaction.ProcessedAt,
                readableTLVTags
            );

            return dto;
        }

        public IEnumerable<TransactionListItemDto> ListTransactions()
        { 
            var transactions = _transactionStore.List().Select(x => 
            new TransactionListItemDto(x.PublicId, x.Decision, x.ProcessedAt));

            return transactions;
        }

        public TransactionDto? GetTransactionDetails(Guid id)
        {
            var transaction = _transactionStore.Get(id);
            if (transaction == null)
                return null;

            if (!string.IsNullOrEmpty(transaction.EncryptedDEK))
            {
                var encryptedDataEncryptionKeyBytes = Convert.FromBase64String(transaction.EncryptedDEK);
                var dataEncryptionKey = _dekEncryptionService.Decrypt(encryptedDataEncryptionKeyBytes);
                _tlvTagEncryptionService.DecryptSensitiveTLVTags(transaction.TLVTags, dataEncryptionKey);
            }
            
            var readableTLVTags = transaction.TLVTags.Select(x => (x.Key, x.Value.BytesToHex())).ToDictionary();
            Sanitize(readableTLVTags);

            var dto = new TransactionDto(
                transaction.PublicId,
                transaction.Decision,
                transaction.ProcessedAt,
                readableTLVTags
            );

            return dto;
        }

        private void Sanitize(Dictionary<string, string> tlvs)
        {
            var sensitiveTLVs = tlvs.Where(x => _sensitiveDataSanitizer.SensitiveTlvTagCheck(x.Key));

            foreach (var tlv in sensitiveTLVs)
            {
                tlvs[tlv.Key] = _sensitiveDataSanitizer.Sanitize(tlv.Key, tlv.Value);
            }
        }
    }
}
