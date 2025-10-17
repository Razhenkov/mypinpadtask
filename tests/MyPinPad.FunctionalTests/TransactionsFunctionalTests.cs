
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyPinPad.WebApi.Models.Requests;
using MyPinPad.WebApi.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace MyPinPad.FunctionalTests
{
    public class TransactionsFunctionalTests : IClassFixture<WebApplicationFactory<Program>>
    {
        const string INTEGRITY_SIGNATURE_HEADER_NAME = "X-Integrity-Signature";
        const string ENDPOINT_PREFIX = "api/transactions";

        [Fact]
        public async Task Process_WhenValidData_Return201CreatedWithTransactionResponse()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var request = new TransactionRequest("9C01009F02060000000001005A081234567890123456");
            var integritySignature = "HEi9c4/O/eJhaqs2rFFPjS0kjfuzzPzz12vNw1PQQ4o=";

            httpClient.DefaultRequestHeaders.Add(INTEGRITY_SIGNATURE_HEADER_NAME, integritySignature);

            // Act
            var response = await httpClient.PostAsJsonAsync($"{ENDPOINT_PREFIX}/process", request);
            var responsePayload = await response.Content.ReadFromJsonAsync<TransactionResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responsePayload.Should().NotBeNull();
            responsePayload.Id.Should().NotBeEmpty();
            responsePayload.Status.Should().Be(TransactionStatus.Approved);
            responsePayload.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public async Task Process_WhenInvalidIntegritySignature_Return400BadRequest()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var request = new TransactionRequest("9C01009F02060000000001005A081234567890123456");
            var integritySignature = "Invalid signature";

            httpClient.DefaultRequestHeaders.Add(INTEGRITY_SIGNATURE_HEADER_NAME, integritySignature);

            // Act
            var response = await httpClient.PostAsJsonAsync($"{ENDPOINT_PREFIX}/process", request);
            var responsePayload = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responsePayload.Should().NotBeNull();
            responsePayload.Should().Contain("The request signature is invalid or does not match the payload.");
        }

        [InlineData("")]
        [InlineData(" ")]
        [Theory]
        public async Task Process_WhenInvalidEMVHex_Return400BadRequest(string emvHex)
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var request = new TransactionRequest(emvHex);
            var integritySignature = "HEi9c4/O/eJhaqs2rFFPjS0kjfuzzPzz12vNw1PQQ4o=";

            httpClient.DefaultRequestHeaders.Add(INTEGRITY_SIGNATURE_HEADER_NAME, integritySignature);

            // Act
            var response = await httpClient.PostAsJsonAsync($"{ENDPOINT_PREFIX}/process", request);
            var responsePayload = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responsePayload.Should().NotBeNull();
            responsePayload.Should().Contain("Missing EMV hex data");
        }

        [Fact] 
        public async Task ListTransactions_ReturnOk()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var createTransactionRequest = new TransactionRequest("9C01009F02060000000001005A081234567890123456");
            var integritySignature = "HEi9c4/O/eJhaqs2rFFPjS0kjfuzzPzz12vNw1PQQ4o=";

            httpClient.DefaultRequestHeaders.Add(INTEGRITY_SIGNATURE_HEADER_NAME, integritySignature);
            var response = await httpClient.PostAsJsonAsync($"{ENDPOINT_PREFIX}/process", createTransactionRequest);

            var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>();
            int expectedTransactionCount = 1;

            // Act
            response = await httpClient.GetAsync($"{ENDPOINT_PREFIX}");
            var listTransactionsResponsePayload = await response!.Content.ReadFromJsonAsync<IEnumerable<TransactionListItemResponse>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            listTransactionsResponsePayload.Should().NotBeNull();
            listTransactionsResponsePayload.Should().HaveCount(expectedTransactionCount);
            listTransactionsResponsePayload.First().Id.Should().Be(transaction!.Id);
        }

        [Fact]
        public async Task GetTransactionDetails_WhenTransactionExists_ReturkOk()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var createTransactionRequest = new TransactionRequest("9C01009F02060000000001005A081234567890123456");
            var integritySignature = "HEi9c4/O/eJhaqs2rFFPjS0kjfuzzPzz12vNw1PQQ4o=";

            httpClient.DefaultRequestHeaders.Add(INTEGRITY_SIGNATURE_HEADER_NAME, integritySignature);
            var response = await httpClient.PostAsJsonAsync($"{ENDPOINT_PREFIX}/process", createTransactionRequest);

            var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>();

            // Act
            response = await httpClient.GetAsync($"{ENDPOINT_PREFIX}/{transaction!.Id}");
            var getTransactionResponsePayload = await response!.Content.ReadFromJsonAsync<TransactionResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            getTransactionResponsePayload.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTransactionDetails_WhenTransactionNotExists_Return404NotFound()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            // Act
            var response = await httpClient.GetAsync($"{ENDPOINT_PREFIX}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}