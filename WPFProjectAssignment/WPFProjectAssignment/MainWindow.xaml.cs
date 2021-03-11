using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;

namespace WPFProjectAssignment
{
    


    public partial class MainWindow : Window
    {
        public static ShoppingCart Cart;

        private ListBox ProductBox = new ListBox();
        private static TextBlock ProductDescription = new TextBlock();
        private Image ImageDisplayed = new Image();
        private static Grid TextAndImageGrid = new Grid();
        private static Grid ButtonGrid = new Grid();
        private static StackPanel InfoPanel = new StackPanel();
        private static StackPanel CartDisplay = new StackPanel();
        private static TextBlock ProductPrice = new TextBlock();
        private Button CheckoutButton = new Button();
        private Button EmptyCartButton = new Button();
        private static TextBox CustomerDiscount = new TextBox();
        private static Label DiscountLabel = new Label();
        private static Grid MainGrid = new Grid();
        private static Grid DiscountGrid = new Grid();


        // We store the most recent selected product here
        private static Product SelectedProduct { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }
        
        private void Start()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            Methods.CopyImagesToTempFolder(@"C:\Windows\Temp\PotionShopTempFiles\Images\");

            Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountFilePath);
            Shared.Products = Methods.LoadProducts(Shared.ProductFilePath);
            Cart = new ShoppingCart();
            
            
            // Window options
            Title = "Potion Shop";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            //Our method for creating grids takes the lenght of an int array for number of rows/columns, and the value of integers for their respective height/width. (relative proportions, not pixels)
            MainGrid = Utilities.Methods.CreateGrid(rows: new[] { 1, 9 }, new[] { 1, 2 });
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = Methods.CreateGrid(rows: new[] { 1, 1 }, columns: null);
            Methods.AddToGui(leftSideGrid, MainGrid, 1 , 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            TextAndImageGrid = Methods.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 });
            Methods.AddToGui(TextAndImageGrid, MainGrid, 1, 1);
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Methods.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
            Methods.AddToGui(ButtonGrid, MainGrid, 1, 1);

            //This grid is to divide the space where we show the discount code so that we can display both a label and a text block.
            DiscountGrid = Methods.CreateGrid(rows: null, columns: new []{1, 1});
            Methods.AddToGui(DiscountGrid, ButtonGrid, 1, 2);
            
            //Setting up Controls
            var heading = new TextBlock
            {
                Text = "Potion Shop",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };

            Methods.AddToGui(heading, MainGrid);
            Grid.SetColumnSpan(heading, 2);
            
            ProductBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Shared.Products)
            {
                ProductBox.Items.Add(new ListBoxItem() { Content = product.Name, Tag = product });
            }
            ProductBox.SelectedIndex = -1;
            Methods.AddToGui(ProductBox, leftSideGrid);
            ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;

            CartDisplay = new StackPanel()
            {
                Margin = new Thickness(2),
                Orientation = Orientation.Vertical,
                
            };
            
            //This is where the user can enter their discount code.
            CustomerDiscount = new TextBox
            {
                Text = "",
                Margin = new Thickness(5),
                FontSize = 12,
                BorderThickness = new Thickness(2),
                Height = 32,
            };
            DiscountGrid.Children.Add(CustomerDiscount);
            Grid.SetColumn(CustomerDiscount, 1);
            DiscountLabel = new Label
            {
                Content = "Enter discount code",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            DiscountGrid.Children.Add(DiscountLabel);
            Grid.SetColumn(DiscountLabel, 0);

            //Creating Buttons
            
            EmptyCartButton = Methods.CreateButton("Empty Cart");
            Methods.AddToGui(EmptyCartButton, ButtonGrid, 1, 0);
            EmptyCartButton.Click += OnEmptyCartButtonClick;
            
            CheckoutButton = Methods.CreateButton("Check Out");
            Methods.AddToGui(CheckoutButton, ButtonGrid, 1, 3);
            CheckoutButton.Click += OnCheckoutClick;

            var saveCartButton = Methods.CreateButton("Save Cart");
            Methods.AddToGui(saveCartButton, ButtonGrid, 1, 1);
            saveCartButton.Click += OnSaveCartClick;
            
            //Now, we check if user has a saved cart or not and display a welcome message.
            
            if (File.Exists(Shared.CartFilePath))
            {
                Cart.LoadFromFile(Shared.CartFilePath);
                UpdateCartGui();
                ShowWelcomeScreen("Welcome Back!","Thanks for coming back to our store. We have stored the cart from your last visit so you can just carry on shopping!");
            }
            else ShowWelcomeScreen("Welcome!", "Whether you’re a serious wizard, lazy student, or simply looking to have a laugh, all our products are infused with carefully selected magical properties to achieve optimum impact.\n \n" +
                                   "We just stocked a new batch of the highly sought-after Polyjuice Potion. They won´t last long, so make sure to snatch one before they´re gone!");

            Methods.AddToGui(CartDisplay, leftSideGrid, 1);
        }



        private void UpdateProductImage(string imagePath)
        {
            ImageDisplayed = CreateImage(imagePath);
            ImageDisplayed.Stretch = Stretch.Uniform;
            TextAndImageGrid.Children.Add(ImageDisplayed);
            Grid.SetColumn(ImageDisplayed, 1);
        }
        
        private static void UpdateDescriptionText(Product product)
        {
            InfoPanel = new StackPanel
            
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
            };
            Methods.AddToGui(InfoPanel, TextAndImageGrid);
                
            // The text heading inside the information panel.
            var productName = new TextBlock
            {
                Text = product.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            Methods.AddToGui(productName, InfoPanel);

            ProductDescription = new TextBlock
            {
                Text = product.Description + "\n",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            Methods.AddToGui(ProductDescription, InfoPanel);
            
            var price = product.Price.ToString(CultureInfo.InvariantCulture);
            ProductPrice = new TextBlock
            {
                Text = price + "kr",
                FontSize = 12,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            
            Methods.AddToGui(ProductPrice, InfoPanel);

            var addToCart = Methods.CreateButton("Add to Cart", SelectedProduct);
            InfoPanel.Children.Add(addToCart);
            addToCart.Click += OnAddClick;
            
            //because we hide the buttons on checkout, we make them visible here in case the customer continues shopping.
            ButtonGrid.Visibility = Visibility.Visible;
        }

        private static void UpdateCartGui()
        {
            var totalSum = Math.Round(Cart.Products.Sum(product => product.Key.Price * product.Value), 2);
            
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
                
                Methods.AddToGui(cartLine, cartGrid);
                Methods.AddToGui(cartGrid, CartDisplay);
                

                var addButton = new Button
                {
                    Content = "+",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                Methods.AddToGui(addButton, cartGrid, 0, 2);
                addButton.Click += OnAddClick;

                var removeButton = new Button
                {
                    Content = "-",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                Methods.AddToGui(removeButton, cartGrid, 0, 1);
                removeButton.Click += OnRemoveClick;
            }

            var totalGrid = Methods.CreateGrid(rows: new []{1}, columns: new []{1});
            var totalLine = new TextBlock
            {
                Text = "Total: " + totalSum,
                HorizontalAlignment = HorizontalAlignment.Right
            };
  
            Methods.AddToGui(totalLine, totalGrid);
            Methods.AddToGui(totalGrid, CartDisplay);

            //This is to make CartDisplay visible again in case customers continues shopping after checking out.
            CartDisplay.Visibility = Visibility.Visible;

        }

        private static Image CreateImage(string filePath)
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



        private void ShowWelcomeScreen(string greeting, string message)
        {
            InfoPanel = new StackPanel
            
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(1)
            };
            TextAndImageGrid.Children.Add(InfoPanel);
            Grid.SetColumn(InfoPanel, 0);
            Grid.SetRow(InfoPanel, 0);

            // The text heading inside the information panel.
            var infoHeading = new TextBlock
            {
                Text = greeting,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            InfoPanel.Children.Add(infoHeading);

            ProductDescription = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            Methods.AddToGui(ProductDescription, InfoPanel);
            ImageDisplayed = CreateImage(Shared.WelcomeImagePath);
            ImageDisplayed.Stretch = Stretch.Uniform;
            Methods.AddToGui(ImageDisplayed, TextAndImageGrid, 0, 1);
        }

        private static void ShowReceipt(Receipt receipt)
        {
            TextAndImageGrid.Children.Clear();

            var colorPicker = 0;

            var receiptPanel = new StackPanel();
            var message = new Label
            {
                Content = "Thanks for your order! Here´s your receipt: \n "
            };

            var columnCategories = Methods.CreateGrid(null, columns: new[] {1, 1, 1, 1});

            Label[] categories =
            {
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

            Methods.AddToGui(receiptPanel, TextAndImageGrid);
            Grid.SetColumnSpan(receiptPanel, 2);
            Methods.AddToGui(message, receiptPanel);
            Methods.AddToGui(columnCategories, receiptPanel);
            foreach (var product in receipt.ItemsBreakdown)
            {
                var productRow = Methods.CreateGrid(null, columns: new []{1, 1, 1, 1});
                Label[] productDetails = new[]
                {
                    new Label
                    {
                        Content = product[0]
                    },
                    new Label
                    {
                        Content = product[1]
                    },
                    new Label
                    {
                        Content = product[2]
                    },
                    new Label
                    {
                        Content = product[3]
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
            //var discountCodeRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            colorPicker = 0;
            foreach (var row in receipt.SumBreakdown)
            {
                var summaryRow = Methods.CreateGrid(null, columns: new []{1, 1, 1, 1});
                Label[] summaryLabels = new[]
                {
                    new Label
                    {
                        Content = ""
                    },
                    new Label
                    {
                        Content = ""
                    },
                    new Label
                    {
                        Content = row[0]
                    },
                    new Label
                    {
                        Content = row[1]
                    }
                };
                //Make total price bold
                if (colorPicker == 3)
                {
                    summaryLabels[3].FontWeight = FontWeights.Bold;
                }
                colorPicker++;

                for (int i = 0; i < summaryLabels.Length; i++)
                {
                    summaryRow.Children.Add(summaryLabels[i]);
                    Grid.SetColumn(summaryLabels[i], i);
                }
                receiptPanel.Children.Add(summaryRow);
            }

            Cart.Clear();
            UpdateCartGui();
            ButtonGrid.Visibility = Visibility.Hidden;
            CartDisplay.Visibility = Visibility.Hidden;
        }

        private static void OnAddClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            Product product = (Product)s.Tag;
            Cart.Add(product, 1);
            UpdateCartGui();
        }

        private static void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var product = (Product)s.Tag;
            Cart.Remove(product, 1);
            UpdateCartGui();
        }

        private static void OnCheckoutClick(object sender, RoutedEventArgs e)
        {
            //We check if a valid discount code is applied to the text box and pass the discount object on to receipt class if they match.
            foreach (var code in Utilities.Shared.DiscountCodes)
            {
                if (CustomerDiscount.Text.ToLower() == code.CodeName.ToLower())
                {
                    ShowReceipt(new Receipt(Cart, code)); 
                }
            }
            
            //if no discount is applied in the text box, we just pass the cart. The receipt class will handle that as no discount.
            if (string.IsNullOrEmpty(CustomerDiscount.Text))
            {
                ShowReceipt(new Receipt(Cart));
            }
            
            //if none of the above were true, it means the user entered an incorrect code and is notified by the discount label turning red.
            DiscountLabel.Content = "Enter Discount Code*";
            DiscountLabel.Foreground = Brushes.Crimson;

        }
        
        private static void OnSaveCartClick(object sender, RoutedEventArgs e)
        {
            Cart.SaveToFile(Utilities.Shared.CartFilePath);
        }
        
        private static void OnEmptyCartButtonClick(object sender, RoutedEventArgs e)
        {
            Cart.Clear();
            UpdateCartGui();
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //First, we store the selected product in our SelectedProduct variable so that other objects can know about it.
            SelectedProduct = (Product)((ListBoxItem)ProductBox.SelectedItem).Tag;

            //Then, we update texts and image
            TextAndImageGrid.Children.Clear();
            UpdateDescriptionText(SelectedProduct);
            UpdateProductImage(SelectedProduct.Image);
        }
    }
}
