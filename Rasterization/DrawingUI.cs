using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Rasterization.DrawingObjects;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;

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
                CancelDrawingObject();
                currentState = UIState.Nothing;
            }
            else
            {
                currentlyPressedButton = sender as ToggleButton;
                CancelDrawingObject();
                selectedPoints.Clear();
                UpdateMainImage();
                currentState = UIState.PreparingToDraw;
            }
                
        }

        bool Antialiesing = false;

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Antialiesing = true;
            UpdateMainImage();
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            Antialiesing = false;
            UpdateMainImage();
        }

        private void ColorPalette_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if(e.NewValue != null)
            {
                System.Windows.Media.Color c = e.NewValue.Value;
                CurrentColor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    
                if (currentState == UIState.SelectingPoints || currentState == UIState.MovingExistingPoints)
                {
                    var map = DrawingObjects.Select(x => (x, x.GetTranslationPoints().ToList()));
                    foreach (var p in selectedPoints.Keys)
                    {
                        foreach (var points in map)
                        {
                            if (points.Item2.Contains(p))
                            {
                                
                                points.Item1.color = CurrentColor;
                            }

                        }
                    }
                }
                UpdateMainImage();
            }
        }

        private void MainImageContainer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Delete)
            {
                switch (currentState)
                {
                    case UIState.SelectingPoints:
                    case UIState.MovingExistingPoints:
                        DrawingObjects.RemoveAll(shape => shape.GetTranslationPoints().Intersect(selectedPoints.Keys).Any());
                        selectedPoints.Clear();
                        UpdateMainImage();
                        break;
                }
            }
        }

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var obj in GetSelectedObjects())
            {
                if(obj is FilledPolygon)
                {
                    var poly = obj as FilledPolygon;
                    poly.Fill = !poly.Fill;
                }
            }
            UpdateMainImage();
        }

        private void TextureSelection_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string ImagePath = openFileDialog.FileName;
                try
                {
                    Bitmap texturebmp = (Bitmap)Image.FromFile(ImagePath);
                    Rectangle rect = new Rectangle(0, 0, texturebmp.Width, texturebmp.Height);
                    BitmapData bmpData = texturebmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, texturebmp.PixelFormat);
                    TextureWrapper texture = new TextureWrapper(bmpData);
                    foreach (var obj in GetSelectedObjects())
                    {
                        obj.useTexture = true;
                        obj.texture = texture;
                    }
                    UpdateMainImage();
                }
                catch (System.NotSupportedException)
                {
                    throw new System.NotSupportedException();
                }
            }
        }
    }
}
