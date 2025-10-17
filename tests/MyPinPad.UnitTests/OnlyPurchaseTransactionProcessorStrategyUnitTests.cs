using MyPinPad.Core.Domains;
using MyPinPad.Core.EMV;
using MyPinPad.Core.Processors;

namespace MyPinPad.UnitTests
{
    public class OnlyPurchaseTransactionProcessorStrategyUnitTests
    {
        [Fact]
        public void Process_WhenPurchase_ThenApproved()
        {
            // Arrange
            var tlvData = new Dictionary<string, byte[]>()
            {
                { TLVDictionary.TransactionType, Convert.FromHexString("00")}
            };

            // Act
            var processorStrategy = new OnlyPurchaseTransactionProcessorStrategy();
            var status = processorStrategy.Process(tlvData);

            // Assert
            Assert.Equal(Decision.Approved, status);
        }

        [Fact]
        public void Process_WhenFundsTransfer_ThenDeclined()
        {
            // Arrange
            var tlvData = new Dictionary<string, byte[]>()
            {
                // Funds transfer between accounts
                { TLVDictionary.TransactionType, Convert.FromHexString("40")}
            };

            // Act
            var processorStrategy = new OnlyPurchaseTransactionProcessorStrategy();
            var status = processorStrategy.Process(tlvData);

            // Assert
            Assert.Equal(Decision.Declined, status);
        }

        [Fact]
        public void Process_WhenNoTransactionCategory_ThenDeclined()
        {
            // Arrange
            var tlvData = new Dictionary<string, byte[]>()
            {
            };

            // Act
            var processorStrategy = new OnlyPurchaseTransactionProcessorStrategy();
            var status = processorStrategy.Process(tlvData);

            // Assert
            Assert.Equal(Decision.Declined, status);
        }
    }
}
