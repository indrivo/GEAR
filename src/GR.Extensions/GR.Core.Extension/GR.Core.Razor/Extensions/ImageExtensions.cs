using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace GR.Core.Razor.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Bytes to memory stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] bytes) => new MemoryStream(bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Image GetImageFromStream(this Stream stream)
        {
            Image response;
            using (var image = Image.FromStream(stream))
            {
                response = (Image)image.Clone();
            }

            return response;
        }

        /// <summary>
        /// Image to bytes
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image image, ImageFormat imageFormat = null)
        {
            if (imageFormat == null) imageFormat = ImageFormat.Gif;
            var ms = new MemoryStream();
            image.Save(ms, imageFormat);
            return ms.ToArray();
        }

        /// <summary>
        /// Resize image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
