using Moq;
using MyPinPad.Core.Domains;
using MyPinPad.Core.Dtos;
using MyPinPad.Core.EMV;
using MyPinPad.Core.EMV.TLVTagEncryption;
using MyPinPad.Core.EncryptionAlgorithms;
using MyPinPad.Core.Persistance;
using MyPinPad.Core.Processors;
using MyPinPad.Core.SensitiveDataSanitizers;
using MyPinPad.Core.Services;

namespace MyPinPad.UnitTests
{
    public class TransactionServiceIntegrationTests
    {
        [Fact]
        public void Process_WhenTransactionWithoutSensitiveData_StoreWithoutSanization()
        {
            // Arrange
            var emvHex = "9F0206000000000100";
            var emvParserMock = new Mock<IEMVParser>();

            emvParserMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(new Dictionary<string, byte[]>
                {
                    { "9C", Convert.FromHexString("00") }
                });

            var processorStrategyMock = new Mock<ITransactionProcessorStrategy>();
            processorStrategyMock
                .Setup(x => x.Process(It.IsAny<Dictionary<string, byte[]>>()))
                .Returns(Decision.Approved);

            var processor = new TransactionProcessor(processorStrategyMock.Object);

            var storeMock = new Mock<ITransactionStore>();

            var sanitizer = new Mock<ISensitiveDataSanitizer>();
            var tlvTagEncryptionService = Mock.Of<ITLVTagEncryptionService>();
            var dekEncryptionService = Mock.Of<IDEKEncryptionService>();

            var service = new TransactionService(
                emvParserMock.Object,
                processor,
                storeMock.Object,
                sanitizer.Object,
                tlvTagEncryptionService,
                dekEncryptionService
            );

            // Act
            service.ProcessTransaction(emvHex);

            // Assert
            storeMock.Verify(x => x.Save(It.IsAny<Transaction>()), Times.Once);
            sanitizer.Verify(x => x.Sanitize(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Process_WhenTransactionWithSensitiveData_StoreWithSanization()
        {
            // Arrange
            var emvHex = "9f02060000000001809f150212349f01031234565a051234567890570a0102030405061d2412235f340212349c01005f200212349f0b021234560212349f20021234990212349f4b02123493021234";
            var emvParser = new EMVParser();

            var processorStrategyMock = new Mock<ITransactionProcessorStrategy>();
            processorStrategyMock
                .Setup(x => x.Process(It.IsAny<Dictionary<string, byte[]>>()))
                .Returns(Decision.Approved);

            var processor = new TransactionProcessor(processorStrategyMock.Object);

            var storeMock = new Mock<ITransactionStore>();

            var sanitizerStrategyMock = new Mock<ISensitiveDataSanitizerStrategy>();
            var sanitazerStrategies = new Dictionary<string, ISensitiveDataSanitizerStrategy>
            {
                { TLVDictionary.PAN, sanitizerStrategyMock.Object },
                { TLVDictionary.Track2Data, sanitizerStrategyMock.Object }
            };

            var sanitizer = new SensitiveDataSanitizer(sanitazerStrategies);

            var tlvTagEncryptionService = Mock.Of<ITLVTagEncryptionService>();
            var dekEncryptionService = Mock.Of<IDEKEncryptionService>();

            var service = new TransactionService(
                emvParser,
                processor,
                storeMock.Object,
                sanitizer,
                tlvTagEncryptionService,
                dekEncryptionService
            );

            // Act
            service.ProcessTransaction(emvHex);

            // Assert
            storeMock.Verify(x => x.Save(It.IsAny<Transaction>()), Times.Once);
            sanitizerStrategyMock.Verify(x => x.Sanitize(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ListTransactions_ReturnTransactionListItems()
        {
            // Arrange 
            var transationsInStore = new List<Transaction>
            {
                new Transaction(Decision.Approved, new Dictionary<string, byte[]>()),
                new Transaction(Decision.Declined, new Dictionary<string, byte[]>())
            };

            var expectedTransactionListItems = new List<TransactionListItemDto>();
            foreach (var tnx in transationsInStore)
            {
                expectedTransactionListItems.Add(new TransactionListItemDto(tnx.PublicId, tnx.Decision, tnx.ProcessedAt));
            }

            var transactionStoreMock = new Mock<ITransactionStore>();
            transactionStoreMock.Setup(x => x.List()).Returns(transationsInStore);

            var service = new TransactionService(
                Mock.Of<IEMVParser>(),
                Mock.Of<ITransactionProcessor>(),
                transactionStoreMock.Object,
                Mock.Of<ISensitiveDataSanitizer>(),
                Mock.Of<ITLVTagEncryptionService>(),
                Mock.Of<IDEKEncryptionService>()
            );

            // Act
            var transactions = service.ListTransactions().ToList();

            // Assert
            Assert.Equal(expectedTransactionListItems, transactions);
        }
    }
}
