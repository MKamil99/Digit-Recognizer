using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using NeuralNetwork;
using System.Linq;
using System.Diagnostics;

namespace DigitRecognizer
{
    public partial class MainWindow : Window
    {
        Point CurrentPoint = new Point();
        Network network;
        int number = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Rysowanie
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
        #endregion

        #region Przyciski
        private void PrepareNetworkButtonClick(object sender, RoutedEventArgs e)
        {
            double[][][] datasets = Data.PrepareDatasets();
            // datasets[0] - dane wejściowe zbioru treningowego
            // datasets[1] - oczekiwane dane wyjściowe zbioru treningowego
            // datasets[2] - dane wejściowe zbioru walidacyjnego
            // datasets[3] - oczekiwane dane wyjściowe zbioru walidacyjnego

            network = new Network(datasets[0][0].Length, 2, 100, datasets[1][0].Length);
            string tmp = network.LoadWeights("weights.txt");
            if (tmp == "SIEĆ JEST GOTOWA. ")
            {
                MathTextBox.Text = tmp + network.CalculatePrecision(datasets);
                PrepareButton.IsEnabled = false;
                CalculateButton.IsEnabled = true;
                ClearButton.IsEnabled = true;
            }
            else MathTextBox.Text = tmp;
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            PaintSurface.Children.Clear();
            MathTextBox.Text = "WPROWADŹ NOWE DZIAŁANIE W STREFIE RYSOWANIA";
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            MathTextBox.Text = "Trwa analiza działania...";
            string path = $"paint{number++}.jpeg";
            SaveCanvas(PaintSurface, path); // Zapisuje canvas jako plik i potem z niego korzysta, usuwa go przy zamykaniu programu
            DigitDetection.DetectDigits(path);  // Wywołanie kolejnych funckji do wycinania i obróbki wczytanych cyfr, znaków

            List<double[]> digits = Data.RemoveSecondDimensions(Data.LoadData("data//"));
            string tmp = "";
            foreach (double[] digit in digits)
            {
                network.PushInputValues(digit);
                var output = network.GetOutput();
                //for (int i = 0; i < output.Count; i++)
                //    Debug.WriteLine(output[i] + " ");
                //Debug.WriteLine("");
                tmp += output.IndexOf(output.Max()) + " ";
            }
            MathTextBox.Text = tmp;
        }
        #endregion

        // Zapis Canvas do pliku:
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
    }
}