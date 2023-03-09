using Com.Checkout.PaymentGateway.Banking;
using Com.Checkout.PaymentGateway.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Com.Checkout.PaymentGateway.Controllers
{
    [Route("api/Payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private IBankOperations bankOperations;

        public PaymentController(ILogger<PaymentController> logger, IBankOperations bankOperations)
        {
            this.logger = logger;
            this.bankOperations = bankOperations;
        }

        // GET api/Payments/5
        [HttpGet("{id}", Name = "Payment")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessedPayment))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProcessedPayment> GetPayment(int id)
        {
            //TODO - try catch for nice error return? Although that's probably better handled by core api infra code we extend at a later date
            var processedPayment = bankOperations.GetPayment(id);

            if (processedPayment == null) return NotFound();

            processedPayment.MaskCardNumber();
            return Ok(processedPayment);
        }

        // POST api/Payments
        [HttpPost]
        public ActionResult<ProcessedPayment> PostPayment(Payment payment)
        {
            //TODO - store payment internally before we send it to ensure audit records, update afterwards with transactionId and status?
            //TODO - Logging/Auditing? Need to be careful to log enough but not too much
            //TODO - Try/Catch? What errors would we need to handle, need nice return of messages back to provider
            //TODO - retry? Do we want to attempt again if the bank is unreachable?

            var processedPayment = bankOperations.ProcessPayment(payment);
            processedPayment.MaskCardNumber();

            return CreatedAtRoute("Payment", new { id = processedPayment.TransactionId }, processedPayment);
        }

        //TODO - Validate & store card stubs
    }
}

