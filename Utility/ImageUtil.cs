using ExifLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Galleria
{
    public class ImageUtil
    {
        public static Stream HandleOrientation(Stream stream, string fileName)
        {
            // figure out the orientation from EXIF data
            stream.Position = 0;
            JpegInfo info = ExifReader.ReadJpeg(stream, fileName);

            var angle = 0;

            switch (info.Orientation)
            {
                case ExifOrientation.TopLeft:
                case ExifOrientation.Undefined:
                    angle = 0;
                    break;
                case ExifOrientation.TopRight:
                    angle = 90;
                    break;
                case ExifOrientation.BottomRight:
                    angle = 180;
                    break;
                case ExifOrientation.BottomLeft:
                    angle = 270;
                    break;
            }

            if (angle > 0d)
            {
                stream = RotateStream(stream, angle);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static Stream Compress(Stream stream)
        {
            BitmapImage bi = new BitmapImage();
            bi.SetSource(stream);
            WriteableBitmap wb = new WriteableBitmap(bi);
            decimal aspectRatio = (bi.PixelWidth * 1.0M) / bi.PixelHeight;
            int width = bi.PixelWidth > 640 ? 640 : bi.PixelWidth;
            int height = (int)(width / aspectRatio);

            MemoryStream ms = new MemoryStream();
            wb.SaveJpeg(ms, width, height, 0, 70);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }
    }
}
