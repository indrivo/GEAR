using System;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        /// Copy to server
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<ResultModel> UploadAsync(this IFormFile file, string filePath)
        {
            var result = new ResultModel();
            try
            {
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.AddError(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Get parent directory path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public static string GetParentDirectory(this string path, string parentName)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Parent == null)
            {
                return null;
            }

            return dir.Parent.Name == parentName
                ? dir.Parent.FullName
                : GetParentDirectory(dir.Parent.FullName, parentName);
        }
    }
}