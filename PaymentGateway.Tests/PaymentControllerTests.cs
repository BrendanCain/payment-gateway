using Com.Checkout.PaymentGateway.Controllers;
using Com.Checkout.PaymentGateway.Models;
using Com.Checkout.PaymentGateway.Banking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Checkout.PaymentGateway.Tests
{
    /// <summary>
    /// Test class for the Payment Controller API
    /// </summary>
    public class PaymentControllerTests
    {
        //TODO - Decide on a consistent field name casing strategy
        private readonly PaymentController controller;
        private readonly Mock<IBankOperations> mockBankOperations;
        private readonly Mock<ILogger<PaymentController>> mockLogger;

        public PaymentControllerTests()
        {
            mockBankOperations = new Mock<IBankOperations>();
            mockLogger = new Mock<ILogger<PaymentController>>();
            controller = new PaymentController(mockLogger.Object, mockBankOperations.Object);
        }

        [Fact]
        public void GetPaymentReturnsOkResult()
        {
            //Arrange
            mockBankOperations.Setup(x => x.GetPayment(It.IsAny<int>())).Returns(GetProcessedPayment());

            //Act
            var actionResult = controller.GetPayment(1);

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);

            // TODO - more asserts
            var processedPayment = result.Value as ProcessedPayment;
            Assert.NotNull(processedPayment);
            Assert.Matches("^[*]{12}[0-9]{4}$", processedPayment.CardNumber);

            mockBankOperations.Verify(x => x.GetPayment(1), Times.Once);
            mockBankOperations.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetPaymentReturnsNotFoundResult()
        {
            //Arrange
            mockBankOperations.Setup(x => x.GetPayment(It.IsAny<int>()))
                .Returns<ProcessedPayment>(null);

            //Act
            var actionResult = controller.GetPayment(1);

            //Assert
            var result = actionResult.Result as NotFoundResult;
            Assert.NotNull(result);

            mockBankOperations.Verify(x => x.GetPayment(1), Times.Once);
            mockBankOperations.VerifyNoOtherCalls();
        }

        [Fact]
        public void PostPaymentReturnsPaymentSucceeded()
        {
            //Arrange
            mockBankOperations.Setup(x => x.ProcessPayment(It.IsAny<Payment>())).Returns(GetProcessedPayment());

            //Act
            var payment = GetPayment();
            var actionResult = controller.PostPayment(payment);

            //Assert
            var result = actionResult.Result as CreatedAtRouteResult;
            Assert.NotNull(result);

            // TODO - more asserts
            var processedPayment = result.Value as ProcessedPayment;
            Assert.NotNull(processedPayment);
            Assert.Matches("^[*]{12}[0-9]{4}$", processedPayment.CardNumber);

            mockBankOperations.Verify(x => x.ProcessPayment(
                It.Is<Payment>(y => y.CardNumber == payment.CardNumber && y.CardExpiry == payment.CardExpiry && y.Amount == payment.Amount))
            , Times.Once);
            mockBankOperations.VerifyNoOtherCalls();
        }

        private ProcessedPayment GetProcessedPayment()
        {
            return new ProcessedPayment(GetPayment())
            {
                TransactionId = 12345,
                PaymentStatus = PaymentStatus.Succeeded
            };
        }

        private Payment GetPayment()
        {
            return new Payment
            {
                CardNumber = "5555555555554444",
                CardExpiry = "12/2023",
                CVV = "123",
                Amount = 100,
                Currency = Currency.USD
            };
        }
    }
}
