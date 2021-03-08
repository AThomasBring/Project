using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFProjectAssignment
{
    public class DiscountCodes
    {
        public string CodeName;
        public int Percentage;
    }
    public partial class MainWindow : Window
    {
        public static DiscountCodes[] DiscountCodes;
        public static Product[] Products;
        public static ShoppingCart Cart;

        public const string DiscountFilePath = "DiscountCodes.txt";
        public const string ProductFilePath = "Products.csv";
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";

        private ListBox ProductBox = new ListBox();
        private TextBlock InfoText = new TextBlock();
        private Image InfoImage = new Image();
        private Grid TextandImageGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        private StackPanel InfoPanel = new StackPanel();
        private static StackPanel CartDisplay = new StackPanel();
        private TextBlock infoPrice = new TextBlock();
        private Button CheckoutButton = new Button();
        private Button RemoveAllProducts = new Button();
        private Button ApplyDiscount = new Button();
        private TextBox DiscountBlock = new TextBox();
        private Label DiscountLabel = new Label();



        // We store the most recent selected product here
        public static Product SelectedProduct { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private static DiscountCodes[] LoadCodes()
        {
            if (!File.Exists(DiscountFilePath))
            {
                MessageBox.Show("Could not read discount file.");
            }
            List<DiscountCodes> codes = new List<DiscountCodes>();
            string[] words = File.ReadAllLines(DiscountFilePath);

            foreach (string discountline in words)
            {
                try
                {
                    var word = discountline.Split(',');
                    var c = new DiscountCodes 
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

        private static Product[] LoadProducts()
        {
            // If the file doesn't exist, stop the program completely.
            if (!File.Exists(ProductFilePath))
            {
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
                    var parts = line.Split(',');

                    // Then create a product with its values set to the different parts of the line.
                    var p = new Product
                    {
                        Code = parts[0],
                        Name = parts[1],
                        Description = parts[2],
                        Price = int.Parse(parts[3]),
                        Image = CreateImage("Images/" + parts[4])
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
            DiscountCodes = LoadCodes();
            Products = LoadProducts();
            Cart = new ShoppingCart();
            // Window options
            Title = "Potion Shop";
            Width = 1000;
            Height = 618;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Main grid (dividing header with rest of layout)
            Grid firstgrid = CreateGrid(rows: new[] { 1, 6 }, columns: null);
            Content = firstgrid;
            // Second Grid, Left side for item list and shopping cart, right side for item description
            var secondGrid = CreateGrid(rows: null, columns: new[] { 1, 2 });

            // Left Side Grid, Top row for list of available products, Bottom row for shopping cart and disocunt Grid
            var leftSideGrid = CreateGrid(rows: new[] { 5, 3, 1, 2 }, columns: null);

            var discountGrid = CreateGrid(rows: new[] { 2, 1}, columns: new[] { 2, 2});

            // This grid is for item description and image, and gets cleared and updated every selection change
            TextandImageGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 }); ;

            ButtonGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1, 1, 1 });

            //Adding grids to the grids
            // add second grid to into second row of first grid

            firstgrid.Children.Add(secondGrid);
            Grid.SetColumn(secondGrid, 0);
            Grid.SetRow(secondGrid, 1);

            // add third grid to into first column of second grid
            secondGrid.Children.Add(leftSideGrid);
            Grid.SetColumn(leftSideGrid, 0);
            Grid.SetRow(leftSideGrid, 1);

            // add description grid to into second column of second grid
            secondGrid.Children.Add(TextandImageGrid);
            Grid.SetColumn(TextandImageGrid, 1);
            Grid.SetRow(TextandImageGrid, 1);


            secondGrid.Children.Add(ButtonGrid);
            Grid.SetColumn(ButtonGrid, 1);
            Grid.SetRow(ButtonGrid, 1);

            leftSideGrid.Children.Add(discountGrid);
            Grid.SetColumn(discountGrid, 0);
            Grid.SetRow(discountGrid, 3);

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

            CartDisplay = new StackPanel()
            {
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical,
                
            };

            ProductBox.SelectedIndex = -1;
            leftSideGrid.Children.Add(ProductBox);
            Grid.SetColumn(ProductBox, 0);
            Grid.SetRow(ProductBox, 0);
            ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;

            //shopping cart text
            leftSideGrid.Children.Add(CartDisplay);
            Grid.SetRow(CartDisplay, 1);

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

            RemoveAllProducts = new Button
            {
                Content = "Remove all products from Cart",
                Margin = new Thickness(2),
                Padding = new Thickness(2),
                FontSize = 16,
                Background = Brushes.White,
            };
            leftSideGrid.Children.Add(RemoveAllProducts);
            Grid.SetRow(RemoveAllProducts, 2);

            DiscountLabel = new Label
            {
                Content = "Enter discount code",
                FontSize = 16,
            };
            discountGrid.Children.Add(DiscountLabel);
            Grid.SetColumn(DiscountLabel, 0);
            Grid.SetRow(DiscountLabel, 0);

            DiscountBlock = new TextBox
            {
                Text = "",
                Margin = new Thickness(5),
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Height = 32,
            };
            discountGrid.Children.Add(DiscountBlock);
            Grid.SetColumn(DiscountBlock, 1);
            Grid.SetRow(DiscountBlock, 0);

            if (DiscountBlock.Text == DiscountCodes[0].CodeName)
            {
                //Apply 15% discount to order
            }
            else if (DiscountBlock.Text == DiscountCodes[1].CodeName)
            {
                //Apply 20% discount to order
            }
            else if (DiscountBlock.Text == DiscountCodes[2].CodeName)
            {
                //Apply 30% discount to order
            }
            else if (DiscountBlock.Text == DiscountCodes[3].CodeName)
            {
                //Apply 50% discount to order
            }
            else if (DiscountBlock.Text != "" && DiscountBlock.Text != DiscountCodes[0].CodeName && DiscountBlock.Text != DiscountCodes[1].CodeName && DiscountBlock.Text != DiscountCodes[2].CodeName && DiscountBlock.Text != DiscountCodes[3].CodeName)
            {
                //Show error message "Please enter a valid discount"
            }
            else
            {
                //Go through to showing reciept without a discount applied
            }

            //ApplyDiscount = new Button
            //{
            //    Content = "Apply discount",
            //    FontSize = 16,
            //};
            //discountGrid.Children.Add(ApplyDiscount);
            //Grid.SetColumnSpan(ApplyDiscount, 2);
            //Grid.SetRow(ApplyDiscount, 1);

            CheckoutButton = new Button
            {
                Content = "Check out",
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Background = Brushes.White,
                Height = 25,
            };
            discountGrid.Children.Add(CheckoutButton);
            Grid.SetColumnSpan(CheckoutButton, 2);
            Grid.SetRow(CheckoutButton, 1);
            CheckoutButton.Click += OnCheckoutClick;
            
            var saveCartButton = new Button
            {
                Content = "Save Cart",
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Background = Brushes.White,

            };
            ButtonGrid.Children.Add(saveCartButton);
            Grid.SetColumn(saveCartButton, 2);
            Grid.SetColumnSpan(saveCartButton, 2);
            Grid.SetRow(saveCartButton, 1);
            saveCartButton.Click += OnSaveCartClick;
            
            //Read saved shoppingcart
            if (File.Exists(CartFilePath))
            {
                Cart.LoadFromFile(CartFilePath);
                UpdateCartDisplay();
            }



        }


        private void UpdateDescriptionText(Product product)
        {
            InfoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
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

            var addToCart = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                FontSize = 16,
                BorderThickness = new Thickness(2),
                Background = Brushes.White,
                Tag = SelectedProduct
            };
            ButtonGrid.Children.Add(addToCart);
            Grid.SetColumn(addToCart, 1);
            Grid.SetRow(addToCart, 1);
            addToCart.Click += OnAddClick;
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //First, we store the selected product in our SelectedProduct variable so that other objects can know about it.
            SelectedProduct = (Product)((ListBoxItem)ProductBox.SelectedItem).Tag;

            //Then, we update texts and image
            TextandImageGrid.Children.Clear();
            UpdateDescriptionText(SelectedProduct);
            UpdateProductImage(SelectedProduct.Image);
            UpdatePrice(SelectedProduct);
        }

        private static void OnAddClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            Product product = (Product)s.Tag;
            Cart.Add(product, 1);
            UpdateCartDisplay();
        }

        private static void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var product = (Product)s.Tag;
            Cart.Remove(product, 1);
            UpdateCartDisplay();
        }


        private static void OnCheckoutClick(object sender, RoutedEventArgs e)
        {
            Cart.Clear();
            if (File.Exists(CartFilePath))
            {
                File.Delete(CartFilePath);
            }
            UpdateCartDisplay();
            MessageBox.Show("Show receipt not yet implemented.");

        }
        
        private void OnSaveCartClick(object sender, RoutedEventArgs e)
        {
            Cart.SaveToFile(CartFilePath);
        }



        
        private void UpdatePrice(Product product)
        {
            string a = product.Price.ToString();
            infoPrice.Text = a + "Kr";
        }



        //This method gets called when a new product has been selected
        private void UpdateProductImage(Image image)
        {
            InfoImage = image;
            InfoImage.Stretch = Stretch.Uniform;
            TextandImageGrid.Children.Add(InfoImage);
            Grid.SetColumn(InfoImage, 1);
        }

        public static void UpdateCartDisplay()
        {
            CartDisplay.Children.Clear();
            foreach (var item in Cart.Products)
            {
                var cartGrid = new Grid();
                cartGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
                cartGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(6, GridUnitType.Star)});
                cartGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
                cartGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
                var cartLine = new TextBlock
                {
                    Text = item.Value + "x " + item.Key.Name + " " + item.Key.Price + "kr." + "\n"
                };
                //total += item.Key.Price * item.Value;
                cartGrid.Children.Add(cartLine);
                CartDisplay.Children.Add(cartGrid);
                var addButton = new Button
                {
                    Content = "+",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                cartGrid.Children.Add(addButton);
                Grid.SetColumn(addButton, 2);
                addButton.Click += OnAddClick;

                var removeButton = new Button
                {
                    Content = "-",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                cartGrid.Children.Add(removeButton);
                Grid.SetColumn(removeButton, 1);
                removeButton.Click += OnRemoveClick;
            }
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
