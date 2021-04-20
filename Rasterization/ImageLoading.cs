﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        Bitmap mainBitmap;

        private void MainImageContainer_Loaded(object sender, RoutedEventArgs e)
        {
            mainBitmap = new Bitmap((int)MainImageContainer.ActualWidth, (int)MainImageContainer.ActualHeight);
            MainImageContainer.Fill = new ImageBrush(BitmapToImageSource(mainBitmap));
        }

        //from https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void MainImageContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainBitmap = new Bitmap((int)MainImageContainer.ActualWidth, (int)MainImageContainer.ActualHeight);
            MainImageContainer.Fill = new ImageBrush(BitmapToImageSource(mainBitmap));
        }

        private void MainImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainBitmap.SetPixel((int)e.GetPosition(sender as IInputElement).X, (int)e.GetPosition(sender as IInputElement).Y, System.Drawing.Color.Red);
            MainImageContainer.Fill = new ImageBrush(BitmapToImageSource(mainBitmap));
        }
    }
}
