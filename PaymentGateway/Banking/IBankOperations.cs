using Com.Checkout.PaymentGateway.Models;

namespace Com.Checkout.PaymentGateway.Banking
{
    public interface IBankOperations
    {
        /// <summary>
        /// Retrieves the payment information and status for a payment processed by the bank
        /// </summary>
        /// <param name="transactionId">The banking system's transaction Id</param>
        /// <returns>An instance of a previously processed payment by that bank, null if not found</returns>
        public ProcessedPayment GetPayment(int transactionId);

        /// <summary>
        /// Sends a payment to the bank to be processed
        /// </summary>
        /// <param name="payment">The payment details</param>
        /// <returns>The resulting processed payment including status and transaction Id</returns>
        public ProcessedPayment ProcessPayment(Payment payment);
    }
}
