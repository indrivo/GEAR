using System;
using System.Collections.Generic;
using System.IO;

namespace GR.Core.Helpers.MimeTypes
{
    /// <summary>
    /// Credit: https://github.com/Microsoft/referencesource/blob/master/System.Web/MimeMapping.cs
    /// </summary>
    public static class MimeMapping
    {
        private static readonly MimeMappingDictionaryBase MappingDictionary = new MimeMappingDictionaryClassic();

        public static string GetMimeMapping(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            return MappingDictionary.GetMimeMapping(fileName);
        }

        private abstract class MimeMappingDictionaryBase
        {
            private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            private static readonly char[] PathSeparatorChars = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar }; // from Path.GetFileName()

            private bool _isInitialized;

            protected void AddMapping(string fileExtension, string mimeType)
            {
                _mappings.Add(fileExtension, mimeType);
            }

            private void AddWildcardIfNotPresent()
            {
                if (!_mappings.ContainsKey(".*"))
                {
                    AddMapping(".*", "application/octet-stream");
                }
            }

            private void EnsureMapping()
            {
                // Ensure initialized only once
                if (!_isInitialized)
                {
                    lock (this)
                    {
                        if (!_isInitialized)
                        {
                            PopulateMappings();
                            AddWildcardIfNotPresent();
                            _isInitialized = true;
                        }
                    }
                }
            }

            // The derived class will override this method and call AddMapping for each mapping it knows about
            protected abstract void PopulateMappings();

            // This method is similar to Path.GetFileName(), but it doesn't fail on invalid path characters
            private static string GetFileName(string path)
            {
                int pathSeparatorIndex = path.LastIndexOfAny(PathSeparatorChars);
                return pathSeparatorIndex >= 0 ? path.Substring(pathSeparatorIndex) : path;
            }

            public string GetMimeMapping(string fileName)
            {
                EnsureMapping();
                fileName = GetFileName(fileName); // strip off path separators

                // some MIME types have complex extensions (like ".exe.config"), so we need to work left-to-right
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (fileName[i] == '.')
                    {
                        // potential extension - consult dictionary
                        string mimeType;
                        if (_mappings.TryGetValue(fileName.Substring(i), out mimeType))
                        {
                            // found!
                            return mimeType;
                        }
                    }
                }

                // If we reached this point, either we couldn't find an extension, or the extension we found
                // wasn't recognized. In either case, the ".*" mapping is guaranteed to exist as a fallback.
                return _mappings[".*"];
            }
        }

        // This can provide fallback mappings if we don't have an actual applicationHost.config from which to read
        private sealed class MimeMappingDictionaryClassic : MimeMappingDictionaryBase
        {
            protected override void PopulateMappings()
            {
                foreach (var mime in MimeTypes.Dictionary) AddMapping(mime.Key, mime.Value);
            }
        }
    }
}
