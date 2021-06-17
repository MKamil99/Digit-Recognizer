using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using NeuralNetwork;

namespace DigitRecognizer
{
    public partial class MainWindow : Window
    {
        Point CurrentPoint = new Point();
        Network network;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Drawing
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                CurrentPoint = e.GetPosition(this);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    StrokeThickness = 4,
                    Stroke = SystemColors.WindowTextBrush,
                    X1 = CurrentPoint.X,
                    Y1 = CurrentPoint.Y,
                    X2 = e.GetPosition(this).X,
                    Y2 = e.GetPosition(this).Y
                };

                CurrentPoint = e.GetPosition(this);
                PaintSurface.Children.Add(line);
            }
        }
        #endregion

        #region Buttons
        private void LaunchNetworkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                network = Network.LoadNetworkFromFile("weights.txt");
                LaunchButton.IsEnabled = false;
                CalculateButton.IsEnabled = true;
                ClearButton.IsEnabled = true;
                MathTextBox.Text = "NETWORK IS READY!";
            }
            catch { MathTextBox.Text = "THE SYSTEM HASN'T FOUND CORRECT FILE WITH WEIGHTS!"; }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            PaintSurface.Children.Clear();
            MathTextBox.Text = "ENTER ANOTHER ARITHMETICAL EXPRESSION";
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            var picture = SaveCanvas(PaintSurface);                                     // Saving canvas in memory
            var DigitsInTwoDimensions = DigitDetection.DetectDigits(picture);           // Cutting out (and postprocessing) drawn digits and signs

            //for (int i = 0; i < DigitsInTwoDimensions.Count; i++) // for every digit/sign
            //{
            //    for (int j = 0; j < DigitsInTwoDimensions[i].Length; j++) // row
            //    {
            //        for (int k = 0; k < DigitsInTwoDimensions[i][j].Length; k++) // column
            //            Debug.Write(DigitsInTwoDimensions[i][j][k] + " ");
            //        Debug.WriteLine("");
            //    }
            //    Debug.WriteLine("");
            //}

            List<double[]> digits = Data.RemoveSecondDimensions(DigitsInTwoDimensions); // Preparation before teaching the network

            string tmp = DigitDetection.RecognizeDigits(digits, network);
            if (tmp != "") MathTextBox.Text = tmp;
            string result = Calculation.Calculate(tmp).ToString();
            if (result == "INCORRECT EXPRESSION!" || result == "YOU CAN'T DIVIDE BY ZERO!") MathTextBox.Text = result;
            else MathTextBox.Text += result;
        }
        #endregion

        // Zapis Canvas:
        private MemoryStream SaveCanvas(Canvas canvas)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);

            return stream;
        }
    }
}