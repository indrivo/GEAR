using System;
using System.Linq;
using System.Text.RegularExpressions;
using GR.Card.Abstractions.Models;
using GR.Card.Abstractions.Enums;
using GR.Core;

namespace GR.Card.Abstractions.Helpers
{
    public static class CreditCardValidator
    {
        /// <summary>
        /// Check if credit creditCard data is valid
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        public static bool IsCreditCardInfoValid(CreditCardPayViewModel creditCard)
            => IsCreditCardInfoValid(creditCard.CardNumber, creditCard.Month, creditCard.Year, creditCard.CardCode);

        /// <summary>
        /// Check if credit creditCard data is valid
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="expiryYear"></param>
        /// <param name="cvv"></param>
        /// <param name="expiryMonth"></param>
        /// <returns></returns>
        public static bool IsCreditCardInfoValid(string cardNo, string expiryMonth, string expiryYear, string cvv)
        {
            var cardCheck = new Regex(GlobalResources.RegularExpressions.CREDIT_CARD);
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck = new Regex(@"^\d{3}$");

            if (!cardCheck.IsMatch(cardNo))
                return false;
            if (!cvvCheck.IsMatch(cvv))
                return false;

            if (!monthCheck.IsMatch(expiryMonth) || !yearCheck.IsMatch(expiryYear))
                return false;

            var year = int.Parse(expiryYear);
            var month = int.Parse(expiryMonth);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month);
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            return cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6);
        }

        /// <summary>
        /// Get card type
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public static CreditCardType GetCardType(string cardNumber)
        {
            return new Regex(@"^4[0-9]{6,}$").IsMatch(cardNumber) ? CreditCardType.Visa :
                   new Regex(@"^5[1-5][0-9]{5,}|222[1-9][0-9]{3,}|22[3-9][0-9]{4,}|2[3-6][0-9]{5,}|27[01][0-9]{4,}|2720[0-9]{3,}$").IsMatch(cardNumber) ? CreditCardType.MasterCard :
                   new Regex(@"^3[47][0-9]{5,}$").IsMatch(cardNumber) ? CreditCardType.AmericanExpress :
                   new Regex(@"^65[4-9][0-9]{13}|64[4-9][0-9]{13}|6011[0-9]{12}|(622(?:12[6-9]|1[3-9][0-9]|[2-8][0-9][0-9]|9[01][0-9]|92[0-5])[0-9]{10})$").IsMatch(cardNumber) ? CreditCardType.Discover :
                   new Regex(@"^3[47][0-9]{13}$").IsMatch(cardNumber) ? CreditCardType.Amex :
                   new Regex(@"^(6541|6556)[0-9]{12}$").IsMatch(cardNumber) ? CreditCardType.BCGlobal :
                   new Regex(@"^389[0-9]{11}$").IsMatch(cardNumber) ? CreditCardType.CarteBlanch :
                   new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$").IsMatch(cardNumber) ? CreditCardType.DinersClub :
                   new Regex(@"^63[7-9][0-9]{13}$").IsMatch(cardNumber) ? CreditCardType.InstaPaymentCard :
                   new Regex(@"^(?:2131|1800|35\d{3})\d{11}$").IsMatch(cardNumber) ? CreditCardType.JCBCard :
                   new Regex(@"^9[0-9]{15}$").IsMatch(cardNumber) ? CreditCardType.KoreanLocal :
                   new Regex(@"^(6304|6706|6709|6771)[0-9]{12,15}$").IsMatch(cardNumber) ? CreditCardType.LaserCard :
                   new Regex(@"^(5018|5020|5038|6304|6759|6761|6763)[0-9]{8,15}$").IsMatch(cardNumber) ? CreditCardType.Maestro :
                   new Regex(@"^(6334|6767)[0-9]{12}|(6334|6767)[0-9]{14}|(6334|6767)[0-9]{15}$").IsMatch(cardNumber) ? CreditCardType.Solo :
                   new Regex(@"^(4903|4905|4911|4936|6333|6759)[0-9]{12}|(4903|4905|4911|4936|6333|6759)[0-9]{14}|(4903|4905|4911|4936|6333|6759)[0-9]{15}|564182[0-9]{10}|564182[0-9]{12}|564182[0-9]{13}|633110[0-9]{10}|633110[0-9]{12}|633110[0-9]{13}$").IsMatch(cardNumber) ? CreditCardType.SwitchCard :
                   new Regex(@"^(62[0-9]{14,17})$").IsMatch(cardNumber) ? CreditCardType.UnionPay :
                   cardNumber.Where((e) => e >= '0' && e <= '9').Reverse().Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2)).Sum((e) => e / 10 + e % 10) == 0 ? CreditCardType.NotFormatted :
                   CreditCardType.Unknown;
        }
    }
}