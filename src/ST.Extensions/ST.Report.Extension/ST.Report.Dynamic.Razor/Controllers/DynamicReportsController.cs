using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Attributes;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Extensions;
using ST.Report.Abstractions.Models;
using ST.Report.Abstractions.Models.Enums;
using ST.Report.Dynamic.Razor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST.Report.Dynamic.Razor.Controllers
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
            _service.CreateFolder(folder.Name);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ManageDynamicReportFolders()
        {
            var model = _service.GetAllFolders();
            return View(model);
        }

        [HttpPost]
        public IActionResult EditFolder(Guid id, string name)
        {
            if (string.IsNullOrEmpty(name)) return Json(new { success = false, message = "Folder name cannot be empty!" });
            if (id == Guid.Empty) return Json(new { success = false, message = "This folder does not exist!" });
            try
            {
                var folder = _service.GetFolder(id);
                if (folder == null) return Json(new { success = false, message = "This folder does not exist!" });
                folder.Name = name;
                _service.EditFolder(folder);
                return Json(new { success = true, message = "Saved!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!" });
            }
        }

        [HttpPost]
        public IActionResult DeleteReportFolder(Guid id)
        {
            if (id == Guid.Empty) return Json(new { success = false, message = "This folder does not exist!" });
            try
            {
                _service.DeleteFolder(id);
                return Json(new { success = true, message = "Deleted" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!" });
            }
        }

        #endregion

        #region DynamicReports

        [HttpGet]
        public IActionResult Save(Guid id)
        {
            DynamicReport result = null;
            if (id != Guid.Empty)
            {
                result = _service.GetReport(id);
            }
            var model = result != null ?
            new DynamicReportViewModel()
            {
                Id = result.Id,
                Name = result.Name,
                ReportDataModel = result.ReportDataModel,
                DynamicReportFolderId = result.DynamicReportFolderId
            }
            : new DynamicReportViewModel();

            ViewBag.Folders = _service.GetAllFolders();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(DynamicReportViewModel dto)
        {
            if (dto.DynamicReportFolderId == Guid.Empty) return Json(new { success = false, message = "Report folder cannot be empty!" });
            var reportDb = new DynamicReport()
            {
                Id = dto.Id,
                Name = dto.Name,
                ReportDataModel = dto.ReportDataModel,
                DynamicReportFolderId = dto.DynamicReportFolderId
            };
            var result = _service.SaveReport(reportDb);
            if (result.IsSuccess)
            {
                return Json(new
                {
                    success = result.IsSuccess,
                    message = "Data saving successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Errors.Any() ? result.Errors.First().Message : ""
                });

            }
        }


        [HttpPost]
        public IActionResult DeleteReport(Guid id)
        {
            if (id == Guid.Empty) return Json(new { success = false, message = "This report does not exist!" });
            try
            {
                _service.DeleteReport(id);
                return Json(new { success = true, message = "Deleted" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!" });
            }
        }


        [HttpPost]
        public IActionResult GetReportData(DynamicReportDataModel dto)
        {
            return Json(new { charts = dto.DynamicReportCharts, data = _service.GetReportContent(dto) });
        }

        [HttpPost]
        public IActionResult GetReportDataById(Guid id)
        {
            var report = _service.GetReport(id);
            var data = _service.GetReportContent(report.ReportDataModel);
            return Json(new { charts = report.ReportDataModel.DynamicReportCharts, data });
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
            if (resultDict != null)
            {
                result = resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();
            }
            return Json(result);
        }


        public ActionResult GetChartFieldTypes(ChartType chartType)
        {
            var resultDict = Enum<ChartFieldType>.ToDictionary().ToList();
            switch (chartType)
            {
                case ChartType.Grid:
                    resultDict = resultDict.Where(s => s.Key == ChartFieldType.Normal).ToList();
                    break;
                case ChartType.PivotGrid:
                case ChartType.Line:
                    resultDict = resultDict.Where(s => new List<ChartFieldType> { ChartFieldType.Label, ChartFieldType.XAxis, ChartFieldType.YAxis }.Contains(s.Key)).ToList();
                    break;
                case ChartType.BarHorizontal:
                case ChartType.BarVertical:
                case ChartType.Pie:
                case ChartType.Doughnut:
                    resultDict = resultDict.Where(s => new List<ChartFieldType> { ChartFieldType.Label, ChartFieldType.XAxis }.Contains(s.Key)).ToList();
                    break;
            }

            var result = resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();

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
            return Json(new { success = false, message = "There is no data!" });
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