﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace WPFProjectAssignmentTests1
{
    [TestClass]
    public class Tests
    {
        private const string TestPath = @"C:\Windows\Temp\PotionShopTempFiles\";
        [TestMethod]
        public void ShopAndCheckReceipt()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
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
        
        [TestMethod]
        public void CheckLargeNumbers()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var shoppingCart = new ShoppingCart();
            var discountCode = new DiscountCode
            {
                CodeName = "Discount33",
                Percentage = 33
            };

            var expensiveThing = new Product
            {
                Code = "001",
                Price = 55000.55M
            };

            var cheapButMany = new Product
            {
                Code = "002",
                Price = 4.50M
            };

            shoppingCart.Add(expensiveThing, 1);
            shoppingCart.Add(cheapButMany, 120000);

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
            
            //(55000.55 + 540000) 595000,55 - 196350,1815 = 398650,3685
            var expected = "Discount code: Discount33 Total: 595000.55kr Your discount: 196350.18kr (33%) After discount: 398650.37kr";
            
            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void CheckZeroAndNegativeValues()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var shoppingCart = new ShoppingCart();

            var freebie = new Product
            {
                Code = "001",
                Price = 0
            };

            var refund = new Product
            {
                Code = "002",
                Price = -50M
            };

            shoppingCart.Add(freebie, 5);
            shoppingCart.Add(refund, 2);

            var receipt = new Receipt(shoppingCart);
            
            var actual = "";

            foreach (var array in receipt.SumBreakdown)
            {
                foreach (var s in array)
                {
                    actual += s + " ";
                }
            }
            actual = actual.TrimEnd();
            
            //(55000.55 + 540000) 595000,55 - 196350,1815 = 398650,3685
            var expected = "Discount code: No discount Total: -100kr Your discount: 0kr (0%) After discount: -100kr";
            
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SaveCart()
        {
            Methods.CopyToTempFolder("TestProducts.csv", TestPath+"TestProducts.csv");
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            var shoppingCart = new ShoppingCart();
            Product[] testProducts = Methods.LoadProducts(TestPath + "TestProducts.csv");

            shoppingCart.Add(testProducts[0], 2);
            shoppingCart.Add(testProducts[1], 3);
            shoppingCart.SaveToFile(TestPath+"TestCart.csv");

            string expected = "";
            foreach (var item in shoppingCart.Products)
            {
                expected += item.Key.Code +@"\" +item.Value;
            }
          
        
            string[] readFromFile = File.ReadAllLines(TestPath + "TestCart.csv");
            string actual = "";
          
            foreach (var s in readFromFile)
            {
                actual += s;
            }
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoadCart()
        {
            Methods.CopyToTempFolder("TestProducts.csv", TestPath + "TestProducts.csv");
            Methods.CopyToTempFolder("TestCart.csv", TestPath + "TestCart.csv");
            
            
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var shoppingCart = new ShoppingCart();
            Product[] testProducts = Methods.LoadProducts(TestPath + "TestProducts.csv");
            shoppingCart.LoadFromFile(TestPath + "TestCart.csv", testProducts);

            string expected = @"001\5002\2";
            string actual = "";
            foreach (var item in shoppingCart.Products)
            {
                actual += item.Key.Code + @"\" + item.Value;
            }
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoadWithNoSavedCart()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            var shoppingCart = new ShoppingCart();
            Product[] testProducts = Methods.LoadProducts(TestPath + "TestProducts.csv");
            shoppingCart.LoadFromFile(TestPath+"NoSuchFile.csv", testProducts);

            Assert.AreEqual(0, shoppingCart.Products.Count);
        }

    }
}