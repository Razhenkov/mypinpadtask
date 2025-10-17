using MyPinPad.Core.KeyProviders.Models;

namespace MyPinPad.Core.KeyProviders
{
    public interface IKeyProvider
    {
        SymetricKey GetSymetricKey(string keyId);
        AsymetricKey GetAsymetricKey(string keyId);
    }
}
