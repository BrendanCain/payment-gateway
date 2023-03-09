using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Com.Checkout.PaymentGateway.Models
{
    /// <summary>
    /// Represents a Payment request
    /// </summary>
    public class Payment
    {
        //TODO - create classes for payment card and money, better objects around this in general
        //TODO - Payments can also be other methods possible? Bank payments? Paypal?
        //TODO - also builders to help with construction

        [Required, CreditCard]
        public string CardNumber { get; set; }

        [Required, RequiredValidCardExpiry]
        public string CardExpiry { get; set; }

        //TODO -- Only accept 4 digits if Card Number format matches AMEX
        [Required, RegexStringValidator("^[0-9]{3,4}$")]
        public string CVV { get; set; }

        [Required, RequiredDecimalGreaterThanZero]
        public decimal Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }
    }

    /// <summary>
    /// Represents a Payment that has been received by the bank
    /// </summary>
    public class ProcessedPayment : Payment
    {
        public ProcessedPayment() { }

        public ProcessedPayment(Payment payment)
        {
            //TODO - this should be done with a mapper, if this constructor existed at all, which it probably shouldn't
            this.CardNumber = payment.CardNumber;
            this.CardExpiry = payment.CardExpiry;
            this.CVV = payment.CVV;
            this.Amount = payment.Amount;
            this.Currency = payment.Currency;
        }

        //TODO - String? Long?
        public int TransactionId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        //TODO - There's probably a clever way to do the serialisation of a masked card number via JSON serialisers or overriding properties/getters
        //TODO - This would never go live with this code but I'm cutting off at around the 3 hour mark and I don't have time to come up with a really clever solution for this
        /// <summary>
        /// Masks the card number showing asterisks until the last 4 digits
        /// </summary>
        public void MaskCardNumber()
        {
            CardNumber = new string('*', CardNumber.Length - 4) + CardNumber.Substring(CardNumber.Length - 4);
        }


    }

    public class RequiredDecimalGreaterThanZero : ValidationAttribute
    {
        /// <summary>
        /// Ensures the decimal is greater than zero
        /// </summary>
        /// <param name="value">The integer value of the selection</param>
        /// <returns>True if value is greater than zero</returns>
        public override bool IsValid(object value)
        {
            // return true if value is a non-null number > 0, otherwise return false
            decimal i;
            return value != null && decimal.TryParse(value.ToString(), out i) && i > 0;
        }
    }

    public class RequiredValidCardExpiry : ValidationAttribute
    {
        /// <summary>
        /// Ensures the card expiry is a valid month & year that is not in the past
        /// </summary>
        /// <param name="value">The string value of the card expiry date in MM/YYYY</param>
        /// <returns>True if the expiry is a valid month and year in MM/YYYY format that is not in the past</returns>
        public override bool IsValid(object value)
        {
            // return true if value is a present or future dated MM/YYYY string, otherwise false
            if (value == null) return false;
            string expiry = value.ToString();

            // Check if it's a valid MM/YYYY numeric string with a valid month
            if (!Regex.IsMatch(expiry, "^(0[1-9]|1[0-2])/[0-9]{4}$")) return false;

            // Check if it's at least the current month of the current year
            // Some very edge case issues around timezones possible if someone's card hasn't expired in their timezone but has in ours or vice versa
            var expiryYear = int.Parse(expiry.Split('/')[1]);
            if (expiryYear > DateTime.Now.Year)
                return true;
            else if (expiryYear == DateTime.Now.Year)
            {
                var expiryMonth = int.Parse(expiry.Split('/')[0]);
                if (expiryMonth >= DateTime.Now.Month)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
