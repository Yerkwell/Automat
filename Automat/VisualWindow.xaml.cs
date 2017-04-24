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
using System.Windows.Shapes;

namespace Automat
{
    /// <summary>
    /// Interaction logic for VisualWindow.xaml
    /// </summary>
    public partial class VisualWindow : Window
    {
        List<Shape> shapes;
        public VisualWindow()
        {
            InitializeComponent();
        }
        public VisualWindow(List<Shape> shapes)
        {
            InitializeComponent();
            this.shapes = shapes;
        }

        private void visualWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            shapes.ForEach(s => canvas1.Children.Add(s));
            shapes[0].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            visualWindow1.Width = shapes[0].DesiredSize.Width + 30;
            visualWindow1.Height = shapes[0].DesiredSize.Height + 90;
            canvas1.Width = visualWindow1.Width;
            canvas1.Height = visualWindow1.Height;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            //var margin = canvas1.Margin;
            //  canvas1.Margin = default(Thickness);
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "register";
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Images (.png)|*.png"; // Filter files by extension
            if (dlg.ShowDialog().Value)
            {
                Rect rect = new Rect(canvas1.DesiredSize);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
                  (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);

                rtb.Render(canvas1);
                //endcode as PNG
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                //save to memory stream
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(dlg.FileName, ms.ToArray());
                MessageBox.Show("Успешно сохранено");
            }
        }

        private void visualWindow1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
