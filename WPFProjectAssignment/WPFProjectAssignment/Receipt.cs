using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WPFProjectAssignment
{
    public class Receipt
    {

        public string[][] ItemsBreakdown;
        public string[][] SumBreakdown;

        public Receipt(ShoppingCart cart, DiscountCode discountCode = null)
        {
            //if no discount code was passed, we make a DiscountCode object that wont apply a discount so our calculations and receipt layout will work later.
            if (discountCode == null)
            {
                discountCode = new DiscountCode
                {
                    CodeName = "No discount",
                    Percentage = 0
                };
            }

            var totalAmount = cart.Products.Sum(product => product.Key.Price * product.Value);
            
            List<string[]> receiptLines = new List<string[]>();

            foreach (var product in cart.Products)
            {
                string[] receiptLine = new[]
                {
                    product.Key.Name,
                    product.Value.ToString(),
                    product.Key.Price + "kr",
                    product.Key.Price * product.Value + "kr"
                };
                receiptLines.Add(receiptLine);
            }
            
            List<string[]> summaryLines = new List<string[]>();

            string[] discountCodeLine = new[]
            {
                "Discount code:",
                discountCode.CodeName,
            };
            summaryLines.Add(discountCodeLine);
            
            string[] totalLine = new[]
            {
                "Total:",
                totalAmount + "kr"
            };
            summaryLines.Add(totalLine);

            var appliedDiscount = Math.Round(totalAmount*discountCode.Percentage / 100, 2);
            var appliedDiscountString = appliedDiscount.ToString();
            
            string[] appliedDiscountLine = new[]
            {
                "Your discount:",
                appliedDiscountString + "kr (" +discountCode.Percentage + "%)"
            };
            summaryLines.Add(appliedDiscountLine);
            
            var totalWithDiscountString = Convert.ToString(totalAmount - (totalAmount*discountCode.Percentage/100), CultureInfo.InvariantCulture);
            string[] afterDiscountLine =
            {
                "After discount:",
                totalWithDiscountString + "kr"
                
            };
            summaryLines.Add(afterDiscountLine);

            ItemsBreakdown = receiptLines.ToArray();
            SumBreakdown = summaryLines.ToArray();


        }
    }
}