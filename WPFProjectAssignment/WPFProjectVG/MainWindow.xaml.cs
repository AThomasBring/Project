﻿using System;
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
        private bool hasImage = false;
        private bool isNew = false;

        Grid MainGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        public Button EditButton = new Button();
        StackPanel EditPanel = new StackPanel();

        private Grid DiscountGrid = new Grid();
        private Grid leftSideGrid = new Grid();
        
        public string TempImagePath;
        public string TempImageFileName;

        public static DiscountCode SelectedDiscountCode;

        private TextBox EditProductCode = new TextBox();
        private TextBox EditDiscountCodeName = new TextBox();
        private TextBox EditDiscountPercent = new TextBox();
        private Label DiscountCodeNameLabel = new Label();
        private Label DiscountPercentLabel = new Label();

        private TextBox EditProductName = new TextBox();

        private TextBox EditProductDescription = new TextBox();

        private TextBox EditProductPrice = new TextBox();

        private Label ProductPriceLabel = new Label();
        private Label ProductCodeLabel = new Label();
        private Label ProductDescriptionLabel = new Label();
        private Label ProductNameLabel = new Label();

        private List<TextBox> TextBoxes = new List<TextBox>();
        private List<Label> Labels = new List<Label>();
        
        private Label Message = new Label();
        private ListBox DiscountBox = new ListBox();

        private Label DiscountPercentage = new Label();
        private TextBox InsertNewPercentage = new TextBox();
        private Label NewDiscount = new Label();
        private TextBox InsertNewDiscount = new TextBox();


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
            

            Button saveChangesButton = Methods.CreateButton("Save Changes", SelectedDiscountCode);
            saveChangesButton.Click += OnSaveChangesClick;

            EditPanel.Children.Clear();
            Methods.AddToGui(EditPanel, Shared.TextAndImageGrid, 0, 0);
            Methods.AddToGui(DiscountCodeNameLabel, EditPanel);
            Methods.AddToGui(EditDiscountCodeName, EditPanel);
            Methods.AddToGui(DiscountPercentLabel, EditPanel);
            Methods.AddToGui(EditDiscountPercent, EditPanel);
            Methods.AddToGui(saveChangesButton, ButtonGrid, 1, 1);
            Methods.AddToGui(Message, EditPanel);
        }
        
        
        private void OnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            var s = (Button) sender;
            
            if (s.Tag.Equals(Shared.SelectedProduct))
            {
                
                Product productTag;
                productTag = (Product) s.Tag;
                
                //First we make sure that the price can be stored as a decimal
                EditProductPrice.Text = EditProductPrice.Text.Replace(',', '.');
                EditProductPrice.Text = EditProductPrice.Text.Trim();
                decimal price;
                if (!decimal.TryParse(EditProductPrice.Text, out price))
                {
                    ProductPriceLabel.Foreground = Brushes.Crimson;
                    ProductPriceLabel.FontWeight = FontWeights.Bold;
                    Message.Content = "The price is not a valid number.";
                
                    return;
                }
            
                //We make sure the texboxes have no '\' characters, because we use them as separators in the text files.
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

                if (hasImage == false)
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
                
                SaveProductToFile(tempProduct, Shared.ProductFilePath);
                

            }
            
            if (s.Tag.Equals(SelectedDiscountCode))
            {
                
                //DiscountCode discountTag;
                //discountTag = (DiscountCode) s.Tag;
                
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
                SaveDiscountToFile(tempDiscountCode, Shared.DiscountFilePath, isNew);
            }

        }

        private void SaveDiscountToFile(DiscountCode tempDiscountCode, string path, bool isNew)
        {
            List<DiscountCode> discounts = new List<DiscountCode>();
            
            if(isNew == false)
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
            Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountFilePath);
            DiscountBox = CreateDiscountBox();
            

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

        private void SaveProductToFile(Product tempProduct, string path)
        {
            List<Product> products = new List<Product>();

            if (!isNew)
            {
                foreach (var p in Shared.Products)
                {
                    if (p.Code != Shared.SelectedProduct.Code)
                    {
                        products.Add(p);
                    }
                    else
                    {
                        products.Add(tempProduct);
                    }
                }
            }
            else
            {
                products = Shared.Products.ToList();
                products.Add(tempProduct);
            }


            String[] newProductArray = new string[products.Count];

            for (int i = 0; i < products.Count; i++)
            {
                newProductArray[i] = products[i].Code + @"\";
                newProductArray[i] += products[i].Name + @"\";
                newProductArray[i] += products[i].Description + @"\";
                newProductArray[i] += products[i].Price + @"\";
                newProductArray[i] += products[i].Image;
            }
            
            File.WriteAllLines(path, newProductArray);
            
            Shared.Products = Methods.LoadProducts(Shared.ProductFilePath);
            
            Shared.TextAndImageGrid.Children.Clear();
            MessageBox.Show("Product Saved!");

            Shared.ProductBox = CreateProductBox();


            //File.WriteAllLines(path);
            if (TempImagePath != Shared.ImageFolderPath + Shared.SelectedProduct.Image)
            {
                //Copy Image if changed.
                File.Copy(TempImagePath, Shared.ImageFolderPath + tempProduct.Image, true);
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
            hasImage = true;
        }

        private void ProductBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isNew = false;
            //We reload products to update any saved data to the csv file
            //Shared.Products = Methods.LoadProducts(Shared.ProductFilePath);
            

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
            isNew = false;
            //We reload codes to update any saved data to the csv file
            //Shared.DiscountCodes = Methods.LoadCodes(Shared.DiscountFilePath);

            var s = (ListBox) sender;
            var tag = (DiscountCode) s.Tag;
            

            //We store the selected code in our SelectedProduct variable so that other parts of the program know about it.
            SelectedDiscountCode = (DiscountCode) ((ListBoxItem) DiscountBox.SelectedItem).Tag;
            

            //Then, we update texts and image
            Shared.TextAndImageGrid.Children.Clear();
            Methods.UpdateDescriptionText(SelectedDiscountCode);
            UpdateEditDisplay(SelectedDiscountCode);
            Methods.UpdateProductImage(Shared.WelcomeImagePath);
        }

        
    }
}
