using ST.Entities.ViewModels.DynamicEntities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace ST.Entities.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// Set default values on add new dynamic insert
        /// </summary>
        /// <param name="model"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SetDefaultValues(this Dictionary<string, object> model, EntityViewModel table)
        {
            if (!model.ContainsKey("Changed"))
                model.Add("Changed", DateTime.Now);

            if (!model.ContainsKey("Created"))
                model.Add("Created", DateTime.Now);

            //Set default value for author
            if (!model.ContainsKey("Author"))
                model.Add("Author", "user");
            else if (model["Author"] == null)
                model["Author"] = "user";

            //Set default value for modified by field
            if (!model.ContainsKey("ModifiedBy"))
                model.Add("ModifiedBy", "user");
            else if (model["ModifiedBy"] == null)
            {
                model["ModifiedBy"] = "user";
            }

            if (!model.ContainsKey("IsDeleted"))
                model.Add("IsDeleted", false);
            if (table == null) return model;

            foreach (var item in table.Fields)
            {
                if (model.ContainsKey(item.ColumnName)) continue;
                model.Add(item.ColumnName, "empty");
            }

            return model;
        }
        /// <summary>
        /// Generic parsing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static T DictionaryToObject<T>(this Dictionary<string, object> dictionary) where T : class
        {
            try
            {
                var ser = JsonConvert.SerializeObject(dictionary);
                return JsonConvert.DeserializeObject<T>(ser);
            }
            catch
            {
                return default;
            }
        }
        /// <summary>
        /// dictionary to object
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static dynamic DictionaryToObject(this Dictionary<string, object> dictionary)
        {
            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;

            foreach (var keyValuePair in dictionary)
            {
                expandoObjCollection.Add(keyValuePair);
            }
            dynamic eoDynamic = expandoObj;

            return eoDynamic;
        }
    }
}
