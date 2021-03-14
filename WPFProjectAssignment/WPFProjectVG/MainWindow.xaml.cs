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
        
        private bool hasImage;
        public static bool isNew = true;
        
        public string TempImagePath;
        public string TempImageFileName;
        public static DiscountCode SelectedDiscountCode;

        public Grid MainGrid = new Grid();
        public Grid ButtonGrid = new Grid();
        private Grid DiscountGrid = new Grid();
        private Grid leftSideGrid = new Grid();
        
        public Button EditButton = new Button();
        public Button DeleteProductButton = new Button();
        public Button DeleteDiscountButton = new Button();
        public Button SaveChangesButton = new Button();

        private TextBox EditProductCode = new TextBox();
        private TextBox EditDiscountCodeName = new TextBox();
        private TextBox EditDiscountPercent = new TextBox();
        private TextBox EditProductName = new TextBox();
        private TextBox EditProductDescription = new TextBox();
        private TextBox EditProductPrice = new TextBox();
        
        private Label DiscountCodeNameLabel = new Label();
        private Label DiscountPercentLabel = new Label();
        private Label ProductPriceLabel = new Label();
        private Label ProductCodeLabel = new Label();
        private Label ProductDescriptionLabel = new Label();
        private Label ProductNameLabel = new Label();
        private Label Message = new Label();
        
        private ListBox DiscountBox = new ListBox();
        StackPanel EditPanel = new StackPanel();

        
        public MainWindow()

        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            //Copy files and Load Product and Discounts
            Methods.CopyImagesToTempFolder(@"C:\Windows\Temp\PotionShopTempFiles\Images\");
            Methods.CopyToTempFolder("DiscountCodes.csv", Shared.DiscountCodesPath);
            Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountCodesPath);
            Shared.Products = Methods.LoadProducts(Shared.ProductsPath);

            // Window options
            Title = "Behind the magic curtain";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Our method for creating grids takes the lenght of an int array for number of rows/columns, and the value of integers for their respective height/width. (relative proportions, not pixels)
            MainGrid = Methods.CreateGrid(rows: new[] {1, 9}, new[] {1, 2});
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            leftSideGrid = Methods.CreateGrid(rows: new[] {1, 1}, columns: null);
            Methods.AddToGui(leftSideGrid, MainGrid, 1, 0);
            //This gird is for adding new discountcodes
            DiscountGrid = Methods.CreateGrid(rows: new[] { 3, 1 }, new[] { 1 });
            Methods.AddToGui(DiscountGrid, leftSideGrid, 1, 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            Shared.TextAndImageGrid = Methods.CreateGrid(rows: new[] {5, 1}, columns: new[] {1, 1});
            Methods.AddToGui(Shared.TextAndImageGrid, MainGrid, 1, 1);

            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Methods.CreateGrid(rows: new[] {5, 1}, columns: new[] {2, 2, 3, 1});
            Methods.AddToGui(ButtonGrid, MainGrid, 1, 1);

            //MainGrid.ShowGridLines = true;



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

            Shared.ProductBox = CreateProductBox();

            DiscountBox = CreateDiscountBox();

            Grid AddButtonsGrid = Methods.CreateGrid(null, new[] {1, 1});
            Methods.AddToGui(AddButtonsGrid, DiscountGrid, 1, 0);
            Button AddNewDicountCode = Methods.CreateButton("Add new discount code", SelectedDiscountCode);
            AddNewDicountCode.Width = 150;

            Methods.AddToGui(AddNewDicountCode, AddButtonsGrid, 0, 1);
            AddNewDicountCode.Click += AddDiscountButton_Click;

            Button AddNewProduct = Methods.CreateButton("Add new Product", Shared.SelectedProduct);
            AddNewProduct.Width = 150;
            Methods.AddToGui(AddNewProduct, AddButtonsGrid, 0, 0);
            AddNewProduct.Click += AddProductButton_Click;
        }

        private ListBox CreateDiscountBox()
        {
            var discountBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var discountCode in Shared.DiscountCodes)
            {
                discountBox.Items.Add(new ListBoxItem() {Content = discountCode.CodeName, Tag = discountCode});
            }

            discountBox.SelectedIndex = -1;
            Methods.AddToGui(discountBox, DiscountGrid, 0, 0);
            discountBox.SelectionChanged += DiscountBox_SelectionChanged;

            return discountBox;
        }

        private ListBox CreateProductBox()
        {
            var listBox = new ListBox
            {
                Margin = new Thickness(5)
            };

            foreach (var product in Shared.Products)
            {
                listBox.Items.Add(new ListBoxItem() {Content = product.Name, Tag = product});
            }
            
            Methods.AddToGui(listBox, leftSideGrid);
            listBox.SelectedIndex = -1;
            listBox.SelectionChanged += ProductBoxOnSelectionChanged;
            
            return listBox;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            Shared.TextAndImageGrid.Children.Remove(Shared.ImageDisplayed);
            var newProduct = new Product();
            hasImage = false;
            UpdateEditDisplay(newProduct);
        }

        private void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            Shared.TextAndImageGrid.Children.Clear();
            var newDiscount = new DiscountCode();
            
            UpdateEditDisplay(newDiscount);
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            Product product = (Product) s.Tag;

            UpdateEditDisplay(product, Shared.ImageFolderPath + Shared.SelectedProduct.Image);

            ButtonGrid.Children.Remove(EditButton);
            DeleteProductButton = Methods.CreateButton("Delete", Shared.SelectedProduct);

            Methods.AddToGui(DeleteProductButton, ButtonGrid, 1);
            DeleteProductButton.Click += OnDeleteProductButton_Click;
        }

        private void OnDeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            Product[] newProducts = new Product[Shared.Products.Length -1];
            foreach(Product p in Shared.Products)
            {
                if (p != Shared.SelectedProduct)
                {
                    newProducts[counter] = p;
                    counter++;
                }
            }
            Shared.Products = newProducts;

            String[] newProductArray = new string[newProducts.Length];

            for (int i = 0; i < newProducts.Length; i++)
            {
                newProductArray[i] = newProducts[i].Code + @"\";
                newProductArray[i] += newProducts[i].Name + @"\";
                newProductArray[i] += newProducts[i].Description + @"\";
                newProductArray[i] += newProducts[i].Price + @"\";
                newProductArray[i] += newProducts[i].Image;
            }

            File.WriteAllLines(Shared.ProductsPath, newProductArray);
            
            //Update GUI
            Shared.Products = Methods.LoadProducts(Shared.ProductsPath);
            Shared.TextAndImageGrid.Children.Clear();
            MessageBox.Show("Product Deleted!");
            Shared.ProductBox = CreateProductBox();
        }

        private void UpdateEditDisplay(Product product, string imagePath = null)
        {
            Shared.TextAndImageGrid.Children.Clear();
            ButtonGrid.Children.Clear();
            if (imagePath != null)
            {
                Methods.UpdateProductImage(imagePath);
                
            }
            
            ProductCodeLabel = new Label
            {
                Content = "Product Code",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular,
            };

            EditProductCode.Text = product.Code;
            EditProductCode.Tag = ProductCodeLabel;
            EditProductCode.SelectionChanged += OnEditBoxSelectionChange;

            ProductNameLabel = new Label
            {
                Content = "Product Name",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };

            EditProductName.Text = product.Name;
            EditProductName.Tag = ProductNameLabel;
            EditProductName.SelectionChanged += OnEditBoxSelectionChange;

            ProductDescriptionLabel = new Label
            {
                Content = "Product Description",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };

            EditProductDescription.Text = product.Description;
            EditProductDescription.Tag = ProductDescriptionLabel;
            EditProductDescription.SelectionChanged += OnEditBoxSelectionChange;
            


            ProductPriceLabel = new Label
            {
                Content = "Product Price",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };
            
            EditProductPrice.Text = product.Price.ToString();
            EditProductPrice.Tag = ProductPriceLabel;
            EditProductPrice.SelectionChanged += OnEditBoxSelectionChange;

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
            Methods.AddToGui(ProductCodeLabel, EditPanel);
            Methods.AddToGui(EditProductCode, EditPanel);
            Methods.AddToGui(ProductDescriptionLabel, EditPanel);
            Methods.AddToGui(EditProductDescription, EditPanel);
            Methods.AddToGui(ProductNameLabel, EditPanel);
            Methods.AddToGui(EditProductName, EditPanel);
            Methods.AddToGui(ProductPriceLabel, EditPanel);
            Methods.AddToGui(EditProductPrice, EditPanel);
            Methods.AddToGui(imageLabel, EditPanel);
            Methods.AddToGui(uploadImageButton, EditPanel);
            Methods.AddToGui(saveChangesButton, ButtonGrid, 1, 1);
            Methods.AddToGui(Message, EditPanel);
        }

        private void OnEditBoxSelectionChange(object sender, RoutedEventArgs e)
        {
            var editBox = (TextBox) sender;
            var label = (Label) editBox.Tag;

            //We check for invalid character every boxselection change
            if (!CheckInvalidCharacter(editBox, label, '\\'))
            {
                label.Foreground = Brushes.Black;
                label.FontWeight = FontWeights.Regular;
                Message.Content = "";
            }


        }

        private void UpdateEditDisplay(DiscountCode discountCode)
        {
            Shared.TextAndImageGrid.Children.Clear();
            ButtonGrid.Children.Clear();
            Methods.UpdateProductImage(Shared.WelcomeImagePath);
            
            DiscountCodeNameLabel = new Label
            {
                Content = "Discount Code",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular,
            };

            EditDiscountCodeName.Text = discountCode.CodeName;
            EditDiscountCodeName.Tag = DiscountCodeNameLabel;
            EditDiscountCodeName.SelectionChanged += OnEditBoxSelectionChange;

            DiscountPercentLabel = new Label
            {
                Content = "Discount Percent",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Regular
            };

            EditDiscountPercent.Text = discountCode.Percentage.ToString();
            EditDiscountPercent.Tag = DiscountPercentLabel;
            EditDiscountPercent.SelectionChanged += OnEditBoxSelectionChange;
            

            SaveChangesButton = Methods.CreateButton("Save Changes", SelectedDiscountCode);
            SaveChangesButton.Click += OnSaveChangesClick;

            EditPanel.Children.Clear();
            Methods.AddToGui(EditPanel, Shared.TextAndImageGrid, 0, 0);
            Methods.AddToGui(DiscountCodeNameLabel, EditPanel);
            Methods.AddToGui(EditDiscountCodeName, EditPanel);
            Methods.AddToGui(DiscountPercentLabel, EditPanel);
            Methods.AddToGui(EditDiscountPercent, EditPanel);
            Methods.AddToGui(SaveChangesButton, ButtonGrid, 1, 1);
            Methods.AddToGui(Message, EditPanel);
        }
        
        
        private void OnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            Message.Content = "";
            
            var s = (Button) sender;
            
            //We check the tag to see if we are saving a product or a discount code
            if (s.Tag == Shared.SelectedProduct)
            {

                if (string.IsNullOrEmpty(EditProductCode.Text))
                {
                    Message.Content += ProductCodeLabel.Content + " can´t be empty.\n";
                    ProductCodeLabel.Foreground = Brushes.Crimson;
                    ProductCodeLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                if (string.IsNullOrEmpty(EditProductName.Text))
                {
                    Message.Content += ProductNameLabel.Content + " can´t be empty.\n";;
                    ProductNameLabel.Foreground = Brushes.Crimson;
                    ProductNameLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                if (string.IsNullOrEmpty(EditProductDescription.Text))
                {
                    Message.Content += ProductDescriptionLabel.Content + " can´t be empty.\n";;
                    ProductDescriptionLabel.Foreground = Brushes.Crimson;
                    ProductDescriptionLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                if (string.IsNullOrEmpty(EditProductPrice.Text))
                {
                    Message.Content += ProductPriceLabel.Content + "can´t be empty.\n";
                    ProductPriceLabel.Foreground = Brushes.Crimson;
                    ProductPriceLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                
                //First we make sure that the price can be stored as a decimal
                EditProductPrice.Text = EditProductPrice.Text.Replace(',', '.');
                EditProductPrice.Text = EditProductPrice.Text.Trim();
                decimal price;
                if (!decimal.TryParse(EditProductPrice.Text, out price))
                {
                    ProductPriceLabel.Foreground = Brushes.Crimson;
                    ProductPriceLabel.FontWeight = FontWeights.Bold;
                    Message.Content = "The price is not a valid number. \n";
                
                    return;
                }


                //We make sure the textboxes have no '\' characters, because we use them as separators in the text files.
                if (CheckInvalidCharacter(EditProductCode, ProductCodeLabel, '\\'))
                {
                    return;
                }
                if (CheckInvalidCharacter(EditProductName, ProductNameLabel, '\\'))
                {
                    return;
                }
                if (CheckInvalidCharacter(EditProductDescription, ProductDescriptionLabel, '\\'))
                {
                    return;
                }

                if (hasImage == false && isNew)
                {
                    Message.Content = "You need to upload a picture in order to save.";
                    return;
                }

                var tempProduct = new Product();
                tempProduct.Code = EditProductCode.Text;
                tempProduct.Name = EditProductName.Text;
                tempProduct.Description = EditProductDescription.Text;
                tempProduct.Price = price;
                tempProduct.Image = TempImageFileName;
            
                //Check if product code duplicates and update Temp Products
                if (CheckProductCodeDuplicates(tempProduct))
                {
                    Message.Content = "Every product code needs to be unique";
                    return;
                }
                
                SaveProductToFile(tempProduct, Shared.Products, Shared.SelectedProduct, Shared.ProductsPath);
                //Copy Image to image folder if changed.
                if (TempImagePath != Shared.ImageFolderPath + Shared.SelectedProduct.Image)
                {
                    File.Copy(TempImagePath, Shared.ImageFolderPath+tempProduct.Image, true);
                }
                
                //Update GUI
                Shared.Products = Methods.LoadProducts(Shared.ProductsPath);
                Shared.TextAndImageGrid.Children.Clear();
                MessageBox.Show("Product Saved!");
                Shared.ProductBox = CreateProductBox();


            }
            
            if (s.Tag.Equals(SelectedDiscountCode))
            {
                
                //DiscountCode discountTag;
                //discountTag = (DiscountCode) s.Tag;
                if (string.IsNullOrEmpty(EditDiscountCodeName.Text))
                {
                    Message.Content += DiscountCodeNameLabel.Content + "can´t be empty.\n";
                    DiscountCodeNameLabel.Foreground = Brushes.Crimson;
                    DiscountCodeNameLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                
                if (string.IsNullOrEmpty(EditDiscountPercent.Text))
                {
                    Message.Content += DiscountPercentLabel.Content + "can´t be empty.\n";
                    DiscountPercentLabel.Foreground = Brushes.Crimson;
                    DiscountPercentLabel.FontWeight = FontWeights.Bold;
                    
                    return;
                }
                
                int percent;
                if (!int.TryParse(EditDiscountPercent.Text, out percent))
                {
                    DiscountPercentLabel.Foreground = Brushes.Crimson;
                    DiscountPercentLabel.FontWeight = FontWeights.Bold;
                    Message.Content = "Enter a valid percentage";
                
                    return;
                }
                
                //We make sure the texboxes have no '\' characters, because we use them as separators in the text files.
                if (CheckInvalidCharacter(EditDiscountCodeName, DiscountCodeNameLabel, '\\'))
                {
                    return;
                }
                if (CheckInvalidCharacter(EditDiscountPercent, DiscountPercentLabel, '\\'))
                {
                    return;
                }



                var tempDiscountCode= new DiscountCode();
                tempDiscountCode.CodeName = EditDiscountCodeName.Text;
                tempDiscountCode.Percentage = percent;

                //Check if product code duplicates and update Temp Products
                if (CheckDiscountCodeDuplicates(tempDiscountCode))
                {
                    Message.Content = "Every product code needs to be unique";
                    return;
                }
                
                //SaveToFile();
                SaveDiscountToFile(tempDiscountCode, Shared.DiscountCodesPath, isNew);
            }

        }


        private void SaveDiscountToFile(DiscountCode tempDiscountCode, string path, bool isProductNew)
        {
            List<DiscountCode> discounts = new List<DiscountCode>();
            
            if(isProductNew == false)
            {
                foreach (var d in Shared.DiscountCodes)
                {
                    if (d.CodeName != SelectedDiscountCode.CodeName)
                    {
                        discounts.Add(d);
                    }
                    else
                    {
                        discounts.Add(tempDiscountCode);
                    }
                }
            }
            else
            {
                discounts = Shared.DiscountCodes.ToList();
                discounts.Add(tempDiscountCode);
            }

            String[] newDiscountArray = new string[discounts.Count];

            for (int i = 0; i < discounts.Count; i++)
            {
                newDiscountArray[i] = discounts[i].CodeName + @"\";
                newDiscountArray[i] += discounts[i].Percentage;
            }
            
            File.WriteAllLines(path, newDiscountArray);
            
            Shared.TextAndImageGrid.Children.Clear();
            MessageBox.Show("Discount Code Saved!");
            Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountCodesPath);
            DiscountBox = CreateDiscountBox();
            

        }

        private static bool CheckProductCodeDuplicates(Product tempProduct)
        {
            //We make a temporary list to run some tests on
            List<Product> productList = new List<Product>();
            //We make a temporary list to run some tests on

            if (!isNew)
            {
                foreach (var product in Shared.Products)
                {
                    if (product.Code != Shared.SelectedProduct.Code)
                    {
                        //all un-edited codes gets added first
                        productList.Add(product);
                        
                    }
                    else
                    {
                        //If the selected product is a match, we add the temp product since this is the one the user have edited.
                        productList.Add(tempProduct);
                    }
                }
            }
            else
            {
                productList = Shared.Products.ToList();
                productList.Add(tempProduct);
            }
            
            
            // now we check if there are duplicates
            if (productList.Count() != productList.Select(p => p.Code).Distinct().Count())
            {
                return true;
            }
            
            //all codes are unique
            return false;

        }
        
        private static bool CheckDiscountCodeDuplicates(DiscountCode tempDiscountCode)
        {
            //We make a temporary list to run some tests on
            List<DiscountCode> discountList = new List<DiscountCode>();
            
            foreach (var code in Shared.DiscountCodes)
            {
                if (code.CodeName != SelectedDiscountCode.CodeName)
                {
                    //all un-edited codes gets added first
                    discountList.Add(code);
                }
                else
                {
                    //If the selected product is a match, we add the temp product since this is the one the user have edited.
                    discountList.Add(tempDiscountCode);
                }
            }
            
            // if there are duplicates
            if (discountList.Count() != discountList.Select(p => p.CodeName).Distinct().Count())
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
            Message.Content = label.Content + " can not include any " +'"' +c +'"'+ "characters.";
            return true;
        }

        private void SaveProductToFile(Product newProduct, Product[] products, Product selectedProduct, string path)
        {
            List<Product> tempProducts = new List<Product>();

            if (!isNew)
            {
                foreach (var p in products)
                {
                    if (p.Code != selectedProduct.Code)
                    {
                        tempProducts.Add(p);
                    }
                    else
                    {
                        tempProducts.Add(newProduct);
                    }
                }
            }
            else
            {
                tempProducts = products.ToList();
                tempProducts.Add(newProduct);
            }


            String[] newProductArray = new string[tempProducts.Count];

            for (int i = 0; i < tempProducts.Count; i++)
            {
                newProductArray[i] = tempProducts[i].Code + @"\";
                newProductArray[i] += tempProducts[i].Name + @"\";
                newProductArray[i] += tempProducts[i].Description + @"\";
                newProductArray[i] += tempProducts[i].Price + @"\";
                newProductArray[i] += tempProducts[i].Image;
            }
            
            File.WriteAllLines(path, newProductArray);

        }

        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            
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
            hasImage = true;
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonGrid.Children.Remove(SaveChangesButton);
            isNew = false;

            //We store the selected product in our SelectedProduct variable so that other parts of the program know about it.
            Shared.SelectedProduct = (Product) ((ListBoxItem) Shared.ProductBox.SelectedItem).Tag;
            
            //We copy the selected product to temporary placeholders until customer save.
            TempImagePath = Shared.ImageFolderPath + Shared.SelectedProduct.Image;
            TempImageFileName = Shared.SelectedProduct.Image;

            //Then, we update texts and image
            Shared.TextAndImageGrid.Children.Clear();
            Methods.UpdateDescriptionText(Shared.SelectedProduct);
            Methods.UpdateProductImage(Shared.ImageFolderPath + Shared.SelectedProduct.Image);
            EditButton = Methods.CreateButton("Edit Product", Shared.SelectedProduct);
            
            Methods.AddToGui(EditButton, ButtonGrid, 1);
            EditButton.Click += OnEditButtonClick;

        }
        
        private void DiscountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //A discount is chosen from the listbox, so it´s not a new code.
            isNew = false;

            //We store the selected code in our SelectedDiscount variable so that other parts of the program know about it.
            SelectedDiscountCode = (DiscountCode) ((ListBoxItem) DiscountBox.SelectedItem).Tag;

            //Then, we update texts and image
            Shared.TextAndImageGrid.Children.Clear();
            Methods.UpdateDescriptionText(SelectedDiscountCode);
            UpdateEditDisplay(SelectedDiscountCode);
            Methods.UpdateProductImage(Shared.WelcomeImagePath);

            //Create a Button for Removing Discount codes
            ButtonGrid.Children.Remove(DeleteProductButton);
            DeleteDiscountButton = Methods.CreateButton("Delete", SelectedDiscountCode);

            Methods.AddToGui(DeleteDiscountButton, ButtonGrid, 1);
            DeleteDiscountButton.Click += OnDeleteDiscountButton_Click;
        }

        private void OnDeleteDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            DiscountCode[] newDiscounts = new DiscountCode[Shared.DiscountCodes.Length - 1];
            foreach (DiscountCode p in Shared.DiscountCodes)
            {
                if (p != SelectedDiscountCode)
                {
                    newDiscounts[counter] = p;
                    counter++;
                }
            }
            Shared.DiscountCodes = newDiscounts;

            String[] newDiscountArray = new string[newDiscounts.Length];

            for (int i = 0; i < newDiscounts.Length; i++)
            {
                newDiscountArray[i] = newDiscounts[i].CodeName + @"\";
                newDiscountArray[i] += newDiscounts[i].Percentage;
                
            }

            File.WriteAllLines(Shared.DiscountCodesPath, newDiscountArray);
            //Update GUI
            Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountCodesPath);
            Shared.TextAndImageGrid.Children.Clear();
            MessageBox.Show("Discount Deleted!");
            DiscountBox = CreateDiscountBox();
        }
    }
}
