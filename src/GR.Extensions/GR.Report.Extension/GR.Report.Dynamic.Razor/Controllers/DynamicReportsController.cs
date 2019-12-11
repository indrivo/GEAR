using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.Helpers;
using GR.Report.Abstractions;
using GR.Report.Abstractions.Extensions;
using GR.Report.Abstractions.Helpers;
using GR.Report.Abstractions.Models.Dto;
using GR.Report.Abstractions.Models.Enums;
using GR.Report.Abstractions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Report.Dynamic.Razor.Controllers
{
    public class DynamicReportsController : Controller
    {
        private readonly IDynamicReportsService _service;

        public DynamicReportsController(IDynamicReportsService service)
        {
            _service = service;
        }

        #region ReportFolders


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load forms with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadPageData(DTParameters param)
        {
            var result = _service.GetFilteredReports(param);
            return Json(result);
        }

        [HttpGet]
        public IActionResult CreateFolder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFolder(DynamicReportFolderViewModel folder)
        {
            if (!ModelState.IsValid) return View();

            var result = _service.CreateFolder(folder.Name);

            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Message);
                }
            }

            return View(folder);
        }

        [HttpGet]
        public IActionResult ManageDynamicReportFolders()
        {
            var model = _service.GetAllFolders();
            return View(model);
        }

        [HttpPost]
        public IActionResult EditFolder(DynamicReportFolderViewModel folderModel)
        {
            var result = _service.EditFolder(folderModel);

            if (result.IsSuccess)
            {
                return Json(new
                {
                    success = result.IsSuccess,
                    message = ResultMessagesEnum.SaveSuccess.GetEnumDescription()
                });
            }

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Errors.Any() ? result.Errors.First().Message : string.Empty
            });
        }


        /// <summary>
        /// Delete report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteReportFolder(Guid? id)
        {
            var result = await _service.DeleteFolderAsync(id);
            return Json(result);
        }

        #endregion

        #region DynamicReports

        [HttpGet]
        public IActionResult Save(Guid id)
        {
            DynamicReportViewModel result = null;
            if (id != Guid.Empty)
            {
                result = _service.GetReport(id).Result;
            }
            var model = result != null ? new DynamicReportViewModel()
            {
                Id = result.Id,
                Name = result.Name,
                ReportDataModel = result.ReportDataModel,
                DynamicReportFolder = new DynamicReportFolderViewModel(result.DynamicReportFolder.Id, result.DynamicReportFolder.Name)
            }
                : new DynamicReportViewModel();

            ViewBag.Folders = _service.GetAllFolders();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(DynamicReportViewModel model)
        {
            var result = model.Id != Guid.Empty ? _service.EditReport(model) : _service.CreateReport(model);

            if (result.IsSuccess)
            {
                return Json(new
                {
                    success = result.IsSuccess,
                    message = ResultMessagesEnum.SaveSuccess.GetEnumDescription()
                });
            }

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Errors.Any() ? result.Errors.First().Message : ""
            });
        }

        /// <summary>
        /// Delete report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteReport(Guid? id)
        {
            var result = await _service.DeleteReportAsync(id);
            result.Result = ResultMessagesEnum.DeleteSuccess.GetEnumDescription();
            return Json(result);
        }


        [HttpPost]
        public IActionResult GetReportData(DynamicReportDto model)
        {
            return Json(new { charts = model.DynamicReportCharts, data = _service.GetReportContent(model) });
        }

        [HttpPost]
        public IActionResult GetReportDataById(Guid id)
        {
            var result = _service.GetReport(id);

            if (result.IsSuccess)
            {
                var data = _service.GetReportContent(result.Result.ReportDataModel);
                return Json(new { charts = result.Result.ReportDataModel.DynamicReportCharts, data });
            }

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Errors.Any() ? result.Errors.First().Message : ""
            });
        }

        #endregion

        #region Database Methods

        public ActionResult GetAggregateTypes()
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<AggregateType>.ToDictionary();
            if (resultDict != null)
            {
                result = resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();
            }

            return Json(result);
        }

        public ActionResult GetFilterTypes()
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<FilterType>.ToDictionary();
            if (resultDict != null)
            {
                result = resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();
            }

            return Json(result);
        }


        public ActionResult GetChartTypes()
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<ChartType>.ToDictionary();
            if (resultDict != null) result = resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();
            return Json(result);
        }


        public ActionResult GetChartFieldTypes(ChartType chartType)
        {
            var result = _service.GetChartFieldTypes(chartType);
            return Json(result);
        }


        /// <summary>
        /// Get the list of table in the database/Context/Schema
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTablesAjax()
        {
            var tables = _service.GetTableNames();
            var schemas = _service.GetUserSchemas();
            if (schemas != null && tables != null)
            {
                var result = schemas.Select(s => new
                {
                    id = s,
                    text = s,
                    children = tables.Where(x => x.id == s).Select(x => new { id = x.id + "." + x.text, x.text }).ToList()
                });
                return Json(result);
            }

            return Json(new { success = false, message = ResultMessagesEnum.EmptyResult.GetEnumDescription() });
        }

        /// <summary>
        /// Get the table columns for a specific table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ActionResult GetTableColumnsAjax(string tableName)
        {
            var result = _service.GetTableColumns(tableName);
            return Json(result);
        }

        #endregion
    }

}