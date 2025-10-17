namespace MyPinPad.WebApi.Options
{
    public class SecurityKeyNamesOptions
    {
        public static string SettingsName => "SecurityKeyNames";

        public string IntegrityKey { get; set; }

        public string MasterKey { get; set; }
    }
}
