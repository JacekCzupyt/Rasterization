using Rasterization.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                    HandlePointSelection(e);
                    UpdateMainImage();
                    break;
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


        private void MainImageContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (currentState)
            {
                case UIState.PreparingToDraw:
                    currentlyPressedButton.IsChecked = false;
                    currentlyPressedButton = null;
                    currentState = UIState.Nothing;
                    break;
                case UIState.DrawingNewObject:
                    CancelDrawingObject();
                    currentState = UIState.PreparingToDraw;
                    break;
            }
        }

        private void HandlePointSelection(MouseButtonEventArgs e)
        {
            const float MaximumSelectDistance = 10;
            const float PointRadius = 3;

            Vector2 pos = e.GetPosition(MainImageContainer).ToVector2();

            IEnumerable<DrawingPoint> ClosestPoints = DrawingObjects.Select(obj => obj.GetClosestPoint(pos));

            if (ClosestPoints.Any())
            {
                DrawingPoint ClosestPoint = ClosestPoints.Aggregate((min, x) => min.dist(pos) < x.dist(pos) ? min : x);

                if (ClosestPoint.dist(pos) <= MaximumSelectDistance)
                {
                    if (selectedPoints.ContainsKey(ClosestPoint))
                    {
                        selectedPoints.Remove(ClosestPoint);
                    }
                    else
                    {
                        selectedPoints.Add(ClosestPoint, new FilledCircle(ClosestPoint, PointRadius, System.Drawing.Color.Blue));
                    }
                }
            }
        }
    }
}
