using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace WPFProjectAssignment
{
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
                foreach (Product p in MainWindow.Products)
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