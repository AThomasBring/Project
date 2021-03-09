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
        private static TextBlock InfoText = new TextBlock();
        private Image InfoImage = new Image();
        private static Grid textAndImageGrid = new Grid();
        private static Grid buttonGrid = new Grid();
        private static StackPanel InfoPanel = new StackPanel();
        private static StackPanel CartDisplay = new StackPanel();
        private static TextBlock infoPrice = new TextBlock();
        private Button CheckoutButton = new Button();
        private Button RemoveAllProducts = new Button();
        private TextBox DiscountBlock = new TextBox();
        private Label DiscountLabel = new Label();
        private static Grid firstGrid = new Grid();
        private static Grid discountGrid = new Grid();



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
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Main grid
            firstGrid = CreateGrid(rows: new[] { 1, 9 }, new[] { 1, 2 });
            Content = firstGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = CreateGrid(rows: new[] { 1, 1 }, columns: null);

            // This grid is for item description and image, and gets cleared and updated every selection change
            textAndImageGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 });
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            buttonGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
            
            //This grid is to divide the space where we show the discount code so that we can display both a label and a text block.
            discountGrid = CreateGrid(rows: null, columns: new []{1, 1});

            
            //Setting up the grids
            
            firstGrid.Children.Add(leftSideGrid);
            Grid.SetColumn(leftSideGrid, 0);
            Grid.SetRow(leftSideGrid, 1);

            // add description grid to into second column of second grid
            firstGrid.Children.Add(textAndImageGrid);
            Grid.SetColumn(textAndImageGrid, 1);
            Grid.SetRow(textAndImageGrid, 1);


            firstGrid.Children.Add(buttonGrid);
            Grid.SetColumn(buttonGrid, 1);
            Grid.SetRow(buttonGrid, 1);

            // A text heading.
            var heading = new TextBlock
            {
                Text = "Potion Shop",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };


            firstGrid.Children.Add(heading);
            Grid.SetColumnSpan(heading, 2);
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
            leftSideGrid.Children.Add(ProductBox);
            Grid.SetColumn(ProductBox, 0);
            Grid.SetRow(ProductBox, 0);
            ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;

            CartDisplay = new StackPanel()
            {
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical,
                
            };
            
            if (File.Exists(CartFilePath))
            {
                Cart.LoadFromFile(CartFilePath);
                UpdateCartDisplay();
                UpdateButtons();
            }
            //shopping cart text
            
            leftSideGrid.Children.Add(CartDisplay);
            Grid.SetRow(CartDisplay, 1);

            
            
            //Read saved shoppingcart




        }

        private static void RemoveAllProducts_Click(object sender, RoutedEventArgs e)
        {
            
            Cart.Clear();
            UpdateCartDisplay();

        }

        private void UpdateDescriptionText(Product product)
        {
            InfoPanel = new StackPanel
            
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
            };
            textAndImageGrid.Children.Add(InfoPanel);
            Grid.SetColumn(InfoPanel, 0);
            Grid.SetRow(InfoPanel, 0);

            // The text heading inside the information panel.
            TextBlock infoHeading = new TextBlock
            {
                Text = product.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            InfoPanel.Children.Add(infoHeading);

            InfoText = new TextBlock
            {
                Text = product.Description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            InfoPanel.Children.Add(InfoText);
            InfoPanel.Children.Add(new TextBlock());

            infoPrice = new TextBlock
            {
                Text = "",
                FontSize = 12,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            InfoPanel.Children.Add(infoPrice);
            
            
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var addToCart = CreateButton("Add to Cart", SelectedProduct);
            InfoPanel.Children.Add(addToCart);
            addToCart.Click += OnAddClick;


            RemoveAllProducts = CreateButton("Empty Cart");
            buttonGrid.Children.Add(RemoveAllProducts);
            Grid.SetRow(RemoveAllProducts, 1);
            RemoveAllProducts.Click += RemoveAllProducts_Click;

            DiscountLabel = new Label
            {
                Content = "Enter discount code",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            discountGrid.Children.Add(DiscountLabel);
            Grid.SetColumn(DiscountLabel, 0);

            DiscountBlock = new TextBox
            {
                Text = "",
                Margin = new Thickness(5),
                FontSize = 12,
                BorderThickness = new Thickness(2),
                Height = 32,
            };
            discountGrid.Children.Add(DiscountBlock);
            Grid.SetColumn(DiscountBlock, 1);
            
            buttonGrid.Children.Add(discountGrid);
            Grid.SetColumn(discountGrid, 2);
            Grid.SetRow(discountGrid, 1);

            CheckoutButton = CreateButton("Check Out");
            buttonGrid.Children.Add(CheckoutButton);
            Grid.SetRow(CheckoutButton, 1);
            Grid.SetColumn(CheckoutButton, 3);
            CheckoutButton.Click += OnCheckoutClick;

            var saveCartButton = CreateButton("Save Cart");
            buttonGrid.Children.Add(saveCartButton);
            Grid.SetColumn(saveCartButton, 1);
            Grid.SetRow(saveCartButton, 1);
            saveCartButton.Click += OnSaveCartClick;
        }

        private static void ShowReceipt()
        {
            textAndImageGrid.Children.Clear();

            double totalAmount = 0;
            double discountRatio = 0;

            foreach (var product in Cart.Products)
            {
                totalAmount += (double)product.Key.Price * product.Value;
            }

            StackPanel receiptPanel = new StackPanel();
            textAndImageGrid.Children.Add(receiptPanel);
            Grid.SetColumn(receiptPanel, 0);
            Grid.SetColumnSpan(receiptPanel, 2);
            Grid.SetRow(receiptPanel, 0);

            Label message = new Label
            {
                Content = "Thanks for your order! Here´s your reciept: \n "
            };

            receiptPanel.Children.Add(message);

            Grid columnCategories = CreateGrid(null, columns: new []{1, 1, 1, 1});
            
            Label[] categories = {
                new Label
                {
                    Content = "Product",
                    Background = Brushes.LightBlue
                },
                new Label
                {
                    Content = "Quantity",
                    Background = Brushes.LightBlue
                },
                new Label
                {
                    Content = "Unit Price",
                    Background = Brushes.LightBlue
                },
                new Label
                {
                    Content = "Total Price",
                    Background = Brushes.LightBlue
                }
            };

            for (int i = 0; i < categories.Length; i++)
            {
                columnCategories.Children.Add(categories[i]);
                Grid.SetColumn(categories[i], i);
            }
            
            receiptPanel.Children.Add(columnCategories);

            var colorPicker = 0;
            foreach (var product in Cart.Products)
            {
                Grid productRow = CreateGrid(null, columns: new []{1, 1, 1, 1});
                Label[] productDetails = new[]
                {
                    new Label
                    {
                        Content = product.Key.Name
                    },
                    new Label
                    {
                        Content = product.Value
                    },
                    new Label
                    {
                        Content = product.Key.Price + "kr"
                    },
                    new Label
                    {
                        Content = product.Key.Price * product.Value + "kr"
                    },
                };

                if (colorPicker %2 != 0)
                {
                    foreach (var label in productDetails)
                    {
                        label.Background = Brushes.Honeydew;
                    }
                }
                
                for (int i = 0; i < productDetails.Length; i++)
                {
                    productRow.Children.Add(productDetails[i]);
                    Grid.SetColumn(productDetails[i], i);
                }

                receiptPanel.Children.Add(productRow);
                colorPicker++;
            }

            
            
            Grid discountCodeRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            Label discountLabel = new Label
            {
                Content = "Discount Code"
            };
            discountCodeRow.Children.Add(discountLabel);
            Grid.SetColumn(discountLabel, 2);

            Label discountUsed = new Label
            {
                Content = "Not implemented."
            };
            discountCodeRow.Children.Add(discountUsed);
            Grid.SetColumn(discountUsed, 3);
            
            //Tom label för att skapa lite mellanrum.
            receiptPanel.Children.Add(new Label());
            
            receiptPanel.Children.Add(discountCodeRow);


            Grid sumRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            Label sumLabel = new Label
            {
                Content = "Total:",
            };
            sumRow.Children.Add(sumLabel);
            Grid.SetColumn(sumLabel, 2);

            var sumstring = Convert.ToString(totalAmount);
            Label sumAmount = new Label
            {
                Content = sumstring + "kr",
            };
            sumRow.Children.Add(sumAmount);
            Grid.SetColumn(sumAmount, 3);

            receiptPanel.Children.Add(sumRow);
            
            
            
            Grid appliedDiscountRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            Label appliedDiscountLabel = new Label
            {
                Content = "Your discount:"
            };
            appliedDiscountRow.Children.Add(appliedDiscountLabel);
            Grid.SetColumn(appliedDiscountLabel, 2);

            var appliedDiscountString = Convert.ToString(totalAmount*discountRatio);
            Label appliedDiscountAmount = new Label
            {
                Content = appliedDiscountString + "kr (" +discountRatio*100 + "%)"
            };
            appliedDiscountRow.Children.Add(appliedDiscountAmount);
            Grid.SetColumn(appliedDiscountAmount, 3);

            receiptPanel.Children.Add(appliedDiscountRow);
            
            
            
            Grid totalWithDiscountRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            Label totalWithDiscountLabel = new Label
            {
                Content = "After Discount:"
            };
            totalWithDiscountRow.Children.Add(totalWithDiscountLabel);
            Grid.SetColumn(totalWithDiscountLabel, 2);

            var totalWithDiscountString = Convert.ToString(totalAmount - totalAmount*discountRatio);
            Label totalWithDiscountAmount = new Label
            {
                Content = totalWithDiscountString + "kr",
                FontWeight = FontWeights.Bold
            };
            totalWithDiscountRow.Children.Add(totalWithDiscountAmount);
            Grid.SetColumn(totalWithDiscountAmount, 3);

            receiptPanel.Children.Add(totalWithDiscountRow);
            
            
            Cart.Clear();
            UpdateCartDisplay();
            buttonGrid.Children.Clear();



        }


        
        private static Button CreateButton(string content)
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
            return newButton;
        }
        
        //We overload this method so we can add a tag to the buttons who need it.
        private static Button CreateButton(string content, Product tag)
        {
            var newButton = CreateButton(content);
            newButton.Tag = tag;
            return newButton;
        }




        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //First, we store the selected product in our SelectedProduct variable so that other objects can know about it.
            SelectedProduct = (Product)((ListBoxItem)ProductBox.SelectedItem).Tag;

            //Then, we update texts and image
            textAndImageGrid.Children.Clear();
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
            var a = (Button)sender;
            
            //if (DiscountBlock.Text == DiscountCodes[0].CodeName)
            //{
            //    //Apply 15% discount to order
            //}
            //else if (DiscountBlock.Text == DiscountCodes[1].CodeName)
            //{
            //    //Apply 20% discount to order
            //}
            //else if (DiscountBlock.Text == DiscountCodes[2].CodeName)
            //{
            //    //Apply 30% discount to order
            //}
            //else if (DiscountBlock.Text == DiscountCodes[3].CodeName)
            //{
            //    //Apply 50% discount to order
            //}
            //else if (DiscountBlock.Text != "" && DiscountBlock.Text != DiscountCodes[0].CodeName && DiscountBlock.Text != DiscountCodes[1].CodeName && DiscountBlock.Text != DiscountCodes[2].CodeName && DiscountBlock.Text != DiscountCodes[3].CodeName)
            //{
            //    //Show error message "Please enter a valid discount"
            //}
            //else
            //{
            //    //Go through to showing reciept without a discount applied
            //}
            
            ShowReceipt();

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
            textAndImageGrid.Children.Add(InfoImage);
            Grid.SetColumn(InfoImage, 1);
        }

        public static void UpdateCartDisplay()
        {
            decimal totalSum = 0;
            
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

                totalSum = totalSum + item.Value * item.Key.Price;
            }

            var totalGrid = CreateGrid(rows: new []{1}, columns: new []{1});
            var totalLine = new TextBlock
            {
                Text = "Total: " + totalSum,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            //total += item.Key.Price * item.Value;
            totalGrid.Children.Add(totalLine);
            CartDisplay.Children.Add(totalGrid);
            
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
