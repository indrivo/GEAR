using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GR.Core.Helpers
{
    public static class JsonParser
    {
        /// <summary>
		/// Read data from json array
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static T ReadArrayDataFromJsonFile<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                T data;

                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JArray.Load(reader);
                    data = fileObj.ToObject<T>();
                }

                return data;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return null;
        }


        /// <summary>
        /// Read data from json array
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ReadDataListFromJsonWithTypeParameter(string filePath, Type entity)
        {
            if (!File.Exists(filePath))
                return null;
            var list = typeof(List<>);
            var listOfType = list.MakeGenericType(entity);
            var result = Activator.CreateInstance(listOfType);

            try
            {

                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sReader = new StreamReader(str))
                {
                    var strArray = sReader.ReadToEnd();

                    result = JsonConvert.DeserializeObject(strArray, listOfType);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return (dynamic)result;
        }

        /// <summary>
        /// Read object data from json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ReadObjectDataFromJsonFile<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                T data;

                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JObject.Load(reader);
                    data = fileObj.ToObject<T>();
                }

                return data;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return null;
        }
    }
}
