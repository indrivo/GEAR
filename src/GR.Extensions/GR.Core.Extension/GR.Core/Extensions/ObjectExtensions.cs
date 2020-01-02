using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize settings
        /// </summary>
        internal static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static string SerializeAsJson(this object obj, JsonSerializerSettings serializerSettings = null)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, serializerSettings ?? SerializeSettings);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="source"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static TOutput Deserialize<TOutput>(this string source, JsonSerializerSettings serializerSettings = null) where TOutput : class
        {
            if (source.IsNullOrEmpty()) return null;
            if (typeof(TOutput) == typeof(string)) return source as TOutput;
            if (typeof(TOutput) == typeof(int))
            {
                int.TryParse(source, out var numberValue);
                return numberValue as TOutput;
            }
            try
            {
                return JsonConvert.DeserializeObject<TOutput>(source, serializerSettings ?? SerializeSettings);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }


        /// <summary>
        /// Makes a copy from the object.
        /// Doesn't copy the reference memory, only data.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <param name="item">Object to be copied.</param>
        /// <returns>Returns the copied object.</returns>
        public static T Clone<T>(this object item)
        {
            if (item == null) return default(T);
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);

            var result = (T)formatter.Deserialize(stream);

            stream.Close();

            return result;
        }

        /// <summary>
        /// It returns deep copy of the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T input) where T : ISerializable
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, input);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// if the object this method is called on is not null, runs the given function and returns the value.
        /// if the object is null, returns default
        /// </summary>
        public static TResult IfNotNull<T, TResult>(this T target, Func<T, TResult> getValue)
         => target != null ? getValue(target) : default;


        /// <summary>
        /// Parse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(this string value)
        {
            // Get default value for type so if string
            // is empty then we can return default value.
            var result = default(T);
            if (string.IsNullOrEmpty(value)) return result;
            // we are not going to handle exception here
            // if you need SafeParse then you should create
            // another method specially for that.
            var tc = TypeDescriptor.GetConverter(typeof(T));
            result = (T)tc.ConvertFrom(value);
            return result;
        }

        /// <summary>
        /// Adapt extension of Mapster
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T MapType<T>(this object obj)
            => obj.Adapt<T>();

        /// <summary>
        /// Check object type and return cast object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Is<T>(this object obj)
        {
            switch (obj)
            {
                case null:
                    return default;
                case T mapped:
                    return mapped;
                default:
                    return default;
            }
        }
    }
}
