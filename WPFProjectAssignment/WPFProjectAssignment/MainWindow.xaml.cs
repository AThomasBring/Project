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

            //Todo skapa metod för skapandet av grids
            // Main grid (For dividing header with rest of layout
            Grid firstgrid = new Grid();
            firstgrid.ShowGridLines = true;
            root.Content = firstgrid;
            firstgrid.Margin = new Thickness(5);
            
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            firstgrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            
            // Second Grid, Left side for item list and shopping cart, right side for item description
            Grid secondGrid = new Grid();
            secondGrid.ShowGridLines = true;
            secondGrid.Margin = new Thickness(5);

            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            secondGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            
            // Third Grid, Top row for list of available products, Bottom row for shopping cart
            Grid thirdGrid = new Grid();
            thirdGrid.ShowGridLines = true;
            thirdGrid.Margin = new Thickness(5);

            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            thirdGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            
            // Fourth Grid, Left column for items in shopping cart, right for checkout
            Grid fourthGrid = new Grid();
            fourthGrid.ShowGridLines = true;
            fourthGrid.Margin = new Thickness(5);

            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            fourthGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Fifth Grid, Left column for items in shopping cart, right for checkout
            Grid fifthGrid = new Grid();
            fifthGrid.ShowGridLines = true;
            fifthGrid.Margin = new Thickness(5);

            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            fifthGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            
            //Adding grids to the grids
            
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
            
            secondGrid.Children.Add(CreateLayoutPanel());
            Grid.SetColumn(CreateLayoutPanel(), 1);
            Grid.SetRow(CreateLayoutPanel(), 0);
            
        }
        //Har bara kopierat denna från Kitchensink programmet
        private Grid CreateLayoutPanel()
        {
            // The main layout is a grid with two columns and four rows.
            // All rows are sized to their content ("auto") except the second row, which takes up all the remaining space.
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // A text heading.
            TextBlock heading = new TextBlock
            {
                Text = "Products",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(heading);
            Grid.SetColumn(heading, 0);
            Grid.SetRow(heading, 0);
            Grid.SetColumnSpan(heading, 2);

            // The list box allowing us to select a product.
            // Fills the left half of the second row.
            //Add code here to read products in from CSV file.
            ListBox productBox = new ListBox { Margin = new Thickness(5) };
            productBox.Items.Add("Product1");
            productBox.Items.Add("Product2");
            productBox.Items.Add("Product3");
            productBox.SelectedIndex = 0;
            grid.Children.Add(productBox);
            Grid.SetColumn(productBox, 0);
            Grid.SetRow(productBox, 1);

            // The information panel describing a specific potion.
            // Fills the right half of the second row.
            StackPanel infoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            grid.Children.Add(infoPanel);
            Grid.SetColumn(infoPanel, 1);
            Grid.SetRow(infoPanel, 1);

            // The text heading inside the information panel.
            TextBlock infoHeading = new TextBlock
            {
                Text = "Potion1",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };
            infoPanel.Children.Add(infoHeading);

            // The image inside the information panel.
            Image infoImage = CreateImage("Potion1");
            infoImage.Stretch = Stretch.Uniform;
            infoPanel.Children.Add(infoImage);

            // The descriptive text inside the information panel.
            TextBlock infoText = new TextBlock
            {
                //Add code to read CSV file of descriptions
                Text = "Här ska vi läsa in texten om varje product från CSV fil",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 12
            };
            infoPanel.Children.Add(infoText);

            // A button that stretches across both columns.
            Button addToCartButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 16
            };
            grid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 0);
            Grid.SetRow(addToCartButton, 2);
            Grid.SetColumnSpan(addToCartButton, 2);


            return grid;
        }

        private Image CreateImage(string filePath)
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
