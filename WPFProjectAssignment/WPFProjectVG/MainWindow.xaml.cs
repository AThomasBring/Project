using System;
using System.Collections.Generic;
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

namespace WPFProjectVG
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Potion Shop";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            //Our method for creating grids takes the lenght of an int array for number of rows/columns, and the value of integers for their respective height/width. (relative proportions, not pixels)
            MainGrid = Shared.CreateGrid(rows: new[] { 1, 9 }, new[] { 1, 2 });
            Content = MainGrid;

            // This grid is for dividing the left side of the main window to display available products and shopping cart
            var leftSideGrid = Shared.CreateGrid(rows: new[] { 1, 1 }, columns: null);
            Shared.AddToGui(leftSideGrid, MainGrid, 1 , 0);

            // This grid is for item description and image, and gets cleared and updated every product selection change
            TextAndImageGrid = Shared.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 1, 1 });
            Shared.AddToGui(TextAndImageGrid, MainGrid, 1, 1);
            
            // This grid is where we put the buttons for check out, save/clear cart as well as discount code.
            ButtonGrid = Shared.CreateGrid(rows: new[] { 5, 1 }, columns: new[] { 2, 2, 3, 1 });
            Shared.AddToGui(ButtonGrid, MainGrid, 1, 1);

            //This grid is to divide the space where we show the discount code so that we can display both a label and a text block.
            DiscountGrid = Shared.CreateGrid(rows: null, columns: new []{1, 1});
            Shared.AddToGui(DiscountGrid, ButtonGrid, 1, 2);
        }
    }
}
