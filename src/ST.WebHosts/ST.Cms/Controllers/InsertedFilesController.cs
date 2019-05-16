using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST.Cms.ViewModels.FilesViewModels;
using ST.Core;
using ST.Entities.Data;
using ST.Identity.Abstractions;
using ST.Identity.Data;

namespace ST.Cms.Controllers
{
	public class InsertedFilesController : Controller
	{
		private readonly EntitiesDbContext _context;
		private readonly ApplicationDbContext _applicationDbContext;

		public InsertedFilesController(EntitiesDbContext context, ApplicationDbContext applicationDbContext)
		{
			_context = context;
			_applicationDbContext = applicationDbContext;
		}

		public IActionResult Index()
		{
			var list = _context.Documents.ToList();
			return View(list);
		}
		public IActionResult _InsertedFiles()
		{
			return PartialView();
		}

		public IActionResult Create()
		{
			var model = LoadViewRequirements();
			return View(model);
		}

		[NonAction]
		private DocumentCreateUpdateViewModel LoadViewRequirements(DocumentCreateUpdateViewModel model = null)
		{
			if (model == null) model = new DocumentCreateUpdateViewModel();
			model.AvailableUsers = _applicationDbContext.Users.Where(x => !x.IsDeleted).ToList();
			return model;
		}

		[HttpPost]
		public async Task<IActionResult> Create( DocumentCreateUpdateViewModel model)
		{
			model = LoadViewRequirements(model);
			if (model.FileBlob == null)
			{
				ModelState.AddModelError(string.Empty, "No attached file!");
				return View(model);
			}
			if (ModelState.IsValid)
			{

				string extension = Path.GetExtension(model.FileBlob?.FileName);
				Console.WriteLine(extension);

				// full path to file in temp location
				var filePath = Path.GetTempFileName();
				Console.WriteLine("Inserted Title" + model.Title);
				model.Id = Guid.NewGuid();
				model.Author = User.Identity.Name;
				model.Extension = extension;
				model.File = filePath;
				model.Status = "Draft";
				model.Version = 1;

				_context.Documents.Add(model);

				_context.SaveChanges();

				long size = model.FileBlob.Length;

				if (size > 0)
				{
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await model.FileBlob?.CopyToAsync(stream);
					}
				}
				return RedirectToAction("Index", "Files");
			}
			return View(model);
		}
		[HttpPost]

		public async Task<JsonResult> IsoFilesList(DTParameters param)
		{
			try
			{
				var filtered = GetIsoFilesFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
				out var totalCount);

				var filesList = filtered.Select(x => new ListIsoFileViewModel
				{
					Id = x.Id,
					Title = x.Title,
					Path = x.File,
					Created = x.Created.ToShortDateString(),
					Author = x.Author,
					CodDocument = x.CodDocument,
					Description = x.Description,
					TargetGroup = x.TargetGroup,
					Extension = x.Extension,
					Status = x.Status,
					File = x.File,
					ExternalLink = x.ExternalLink,
					Version = x.Version,
					Responsible = _applicationDbContext.Users.FirstOrDefault(y => y.Id == x.ResponsibleId.ToString())?.UserName
				});

				var finalResult = new DTResult<ListIsoFileViewModel>
				{
					Draw = param.Draw,
					Data = filesList.ToList(),
					RecordsFiltered = totalCount,
					RecordsTotal = filtered.Count
				};
				return Json(finalResult);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return Json("");
		}

		private List<Document> GetIsoFilesFiltered(string search, string sortOrder, int start, int length,
			  out int totalCount)
		{

			var result = _context.Documents.OrderByDescending(x => x.Created).ToList();
			totalCount = result.Count;

			result = result.Skip(start).Take(length).ToList();


			return result.ToList();
		}


		public FileResult Download()
		{

			string id = HttpContext.Request.Query["id"].ToString();
			var Gid = Guid.Parse(id);
			Document doc = _context.Documents.Where(x => x.Id == Gid).FirstOrDefault();

			byte[] fileBytes = System.IO.File.ReadAllBytes(@doc.File);
			string fileName = doc.Title + doc.Extension;
			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}


		// GET: Periods/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var doc = await _context.Documents.FindAsync(id);
			if (doc == null)
			{
				return NotFound();
			}

			var model = LoadViewRequirements(doc.Adapt<DocumentCreateUpdateViewModel>());

			return View(model);
		}

		// POST: Periods/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]

		public async Task<IActionResult> Edit(DocumentCreateUpdateViewModel doc)
		{
			var currentDoc = _context.Documents.FirstOrDefault(x => x.Id == doc.Id);
			if (currentDoc != null)
			{
				currentDoc.Title = doc.Title;
				currentDoc.CodDocument = doc.CodDocument;
				currentDoc.Status = doc.Status;
				currentDoc.Description = doc.Description;
				currentDoc.TargetGroup = doc.TargetGroup;
				currentDoc.Version = ++doc.Version;
				currentDoc.ExternalLink = doc.ExternalLink;
				currentDoc.ResponsibleId = doc.ResponsibleId;
			}
			//if (id != doc.Id)
			//{
			//	return NotFound();
			//}

			//if (ModelState.IsValid)
			//{
			//	try
			//	{
			//_context.Update(doc);
			await _context.SaveChangesAsync();
			//	}
			//	catch (Exception ex)
			//	{
			//		c
			//	}
			return RedirectToAction("Index", "Files");
			//}
			//return View(doc);
		}

	}
	public class InsertedFile : BaseModel
	{
		public string Name { get; set; }
		public string Path { get; set; }
	}
	public class ListIsoFileViewModel
	{
		public Guid Id { get; set; }
		public string Created { get; set; }
		public string Title { get; set; }
		public string Path { get; set; }

		public string Author { get; set; }
		public string CodDocument { get; set; }
		public string Description { get; set; }
		public string TargetGroup { get; set; }
		public string File { get; set; }
		public string Link { get; set; }
		public string Comment { get; set; }
		public string Extension { get; set; }
		public string Status { get; set; }
		public string Responsible { get; set; }
		public int Version { get; set; }
		public string ExternalLink { get; set; }
	}
}