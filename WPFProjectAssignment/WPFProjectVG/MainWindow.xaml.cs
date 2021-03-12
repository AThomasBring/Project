using System;
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

        Grid MainGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        public Button EditButton = new Button();


        public Product TempProduct;
        public string TempImagePath;
        public Product[] TempProducts;
        

        private TextBox editCode = new TextBox();

        private TextBox editName = new TextBox();

        private TextBox editDescription = new TextBox();

        private TextBox editPrice = new TextBox();

        
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
            MainGrid = Methods.CreateGrid(rows: new[] { 1, 9 }, new[] { 1, 2 });
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = Methods.CreateGrid(rows: new[] { 1, 1 }, columns: null);
            Methods.AddToGui(leftSideGrid, MainGrid, 1 , 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            Shared.TextAndImageGrid = Methods.CreateGrid(rows: new[] {5, 1}, columns: new[] {1, 1});
            Methods.AddToGui(Shared.TextAndImageGrid, MainGrid, 1, 1);
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Methods.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
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
                Shared.ProductBox.Items.Add(new ListBoxItem() { Content = product.Name, Tag = product });
            }
            Shared.ProductBox.SelectedIndex = -1;
            Methods.AddToGui(Shared.ProductBox, leftSideGrid);
            Shared.ProductBox.SelectionChanged += ProductBoxOnSelectionChanged;
            



        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            Product product = (Product) s.Tag;
            Shared.TextAndImageGrid.Children.Clear();
            UpdateEditDisplay(product);
        }

        private void UpdateEditDisplay(Product product)
        {

            Methods.UpdateProductImage(Shared.ImageFolderPath + Shared.SelectedProduct.Image);
            StackPanel editPanel = new StackPanel();
            Label codeLabel = new Label
            {
                Content = "Product Code:"
            };

            editCode.Text = product.Code;

            Label nameLabel = new Label
            {
                Content = "Product Name:"
            };

            editName.Text = product.Name;

            Label descriptionLabel = new Label
            {
                Content = "Product Description:"
            };

            editDescription.Text = product.Description;
           
            
            Label priceLabel = new Label
            {
                Content = "Product Price:"
            };

            editPrice.Text = product.Price.ToString();

            Label imageLabel = new Label
            {
                Content = "Upload Image:"
            };

            Button saveChangesButton = Methods.CreateButton("Save Changes", Shared.SelectedProduct);
            saveChangesButton.Click += OnSaveChangesClick;
            
            Button uploadImageButton = Methods.CreateButton("Select File", Shared.SelectedProduct);
            uploadImageButton.Click += OnUploadButtonClick;

            Methods.AddToGui(editPanel, Shared.TextAndImageGrid, 0, 0 );
            Methods.AddToGui(codeLabel, editPanel);
            Methods.AddToGui(editCode, editPanel);
            Methods.AddToGui(descriptionLabel, editPanel);
            Methods.AddToGui(editDescription, editPanel);
            Methods.AddToGui(nameLabel, editPanel);
            Methods.AddToGui(editName, editPanel);
            Methods.AddToGui(priceLabel, editPanel);
            Methods.AddToGui(editPrice, editPanel);
            Methods.AddToGui(imageLabel, editPanel);
            Methods.AddToGui(uploadImageButton, editPanel);
            Methods.AddToGui(saveChangesButton, ButtonGrid, 1, 1);
            
        }

        private void OnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            var productTag = (Product)s.Tag;

            List<Product> checkDuplicateCodes = new List<Product>();
            foreach (var product in Shared.Products)
            {
                if (productTag.Code != product.Code)
                {
                    checkDuplicateCodes.Add(product);
                }
                else checkDuplicateCodes.Add(TempProduct);
            }

            var duplicates = checkDuplicateCodes.GroupBy(p => p.Code).Any(p => p.Count() > 1);
            MessageBox.Show(duplicates.ToString());

            if (duplicates)
            {
                MessageBox.Show("Please enter a unique code for each product.");
                return;
            }
            
                        
            //Copy Image
            File.Copy(TempImagePath, Shared.ImageFolderPath + TempProduct.Image, true);



            TempProduct.Code = editCode.Text;
            TempProduct.Name = editName.Text;
            TempProduct.Description = editDescription.Text;
            try
            {
                TempProduct.Price = Convert.ToDecimal(editPrice.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Not a valid price");
            }
            


            SaveToFile(productTag);
        }

        private void SaveToFile(Product editedProduct)
        {
            TempProducts = new Product[Shared.Products.Length];
            for (int i = 0; i < Shared.Products.Length; i++)
            {
                if (Shared.Products[i].Code == editedProduct.Code)
                {
                    TempProducts[i] = TempProduct;
                }
                else
                    TempProducts[i] = Shared.Products[i];
            }

            string[] products = new string[TempProducts.Length];
            for (int i = 0; i < TempProducts.Length; i++)
            {
                products[i] = TempProducts[i].Code + @"\";
                products[i] += TempProducts[i].Name + @"\";
                products[i] += TempProducts[i].Description + @"\";
                products[i] += TempProducts[i].Price + @"\";
                products[i] += TempProducts[i].Image;
            }

            File.WriteAllLines(Shared.ProductFilePath, products);
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
            TempProduct.Image = fileName;
            TempImagePath = dialog.FileName;
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //We reload products to update any saved data to the csv file
            Shared.Products = Methods.LoadProducts(Shared.ProductFilePath);

            //We store the selected product in our SelectedProduct variable so that other parts of the program know about it.
            Shared.SelectedProduct = (Product) ((ListBoxItem) Shared.ProductBox.SelectedItem).Tag;
            
            //We set the temp product and path to a copy of the selected product
            TempProduct = Shared.SelectedProduct;
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
