using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFProjectAssignment
{
    public class Product
    {
        public string Code;
        public string Name;
        public string Description;
        public int Price;
        public Image Image;
    }

    public partial class MainWindow : Window
    {
        // A static variable in the `Program` class is available in every method of that class. Effectively a "global" variable.

        // An array of all the products available, loaded from "Products.csv".
        public static Product[] Products;

        // A shopping cart is a dictionary mapping a Product object to the number of copies of that product we have added.
        //public static Dictionary<Product, int> Cart;

        // We store product information in a CSV file in the project directory.
        public const string ProductFilePath = "Products.csv";

        // We store the saved shopping cart in a CSV file outside the project directory, because then it will not be overwritten everytime we start the program.
        //public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";

        private ListBox productBox = new ListBox();
        private TextBlock infoText = new TextBlock();
        private Image infoImage = new Image();
        private Grid productWindowGrid = new Grid();
        private StackPanel infoPanel = new StackPanel();
        
        // We store the most recent selected product here
        private Product SelectedProduct { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private static Product[] LoadProducts()
        {
            // If the file doesn't exist, stop the program completely.
            if (!File.Exists(ProductFilePath))
            {
                Console.WriteLine(ProductFilePath + " finns inte, eller har inte blivit satt till 'Copy Always'.");
                Environment.Exit(1);
            }

            // Create an empty list of products, then go through each line of the file to fill it.
            List<Product> products = new List<Product>();
            string[] lines = File.ReadAllLines(ProductFilePath);

            foreach (string line in lines)
            {
                try
                {
                    // First, split the line on commas (CSV means "comma-separated values").
                    string[] parts = line.Split(',');

                    // Then create a product with its values set to the different parts of the line.
                    Product p = new Product
                    {
                        Code = parts[0],
                        Name = parts[1],
                        Description = parts[2],
                        Price = int.Parse(parts[3]),
                        Image = CreateImage("Images/"+parts[4])
                    };
                    products.Add(p);
                }
                catch
                {
                    Console.WriteLine("Fel vid inläsning av en produkt!");
                }
            }

            // The method returns an array rather than a list (because the products are fixed after the program has started), so we need to convert it before returning.
            return products.ToArray();
        }


        private void Start()
        {

            Products = LoadProducts();
            
            
            // Window options
            Title = "Potion Shop";
            Width = 1000;
            Height = 618;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Main grid (dividing header with rest of layout)
            Grid firstgrid = CreateGrid(rows: new []{1, 6}, columns: null);
            Content = firstgrid;
            // Second Grid, Left side for item list and shopping cart, right side for item description
            var secondGrid = CreateGrid(rows: null, columns: new []{1, 2});

            // Third Grid, Top row for list of available products, Bottom row for shopping cart
            var thirdGrid = CreateGrid(rows: new []{2, 1}, columns: null);

            // This grid gets cleared and updated every selection change
            productWindowGrid = CreateGrid(rows: new []{5, 1}, columns: new []{1, 1});;

            var buttonGrid = CreateGrid(rows: new[] {5, 1}, columns: new[] {1, 1, 1, 1});

            //Adding grids to the grids
            // add second grid to into second row of first grid
            
            firstgrid.Children.Add(secondGrid);
            Grid.SetColumn(secondGrid, 0);
            Grid.SetRow(secondGrid, 1);
            
            // add third grid to into first column of second grid
            secondGrid.Children.Add(thirdGrid);
            Grid.SetColumn(thirdGrid, 0);
            Grid.SetRow(thirdGrid, 1);
            
            // add description grid to into second column of second grid
            secondGrid.Children.Add(productWindowGrid);
            Grid.SetColumn(productWindowGrid, 1);
            Grid.SetRow(productWindowGrid, 1);
            
            //add fourth grid into third
            //thirdGrid.Children.Add(fourthGrid);
            //Grid.SetColumn(fourthGrid, 0);
            //Grid.SetRow(fourthGrid, 1);
            
            //add fourth grid into third
            //fourthGrid.Children.Add(fifthGrid);
            //Grid.SetColumn(fifthGrid, 1);
            //Grid.SetRow(fifthGrid, 0);

            secondGrid.Children.Add(buttonGrid);
            Grid.SetColumn(buttonGrid, 1);
            Grid.SetRow(buttonGrid, 1);

            // A text heading.
            TextBlock heading = new TextBlock
            {
                Text = "Potion Shop",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            firstgrid.Children.Add(heading);
            Grid.SetColumn(heading, 0);
            Grid.SetRow(heading, 0);
            //Grid.SetColumnSpan(heading, 2);
            
            // The list box allowing us to select a product.
            // Fills the left half of the second row.
            //Add code here to read products in from CSV file.

            
            productBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Products)
            {
                productBox.Items.Add(new ListBoxItem() { Content = product.Name, Tag = product });
            }
            
            productBox.SelectedIndex = -1;
            thirdGrid.Children.Add(productBox);
            Grid.SetColumn(productBox, 0);
            Grid.SetRow(productBox, 0);
            productBox.SelectionChanged += ProductBoxOnSelectionChanged;

            // Add to Cart button
            Button addToCartButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(15),
                Padding = new Thickness(5),
                FontSize = 12,
                Background = Brushes.White
            };
            buttonGrid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 1);
            Grid.SetRow(addToCartButton, 1);
            
            // Add to Cart button
            Button checkoutButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(15),
                Padding = new Thickness(5),
                FontSize = 12,
                Background = Brushes.White
            };
            buttonGrid.Children.Add(checkoutButton);
            Grid.SetColumn(checkoutButton, 3);
            Grid.SetRow(checkoutButton, 1);
            
            
            //Grid.SetColumnSpan(addToCartButton, 2);
            addToCartButton.Click += AddToCartButtonOnClick;
            
        }

        private void UpdateDescription(Product product)
        {
            infoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            productWindowGrid.Children.Add(infoPanel);
            Grid.SetColumn(infoPanel, 0);
            Grid.SetRow(infoPanel, 0);

            // The text heading inside the information panel.
            TextBlock infoHeading = new TextBlock
            {
                Text = product.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            infoPanel.Children.Add(infoHeading);
            
            infoText = new TextBlock
            {
                //Add code to read CSV file of descriptions
                Text = product.Description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 12
            };
            infoPanel.Children.Add(infoText);
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Först, Lagra det valda objektet i SelectedProduct
            SelectedProduct = (Product)((ListBoxItem)productBox.SelectedItem).Tag;
            
            //Sedan uppdatera text och grafik
            productWindowGrid.Children.Clear();
            UpdateDescription(SelectedProduct);
            UpdateImage(SelectedProduct);
        }

        private void UpdateImage(Product product)
        {
            infoImage = product.Image;
            infoImage.Stretch = Stretch.Uniform;
            productWindowGrid.Children.Add(infoImage);
            Grid.SetColumn(infoImage, 1);
        }


        private void AddToCartButtonOnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Adding " +SelectedProduct.Name +"to Cart (Not implemented)");
        }

        private static Grid CreateGrid(int[] rows, int[] columns)
        {
            Grid grid = new Grid();
            grid.ShowGridLines = true;
            grid.Margin = new Thickness(5);

            if (rows != null)
            {
                foreach (var height in rows)
                {
                    grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(height, GridUnitType.Star)});
                }
            }

            if (columns != null)
            {
                foreach (var width in columns)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(width, GridUnitType.Star)});
                }
            }

            return grid;
        }


        private static Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
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
}
