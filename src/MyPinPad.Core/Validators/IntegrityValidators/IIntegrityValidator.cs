namespace MyPinPad.Core.Validators.IntegrityValidators
{
    public interface IIntegrityValidator
    {
        string Compute(string data);

        bool Verify(string data, string signature);
    }
}
