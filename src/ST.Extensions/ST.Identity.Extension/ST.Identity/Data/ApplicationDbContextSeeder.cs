using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ST.Identity.Abstractions.Models.AddressModels;

namespace ST.Identity.Data
{
    public static class ApplicationDbContextSeeder
    {
        /// <summary>
        /// Get countries
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Country> GetCountriesFromJsonFile()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "countries.json");
            using (var reader = new FileStream(path, FileMode.Open))
            {
                if (!reader.CanRead) throw new FileNotFoundException("countries.json not found for seed countries");
                using (var fileReader = new StreamReader(reader))
                {
                    var fileContent = fileReader.ReadToEnd();
                    if (string.IsNullOrEmpty(fileContent))
                    {
                        return new List<Country>();
                    }

                    var parse = JsonConvert.DeserializeObject<IEnumerable<Country>>(fileContent);
                    return parse;
                }
            }
        }
    }
}
