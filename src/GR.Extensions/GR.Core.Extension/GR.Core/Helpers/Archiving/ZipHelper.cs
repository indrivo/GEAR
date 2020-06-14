using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;

namespace GR.Core.Helpers.Archiving
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Helper for create zip archives")]
    public static class ZipHelper
    {
        /// <summary>
        /// Create zip archive
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
    }
}
