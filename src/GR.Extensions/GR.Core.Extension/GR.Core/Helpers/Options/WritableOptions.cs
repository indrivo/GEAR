using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GR.Core.Abstractions;

namespace GR.Core.Helpers.Options
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IHostingEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly string _section;
        private readonly string _file;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="options"></param>
        /// <param name="section"></param>
        public WritableOptions(
            IHostingEnvironment environment,
            IOptionsMonitor<T> options,
            string section)
        {
            _environment = environment;
            _options = options;
            _section = section;
            _file = ResourceProvider.AppSettingsFilepath(environment);
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        /// <inheritdoc />
        /// <summary>
        /// Update options in json
        /// </summary>
        /// <param name="applyChanges"></param>
        /// <param name="filePath"></param>
        public void Update(Action<T> applyChanges, string filePath = null)
        {
            if (filePath != null) filePath = Path.Combine(_environment.ContentRootPath, filePath);

            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(filePath ?? _file);
            var physicalPath = fileInfo.PhysicalPath ?? fileInfo.Name;

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
            var sectionObject = jObject.TryGetValue(_section, out var section) ? JsonConvert.DeserializeObject<T>(section.ToString()) : Value ?? new T();
            applyChanges(sectionObject);
            if (!jObject.ContainsKey(_section))
            {
                jObject.Add(sectionObject.IsList()
                    ? new JProperty(_section, new JArray(JArray.Parse(JsonConvert.SerializeObject(sectionObject))))
                    : new JProperty(_section, new JObject(JObject.Parse(JsonConvert.SerializeObject(sectionObject)))));
            }
            else
            {
                if (sectionObject.IsList())
                    jObject[_section] = JArray.Parse(JsonConvert.SerializeObject(sectionObject));
                else
                    jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            }

            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }
    }
}
