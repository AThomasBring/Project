using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WPFProjectAssignment
{
    public class ShoppingCart
    {
        public Dictionary<Product, int> Items;

        public ShoppingCart()
        {
            Items = new Dictionary<Product, int>();
        }

        public void Add(Product item)
        {
            if (Items.ContainsKey(item))
            {
                Items[item]++;

            }
            else
            {
                Items[item] = 1;
            }
        }
        
        public void Remove(Product item)
        {
            if (Items.ContainsKey(item))
            {
                Items[item]--;
            }
        }
        
        public void Clear()
        {
            Items.Clear();
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
                Items[current] = amount;
            }
        }

        public void SaveToFile(string path)
        {
            
        }
    }
}