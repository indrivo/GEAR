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
            _service.CreateFolder(new DynamicReportFolder()
            {
                Name = folder.Name,
                Id = new Guid(),
                IsDeleted = false,
                ModifiedBy = User.Identity.Name,
                Author = User.Identity.Name,
                Changed = DateTime.Now,
                Created = DateTime.Now
            });
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
            if (name == "") return Json(new { success = false, message = "Name Empty!!!" });
            try
            {
                var folder = _service.GetFolder(id);
                folder.Name = name;
                _service.EditFolder(folder);
                return Json(new { success = true, message = "Saved!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!!!" });
            }
        }

        [HttpPost]
        public IActionResult DeleteReportFolder(Guid id)
        {
            if (id == Guid.Empty) return Json(new { success = false, message = "Id Empty!!!" });
            try
            {
                _service.DeleteFolder(id);
                return Json(new { success = true, message = "Deleted" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!!!" });
            }
        }

        #endregion

        #region DynamicReports

        [HttpGet]
        public IActionResult Save(Guid id)
        {
            var result = _service.GetReport(id);
            DynamicReportViewModel model = new DynamicReportViewModel();
            if (result != null)
            {
                model = new DynamicReportViewModel()
                {
                    Id = result.Id,
                    Name = result.Name,
                    ReportDataModel = result.ReportDataModel,
                    DynamicReportFolderId = result.DynamicReportFolderId
                };
            }
            ViewBag.Folders = _service.GetAllFolders();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(DynamicReportViewModel dto)
        {
            var reportDb = new DynamicReport()
            {
                Id = dto.Id,
                Name = dto.Name,
                ReportDataModel = dto.ReportDataModel,
                IsDeleted = false,
                Author = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                DynamicReportFolderId = dto.DynamicReportFolderId
            };
            _service.SaveReport(reportDb);
            return Json(new
            {
                success = true,
                message = "Data Gathered successfully"
            });
        }

 
        [HttpPost]
        public IActionResult DeleteReport(Guid id)
        {
            if (id == Guid.Empty) return Json(new { success = false, message = "Id Empty!!!" });
            try
            {
                _service.DeleteReport(id);
                return Json(new { success = true, message = "Deleted" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error!!!" });
            }
        }        


        [HttpPost]
        public IActionResult GetReportData(DynamicReportDataModel dto)
        {
            var data = _service.GetReportContent(dto);
            return Json(new { charts = dto.DynamicReportCharts, data });
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
                result = resultDict.Select(s => new SelectOption { id = (int)s.Key, text = s.Value }).ToList();
            }
            return Json(result);
        }

        public ActionResult GetFilterTypes()
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<FilterType>.ToDictionary();
            if (resultDict != null)
            {
                result = resultDict.Select(s => new SelectOption { id = (int)s.Key, text = s.Value }).ToList();
            }
            return Json(result);
        }


        public ActionResult GetChartTypes()
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<ChartType>.ToDictionary();
            if (resultDict != null)
            {
                result = resultDict.Select(s => new SelectOption { id = (int)s.Key, text = s.Value }).ToList();
            }
            return Json(result);
        }


        public ActionResult GetChartFieldTypes(ChartType chartType)
        {
            var result = new List<SelectOption>();
            var resultDict = Enum<ChartFieldType>.ToDictionary().ToList();
            if (resultDict != null)
            {
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
                result = resultDict.Select(s => new SelectOption { id = (int)s.Key, text = s.Value }).ToList();
            }
            return Json(result);
        }


        /// <summary>
        /// Get the list of table in the database/Context/Schema
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTablesAjax()
        {
            var result = _service.GetTableNames();
            return Json(result);
        }

        /// <summary>
        /// Get the Column type for changing the input field
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public ActionResult GetColumnTypeClient(string tableName, string columnName)
        {
            string result = _service.GetColumnType(tableName, columnName);
            return Json(new { success = true, message = result });
        }

        /// <summary>
        /// Get the select options for the foreign keys or enums on the client
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public ActionResult GetForeignColumnDataForSelection(string tableName, string columnName)
        {
            var response = _service.GetForeignKeySelectValues(tableName, columnName);
            var result = new List<ResponseClass>();

            if (response.GetType() == new List<DynamicReportQueryResultViewModel>().GetType())
            {
                foreach (var item in response)
                {
                    result.Add(new ResponseClass()
                    { Name = item.Columns[1].Value.ToString(), Id = item.Columns[0].Value.ToString() });
                }
            }
            else
            {
                foreach (var item in response)
                {
                    result.Add(new ResponseClass() { Name = item.ToString(), Id = item.ToString() });
                }
            }

            return Json(new { success = true, message = Json(result) });
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