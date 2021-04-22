using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        ToggleButton currentlyPressedButton = null;

        private void DrawingButtonClick(object sender, RoutedEventArgs e)
        {
            if (currentlyPressedButton != null)
            {
                currentlyPressedButton.IsChecked = false;
            }
                
                
            if (sender as ToggleButton == currentlyPressedButton)
            {
                currentlyPressedButton = null;
                currentState = UIState.Nothing;
            }
            else
            {
                currentlyPressedButton = sender as ToggleButton;
                currentState = UIState.PreparingToDraw;
            }
                
        }
    }
}
