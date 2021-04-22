using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        enum UIState
        {
            Nothing,
            PreparingToDraw,
            DrawingNewObject,
            SelectingPoints,
            MovingExistingPoints
        }

        UIState currentState = UIState.Nothing;

        private void MainImageContainer_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentState)
            {
                case UIState.DrawingNewObject:
                    currentlyDrawnPoint.Point = e.GetPosition(MainImageContainer).ToVector2();
                    UpdateMainImage();
                    break;
            }
        }
        
        private void MainImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (currentState)
            {
                case UIState.Nothing:
                    throw new NotImplementedException();
                case UIState.PreparingToDraw:
                    currentState = UIState.DrawingNewObject;
                    BeginDrawingObject(e);
                    UpdateMainImage();
                    break;
                case UIState.DrawingNewObject:
                    ContinueDrawingObject(e);
                    UpdateMainImage();
                    break;
                default:
                    throw new NotImplementedException();
                    
            }
        }
    }
}
