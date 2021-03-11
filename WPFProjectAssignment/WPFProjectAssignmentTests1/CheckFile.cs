using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilites;

namespace WPFProjectAssignmentTests1
{
    [TestClass]
    public class CheckFile
    {
        [TestMethod]
        public void SaveAndLoadCart()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            var shoppingCart = new ShoppingCart();
            Utilites.Shared.Products = Utilites.Methods.LoadProducts("TestProducts.csv");
            
            shoppingCart.Add(Utilites.Shared.Products[0], 2);
            shoppingCart.Add(Utilites.Shared.Products[1], 3);
            shoppingCart.SaveToFile("TestCart.csv");

            var newCart = new ShoppingCart();
            newCart.LoadFromFile("TestCart.csv");

            string expected = "";
            foreach (var item in shoppingCart.Products)
            {
                expected += item.Key.Code + " " + item.Value;
            }
            string actual = "";
            foreach (var item in newCart.Products)
            {
                actual += item.Key.Code + " " + item.Value;
            }
            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void LoadWithNoSavedCart()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            var shoppingCart = new ShoppingCart();
            Utilites.Shared.Products = Utilites.Methods.LoadProducts("TestProducts.csv");
            
            shoppingCart.SaveToFile("TestCart.csv");
            shoppingCart.LoadFromFile("TestCart.csv");
            
            Assert.AreEqual(0, shoppingCart.Products.Count);
        }
    }
}