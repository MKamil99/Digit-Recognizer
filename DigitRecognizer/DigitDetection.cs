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
        static List<int> ColumnSearch(Bitmap btm)
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
        static void IntervalsCounting(List<int> columnsWithBlackPoints, Bitmap btm)
        {
            if (columnsWithBlackPoints.Count == 0)
                return;

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
            ImageCropping(Start, Stop, btm);
        }

        // Dla obliczoncyh przedziałów wycinamy obrazy i wywołujemy funkcję skalującą wycięte obrazy:
        static void ImageCropping(List<int> Start, List<int> Stop, Bitmap btm)
        {
            int width;
            Bitmap bmpImage = new Bitmap(btm);

            for (int i = 0; i < Start.Count; i++)
            {
                width = Stop[i] - Start[i];
                if (width != 0)
                {
                    Bitmap bmpCrop = bmpImage.Clone(new Rectangle(Start[i], 0, width, btm.Height), bmpImage.PixelFormat);
                    ResizeImage(bmpCrop, i);
                }  
            }
        }
        
        // Funckja zmieniająca rozdzielczość na 28x28 i zapisująca do folderu /output/ wycięte obrazy:
        public static void ResizeImage(Image image, int i)
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
            Data.BitmapToTxtFile(resizedImage, $"data//cropp{i}.txt");
        }

        // Czyści folder output z poprzednich wycięć:
        static void ClearOutput()
        {
            DirectoryInfo di = new DirectoryInfo("data//");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        // Główna funckja wywołująca sekwencję:
        public static void DetectDigits(string path)
        {
            Bitmap btm = new Bitmap(path);
            ClearOutput();                              // Czyszczenie poprzednich wycięć
            IntervalsCounting(ColumnSearch(btm), btm);  // Analiza działania, wycięcie i zapis
        }
    }
}
