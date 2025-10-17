using Microsoft.Extensions.Options;
using MyPinPad.Core.EMV;
using MyPinPad.Core.EMV.TLVTagEncryption;
using MyPinPad.Core.EncryptionAlgorithms;
using MyPinPad.Core.KeyProviders;
using MyPinPad.Core.Persistance;
using MyPinPad.Core.Processors;
using MyPinPad.Core.SensitiveDataSanitizers;
using MyPinPad.Core.SensitiveDataSanitizers.Strategies;
using MyPinPad.Core.Services;
using MyPinPad.Core.Validators.IntegrityValidators;
using MyPinPad.WebApi.Options;

namespace MyPinPad.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyPinPadServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .Configure<SecurityKeyNamesOptions>(configuration.GetSection(SecurityKeyNamesOptions.SettingsName))
            .AddSensitiveDataSanitizer()
            .AddCrypto()
            .AddProcessorServices()
            .AddPersistance()
            .AddCoreServices();

            return services;
        }

        private static IServiceCollection AddSensitiveDataSanitizer(this IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var sanitizers = new Dictionary<string, ISensitiveDataSanitizerStrategy>
                {
                    { TLVDictionary.PAN, new PANSanitizerStrategy() },
                    { TLVDictionary.Track2Data, new Track2DataSanitizerStrategy() }
                };

                return sanitizers;
            });

            services.AddScoped<ISensitiveDataSanitizer, SensitiveDataSanitizer>();

            return services;
        }

        private static IServiceCollection AddCrypto(this IServiceCollection services)
        {
            services.AddScoped<IKeyProvider, LocalKeyProvider>();

            services.AddScoped<IIntegrityValidator, HmacSha256IntegrityValidator>(provider =>
            {
                var securityKeyNamesOption = provider.GetService<IOptions<SecurityKeyNamesOptions>>()!.Value;

                var keyProvider = provider.GetRequiredService<IKeyProvider>();
                var secretKey = keyProvider.GetSymetricKey(securityKeyNamesOption.IntegrityKey).SharedKey;

                return new HmacSha256IntegrityValidator(secretKey);
            });

            services.AddScoped<IEncryptionAlgorithm, AesGcmEncryptionAlgorithm>();

            services.AddScoped<ITLVTagEncryptionService, TLVTagEncryptionService>(provider =>
            {
                var encryptionAlgorithm = provider.GetService<IEncryptionAlgorithm>();
                var service = new TLVTagEncryptionService(
                    new List<string> 
                    { 
                        TLVDictionary.PAN, 
                        TLVDictionary.Track2Data 
                    }, encryptionAlgorithm!);

                return service;
            });

            services.AddScoped<IDEKEncryptionService, DEKEncryptionService>(provider =>
            {
                var securityKeyNamesOption = provider.GetService<IOptions<SecurityKeyNamesOptions>>()!.Value;

                var keyProvider = provider.GetRequiredService<IKeyProvider>();
                var secretKey = keyProvider.GetAsymetricKey(securityKeyNamesOption.MasterKey);

                return new DEKEncryptionService(secretKey);
            });

            return services;
        }

        private static IServiceCollection AddProcessorServices(this IServiceCollection services)
        {
            services.AddScoped<ITransactionProcessorStrategy, OnlyPurchaseTransactionProcessorStrategy>();
            services.AddScoped<ITransactionProcessor, TransactionProcessor>();

            return services;
        }

        private static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IEMVParser, EMVParser>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services)
        {
            services.AddSingleton<ITransactionStore, TransactionStore>();

            return services;
        }
    }
}
