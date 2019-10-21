using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GR.Localization.Razor.Api
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class LocalizationApiController : Controller
    {
        /// <summary>
        /// Get jquery data table translations
        /// </summary>
        /// <param name="language"></param>
        /// <param name="customReplace"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetJQueryTableTranslations(string language, string customReplace = null)
        {
            if (string.IsNullOrEmpty(language))
            {
                return new JsonResult(new object());
            }
            var link = $"http://cdn.datatables.net/plug-ins/1.10.19/i18n/{language}.json";
            using (var ctx = new WebClient())
            {
                try
                {
                    var jsonStr = ctx.DownloadString(link);
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
                    if (string.IsNullOrEmpty(customReplace))
                        return new JsonResult(json);
                    try
                    {
                        var data = JsonConvert.DeserializeObject<IList<KeyValuePair<string, object>>>(customReplace);
                        if (!data.Any())
                            return new JsonResult(json);
                        foreach (var (key, value) in data)
                        {
                            if (json.ContainsKey(key))
                            {
                                json[key] = value;
                            }
                        }
                        return new JsonResult(json);
                    }
                    catch
                    {
                        return new JsonResult(json);
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return new JsonResult(new object());
                }
            }
        }
    }
}
