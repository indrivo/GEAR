using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST.Configuration;
using ST.CORE.Installation;
using ST.Entities.Data;

namespace ST.CORE.Controllers
{
	[Authorize(Roles = Settings.SuperAdmin)]
	public class ExportController : Controller
	{
		private readonly EntitiesDbContext _entitiesDbContext;

		public ExportController(EntitiesDbContext entitiesDbContext)
		{
			_entitiesDbContext = entitiesDbContext;
		}

		public IActionResult Export()
		{
			//Export entities
			var entities = _entitiesDbContext.Table
				.Include(x => x.TableFields);

			var entitiesSerialized = JsonConvert.SerializeObject(entities, Formatting.Indented, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

			//Export forms
			var forms = _entitiesDbContext.Forms.Include(x => x.Table)
				.Include(x => x.Columns)
				.Include(x => x.Rows)
				.Include(x => x.Fields)
				.Include(x => x.Stages)
				.Include(x => x.Settings).ToList();

			var formsSerialized = JsonConvert.SerializeObject(forms, Formatting.Indented, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

			//Export pages
			var pages = _entitiesDbContext.Pages
				.Include(x => x.PageScripts)
				.Include(x => x.PageStyles)
				.Include(x => x.PageType)
				.Include(x => x.Settings).ToArray();
			var serializedPages = JsonConvert.SerializeObject(pages, Formatting.Indented, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

			var zipStream = DataIo.Export(new Dictionary<string, MemoryStream>
			{
				{
					"forms.json", new MemoryStream(Encoding.ASCII.GetBytes(formsSerialized))
				},
				{
					"pages.json", new MemoryStream(Encoding.ASCII.GetBytes(serializedPages))
				},
				{
					"entities.json", new MemoryStream(Encoding.ASCII.GetBytes(entitiesSerialized))
				}
			});
			var date = DateTime.Now;
			return File(zipStream, "application/octet-stream", $"export_system_{date.Minute}_{date.Hour}_{date.Day}_{date.Month}_{date.Year}.zip");
		}
	}
}