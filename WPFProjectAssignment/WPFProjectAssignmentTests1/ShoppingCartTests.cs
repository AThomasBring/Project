using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFProjectAssignment;

namespace WPFProjectAssignmentTests1
{
    [TestClass()]
    public class ShoppingCartTests
    {
        [TestMethod()]
        public void LoadFromFileTest()
        {
            var shoppingCart = new ShoppingCart();
            var noDiscount = new DiscountCode
            {
                CodeName = "No Discount",
                Percentage = 0
            };
            
            Product testProduct = new Product
            {
                Code = "001",
                Name = "Test Product",
                Price = 100
            };
            
            shoppingCart.Add(testProduct, 5);

            var testReciept = new Receipt(noDiscount, shoppingCart);

            var amountAfterDiscount = testReciept.AmountSummary[3][1];
            
            Assert.AreEqual(500, amountAfterDiscount);

        }
    }
}