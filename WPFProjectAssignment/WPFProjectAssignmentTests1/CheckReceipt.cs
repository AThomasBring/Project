using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFProjectAssignment;

namespace WPFProjectAssignmentTests1
{
    [TestClass]
    public class CheckReceipt
    {
        [TestMethod]
        public void ShopAndCheckReceipt()
        {
            var shoppingCart = new ShoppingCart();
            var discountCode = new DiscountCode
            {
                CodeName = "Yay35Percent",
                Percentage = 35
            };

            var orange = new Product
            {
                Code = "001",
                Price = 4
            };

            var lemon = new Product
            {
                Code = "002",
                Price = 7
            };

            shoppingCart.Add(orange, 4);
            shoppingCart.Add(lemon, 1);

            var receipt = new Receipt(shoppingCart, discountCode);
            
            var actual = "";

            foreach (var array in receipt.SumBreakdown)
            {
                foreach (var s in array)
                {
                    actual += s + " ";
                }
            }
            actual = actual.TrimEnd();

            var expected = "Discount code: Yay35Percent Total: 23kr Your discount: 8.05kr (35%) After discount: 14.95kr";
            
            Assert.AreEqual(expected, actual);
        }
    }
}