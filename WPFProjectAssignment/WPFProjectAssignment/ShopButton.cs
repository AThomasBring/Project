using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFProjectAssignment
{
    public class ShopButton : Button
    {
        //Types of buttons:
        public enum Types 
        {
            Plus,
            Minus,
            AddToCart,
            Checkout,
            SaveCart,
            Clear
        }
       
        public Product Item;
        public Types Type;
        public ShoppingCart Cart;


        public ShopButton(Product item, Types type, ShoppingCart cart)
        {
     
            Background = Brushes.White;
            Margin = new Thickness(5);
     
            Item = item;
            Type = type;
            Cart = cart;
            Content = Type switch
            {
                Types.Plus => "+",
                Types.Minus => "-",
                Types.AddToCart => "Add to cart",
                Types.Checkout => "Check out",
                Types.SaveCart => "Save Cart",
                Types.Clear => "Clear Cart",
                _ => Content
            };
            //Trigger event on click
            Click += MainWindow.ButtonOnClick;
        }
    }
}