using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using NeuralNetwork;

namespace DigitRecognizer
{
    class DigitDetection
    {
        // Przeszukuje kolumny w celu znalezienia punktów innych niż białe:
        private static List<int> ColumnSearch(Bitmap btm)
        {
            List<int> Cols = new List<int>();
            Color color;
            for (int j = 0; j < btm.Width; j += 5)
                for (int i = 0; i < btm.Height; i += 5)
                {
                    color = btm.GetPixel(j, i);
                    if (color != Color.FromArgb(255, 255, 255))
                    {
                        Cols.Add(j);
                        break;
                    }
                }

            return Cols;
        }

        // Oblicza przerwy miedzy kolumnami, które są tylko białe - pomiędzy nimi znajdują się cyfry/znaki, je będziemy wycinać:
        private static List<double[][]> IntervalsCounting(List<int> columnsWithBlackPoints, Bitmap btm)
        {
            if (columnsWithBlackPoints.Count == 0)
                return new List<double[][]>();

            List<int> Start = new List<int>();
            List<int> Stop = new List<int>();
            Start.Add(columnsWithBlackPoints[0]);
            for (int i = 1; i < columnsWithBlackPoints.Count - 1; i++)
                if (columnsWithBlackPoints[i + 1] - columnsWithBlackPoints[i] > 5)
                {
                    Start.Add(columnsWithBlackPoints[i + 1]);
                    Stop.Add(columnsWithBlackPoints[i]);
                }
            Stop.Add(columnsWithBlackPoints[columnsWithBlackPoints.Count - 1]);
            return ImageCropping(Start, Stop, btm);
        }

        // Dla obliczoncyh przedziałów wycinamy obrazy i wywołujemy funkcję skalującą wycięte obrazy:
        private static List<double[][]> ImageCropping(List<int> Start, List<int> Stop, Bitmap btm)
        {
            int width;
            Bitmap bmpImage = new Bitmap(btm);
            List<double[][]> digits = new List<double[][]>();

            for (int i = 0; i < Start.Count; i++)
            {
                width = Stop[i] - Start[i];
                if (width != 0)
                {
                    Bitmap bmpCrop = bmpImage.Clone(new Rectangle(Start[i], 0, width, btm.Height), bmpImage.PixelFormat);
                    digits.Add(ResizeImage(bmpCrop, i));
                } 
            }
            return digits;
        }
        
        // Funkcja zmieniająca rozdzielczość na 28x28 i zwracająca bitmapę w postaci tablicy dwuwymiarowej:
        private static double[][] ResizeImage(Image image, int i)
        {
            int width = 28, height = 28;
            Rectangle croppSize = new Rectangle(0, 0, width, height);
            Bitmap resizedImage = new Bitmap(width, height);
            resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, croppSize, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return Data.BitmapToArray(resizedImage);
        }

        // Główna funckja wywołująca sekwencję:
        public static List<double[][]> DetectDigits(MemoryStream picture)
        {
            Bitmap btm = new Bitmap(picture);
            return IntervalsCounting(ColumnSearch(btm), btm);  // Analiza działania, wycięcie i zapis
        }
    }
}
