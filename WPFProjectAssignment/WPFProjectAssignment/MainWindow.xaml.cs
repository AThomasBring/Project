using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;

namespace WPFProjectAssignment
{
    public class DiscountCode
    {
        public string CodeName;
        public int Percentage;
    }

    public partial class MainWindow : Window
    {
        public static DiscountCode[] DiscountCodes;
        public static Product[] Products;
        public static ShoppingCart Cart;

        public const string DiscountFilePath = "DiscountCodes.txt";
        public const string ProductFilePath = "Products.csv";
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";
        public const string WelcomeImagePath = "Images/welcome.jpg";

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

        private static DiscountCode[] LoadCodes()
        {
            if (!File.Exists(DiscountFilePath))
            {
                MessageBox.Show("Could not read discount file.");
            }
            List<DiscountCode> codes = new List<DiscountCode>();
            string[] words = File.ReadAllLines(DiscountFilePath);

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
                    //We are using \ as separator because we use commas in the text file.
                    var parts = line.Split('\\');

                    // Then create a product with its values set to the different parts of the line.
                    var p = new Product
                    {
                        Code = parts[0],
                        Name = parts[1],
                        Description = parts[2],
                        Price = int.Parse(parts[3]),
                        Image = CreateImage(@"C:\Windows\Temp\PotionShopTempFiles\Images\" + parts[4])
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


        private void Start()
        {
            //Copy images to temp folder
            Directory.CreateDirectory(@"C:\Windows\Temp\PotionShopTempFiles\Images\");
            foreach (string newPath in Directory.GetFiles(@"Images\"))
                //We need to extract the filename from the path
            {
                int fileNameIndex = newPath.LastIndexOf('\\');
                string fileName = newPath.Substring(fileNameIndex + 1);
                
                File.Copy(newPath, @"C:\Windows\Temp\PotionShopTempFiles\Images\"+fileName, true);
            }

            
            
            
            DiscountCodes = LoadCodes();
            Products = LoadProducts();
            Cart = new ShoppingCart();
            
            
            // Window options
            Title = "Potion Shop";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            //Our method for creating grids takes the lenght of an int array for number of rows/columns, and the value of integers for their respective height/width. (relative proportions, not pixels)
            MainGrid = CreateGrid(rows: new[] { 1, 9 }, new[] { 1, 2 });
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = CreateGrid(rows: new[] { 1, 1 }, columns: null);
            AddToGui(leftSideGrid, MainGrid, 1 , 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            TextAndImageGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 });
            AddToGui(TextAndImageGrid, MainGrid, 1, 1);
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
            AddToGui(ButtonGrid, MainGrid, 1, 1);

            //This grid is to divide the space where we show the discount code so that we can display both a label and a text block.
            DiscountGrid = CreateGrid(rows: null, columns: new []{1, 1});
            AddToGui(DiscountGrid, ButtonGrid, 1, 2);


            
            
            //Setting up Controls
            var heading = new TextBlock
            {
                Text = "Potion Shop",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };

            AddToGui(heading, MainGrid);
            Grid.SetColumnSpan(heading, 2);
            
            ProductBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Products)
            {
                ProductBox.Items.Add(new ListBoxItem() { Content = product.Name, Tag = product });
            }
            ProductBox.SelectedIndex = -1;
            AddToGui(ProductBox, leftSideGrid);
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
            
            EmptyCartButton = CreateButton("Empty Cart");
            AddToGui(EmptyCartButton, ButtonGrid, 1, 0);
            EmptyCartButton.Click += OnEmptyCartButtonClick;
            
            CheckoutButton = CreateButton("Check Out");
            AddToGui(CheckoutButton, ButtonGrid, 1, 3);
            CheckoutButton.Click += OnCheckoutClick;

            var saveCartButton = CreateButton("Save Cart");
            AddToGui(saveCartButton, ButtonGrid, 1, 1);
            saveCartButton.Click += OnSaveCartClick;
            
            //Now, we check if user has a saved cart or not and display a welcome message.
            
            if (File.Exists(CartFilePath))
            {
                Cart.LoadFromFile(CartFilePath);
                UpdateCartGui();
                ShowWelcomeScreen("Welcome Back!","Thanks for coming back to our store. We have stored the cart from your last visit so you can just carry on shopping!");
            }
            else ShowWelcomeScreen("Welcome!", "Whether you’re a serious wizard, lazy student, or simply looking to have a laugh, all our products are infused with carefully selected magical properties to achieve optimum impact.\n \n" +
                                   "We just stocked a new batch of the highly sought-after Polyjuice Potion. They won´t last long, so make sure to snatch one before they´re gone!");

            AddToGui(CartDisplay, leftSideGrid, 1);
        }
        
        //This method gets called when a new product has been selected
        private void UpdateProductImage(Image image)
        {
            ImageDisplayed = image;
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
            AddToGui(InfoPanel, TextAndImageGrid);
                
            // The text heading inside the information panel.
            var productName = new TextBlock
            {
                Text = product.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            AddToGui(productName, InfoPanel);

            ProductDescription = new TextBlock
            {
                Text = product.Description + "\n",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            AddToGui(ProductDescription, InfoPanel);
            
            var price = product.Price.ToString();
            ProductPrice = new TextBlock
            {
                Text = price + "kr",
                FontSize = 12,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            
            AddToGui(ProductPrice, InfoPanel);

            var addToCart = CreateButton("Add to Cart", SelectedProduct);
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
                
                AddToGui(cartLine, cartGrid);
                AddToGui(cartGrid, CartDisplay);
                

                var addButton = new Button
                {
                    Content = "+",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                AddToGui(addButton, cartGrid, 0, 2);
                addButton.Click += OnAddClick;

                var removeButton = new Button
                {
                    Content = "-",
                    Tag = item.Key,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };
                AddToGui(removeButton, cartGrid, 0, 1);
                removeButton.Click += OnRemoveClick;
            }

            var totalGrid = CreateGrid(rows: new []{1}, columns: new []{1});
            var totalLine = new TextBlock
            {
                Text = "Total: " + totalSum,
                HorizontalAlignment = HorizontalAlignment.Right
            };
  
            AddToGui(totalLine, totalGrid);
            AddToGui(totalGrid, CartDisplay);

            //This is to make CartDisplay visible again in case customers continues shopping after checking out.
            CartDisplay.Visibility = Visibility.Visible;

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
        private static Button CreateButton(string content, Product tag = null)
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
        
        private static void AddToGui(UIElement element, Panel panel, int row = 0, int column = 0)
        {
            panel.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
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
            AddToGui(ProductDescription, InfoPanel);
            ImageDisplayed = CreateImage(WelcomeImagePath);
            ImageDisplayed.Stretch = Stretch.Uniform;
            AddToGui(ImageDisplayed, TextAndImageGrid, 0, 1);
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

            var columnCategories = CreateGrid(null, columns: new []{1, 1, 1, 1});
            
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
            
            AddToGui(receiptPanel, TextAndImageGrid);
            Grid.SetColumnSpan(receiptPanel, 2);
            AddToGui(message, receiptPanel);
            AddToGui(columnCategories, receiptPanel);

            
            foreach (var product in receipt.ItemsBreakdown)
            {
                var productRow = CreateGrid(null, columns: new []{1, 1, 1, 1});
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
            
            var discountCodeRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            colorPicker = 0;
            foreach (var row in receipt.SumBreakdown)
            {
                var summaryRow = CreateGrid(null, columns: new []{1, 1, 1, 1});
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
            

            /*var discountLabel = new Label
            {
                Content = receipt.AmountSummary[0][0]
            };
            AddToGui(discountLabel, discountCodeRow, 0, 2);

            var discountUsed = new Label
            {
                Content = receipt.AmountSummary[0][1]
            };
            AddToGui(discountUsed, discountCodeRow, 0 , 3);

            
            //Tom label för att skapa lite mellanrum.
            receiptPanel.Children.Add(new Label());
            
            AddToGui(discountCodeRow, receiptPanel);
            
            var sumRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            var sumLabel = new Label
            {
                Content = receipt.AmountSummary[1][0]
            };
            sumRow.Children.Add(sumLabel);
            Grid.SetColumn(sumLabel, 2);
            
            AddToGui(new Label
            {
                Content = receipt.AmountSummary[1][1]
            }, sumRow, 0, 3);

            receiptPanel.Children.Add(sumRow);
            
            var appliedDiscountRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            var appliedDiscountLabel = new Label
            {
                Content = receipt.AmountSummary[2][0]
            };
            appliedDiscountRow.Children.Add(appliedDiscountLabel);
            Grid.SetColumn(appliedDiscountLabel, 2);
            
            var appliedDiscountAmount = new Label
            {
                Content = receipt.AmountSummary[2][1]
            };
            appliedDiscountRow.Children.Add(appliedDiscountAmount);
            Grid.SetColumn(appliedDiscountAmount, 3);

            receiptPanel.Children.Add(appliedDiscountRow);

            var totalWithDiscountRow = CreateGrid(null, columns: new []{1, 1, 1, 1});

            var totalWithDiscountLabel = new Label
            {
                Content = receipt.AmountSummary[3][0]
            };
            totalWithDiscountRow.Children.Add(totalWithDiscountLabel);
            Grid.SetColumn(totalWithDiscountLabel, 2);
            
            
            var totalWithDiscountAmount = new Label
            {
                Content = receipt.AmountSummary[3][1],
                FontWeight = FontWeights.Bold
            };
            totalWithDiscountRow.Children.Add(totalWithDiscountAmount);
            Grid.SetColumn(totalWithDiscountAmount, 3);

            receiptPanel.Children.Add(totalWithDiscountRow);*/
            
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
            foreach (var code in DiscountCodes)
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
            Cart.SaveToFile(CartFilePath);
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
