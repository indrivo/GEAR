using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Razor.Extensions
{
    public static class FormFileExtensions
    {
        /// <summary>
        /// Get image from form file
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static Image GetImageFromFormFile(this IFormFile formFile)
        {
            var stream = formFile.OpenReadStream();
            var result = stream.GetImageFromStream();
            stream.Close();
            return result;
        }

        /// <summary>
        /// Get memory stream from form file
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static MemoryStream GetMemoryStream(this IFormFile formFile)
        {
            var stream = new MemoryStream();
            formFile.CopyTo(stream);
            return stream;
        }
    }
}
