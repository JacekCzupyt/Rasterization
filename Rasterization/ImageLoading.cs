using System;
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
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        Bitmap mainBitmap;

        private void MainImageContainer_Loaded(object sender, RoutedEventArgs e)
        {
            mainBitmap = new Bitmap((int)MainImageContainer.ActualWidth, (int)MainImageContainer.ActualHeight);
            mainBitmap = DrawBitmap(mainBitmap);
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
            mainBitmap = DrawBitmap(mainBitmap);
            MainImageContainer.Fill = new ImageBrush(BitmapToImageSource(mainBitmap));
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Vector image|*.vec|JPeg Image|*.jpg|Png Image|*.png";
            dialog.Title = "Save the image";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                using (System.IO.FileStream fs =
                    (System.IO.FileStream)dialog.OpenFile())
                {
                    if (dialog.FilterIndex == 1)
                    {

                        BinaryFormatter formatter = new BinaryFormatter();

                        using (var mem = new MemoryStream())
                        {
                            formatter.Serialize(mem, DrawingObjects);
                            var byteArr = mem.ToArray();
                            fs.Write(byteArr, 0, byteArr.Length);
                        }
                    }
                    else
                    {
                        BitmapEncoder encoder;
                        switch (dialog.FilterIndex)
                        {
                            case 2:
                                encoder = new JpegBitmapEncoder();

                                break;
                            case 3:
                                encoder = new PngBitmapEncoder();
                                break;
                            default:
                                throw new Exception();
                        }
                        encoder.Frames.Add(BitmapFrame.Create(BitmapToImageSource(mainBitmap)));
                        encoder.Save(fs);
                    }
                }
            }
        }
    }
}
