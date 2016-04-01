using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace ColorBlind
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var capture = CreateCapture())
            {

                using (var nextFrame = capture.QueryFrame())
                {
                    if (nextFrame != null)
                    {
                        Image.Source = ToBitmapSource(nextFrame.Bitmap);
                    }
                }
            }
        }

        private Capture CreateCapture()
        {
            var result = new Emgu.CV.Capture(0);

            GetProps(result);


         
            //result.SetCaptureProperty(CapProp.Brightness, brightness.Value);
            //result.SetCaptureProperty(CapProp.Contrast, contrast.Value);
            //result.SetCaptureProperty(CapProp.Staturation, sat.Value);
            //result.SetCaptureProperty(CapProp.Gain, gain.Value);
            //result.SetCaptureProperty(CapProp.Fps, 10);
            //result.SetCaptureProperty(CapProp.Format, 1);

            result.SetCaptureProperty(CapProp.AutoExposure, brightness.Value);

            result.SetCaptureProperty(CapProp.Hue, 10);

            var props = GetProps(result);

            result.Start();

            return result;
        }

        private static Dictionary<string, double> GetProps(Capture result)
        {
            var capProps = Enum.GetValues(typeof(CapProp)).Cast<CapProp>().Distinct();

            return capProps.ToDictionary(capProp => capProp.ToString(), result.GetCaptureProperty);
        }


        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            var hBitmap = source.GetHbitmap();



            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                return null;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
    }
}
