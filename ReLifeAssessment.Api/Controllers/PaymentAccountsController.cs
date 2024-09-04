using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReLifeAssessment.Application.Payment.Contracts;
using ReLifeAssessment.Application.Payment.Dtos;

namespace ReLifeAssessment.Api.Controllers
{
    /// <summary>
    /// Controller for managing payment accounts, including methods for retrieving and updating payment methods,
    /// authorizing and deauthorizing accounts, and checking account status.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentAccountsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentAccountsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAccountsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="paymentService">The payment service instance.</param>
        public PaymentAccountsController(
            ILogger<PaymentAccountsController> logger,
            IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the allowed payment methods for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>An <see cref="IActionResult"/> containing the allowed payment methods.</returns>
        [AllowAnonymous]
        [HttpGet("methods")]
        [ProducesResponseType(typeof(AllowedPaymentMethodsDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMethods(Guid companyId)
        {
            try
            {
                var result = await _paymentService.GetPaymentMethodAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment methods.");
                return BadRequest("An error occurred while retrieving payment methods.");
            }
        }

        /// <summary>
        /// Updates the allowed payment methods for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="model">The payment methods details to update.</param>
        /// <returns>An <see cref="IActionResult"/> containing the updated payment methods.</returns>
        [HttpPut("methods")]
        [ProducesResponseType(typeof(AllowedPaymentMethodsDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SetMethods(Guid companyId, [FromBody] AllowedPaymentMethodsDto model)
        {
            if (model == null)
            {
                return BadRequest("Payment methods model cannot be null.");
            }

            try
            {
                var result = await _paymentService.UpdatePaymentMethodAsync(companyId, model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update payment methods.");
                return BadRequest("An error occurred while updating payment methods.");
            }
        }

        /// <summary>
        /// Retrieves a URL for linking a payment account for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>An <see cref="IActionResult"/> containing the link account URL.</returns>
        [HttpGet("onboardingurl")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetLinkAccountUrl(Guid companyId)
        {
            try
            {
                var url = await _paymentService.GetLinkAccountUrl(companyId);
                return Ok(new { stripeUrl = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate account link URL.");
                return BadRequest("An error occurred while generating the account link URL.");
            }
        }

        /// <summary>
        /// Authorizes a payment account for a specified company.
        /// </summary>
        /// <param name="model">The payment account details to authorize.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
        [HttpPost("authorize")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AuthorizeAccount([FromBody] CreatePaymentAccountDto model)
        {
            if (model == null)
            {
                return BadRequest("Payment account model cannot be null.");
            }

            try
            {
                var result = await _paymentService.AuthorizeAccount(model);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authorize payment account.");
                return BadRequest("An error occurred while authorizing the payment account.");
            }
        }

        /// <summary>
        /// Deauthorizes a payment account for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
        [HttpGet("unauthorize")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeauthorizeAccount(Guid companyId)
        {
            try
            {
                var result = await _paymentService.DeauthorizeAccount(companyId);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deauthorize payment account.");
                return BadRequest("An error occurred while deauthorizing the payment account.");
            }
        }

        /// <summary>
        /// Retrieves the activation status of a payment account for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>An <see cref="IActionResult"/> containing a boolean indicating the account status.</returns>
        [HttpGet("status")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAccountState(Guid companyId)
        {
            try
            {
                var status = await _paymentService.GetAccountState(companyId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve account state.");
                return BadRequest("An error occurred while retrieving the account state.");
            }
        }
    }
}
