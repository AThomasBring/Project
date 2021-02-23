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
            Title = "Butik";
            Width = 800;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.ShowGridLines = true;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            
            Label potionsLabel = "Potions Store";
            grid.Children.Add(potionsLabel);
            Grid.SetColumn(potionsLabel, 0);
            Grid.SetRow(potionsLabel, 0);
            Grid.SetColumnSpan(potionsLabel, 2);

            productPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightBlue,
                Margin = new Thickness(5)
            };
            grid.Children.Add(productPanel);
            Grid.SetColumn(productPanel, 0);
            Grid.SetRow(productPanel, 1);

        }
        public static Label CreateLabel(string header)
        {
            Label label = new Label
            {
                Content = header,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5)
            };
            
            
            
            return label;
        }
    }
}
