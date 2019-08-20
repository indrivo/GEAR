using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ST.Core;
using ST.Core.Attributes;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Models;
using ST.Report.Abstractions.Models.Enums;
using ST.Report.Dynamic.Razor.ViewModels;
using ST.Report.Abstractions.Extensions;
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
        public JsonResult LoadPageData(DTParameters param) => Json(_service.GetFilteredReports(param));

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
            return RedirectToAction("CreateDynamic");
        }

        [HttpGet]
        [Route("manage-report-folders")]
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
        public IActionResult CreateDynamic()
        {
            ViewBag.Folders = _service.GetAllFolders();
            return View();
        }

        [HttpPost]
        public IActionResult CreateDynamic(DynamicReportViewModel dto)
        {
            var reportDb = new DynamicReport()
            {
                Id = new Guid(),
                Name = dto.Name,
                ReportDataModel = dto.ReportDataModel,
                IsDeleted = false,
                Author = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                DynamicReportFolderId = dto.DynamicReportFolderId
            };
            _service.CreateReport(reportDb);
            return Json(new
            {
                success = true,
                message = "Data Gathered successfully"
            });
        }

        //[HttpGet]
        //public IActionResult RunDynamic(Guid id)
        //{
        //    @ViewBag.ReportId = id;
        //    @ViewBag.StartDate = _service.ParseReport(id).StartDateTime;
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult RunDynamicById(Guid id)
        //{
        //    var report = _service.ParseReport(id);
        //    var model = new DynamicReportCreateRunViewModel()
        //    {
        //        ChartDto = new DynamicReportChartDto()
        //        {
        //            ChartType = report.ChartType,
        //            TimeFrameEnum = report.TimeFrameEnum,
        //            GraphType = report.GraphType
        //        },
        //        Name = report.Name,
        //        FiltersList = report.Filters,
        //        ColumnList = report.ColumnList,
        //        TableName = report.InitialTable,
        //        EndDateTime = report.EndDateTime,
        //        StartDateTime = report.StartDateTime
        //    };
        //    return GetDynamicReportData(model);
        //}

        //[HttpGet]
        //public IActionResult EditDynamic(Guid id)
        //{
        //    var model = _service.ParseReport(id);
        //    ViewBag.Folders = _service.GetAllFolders();
        //    ViewBag.Columns = _service.GetTableColumns(model.InitialTable);
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult EditDynamic(DynamicReportCreateRunViewModel model)
        //{
        //    //TODO: Check for auto mapper nested
        //    var databaseReport = _service.ParseReport(model.Id);
        //    databaseReport.Name = model.Name;
        //    databaseReport.ChartType = model.ChartDto.ChartType;
        //    databaseReport.ColumnList = model.ColumnList;
        //    databaseReport.EndDateTime = model.EndDateTime;
        //    databaseReport.StartDateTime = model.StartDateTime;
        //    databaseReport.GraphType = model.ChartDto.GraphType;
        //    databaseReport.DynamicReportFolderId = model.DynamicReportFolderId;
        //    databaseReport.InitialTable = model.TableName;
        //    databaseReport.TimeFrameEnum = model.ChartDto.TimeFrameEnum;
        //    try
        //    {
        //        _service.EditReport(databaseReport);
        //        return Json(new { success = true, message = "Updated" });
        //    }
        //    catch
        //    {
        //        return Json(new { success = false, message = "Server error" });
        //    }
        //}

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

        //[HttpPost]
        //public IActionResult GetDynamicReportData(DynamicReportCreateRunViewModel dto)
        //{
        //    if (dto.ChartDto.GraphType == GraphType.List || dto.ChartDto.GraphType == GraphType.Pie)
        //    {
        //        return
        //            Json(new
        //            {
        //                success = true,
        //                message = "Data Gathered successfully",
        //                queryData = JsonConvert.SerializeObject(_service.GetContent(dto.TableName,
        //                    dto.ColumnList, dto.StartDateTime,
        //                    dto.EndDateTime, dto.FiltersList)),
        //                graphType = dto.ChartDto.GraphType.ToString()
        //            });
        //    }

        //    switch (dto.ChartDto.TimeFrameEnum)
        //    {
        //        case TimeFrameEnum.Day:
        //            return Json(new
        //            {
        //                success = true,
        //                message = "Data Gathered successfully",
        //                queryData = JsonConvert.SerializeObject(_service.GetChartDataForTimeFrame(dto.TableName,
        //                    dto.ColumnList, dto.StartDateTime,
        //                    dto.EndDateTime, dto.FiltersList, dto.ChartDto, 1)),
        //                graphType = dto.ChartDto.GraphType.ToString()
        //            });
        //        case TimeFrameEnum.Week:
        //            return Json(new
        //            {
        //                success = true,
        //                message = "Data Gathered successfully",
        //                queryData = JsonConvert.SerializeObject(_service.GetChartDataForTimeFrame(dto.TableName,
        //                    dto.ColumnList, dto.StartDateTime,
        //                    dto.EndDateTime, dto.FiltersList, dto.ChartDto, 7)),
        //                graphType = dto.ChartDto.GraphType.ToString()
        //            });
        //        default:
        //            return Json(new
        //            {
        //                success = true,
        //                message = "Data Gathered successfully",
        //                queryData = JsonConvert.SerializeObject(_service.GetChartDataForTimeFrame(dto.TableName,
        //                    dto.ColumnList, dto.StartDateTime,
        //                    dto.EndDateTime, dto.FiltersList, dto.ChartDto, 30)),
        //                graphType = dto.ChartDto.GraphType.ToString()
        //            });
        //    }
        //}


        [HttpPost]
        public IActionResult GetReportData(DynamicReportDataModel dto)
        {
            var data = _service.GetReportContent(dto);
            return Json(new { charts = dto.DynamicReportCharts, data });
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

    public class ResponseClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}