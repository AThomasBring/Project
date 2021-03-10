using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFProjectAssignment;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WPFProjectAssignment.Tests
{
    [TestClass()]
    public class ShoppingCartTests
    {
        [TestMethod()]
        public void LoadFromFileTest()
        {
            string newTextFile = "abcd123";
            File.WriteAllText(@"C:\Windows\Temp\TestFile.txt", newTextFile);

            

            Assert.AreEqual
        }
    }
}