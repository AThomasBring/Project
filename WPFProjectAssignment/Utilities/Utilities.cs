using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utilities
{
    public class Methods
    {
        public static void AddToGui(UIElement element, Panel panel, int row = 0, int column = 0)
        {
            panel.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }

        public static void CopyToTempFolder(string source, string destination)
        {
            File.Copy(source, destination, true);
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
                        Image = parts[4]
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
                    var word = discountline.Split('\\');
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
        public static Button CreateButton(string content, object tag = null)
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

        public static void UpdateProductImage(string imagePath)
        {
            Shared.ImageDisplayed = CreateImage(imagePath);
            Shared.ImageDisplayed.Stretch = Stretch.Uniform;
            Shared.TextAndImageGrid.Children.Add(Shared.ImageDisplayed);
            Grid.SetColumn(Shared.ImageDisplayed, 1);
        }

        public static void UpdateDescriptionText(Product product)
        {
            Shared.InfoPanel = new StackPanel

            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
            };
            AddToGui(Shared.InfoPanel, Shared.TextAndImageGrid);

            // The text heading inside the information panel.
            var productName = new TextBlock
            {
                Text = product.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            Methods.AddToGui(productName, Shared.InfoPanel);

            Shared.ProductDescription = new TextBlock
            {
                Text = product.Description + "\n",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            AddToGui(Shared.ProductDescription, Shared.InfoPanel);

            var price = product.Price.ToString(CultureInfo.InvariantCulture);
            Shared.ProductPrice = new TextBlock
            {
                Text = price + "kr",
                FontSize = 12,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            AddToGui(Shared.ProductPrice, Shared.InfoPanel);

        }
        
        public static void UpdateDescriptionText(DiscountCode discountCode)
        {
            Shared.InfoPanel = new StackPanel

            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
            };
            AddToGui(Shared.InfoPanel, Shared.TextAndImageGrid);

            // The text heading inside the information panel.
            var discountName = new TextBlock
            {
                Text = discountCode.CodeName,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            AddToGui(discountName, Shared.InfoPanel);

            Shared.ProductDescription = new TextBlock
            {
                Text = discountCode.Percentage + "\n",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            AddToGui(Shared.ProductDescription, Shared.InfoPanel);

        }
        
        

        public static Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
    }
    
    public static class Shared
    {
        public const string DiscountFilePath = @"C:\Windows\Temp\PotionShopTempFiles\DiscountCodes.csv";
        public const string CartFilePath = @"C:\Windows\Temp\PotionShopTempFiles\Cart.csv";
        public const string WelcomeImagePath = @"C:\Windows\Temp\PotionShopTempFiles\Images\welcome.jpg";
        public const string ImageFolderPath = @"C:\Windows\Temp\PotionShopTempFiles\Images\";
        public const string ProductFilePath = @"C:\Windows\Temp\PotionShopTempFiles\Products.csv";
        public static Product[] Products;
        public static DiscountCode[] DiscountCodes;
        public static ListBox ProductBox = new ListBox();
        public static TextBlock ProductDescription = new TextBlock();
        public static Image ImageDisplayed = new Image();
        public static Grid TextAndImageGrid = new Grid();
        public static Grid ButtonGrid = new Grid();
        public static StackPanel InfoPanel = new StackPanel();
        public static TextBlock ProductPrice = new TextBlock();

        // We store the most recent selected product here
        public static Product SelectedProduct { get; set; }
    }
    public class Product
    {
        public string Code;
        public string Description;
        public string Image;
        public string Name;
        public decimal Price;
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
                Products[current] = amount;
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
        }
    }
    
}