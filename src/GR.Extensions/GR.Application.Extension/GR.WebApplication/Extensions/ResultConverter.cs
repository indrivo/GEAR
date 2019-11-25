using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace GR.WebApplication.Extensions
{
	public class ResultConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(LogLevel);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			var lev = (string)jo["Name"];
			var result = LogLevel.FromString(lev);
			return result;
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
