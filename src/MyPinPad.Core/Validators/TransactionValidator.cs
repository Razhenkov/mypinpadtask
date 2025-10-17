using MyPinPad.Core.Validators.IntegrityValidators;

namespace MyPinPad.Core.Validators
{
    internal class TransactionValidator : ITransactionValidator
    {
        private readonly IIntegrityValidator _integrityValidator;

        public TransactionValidator(IIntegrityValidator integrityValidator)
        {
            _integrityValidator = integrityValidator;
        }

        public bool IsValid(string emvHex, string signature, Dictionary<string, byte[]> tags)
        {
            bool isValid = ValidateIntegrity(emvHex, signature);

            return isValid;
        }

        private bool ValidateIntegrity(string emvHex, string integritySignature)
        {
            return _integrityValidator.Verify(emvHex, integritySignature);
        }
    }    
}
