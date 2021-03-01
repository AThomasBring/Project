using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }
        
        public static Product[] LoadProducts()
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
                        Price = int.Parse(parts[3])
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

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            //Todo skapa metod för skapandet av grids
            // Main grid (For dividing header with rest of layout
            Grid firstgrid = new Grid();
            firstgrid.ShowGridLines = true;
            root.Content = firstgrid;
            firstgrid.Margin = new Thickness(5);
            
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            
            // Second Grid, Left side for item list and shopping cart, right side for item description
            Grid secondGrid = new Grid();
            secondGrid.ShowGridLines = true;
            secondGrid.Margin = new Thickness(5);

            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            
            // Third Grid, Top row for list of available products, Bottom row for shopping cart
            Grid thirdGrid = new Grid();
            thirdGrid.ShowGridLines = true;
            thirdGrid.Margin = new Thickness(5);

            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            
            // Fourth Grid, Left column for items in shopping cart, right for checkout
            Grid fourthGrid = new Grid();
            fourthGrid.ShowGridLines = true;
            fourthGrid.Margin = new Thickness(5);

            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Fifth Grid, Left column for items in shopping cart, right for checkout
            Grid fifthGrid = new Grid();
            fifthGrid.ShowGridLines = true;
            fifthGrid.Margin = new Thickness(5);

            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            
            Grid descriptionGrid = new Grid();
            descriptionGrid.ShowGridLines = true;
            descriptionGrid.Margin = new Thickness(5);

            
            descriptionGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            descriptionGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            descriptionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            descriptionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
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
            secondGrid.Children.Add(descriptionGrid);
            Grid.SetColumn(descriptionGrid, 1);
            Grid.SetRow(descriptionGrid, 1);
            
            //add fourth grid into third
            thirdGrid.Children.Add(fourthGrid);
            Grid.SetColumn(fourthGrid, 0);
            Grid.SetRow(fourthGrid, 1);
            
            //add fourth grid into third
            fourthGrid.Children.Add(fifthGrid);
            Grid.SetColumn(fifthGrid, 1);
            Grid.SetRow(fifthGrid, 0);
            
  
            
            // A text heading.
            TextBlock heading = new TextBlock
            {
                Text = "Products",
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

            
            ListBox productBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch
            };

            foreach (var product in Products)
            {
                productBox.Items.Add(product.Name);
            }
            productBox.SelectedIndex = 0;
            thirdGrid.Children.Add(productBox);
            Grid.SetColumn(productBox, 0);
            Grid.SetRow(productBox, 0);
            ScrollViewer viewer = new ScrollViewer();
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            
            // The information panel describing a specific potion.
            // Fills the right half of the second row.
            StackPanel infoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            descriptionGrid.Children.Add(infoPanel);
            Grid.SetColumn(infoPanel, 0);
            Grid.SetRow(infoPanel, 0);

            // The text heading inside the information panel.
            TextBlock infoHeading = new TextBlock
            {
                Text = "Potion1",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            infoPanel.Children.Add(infoHeading);

            // The image inside the information panel.
            Image infoImage = CreateImage("Potion1");
            infoImage.Stretch = Stretch.Uniform;
            infoPanel.Children.Add(infoImage);

            // The descriptive text inside the information panel.
            TextBlock infoText = new TextBlock
            {
                //Add code to read CSV file of descriptions
                Text = "Här ska vi läsa in texten om varje product från CSV fil",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 12
            };
            infoPanel.Children.Add(infoText);

            // A button that stretches across both columns.
            Button addToCartButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 16
            };
            descriptionGrid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 1);
            Grid.SetRow(addToCartButton, 2);
            Grid.SetColumnSpan(addToCartButton, 2);
            
        }
        
        
        private Image CreateImage(string filePath)
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
