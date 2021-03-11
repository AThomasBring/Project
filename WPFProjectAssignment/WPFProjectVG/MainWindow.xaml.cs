using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
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
using Shared;
using WPFProjectAssignment;

namespace WPFProjectVG
{
    public partial class MainWindow : Window
    {
        Grid MainGrid = new Grid();
        private Grid TextAndImageGrid = new Grid();
        private Grid ButtonGrid = new Grid();
        private Grid DiscountGrid = new Grid();
        public MainWindow()
        
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            Methods.CopyImagesToTempFolder(@"C:\Windows\Temp\PotionShopTempFiles\Images\");
            Shared.Shared.DiscountCodes = Methods.LoadCodes(Shared.Shared.DiscountFilePath);
            Shared.Shared.Products = Methods.LoadProducts(Shared.Shared.ProductFilePath);
            
            // Window options
            Title = "Potion Shop";
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
            TextAndImageGrid = Methods.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 });
            Methods.AddToGui(TextAndImageGrid, MainGrid, 1, 1);
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Methods.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
            Methods.AddToGui(ButtonGrid, MainGrid, 1, 1);

            //This grid is to divide the space where we show the discount code so that we can display both a label and a text block.
            DiscountGrid = Methods.CreateGrid(rows: null, columns: new []{1, 1});
            Methods.AddToGui(DiscountGrid, ButtonGrid, 1, 2);

            MainGrid.ShowGridLines = true;
            
            
            
            
            
        }
    }
}
