using Com.Checkout.PaymentGateway.Models;

namespace Com.Checkout.PaymentGateway.Banking
{
    public class BankOperations : IBankOperations
    {
        /// <summary>
        /// WARNING:
        /// This class is implemented purely for testing purposes, it is completely non functional
        /// This class will return different results depending if the numeric inputs are Odd or Even
        /// The GetPayment call will return a valid transaction if it receives an Even transaction Id, or no transaction otherwise
        /// The ProcessPayment call will return a successful transaction if we receive an Even CVV, or a failed transaction otherwise
        /// </summary>

        public ProcessedPayment? GetPayment(int transactionId)
        {
            if (transactionId % 2 == 0) return null;

            return new ProcessedPayment
            {
                TransactionId = transactionId,
                CardNumber = "5555555555554444",
                CardExpiry = "12/2023",
                CVV = "123",
                Amount = 100,
                Currency = Currency.USD,
                PaymentStatus = PaymentStatus.Succeeded
            };
        }

        public ProcessedPayment ProcessPayment(Payment payment)
        {
            ProcessedPayment processedPayment = new ProcessedPayment(payment);
            processedPayment.TransactionId = 12345;

            int cvvValue = int.Parse(payment.CVV);
            if (cvvValue % 2 == 0)
                processedPayment.PaymentStatus = PaymentStatus.Succeeded;
            else
                processedPayment.PaymentStatus = PaymentStatus.Failed;

            return processedPayment;
        }
    }
}
