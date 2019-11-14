using System;
using System.Collections.Generic;
using System.Dynamic;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.DynamicEntityStorage.Extensions
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
                model["Author"] = "system";

            //Set default value for modified by field
            if (!model.ContainsKey("ModifiedBy"))
                model.Add("ModifiedBy", "system");
            else if (model["ModifiedBy"] == null)
            {
                model["ModifiedBy"] = "system";
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
