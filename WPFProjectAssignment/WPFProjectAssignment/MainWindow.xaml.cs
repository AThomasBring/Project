using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class MainWindow : Window
    {
        // A static variable in the `Program` class is available in every method of that class. Effectively a "global" variable.

        // Array of Products & Dictionary for shopping cart
        public static Product[] Products;
        public static ShoppingCart Cart;
        

        // We store product information in a CSV file in the project directory.
        public const string ProductFilePath = "Products.csv";
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";

        private ListBox ProductBox = new ListBox();
        private TextBlock InfoText = new TextBlock();
        private Image InfoImage = new Image();
        private Grid TextandImageGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        private StackPanel InfoPanel = new StackPanel();
        private TextBlock cartTextBlock = new TextBlock();
        private TextBlock infoPrice = new TextBlock();
        private Button checkoutButton = new Button();
        private Button addToCartButton = new Button();
        private TextBox DiscountBlock = new TextBox();

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
                //todo felhantering
                MessageBox.Show("Could not read product file.");
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
                    //todo felhantering
                    MessageBox.Show("Error when reading product");
                }
            }

            // The method returns an array rather than a list (because the products are fixed after the program has started), so we need to convert it before returning.
            return products.ToArray();
        }
        

        private void Start()
        {

            Products = LoadProducts();

            if (File.Exists(CartFilePath))
            {
                Cart.LoadFromFile(CartFilePath);
            }
            
            //Shoppingcart klassen läser in från fil om den finns, annars skapar tom shoppingcart.
            Cart = new ShoppingCart();


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

            // This grid is for item description and image, and gets cleared and updated every selection change
            TextandImageGrid = CreateGrid(rows: new []{5, 1}, columns: new []{1, 1});;

            ButtonGrid = CreateGrid(rows: new[] {5, 1}, columns: new[] {1, 1, 1, 1});

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
            secondGrid.Children.Add(TextandImageGrid);
            Grid.SetColumn(TextandImageGrid, 1);
            Grid.SetRow(TextandImageGrid, 1);
            

            secondGrid.Children.Add(ButtonGrid);
            Grid.SetColumn(ButtonGrid, 1);
            Grid.SetRow(ButtonGrid, 1);

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
            
            ProductBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Products)
            {
                ProductBox.Items.Add(new ListBoxItem() { Content = product.Name, Tag = product });
            }
            
            ProductBox.SelectedIndex = -1;
            thirdGrid.Children.Add(ProductBox);
            Grid.SetColumn(ProductBox, 0);
            Grid.SetRow(ProductBox, 0);
            ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;
            
            //shopping cart text
            thirdGrid.Children.Add(cartTextBlock);
            Grid.SetRow(cartTextBlock, 1);

            infoPrice = new TextBlock
            {
                Text = "",
                FontSize = 16,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
            };
            ButtonGrid.Children.Add(infoPrice);
            Grid.SetColumn(infoPrice, 0);
            Grid.SetRow(infoPrice, 1);

            // Add to Cart button
            addToCartButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Background = Brushes.White,
            };

            ButtonGrid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 1);
            Grid.SetRow(addToCartButton, 1);
            ////Grid.SetColumnSpan(addToCartButton, 2);
            addToCartButton.Click += AddToCartButtonOnClick;

            //Add discount window


            // Check out button
            checkoutButton = new Button 
            {
                Content = "Checkout",
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Background = Brushes.White,
            };
            
            ButtonGrid.Children.Add(checkoutButton);
            Grid.SetColumn(checkoutButton, 3);
            Grid.SetRow(checkoutButton, 1);
            //Grid.SetColumnSpan(addToCartButton, 2);
            checkoutButton.Click += CheckoutButton_Click;

        }

        private static void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void UpdateDescription(Product product)
        {
            InfoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            TextandImageGrid.Children.Add(InfoPanel);
            Grid.SetColumn(InfoPanel, 0);
            Grid.SetRow(InfoPanel, 0);

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
            InfoPanel.Children.Add(infoHeading);
            
            InfoText = new TextBlock
            {
                //Add code to read CSV file of descriptions
                Text = product.Description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 12
            };
            InfoPanel.Children.Add(InfoText);
        }
        private void PriceUpdate(Product product)
        {
            string a = product.Price.ToString();
            infoPrice.Text = a + "Kr";
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Först, Lagra det valda objektet i SelectedProduct
            SelectedProduct = (Product)((ListBoxItem)ProductBox.SelectedItem).Tag;
            
            //Sedan uppdatera text och grafik
            TextandImageGrid.Children.Clear();
            UpdateDescription(SelectedProduct);
            UpdateImage(SelectedProduct.Image);
            PriceUpdate(SelectedProduct);
        }

        private void UpdateImage(Image image)
        {
            InfoImage = image;
            InfoImage.Stretch = Stretch.Uniform;
            TextandImageGrid.Children.Add(InfoImage);
            Grid.SetColumn(InfoImage, 1);
        }


        private void AddToCartButtonOnClick(object sender, RoutedEventArgs e)
        {
            Cart.Add(SelectedProduct);
            
            double total = 0;
            cartTextBlock.Text = "";
            foreach (var item in Cart.Items)
            {
                Grid cartgrid = CreateGrid(new[] {1}, new[] {1, 1, 1});
                cartTextBlock.Text += item.Value + "x " + item.Key.Name + " " + item.Key.Price + "kr." + "\n";
                total += item.Key.Price * item.Value;
            }

            cartTextBlock.Text += "Total: " + total +"kr";



        }

        private static Grid CreateGrid(int[] rows, int[] columns)
        {
            var grid = new Grid
            {
                Margin = new Thickness(5)
            };
            //grid.ShowGridLines = true;

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
