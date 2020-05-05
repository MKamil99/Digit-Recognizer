using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace MiniPaintForNumbers
{
    class DigitDetection
    {
        //Przeszukuje kolumny w celu znalezienia punktów innych niż białe
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

        //Oblicza przerwy miedzy kolumnami które są tylko białe - pomiędzy nimi znajdują się cyfry/znaki, je będziemy wycinać
        static void IntervalsCounting(List<int> List, Bitmap btm)
        {
            if (List.Count == 0)
                return;

            List<int> Start = new List<int>();
            List<int> Stop = new List<int>();
            Start.Add(List[0]);
            for (int i = 1; i < List.Count - 1; i++)
                if (List[i + 1] - List[i] > 5)
                {
                    Start.Add(List[i + 1]);
                    Stop.Add(List[i]);
                }
            Stop.Add(List[List.Count - 1]);
            ImageCropping(Start, Stop, btm);
        }

        //Dla obliczoncyh przedziałów wycinamy obrazy i wywołujemu funkcje skalującą wycięte obrazy
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
        
        //Funckja zmieniająca rozdzielczość na 28x28 i zapisująca do folderu /output/ wycięte obrazy
        public static void ResizeImage(Image image, int i)
        {
            int width = 28;
            int height = 28;
            var croppSize = new Rectangle(0, 0, width, height);
            var resizedImage = new Bitmap(width, height);
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
            resizedImage.Save($"output//cropp{i}.bmp");
        }

        //Czyści folder output z poprzednich wycięć
        static void ClearOutput()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo("output//");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        //Główna funckja wywołująca sekwencję
        public static void DetectDigits()
        {
            Bitmap btm = new Bitmap(@"paint.jpeg");
            ClearOutput(); //Czyszczenie poprzednich wycięć
            IntervalsCounting(ColumnSearch(btm), btm); //Analiza działania, wycięcie i zapis
        }
    }
}
