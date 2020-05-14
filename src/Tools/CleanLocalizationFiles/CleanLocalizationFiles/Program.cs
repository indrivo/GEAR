using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GR.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CleanLocalizationFiles
{
    class Program
    {
        private static string Path = "C:\\Users\\nicol\\OneDrive\\Документы\\savecrypto\\src\\GR.WebHosts\\GR.Cms\\Localization";
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(Path);
            foreach (var path in files)
            {
                var dict = File.ReadAllText(path).Deserialize<Dictionary<string, object>>();
                var newDict = dict.Where(x => !x.Key.StartsWith("iso_")).ToDictionary(x => x.Key, y => y.Value);
                var newData = JObject.Parse(newDict.SerializeAsJson());
                File.WriteAllText(path, newData.ToString(Formatting.Indented));
            }
            Console.WriteLine("Files are cleaned");
            Console.ReadKey();
        }
    }
}