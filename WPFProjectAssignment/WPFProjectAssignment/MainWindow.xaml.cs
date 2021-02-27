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

namespace WPFProjectAssignment
{
    public partial class MainWindow : Window
    {
        
        private StackPanel productPanel;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Potion Shop";
            Width = 1000;
            Height = 618;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid firstgrid = new Grid();
            firstgrid.ShowGridLines = true;
            root.Content = firstgrid;
            firstgrid.Margin = new Thickness(5);
            
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            
            // Second Grid
            Grid secondGrid = new Grid();
            secondGrid.ShowGridLines = true;
            secondGrid.Margin = new Thickness(5);

            // Left side for item list and shopping cart, right side for item description
            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            
            // Third Grid
            Grid thirdGrid = new Grid();
            thirdGrid.ShowGridLines = true;
            thirdGrid.Margin = new Thickness(5);
            // Top row for list of available products, Bottom row for shopping cart
            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            
            // Fourth Grid
            Grid fourthGrid = new Grid();
            fourthGrid.ShowGridLines = true;
            fourthGrid.Margin = new Thickness(5);
            // Left column for items in shoppingcart, right for checkout
            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Fifth Grid
            Grid fifthGrid = new Grid();
            fifthGrid.ShowGridLines = true;
            fifthGrid.Margin = new Thickness(5);
            // Left column for items in shoppingcart, right for checkout
            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            
            // add second grid to into second row of first grid
            firstgrid.Children.Add(secondGrid);
            Grid.SetColumn(secondGrid, 0);
            Grid.SetRow(secondGrid, 1);
            
            // add third grid to into first column of second grid
            secondGrid.Children.Add(thirdGrid);
            Grid.SetColumn(thirdGrid, 0);
            Grid.SetRow(thirdGrid, 1);
            
            //add fourth grid into third
            thirdGrid.Children.Add(fourthGrid);
            Grid.SetColumn(fourthGrid, 0);
            Grid.SetRow(fourthGrid, 1);
            
            //add fourth grid into third
            fourthGrid.Children.Add(fifthGrid);
            Grid.SetColumn(fifthGrid, 1);
            Grid.SetRow(fifthGrid, 0);
            
            
        }
    }
}
