using System;
using System.Collections.Generic;
using System.IO;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions.Helpers
{
    public static class DynamicJsonReader
    {
        /// <summary>
        /// Get data from path
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> GetEntityDataFromJsonFile(Type entity, string filePath)
        {
            return JsonParser.ReadDataListFromJsonWithTypeParameter(Path.Combine(AppContext.BaseDirectory, filePath), entity);
        }
    }
}
