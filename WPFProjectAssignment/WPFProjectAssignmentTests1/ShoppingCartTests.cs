using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFProjectAssignment;

namespace WPFProjectAssignmentTests1
{
    [TestClass()]
    public class CheckTotalSumOnReceipt
    {
        [TestMethod()]
        public void CheckTotalSumNoDiscount()
        {
            var shoppingCart = new ShoppingCart();
            var noDiscount = new DiscountCode
            {
                CodeName = "No Discount",
                Percentage = 0
            };
            
            var apple = new Product
            {
                Code = "001",
                Name = "Apple",
                Price = 100
            };
            
            var banana = new Product
            {
                Code = "002",
                Name = "Banana",
                Price = 139
            };
            
            shoppingCart.Add(apple, 5);
            shoppingCart.Add(banana, 1);

            var testReciept = new Receipt(noDiscount, shoppingCart);

            var amountAfterDiscount = testReciept.AmountSummary[3][1];

            Assert.AreEqual("639kr", amountAfterDiscount);

        }
        
        [TestMethod()]
        public void CheckTotalSumAfterDiscount()
        {
            var shoppingCart = new ShoppingCart();
            var discount35 = new DiscountCode
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

            
            var testReciept = new Receipt(discount35, shoppingCart);

            var amountAfterDiscount = testReciept.AmountSummary[3][1];
            //We expect 4*4+7 = 23   23 - 23*0,35 = 14.95
            Assert.AreEqual("14,95kr", amountAfterDiscount);

        }
        
    }
}