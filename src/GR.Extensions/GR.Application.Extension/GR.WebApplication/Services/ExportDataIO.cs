using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GR.WebApplication.Services
{
    public static class ExportDataIo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static MemoryStream CreateZipArchive(IDictionary<string, MemoryStream> files)
        {
            var zipStream = new MemoryStream();
            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var o in files)
                {
                    try
                    {
                        var pageEntry = zip.CreateEntry(o.Key);

                        using (var entryStream = pageEntry.Open())
                        {
                            o.Value.CopyTo(entryStream);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            zipStream.Position = 0;
            return zipStream;
        }

        /// <summary>
        /// Decompress
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="action"></param>
        public static void Decompress(MemoryStream stream, Action<ZipArchive> action)
        {
            var zip = new ZipArchive(stream, ZipArchiveMode.Read, true);
            action.Invoke(zip);
        }

        /// <summary>
        /// GetConnectionString data from zip archive
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entries"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<TResult> GetDataFromZipArchiveEntry<TResult>(this IEnumerable<ZipArchiveEntry> entries, string file)
        {
            var entities = entries.FirstOrDefault(x => x.Name.Equals(file));
            if (entities == null) return default;

            var entStream = entities.Open();
            using (var strReader = new StreamReader(entStream))
            {
                var data = await strReader.ReadToEndAsync();
                if (string.IsNullOrEmpty(data)) return default;
                try
                {
                    return JsonConvert.DeserializeObject<TResult>(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return default;
        }
    }
}
