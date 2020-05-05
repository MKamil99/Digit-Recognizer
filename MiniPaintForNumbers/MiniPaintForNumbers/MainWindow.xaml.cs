using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MiniPaintForNumbers
{
    public partial class MainWindow : Window
    {
        Point CurrentPoint = new Point();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                CurrentPoint = e.GetPosition(this);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();
                line.StrokeThickness = 5;
                line.Stroke = SystemColors.WindowTextBrush;
                line.X1 = CurrentPoint.X;
                line.Y1 = CurrentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                CurrentPoint = e.GetPosition(this);
                PaintSurface.Children.Add(line);
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            PaintSurface.Children.Clear();
            MathTextBox.Text = "Wprowadź działanie w strefie rysowania";
        }

        private void CheckButtonClick(object sender, RoutedEventArgs e)
        {
            MathTextBox.Text = "Trwa analiza działania..";
            string path = @"paint.jpeg";
            SaveCanvas(PaintSurface, path); //Zapisuje canvas jako plik i potem z niego korzysta, usuwa go przy zamykaniu programu
            DigitDetection.DetectDigits(); //Wywołanie kolejnych funckji do wycinania i obróbki wczytanych cyfr, znaków
        }
        
        //Zapsis Canwas do pliku
        private void SaveCanvas(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        //Usuwanie pliku pomocniczego przy zamykaniu programu
        private void DeletePaintJPEG(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string path = @"paint.jpeg";
            File.Delete(path);
        }
    }
}
