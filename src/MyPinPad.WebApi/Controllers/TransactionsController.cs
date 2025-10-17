using Microsoft.AspNetCore.Mvc;
using MyPinPad.Core.Services;
using MyPinPad.Core.Validators.IntegrityValidators;
using MyPinPad.WebApi.Models.Requests;
using MyPinPad.WebApi.Models.Responses;
using System.Text.Json;

namespace MyPinPad.WebApi.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IIntegrityValidator _integrityValidator;

        private readonly ILogger<TransactionsController> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public TransactionsController(
            ITransactionService transactionService,
            IIntegrityValidator integrityValidator, 
            ILogger<TransactionsController> logger,
            JsonSerializerOptions jsonOptions)
        {
            _integrityValidator = integrityValidator;
            _transactionService = transactionService;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        [HttpPost("process")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ProcessTransaction(
            [FromBody] TransactionRequest request,
            [FromHeader(Name = "X-Integrity-Signature")] string integritySignature)
        {
            if (string.IsNullOrWhiteSpace(request.EmvHex))
                return CreateValidationProblem(nameof(TransactionRequest.EmvHex), "Missing EMV hex data");

            if (!_integrityValidator.Verify(request.EmvHex, integritySignature))
                return CreateValidationProblem("X-Integrity-Signature", "The request signature is invalid or does not match the payload.");

            var transaction = _transactionService.ProcessTransaction(request.EmvHex);

            if (transaction.Decision == Core.Domains.Decision.Approved)
                _logger.LogInformation($"Transaction {transaction.PublicId} has been approved: {JsonSerializer.Serialize(transaction, _jsonOptions)}");
            else
                _logger.LogWarning($"Transaction {transaction.PublicId} has been declined: {JsonSerializer.Serialize(transaction, _jsonOptions)}");

            var response = Mappers.MapToTransactionResponse(transaction);

            return CreatedAtAction(
                nameof(GetTransactionDetails),
                new { id = response.Id },
                response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionListItemResponse>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TransactionListItemResponse>> ListTransactions()
        {
            var transactions = _transactionService.ListTransactions();
            var response = transactions.Select(Mappers.MapToTransactionListItem);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TransactionResponse> GetTransactionDetails(Guid id)
        {
            var transaction = _transactionService.GetTransactionDetails(id);
            if (transaction == null)
                return NotFound();

            var response = Mappers.MapToTransactionResponse(transaction);

            _logger.LogDebug($"Get details for transaction {id}: {JsonSerializer.Serialize(transaction, _jsonOptions)}");

            return Ok(response);
        }

        private ActionResult CreateValidationProblem(string field, string error)
        {
            ModelState.AddModelError(field, error);

            return ValidationProblem(ModelState);
        }
    }
}