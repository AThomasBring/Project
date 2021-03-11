using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilites
{
    public class Methods
    {
        public static void AddToGui(UIElement element, Panel panel, int row = 0, int column = 0)
        {
            panel.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }
        
        public static void CopyImagesToTempFolder(string path)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            //Copy images to temp folder
            Directory.CreateDirectory(path);
            foreach (string newPath in Directory.GetFiles(@"Images\"))
                //We need to extract the filename from the path
            {
                int fileNameIndex = newPath.LastIndexOf('\\');
                string fileName = newPath.Substring(fileNameIndex + 1);

                File.Copy(newPath, path + fileName, true);
            }
        }
        
        public static Product[] LoadProducts(string path)
        {
            // If the file doesn't exist, stop the program completely.
            if (!File.Exists(path))
            {
                MessageBox.Show("Could not read product file.");
            }

            // Create an empty list of products, then go through each line of the file to fill it.
            List<Product> products = new List<Product>();
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                try
                {
                    //We are using \ as separator because we use commas in the text file.
                    var parts = line.Split('\\');

                    // Then create a product with its values set to the different parts of the line.
                    var p = new Product
                    {
                        Code = parts[0],
                        Name = parts[1],
                        Description = parts[2],
                        Price = decimal.Parse(parts[3]),
                        Image = @"C:\Windows\Temp\PotionShopTempFiles\Images\" + parts[4]
                    };
                    products.Add(p);
                }
                catch
                {
                    MessageBox.Show("Error when reading product");
                }
            }

            return products.ToArray();
        }
        public static DiscountCode[] LoadCodes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //MessageBox.Show("Could not read discount file.");
            }
            List<DiscountCode> codes = new List<DiscountCode>();
            string[] words = File.ReadAllLines(filePath);

            foreach (string discountline in words)
            {
                try
                {
                    var word = discountline.Split(',');
                    var c = new DiscountCode 
                    { 
                        CodeName = word[0],
                        Percentage = int.Parse(word[1]),
                    };
                    
                    codes.Add(c);
                }
                catch
                {
                    MessageBox.Show("Error when reading discountcodes");
                }
            }
        
            return codes.ToArray();
        }

        
        
        public static Grid CreateGrid(int[] rows, int[] columns)
        {
            var grid = new Grid
            {
                Margin = new Thickness(5)
            };

            if (rows != null)
            {
                foreach (var height in rows)
                {
                    grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(height, GridUnitType.Star)});
                }
            }

            if (columns == null) return grid;
            foreach (var width in columns)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(width, GridUnitType.Star)});
            }

            return grid;
        }
        
        //Product parameter is optional, because not all buttons need to be tagged.
        public static Button CreateButton(string content, Product tag = null)
        {
            var newButton = new Button
            {
                Content = content,
                FontSize = 12,
                BorderThickness = new Thickness(1),
                Height = 26,
                Width = 80,
                Background = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            if (tag != null)
            {
                newButton.Tag = tag;
            }
            return newButton;
        }
    }
    public static class Shared
    {
        public static Product[] Products;
        public static DiscountCode[] DiscountCodes;
        
        public const string DiscountFilePath = "DiscountCodes.txt";
        public const string ProductFilePath = "Products.csv";
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";
        public const string WelcomeImagePath = @"C:\Windows\Temp\PotionShopTempFiles\Images\welcome.jpg";









    }
    public class Product
    {
        public string Code;
        public string Name;
        public string Description;
        public decimal Price;
        public string Image;
    }
    
    public class DiscountCode
    {
        public string CodeName;
        public int Percentage;
        
        
    }
    
    public class Receipt
    {

        public string[][] ItemsBreakdown;
        public string[][] SumBreakdown;

        public Receipt(ShoppingCart cart, DiscountCode discountCode = null)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
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
            var appliedDiscountString = Convert.ToString(appliedDiscount, CultureInfo.InvariantCulture);
            
            string[] appliedDiscountLine = new[]
            {
                "Your discount:",
                appliedDiscountString + "kr (" +discountCode.Percentage + "%)"
            };
            summaryLines.Add(appliedDiscountLine);

            decimal totalWithDiscount = Math.Round(totalAmount - totalAmount * discountCode.Percentage / 100, 2);
            var totalWithDiscountString = Convert.ToString(totalWithDiscount, CultureInfo.InvariantCulture);
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
    
    public class ShoppingCart
    {
        public Dictionary<Product, int> Products;

        public ShoppingCart()
        {
            Products = new Dictionary<Product, int>();
        }

        public void Add(Product product, int number)
        {
            if (Products.ContainsKey(product))
            {
                Products[product] += number;

            }
            else
            {
                Products[product] = number;
            }
        }
        
        public void Remove(Product product, int number)
        {

            if (Products.ContainsKey(product))
            {
                if (Products[product] <= number)
                {
                    Products.Remove(product);
                }
                else
                {
                    Products[product]--;   
                }
            }
        }
        
        public void Clear()
        {
            Products.Clear();
        }
        
        public void LoadFromFile(string CartFilePath)
        {
            if (!File.Exists(CartFilePath))
            {
                return;
            }
            // Go through each line and split it on commas, as in `LoadProducts`.
            string[] lines = File.ReadAllLines(CartFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string code = parts[0];
                int amount = int.Parse(parts[1]);

                // We only store the product's code in the CSV file, but we need to find the actual product object with that code.
                // To do this, we access the static `products` variable and find the one with the matching code, then grab that product object.
                Product current = null;
                foreach (Product p in Shared.Products)
                {
                    if (p.Code == code)
                    {
                        current = p;
                    }
                }

                // Save to Items dictionary
                this.Products[current] = amount;
            }
        }

        public void SaveToFile(string path)
        {
            
            if (Products.Count < 1)
            {
                //if user saves empty cart
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                MessageBox.Show("Your shopping cart is empty");
                return;
            }
            List<string> lines = new List<string>();
            foreach (KeyValuePair<Product, int> pair in Products)
            {
                Product p = pair.Key;
                int amount = pair.Value;

                // For each product, we only save the code and the amount.
                // The other info (name, price, description) is already in "Products.csv" and we can look it up when we load the cart.
                lines.Add(p.Code + "," + amount);
            }
            File.WriteAllLines(path, lines);
            MessageBox.Show("Saved shopping cart.");
        }
    }
    
}