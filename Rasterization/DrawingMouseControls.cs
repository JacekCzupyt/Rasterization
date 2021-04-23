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
        Vector2 PreviousMousePosition;

        private void MainImageContainer_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentState)
            {
                case UIState.DrawingNewObject:
                    currentlyDrawnPoint.Point = e.GetPosition(MainImageContainer).ToVector2();
                    UpdateMainImage();
                    break;
                case UIState.MovingExistingPoints:
                    var newMousePos = e.GetPosition(MainImageContainer).ToVector2();
                    foreach (var p in selectedPoints.Keys)
                    {
                        p.Point += newMousePos - PreviousMousePosition;
                    }
                    PreviousMousePosition = newMousePos;
                    break;
            }
        }
        
        private void MainImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (currentState)
            {
                case UIState.MovingExistingPoints:
                case UIState.SelectingPoints:
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

        private void MainImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (currentState)
            {
                case UIState.MovingExistingPoints:
                    currentState = UIState.SelectingPoints;
                    break;
            }
        }


        private void MainImageContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (currentState)
            {
                case UIState.MovingExistingPoints:
                case UIState.SelectingPoints:
                    selectedPoints.Clear();
                    UpdateMainImage();
                    currentState = UIState.Nothing;
                    break;
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

            IEnumerable<(IDrawingObject, DrawingPoint)> ClosestPoints = DrawingObjects.Select(obj => (obj, obj.GetClosestPoint(pos)));

            if (ClosestPoints.Any())
            {
                (IDrawingObject, DrawingPoint) ClosestPoint = ClosestPoints.Aggregate((min, x) => min.Item2.dist(pos) < x.Item2.dist(pos) ? min : x);

                //Ctrl
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    //Clicked point
                    if (ClosestPoint.Item2.dist(pos) <= MaximumSelectDistance)
                    {
                        //Clicked once
                        if (e.ClickCount == 1)
                        {
                            //Point already selected
                            if (selectedPoints.ContainsKey(ClosestPoint.Item2))
                            {
                                //Deselect point
                                selectedPoints.Remove(ClosestPoint.Item2);
                            }
                            //Point not selected
                            else
                            {
                                //Select point
                                selectedPoints.Add(ClosestPoint.Item2, new FilledCircle(ClosestPoint.Item2, PointRadius, System.Drawing.Color.Blue));
                            }
                        }
                        //Clicked twice
                        else
                        {
                            //Select shape
                            foreach(DrawingPoint p in ClosestPoint.Item1.GetTranslationPoints())
                                if(!selectedPoints.ContainsKey(p))
                                    selectedPoints.Add(p, new FilledCircle(p, PointRadius, System.Drawing.Color.Blue));
                        }
                    }
                }
                //no Ctrl
                else
                {
                    //Clicked point
                    if (ClosestPoint.Item2.dist(pos) <= MaximumSelectDistance)
                    {
                        //Clicked once
                        if (e.ClickCount == 1)
                        {
                            //Point not selected
                            if (!selectedPoints.ContainsKey(ClosestPoint.Item2))
                            {
                                //Select only this point
                                selectedPoints.Clear();
                                selectedPoints.Add(ClosestPoint.Item2, new FilledCircle(ClosestPoint.Item2, PointRadius, System.Drawing.Color.Blue));
                            }
                        }
                        //Clicked twice
                        else
                        {
                            //Select only this shape
                            selectedPoints.Clear();
                            foreach (DrawingPoint p in ClosestPoint.Item1.GetTranslationPoints())
                                if (!selectedPoints.ContainsKey(p))
                                    selectedPoints.Add(p, new FilledCircle(p, PointRadius, System.Drawing.Color.Blue));
                        }
                            
                    }
                }
            }
            PreviousMousePosition = e.GetPosition(MainImageContainer).ToVector2();
            currentState = selectedPoints.Count > 0 ? UIState.MovingExistingPoints : UIState.Nothing;
        }
    }
}
