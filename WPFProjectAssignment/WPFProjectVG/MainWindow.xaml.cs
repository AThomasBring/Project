using System;
using System.CodeDom;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
using Utilities;
using WPFProjectAssignment;

namespace WPFProjectVG
{
    public partial class MainWindow : Window
    {
        private Product[] TempProducts;

        Grid MainGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        public Button EditButton = new Button();
        StackPanel EditPanel = new StackPanel();

        private Grid DiscountGrid = new Grid();

        public Product TempProduct;
        public string TempImagePath;
        public string TempImageFileName;

        public Product[] TempProducts;

        private Label DiscountPercentage = new Label();
        private TextBox InsertNewPercentage = new TextBox();
        private Label NewDiscount = new Label();
        private TextBox InsertNewDiscount = new TextBox();

        private TextBox editCode = new TextBox();

        private TextBox editName = new TextBox();

        private TextBox editDescription = new TextBox();

        private TextBox editPrice = new TextBox();

        private Label priceLabel = new Label();
        private Label codeLabel = new Label();
        private Label descriptionLabel = new Label();
        private Label nameLabel = new Label();
        private Label Message = new Label();


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

            // Window options
            Title = "Behind the magic curtain";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Our method for creating grids takes the lenght of an int array for number of rows/columns, and the value of integers for their respective height/width. (relative proportions, not pixels)
            MainGrid = Methods.CreateGrid(rows: new[] {1, 9}, new[] {1, 2});
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = Methods.CreateGrid(rows: new[] {1, 1}, columns: null);
            Methods.AddToGui(leftSideGrid, MainGrid, 1, 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            Shared.TextAndImageGrid = Methods.CreateGrid(rows: new[] {5, 1}, columns: new[] {1, 1});
            Methods.AddToGui(Shared.TextAndImageGrid, MainGrid, 1, 1);

            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Methods.CreateGrid(rows: new[] {5, 1}, columns: new[] {2, 2, 3, 1});
            Methods.AddToGui(ButtonGrid, MainGrid, 1, 1);

            MainGrid.ShowGridLines = true;



            //Setting up Controls
            var heading = new TextBlock
            {
                Text = "Manage Inventory",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };

            Methods.AddToGui(heading, MainGrid);
            Grid.SetColumnSpan(heading, 2);

            Shared.ProductBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Shared.Products)
            {
                Shared.ProductBox.Items.Add(new ListBoxItem() {Content = product.Name, Tag = product});
            }

            Shared.ProductBox.SelectedIndex = -1;
            Methods.AddToGui(Shared.ProductBox, leftSideGrid);
            Shared.ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;

            ListBox DiscountBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var discountCode in Shared.DiscountCodes)
            {
                DiscountBox.Items.Add(new ListBoxItem() { Content = discountCode.CodeName }); 
            }
            DiscountBox.SelectedIndex = -1;
            Methods.AddToGui(DiscountBox, DiscountGrid, 0, 0);
            DiscountBox.SelectionChanged += DiscountBox_SelectionChanged;

            

            Button AddNewDicountCode = new Button
            {
                Content = "Add new discount Code"
            };
            Methods.AddToGui(AddNewDicountCode, DiscountGrid, 1, 0);
            Grid.SetColumnSpan(AddNewDicountCode, 2);
            AddNewDicountCode.Click += AddDiscountButton_Click;

        }

        private void DiscountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Inte implementerat än.
            throw new NotImplementedException();
        }

        private void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        {

            //Inte implementerade än.
            NewDiscount = new Label
            {
                Content = ("Enter a new discount Code: "),

            };
            InsertNewDiscount = new TextBox
            {
                Text = "",
                Margin = new Thickness(5),
                FontSize = 12,
                BorderThickness = new Thickness(2),
                Height = 32,
            };
            DiscountPercentage = new Label
            {
                Content = "Enter the percentage: ",

            };
            InsertNewPercentage = new TextBox
            {
                Text = "",
                Margin = new Thickness(5),
                FontSize = 12,
                BorderThickness = new Thickness(2),
                Height = 32,
            };
            
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            Product product = (Product) s.Tag;

            UpdateEditDisplay(product);
        }

        private void UpdateEditDisplay(Product product)
        {
            Shared.TextAndImageGrid.Children.Clear();
            ButtonGrid.Children.Clear();
            Methods.UpdateProductImage(Shared.ImageFolderPath + Shared.SelectedProduct.Image);
            
            codeLabel = new Label
            {
                Content = "Product Code",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular,
            };

            editCode.Text = product.Code;
            editCode.Tag = codeLabel;

            nameLabel = new Label
            {
                Content = "Product Name",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };

            editName.Text = product.Name;

            descriptionLabel = new Label
            {
                Content = "Product Description",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };

            editDescription.Text = product.Description;
            


            priceLabel = new Label
            {
                Content = "Product Price",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };
            
            editPrice.Text = product.Price.ToString();

            Label imageLabel = new Label
            {
                Content = "Upload Image"
            };

            Button saveChangesButton = Methods.CreateButton("Save Changes", Shared.SelectedProduct);
            saveChangesButton.Click += OnSaveChangesClick;

            Button uploadImageButton = Methods.CreateButton("Select File", Shared.SelectedProduct);
            uploadImageButton.Click += OnUploadButtonClick;

            EditPanel.Children.Clear();
            Methods.AddToGui(EditPanel, Shared.TextAndImageGrid, 0, 0);
            Methods.AddToGui(codeLabel, EditPanel);
            Methods.AddToGui(editCode, EditPanel);
            Methods.AddToGui(descriptionLabel, EditPanel);
            Methods.AddToGui(editDescription, EditPanel);
            Methods.AddToGui(nameLabel, EditPanel);
            Methods.AddToGui(editName, EditPanel);
            Methods.AddToGui(priceLabel, EditPanel);
            Methods.AddToGui(editPrice, EditPanel);
            Methods.AddToGui(imageLabel, EditPanel);
            Methods.AddToGui(uploadImageButton, EditPanel);
            Methods.AddToGui(saveChangesButton, ButtonGrid, 1, 1);
            Methods.AddToGui(Message, EditPanel);
        }
        
        
        private void OnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            var productTag = (Product) s.Tag;
            
            //First we make sure that the price can be stored as a decimal
            editPrice.Text = editPrice.Text.Replace(',', '.');
            editPrice.Text = editPrice.Text.Trim();
            decimal price;
            if (!decimal.TryParse(editPrice.Text, out price))
            {
                priceLabel.Foreground = Brushes.Crimson;
                priceLabel.FontWeight = FontWeights.Bold;
                Message.Content = "The price is not a valid number.";
                
                return;
            }
            
            //We make sure the texboxes have no '\' characters, because we use them as separators in the text files.
            if (CheckInvalidCharacter(editCode, codeLabel, '\\'))
            {
                return;
            }
            if (CheckInvalidCharacter(editName, nameLabel, '\\'))
            {
                return;
            }
            if (CheckInvalidCharacter(editDescription, descriptionLabel, '\\'))
            {
                return;
            }



            var tempProduct = new Product();
            tempProduct.Code = editCode.Text;
            tempProduct.Name = editName.Name;
            tempProduct.Description = editDescription.Text;
            tempProduct.Price = price;
            tempProduct.Image = TempImageFileName;
            
            //Check if product code duplicates and update Temp Products
            if (CheckProductCodeDuplicates(tempProduct))
            {
                Message.Content = "Every product code needs to be unique";
                return;
            }
            
        }

        private static bool CheckProductCodeDuplicates(Product tempProduct)
        {
            //We make a temporary list to run some tests on
            List<Product> productList = new List<Product>();

            for (int i = 0; i < Shared.Products.Length; i++)
            {
                if (Shared.Products[i].Code != Shared.SelectedProduct.Code)
                {
                    //all un-edited products gets added first
                    productList.Add(Shared.Products[i]);
                }
                else
                {
                    //If the selected product is a match, we add the temp product since this is the one the user have edited.
                    productList.Add(tempProduct);
                }
            }
            
            // if there are duplicates
            if (productList.Count() != productList.Select(p => p.Code).Distinct().Count())
            {
                return true;
            }
            
            //all codes are unique
            return false;

        }

        private bool CheckInvalidCharacter(TextBox textbox, Label label, char c)
        {
            if (!textbox.Text.Contains(c)) return false;
            label.Foreground = Brushes.Crimson;
            label.FontWeight = FontWeights.Bold;
            Message.Content = label.Content + " can not include any " + "c" + "characters.";
            return true;
        }

        private void SaveToFile(Product product, string path)
        {
            var products = Shared.Products.ToList();
            var selectedCode = Shared.SelectedProduct.Code;
            
            /*
            var productsString = new []{Shared.Products.Length}
            for (int i = 0; i < Shared.Products.Length; i++)
            {
                productsString[i] = products[i].Code + @"\";
                productsString[i] += products[i].Name + @"\";
                productsString[i] += products[i].Description + @"\";
                productsString[i] += products[i].Price + @"\";
                productsString[i] += products[i].Image;
            }
            */

            //File.WriteAllLines(path);
            if (TempImagePath != Shared.ImageFolderPath + Shared.SelectedProduct.Image)
            {
                //Copy Image if changed.
                File.Copy(TempImagePath, Shared.ImageFolderPath + product.Image, true);
            }
        }

        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            var product = (Product) s.Tag;
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter =  "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png";
            bool? result = dialog.ShowDialog();
            
            Shared.TextAndImageGrid.Children.Remove(Shared.ImageDisplayed);
            if (dialog.ValidateNames)
            {
                
            }
            Methods.UpdateProductImage(dialog.FileName);

            //We only make changes to the temp product until user chooses to save changes.
            int fileNameIndex = dialog.FileName.LastIndexOf('\\');
            string fileName = dialog.FileName.Substring(fileNameIndex + 1);
            TempImageFileName = fileName;
            TempImagePath = dialog.FileName;
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //We reload products to update any saved data to the csv file
            Shared.Products = Methods.LoadProducts(Shared.ProductFilePath);
            

            //We store the selected product in our SelectedProduct variable so that other parts of the program know about it.
            Shared.SelectedProduct = (Product) ((ListBoxItem) Shared.ProductBox.SelectedItem).Tag;
            
            //We copy the selected product to temporary placeholders until customer save.
            TempImagePath = Shared.ImageFolderPath + Shared.SelectedProduct.Image;

            //Then, we update texts and image
            Shared.TextAndImageGrid.Children.Clear();
            Methods.UpdateDescriptionText(Shared.SelectedProduct);
            Methods.UpdateProductImage(Shared.ImageFolderPath + Shared.SelectedProduct.Image);
            EditButton = Methods.CreateButton("Edit Product", Shared.SelectedProduct);
            
            Methods.AddToGui(EditButton, ButtonGrid, 1);
            EditButton.Click += OnEditButtonClick;

        }
        
    }
}
