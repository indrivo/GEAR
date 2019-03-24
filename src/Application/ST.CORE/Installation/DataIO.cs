using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ST.CORE.Installation
{
	public static class DataIo
	{
		public static MemoryStream Export(IDictionary<string, MemoryStream> files)
		{
			var zipStream = new MemoryStream();
			using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
			{
				foreach (var data in files)
				{
					try
					{
						ZipArchiveEntry pageEntry = zip.CreateEntry(data.Key);

						using (Stream entryStream = pageEntry.Open())
						{
							data.Value.CopyTo(entryStream);
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
