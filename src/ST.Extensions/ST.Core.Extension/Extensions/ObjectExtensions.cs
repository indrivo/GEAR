using Newtonsoft.Json;

namespace ST.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
