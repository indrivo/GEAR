var relationId = 0;
var fieldId = 0;
var filterId = 0;
var chartId = 0;
var items = [];
var tables = [];
var allFields = [];
var LoadFinished = false;

function LoadReportData(data) {
	var dfd = $.Deferred();
	var callback = function (tData) {
		tables = $.merge([], tData);
		$('#tableSelect').select2({
			placeholder: " - Select tables - ",
			multiple: true,
			data: tData
		}).change(function () {
			if (LoadFinished) {
				items = $(this).val();
				SetRelValues();

				allFields = [];
				var tableRequests = [];

				$.each(items, function (i, option) {
					tableRequests.push(prepareFields(allFields, option));
				});

				$.when.apply($, tableRequests).done(function () {
					SetFieldValues(allFields);
					SetFilterValues(allFields);
				});
			}
		}).on('select2:unselecting', function () {
			$(this).data('unselecting', true);
		}).on('select2:opening', function (e) {
			if ($(this).data('unselecting')) {
				$(this).removeData('unselecting');
				e.preventDefault();
			}
		});
		if (data.Tables) {
			$('#tableSelect').val(data.Tables).trigger('change');
			items = data.Tables;
			allFields = [];

			var tableRequests = [];

			$.each(items, function (i, option) {
				tableRequests.push(prepareFields(allFields, option));
			});

			$.when.apply($, tableRequests).done(function () {

				$.each(data.FiltersList, function (i, filter) {
					AddFilter();
					$.each($('#pnlFilters > .row'), function (j, row) {
						if (i == j) {
							SetFilterValues(allFields);
							var filterFieldSelector = $(row).find(".filterFieldSelector");
							var filterOperationSelector = $(row).find(".filterOperationSelector");
							var filterValueSelector = $(row).find(".filterValueSelector");
							$.when(updateReportListKeyValue($('#pnlFilters').data('request-url'), null, filterOperationSelector.attr('id'))).done(function () {
								filterOperationSelector.val(filter.FilterType).trigger('change');
							});
							filterFieldSelector.val(filter.FieldName).trigger('change');
							filterValueSelector.val(filter.Value);
						}
					});
				});

				$.each(data.FieldsList, function (i, field) {
					AddField();
					$.each($('#pnlFields > .row'), function (j, row) {
						if (i == j) {
							SetFieldValues(allFields);
							var fieldSelector = $(row).find(".fieldSelector");
							var fieldOperationSelector = $(row).find(".fieldOperationSelector");
							var filterValueSelector = $(row).find(".filterValueSelector");
							$.when(updateReportListKeyValue($('#pnlFields').data('request-url'), null, fieldOperationSelector.attr('id'))).done(function () {
								fieldOperationSelector.val(field.AggregateType).trigger('change');
							});
							fieldSelector.val(field.FieldName).trigger('change');
							filterValueSelector.val(field.FieldAlias);
						}
					});
				});

				$.each(data.Relations, function (i, relation) {
					AddRelationship();
					$.each($('#pnlRelationships > .row'), function (j, row) {
						if (i == j) {
							SetRelValues();
							var primaryKeyTableSelect = $(row).find(".primaryKeyTableSelect");
							var foreignKeyTableSelect = $(row).find(".foreignKeyTableSelect");
							var foreignKeySelect = $(row).find(".foreignKeySelect");
							primaryKeyTableSelect.val(relation.PrimaryKeyTable).trigger('change');
							foreignKeyTableSelect.val(relation.ForeignKeyTable).trigger('change');
							$.when(updateReportList($('#pnlRelationships').data('request-url'), { tableName: relation.ForeignKeyTable }, foreignKeySelect.attr('id'))).done(function () {
								foreignKeySelect.val(relation.ForeignKey).trigger('change');
							});
						}
					});
				});

				var chartRequests = [];


				$.when(updateReportListKeyValue($('#chartSelector').data('request-url'), null, "chartSelector")).done(function () {
					$.each(data.DynamicReportCharts, function (i, chart) {
						chartRequests.push(AddChart(chart.ChartType));
					});

					$.when.apply($, chartRequests).done(function () {
						$.each($('#pnlCharts > .row'), function (i, row) {
							var chartIndex = $(row).find("input[name=chartIndex]").val();
							var chartType = $(row).find("input[name=chartType]").val();
							var chartTitle = $(row).find(".chartTitle");
							chartTitle.val(data.DynamicReportCharts[chartIndex - 1].ChartTitle);
							var chartFields = $(row).find(".chartFieldSelector");
							if (chartType == 1) {
								var multiselectChartFields = [];
								$.map(data.DynamicReportCharts[chartIndex - 1].DynamicReportChartFields, function (val, i) {
									multiselectChartFields.push(val.FieldName);
								});
								$.each(chartFields, function (j, chartField) {
									$(chartField).val(multiselectChartFields).trigger('change');
								});
							}
							else {
								$.each(chartFields, function (j, chartField) {
									$(chartField).val(data.DynamicReportCharts[chartIndex - 1].DynamicReportChartFields[j].FieldName).trigger('change');
								});
							}
						});
						dfd.resolve();
						LoadFinished = true;
					});
				});
			});
		}




	};
	GetDataList($('#tableSelect').data('request-url'), null, callback);





	//$.when(updateReportList($('#tableSelect').data('request-url'), null, "tableSelect")).done(function () {

	//});
	return dfd.promise();
}

function updateReportList(action, data, selectOptions) {
	var dfd = $.Deferred();
	$("#" + selectOptions).html("");
	var callback = function (data) {
		var s;
		for (let i = 0; i < data.length; i++) {
			s += `<option value="${data[i]}">${data[i]}</option>`;
		}
		$("#" + selectOptions).html(s);
		dfd.resolve();
	};
	GetDataList(action, data, callback);
	return dfd.promise();
}

function GetDataList(action, data, callback, param) {
	var requestAction = action;
	$.ajax({
		type: "POST",
		url: requestAction,
		dataType: 'json',
		data: data,
		success: function (data) {
			callback(data, param);
		}
	});
}

function updateReportListKeyValue(action, data, selectOptions) {
	var dfd = $.Deferred();
	$("#" + selectOptions).html("");
	var callback = function (data) {
		var s;
		for (let i = 0; i < data.length; i++) {
			s += `<option value="${data[i].id}">${data[i].text}</option>`;
		}
		$("#" + selectOptions).html(s);
		dfd.resolve();
	};
	GetDataList(action, data, callback);
	return dfd.promise();
}

function AddRelationship() {
	relationId++;
	var currentHtml = `
		<div class="row">
			<div class="col-3">
				<div class="form-group">
					<label for="primaryKeyTableSelect`+ relationId + `">Primary key table:</label>
					<select class="form-control primaryKeyTableSelect" id="primaryKeyTableSelect`+ relationId + `"><option></option></select>
				</div>
			</div>
			<div class="col-3">
				<div class="form-group">
					<label for="foreignKeyTableSelect`+ relationId + `">Foreign key table:</label>
					<select class="form-control foreignKeyTableSelect" id="foreignKeyTableSelect`+ relationId + `"><option></option></select>
				</div>
			</div>
			<div class="col-3">
				<div class="form-group">
					<label for="foreignKeySelect`+ relationId + `">Foreign key:</label>
					<select class="form-control foreignKeySelect" id="foreignKeySelect`+ relationId + `"><option></option></select>
				</div>
			</div>
			<div class="col-1">
				<label>&nbsp;</label>
				<button class="btn btn-danger removeRelationship">Remove Relationship</button>
			</div>
		</div>`;
	$("#pnlRelationships").append(currentHtml);
	$('#primaryKeyTableSelect' + relationId).select2({
		placeholder: " - Select table - ",
		multiple: false
	});
	$('#foreignKeyTableSelect' + relationId).select2({
		placeholder: " - Select table - ",
		multiple: false
	}).change(function () {
		if (LoadFinished) {
			var data = { tableName: $(this).val() };
			updateReportList($('#pnlRelationships').data('request-url'), data, "foreignKeySelect" + relationId);
		}
	});
	$('#foreignKeySelect' + relationId).select2({
		placeholder: " - Select key - ",
		multiple: false
	});
	if (LoadFinished) {
		SetRelValues();
	}
}

function AddField() {
	fieldId++;
	$('#cbFields').show();
	var currentHtml = `
				<div class="row">
			<div class="col-12 row m-t-20">
				<div class="col-md-4">
					<div class="form-group">
						<label>Field:</label>
						<select class="form-control fieldSelector" id="fieldSelector`+ fieldId + `">
						</select>
					</div>
				</div>
				<div class="col-4">
					<div class="form-group">
						<label>Aggregates:</label>
						<select class="form-control fieldOperationSelector" id="fieldOperationSelector`+ fieldId + `">
						</select>
					</div>
				</div>
				<div class="col-md-3">
					<div class="form-group">
						<label>Caption:</label>
						<input class="form-control filterValueSelector" type="text" id="fieldCaptionSelector`+ fieldId + `">
							</div>
					</div>
					<div class="col-1">
						<label>&nbsp;</label>
						<button class="btn btn-danger removeField">Remove Field</button>
					</div>
				</div>
			</div>`;
	$("#pnlFields").append(currentHtml);

	$('#fieldSelector' + fieldId).select2({
		placeholder: " - Select field - ",
		multiple: false
	}).change(function () {
		if (LoadFinished) {
			var fields = [];
			$.each($('#pnlFields > .row'), function (i, option) {
				var fieldsSelect = $(option).find(".fieldSelector");
				var fieldData = fieldsSelect.select2('data')[0];
				if (fieldData) {
					fields.push({ id: fieldData.id, text: fieldData.element.parentElement.value + ' (' + fieldData.text + ')', index: i });
				}
			});
			SetChartValues(fields);
		}
	});
	$('#fieldOperationSelector' + fieldId).select2({
		placeholder: " - Select operation - ",
		multiple: false
	});

	if (LoadFinished) {
		updateReportListKeyValue($('#pnlFields').data('request-url'), null, 'fieldOperationSelector' + fieldId);
		SetFieldValues(allFields);
	}
}

function AddFilter() {
	filterId++;
	$('#cbFilters').show();
	var currentHtml = `
				<div class="row">
				<div class="col-12 row m-t-20 changedRow">
					<div class="col-md-4">
						<div class="form-group">
							<label>Field:</label>
							<select class="form-control filterFieldSelector" id="filterFieldSelector`+ filterId + `">
							</select>
						</div>
					</div>
					<div class="col-4">
						<div class="form-group">
							<label>Operation:</label>
							<select class="form-control filterOperationSelector" id="filterOperationSelector`+ filterId + `">
							</select>
						</div>
					</div>
					<div class="col-md-3">
						<div class="form-group">
							<label>Value:</label>
							<input class="form-control filterValueSelector" type="text" id="filterValueSelector`+ filterId + `">
							</div>
						</div>
						<div class="col-1">
							<label>&nbsp;</label>
							<button class="btn btn-danger removeFilter">Remove Filter</button>
						</div>
					</div>
				</div>`;
	$("#pnlFilters").append(currentHtml);
	$('#filterFieldSelector' + filterId).select2({
		placeholder: " - Select field - ",
		multiple: false
	});
	$('#filterOperationSelector' + filterId).select2({
		placeholder: " - Select operation - ",
		multiple: false
	});
	if (LoadFinished) {
		updateReportListKeyValue($('#pnlFilters').data('request-url'), null, 'filterOperationSelector' + filterId);
		SetFilterValues(allFields);
	}
}

function AddChart(chartType) {
	chartId++;
	var dfd = $.Deferred();
	$('#cbCharts').show();
	var callback = function (data, index) {

		var chartTypeData = $('#chartSelector').find("option[value='" + chartType + "']")[0];
		var currentHtml = `
						<div class="row">
					<div class="col-12 row m-t-20 changedRow">
						<input name="chartType" type="hidden" value="` + chartType + `" />
                        <input name="chartIndex" type="hidden" value="` + index + `" />
						<div class="col-md-1">
							<div class="form-group">
								<label>Chart type:</label>
								<div class="font-weight-bold">
									` + chartTypeData.text + `
										</div>
							</div>
						</div>
						<div class="col-md-2">
							<div class="form-group">
								<label>Chart title:</label>
								<input class="form-control chartTitle" name="chartTitle" type="text" id="chartTitle`+ chartId + `">
									</div>
							</div>

							`;
		$.each(data, function (i, item) {
			currentHtml += `
						<div class="col-md-2">
							<div class="form-group">
								<label>`+ item.text + `:</label>
								<select class="form-control chartFieldSelector" id="chartField_`+ item.id + "_" + chartId + `">
								</select>
								<input name="fieldType" type="hidden" value="`+ item.id + `" />
							</div>
						</div>`;
		});
		currentHtml += `<div class="col-1">
								<label>&nbsp;</label>
								<button class="btn btn-danger removeChart">Remove Chart</button>
							</div>
						</div>
					</div>`;
		//$("#pnlCharts").append(chartType);
		$("#pnlCharts").append(currentHtml);			   

		$('#pnlCharts > .row').each(function () {
			var chartFieldCollection = $(this).find(".chartFieldSelector");
			chartFieldCollection.each(function () {
				var fieldType = chartFieldCollection.parent().find("input[name=fieldType]");
				var isMultiselect = false;
				if (fieldType.val() == 0) {
					isMultiselect = true;
				}
				chartField = $(this);
				chartField.select2({
					placeholder: " - Select field - ",
					multiple: isMultiselect
				});
			});
		});


		var fields = [];
		$.each($('#pnlFields > .row'), function (i, option) {
			var fieldsSelect = $(option).find(".fieldSelector");
			var fieldData = fieldsSelect.select2('data')[0];
			if (fieldData) {
				fields.push({ id: fieldData.id, text: fieldData.element.parentElement.value + ' (' + fieldData.text + ')', index: i });
			}
		});
		SetChartValues(fields);
		dfd.resolve();
	};
	GetDataList($('#pnlCharts').data('request-url'), { chartType: chartType }, callback, chartId);
	return dfd.promise();
}


function SetRelValues() {
	$('#pnlRelationships > .row').each(function () {

		var primaryKeyTableSelect = $(this).find(".primaryKeyTableSelect");
		var foreignKeyTableSelect = $(this).find(".foreignKeyTableSelect");

		var prevValuePKT = primaryKeyTableSelect.val();

		primaryKeyTableSelect.val(null).empty();

		$.each(items, function (i, option) {
			var newOption = new Option(option, option, false, false);
			primaryKeyTableSelect.append(newOption).trigger('change');
			if (prevValuePKT == option) {
				primaryKeyTableSelect.val(option);
			}
		});

		var prevValueFKT = foreignKeyTableSelect.val();

		foreignKeyTableSelect.val(null).empty();

		$.each(items, function (i, option) {
			var newOption = new Option(option, option, false, false);
			foreignKeyTableSelect.append(newOption).trigger('change');
			if (prevValueFKT == option) {
				foreignKeyTableSelect.val(option);
			}
		});

	});
}

function prepareFields($n, option) {
	var dfd = $.Deferred();
	var callback = function (data) {
		var children = new Array();

		var tableData = option.split(".");

		$.each(data, function (i, field) {
			children.push({ id: tableData[0] + '."' + tableData[1] + '"."' + field + '"', text: field });
		});
		$n.push({
			id: option,
			text: option,
			children: children
		});
		dfd.resolve();
	};

	var param = { tableName: option };
	GetDataList($('#pnlRelationships').data('request-url'), param, callback);

	return dfd.promise();
}


function SetFieldValues(data) {
	if (data.length > 0) {
		$('#pnlFields > .row').each(function () {

			var fieldsSelect = $(this).find(".fieldSelector");
			var prevValueFields = [];
			prevValueFields.push(fieldsSelect.val());
			var currentDataFields = [];
			$.each(data, function (i, option) {
				if (option.children != null && option.children.length > 0) {
					$.map(option.children, function (val, i) {
						currentDataFields.push(val.id);
					});
				}
			});

			var intersection = $(prevValueFields).filter(currentDataFields);

			fieldsSelect.val(null).empty();
			fieldsSelect.select2({
				placeholder: " - Select fields - ",
				multiple: false,
				templateSelection: function (item) {
					var $option = $(item.element);
					var $optGroup = $option.parent();
					if ($optGroup.attr('label')) {
						return $optGroup.attr('label') + ' (' + item.text + ')';
					}
					else {
						return " - Select fields - ";
					}
				},
				data: data
			});

			fieldsSelect.val(intersection).trigger('change');
		});
	}
}

function SetFilterValues(data) {
	if (data.length > 0) {
		$('#pnlFilters > .row').each(function () {

			var filterFieldSelector = $(this).find(".filterFieldSelector");
			var prevValueFields = [];
			prevValueFields.push(filterFieldSelector.val());
			var currentDataFields = [];
			$.each(data, function (i, option) {
				if (option.children != null && option.children.length > 0) {
					$.map(option.children, function (val, i) {
						currentDataFields.push(val.id);
					});
				}
			});

			var intersection = $(prevValueFields).filter(currentDataFields);

			filterFieldSelector.val(null).empty();
			filterFieldSelector.select2({
				placeholder: " - Select field - ",
				multiple: false,
				templateSelection: function (item) {
					var $option = $(item.element);
					var $optGroup = $option.parent();
					if ($optGroup.attr('label')) {
						return $optGroup.attr('label') + ' (' + item.text + ')';
					}
					else {
						return " - Select field - ";
					}
				},
				data: data
			});

			filterFieldSelector.val(intersection).trigger('change');
		});
	}
}

function SetChartValues(data) {
	if (data.length > 0) {
		var currentDataFields = [];
		$.map(data, function (val, i) {
			currentDataFields.push(val.id);
		});
		$('#pnlCharts > .row').each(function () {

			var chartFieldCollection = $(this).find(".chartFieldSelector");
			var fieldType = chartFieldCollection.parent().find("input[name=fieldType]");

			var isMultiselect = false;

			if (fieldType.val() == 0) {
				isMultiselect = true;
			}

			chartFieldCollection.each(function () {
				chartFieldSelector = $(this);
				var prevValueFields = [];
				if (fieldType.val() == 0) {
					console.log(chartFieldSelector.val());
				}

				if (chartFieldSelector.val()) {
					if ($.isArray(chartFieldSelector.val())) {
						prevValueFields = chartFieldSelector.val();
					}
					else {
						prevValueFields.push(chartFieldSelector.val());
					}
				}
				var intersection = $(prevValueFields).filter(currentDataFields);
				chartFieldSelector.val(null).empty();
				chartFieldSelector.select2({
					placeholder: " - Select field - ",
					multiple: isMultiselect,
					data: data
				});

				chartFieldSelector.val(intersection).trigger('change');
			});
		});
	}
}
//$(document).ready(function () {

//});

function LoadNew() {
	//updateReportListKeyValue($('#tableSelect').data('request-url'), null, "tableSelect");
	var callback = function (data) {
		tables = $.merge([], data);
		$('#tableSelect').select2({
			placeholder: " - Select tables - ",
			multiple: true,
			data: data
		}).change(function () {
			if (LoadFinished) {
				items = $(this).val();
				SetRelValues();

				allFields = [];
				var tableRequests = [];

				$.each(items, function (i, option) {
					tableRequests.push(prepareFields(allFields, option));
				});

				$.when.apply($, tableRequests).done(function () {
					SetFieldValues(allFields);
					SetFilterValues(allFields);
				});
			}
		}).on('select2:unselecting', function () {
			$(this).data('unselecting', true);
		}).on('select2:opening', function (e) {
			if ($(this).data('unselecting')) {
				$(this).removeData('unselecting');
				e.preventDefault();
			}
		});
	};
	GetDataList($('#tableSelect').data('request-url'), null, callback);
	updateReportListKeyValue($('#chartSelector').data('request-url'), null, "chartSelector");
	LoadFinished = true;
}

$(document).ready(function () {
	//#endregion Common
	//$('#tableSelect').select2({
	//	placeholder: " - Select tables - ",
	//	multiple: true,
	//	query: function (q) {
	//		var data = { results: tables };
	//		q.callback(data);
	//	}
	//}).change(function () {
	//	if (LoadFinished) {
	//		items = $(this).val();
	//		SetRelValues();

	//		allFields = [];
	//		var tableRequests = [];

	//		$.each(items, function (i, option) {
	//			tableRequests.push(prepareFields(allFields, option));
	//		});

	//		$.when.apply($, tableRequests).done(function () {
	//			SetFieldValues(allFields);
	//			SetFilterValues(allFields);
	//		});
	//	}
	//}).on('select2:unselecting', function () {
	//	$(this).data('unselecting', true);
	//}).on('select2:opening', function (e) {
	//	if ($(this).data('unselecting')) {
	//		$(this).removeData('unselecting');
	//		e.preventDefault();
	//	}
	//});

	//script for generation of report
	$('#openReportModal').click(function () {
		$('#saveReportModal').modal('show');
		$('#saveReport').click(function (e) {

			var reportData = GetReportData();

			var data = {
				reportDataModel: reportData,
				name: $('#reportName').val(),
				dynamicReportFolderId: $('#folderSelector').val(),
				id: $('#reportId').val()
			}

			var callback = function (result) {
				swal({
					position: 'top-end',
					type: 'success',
					title: result.message,
					showConfirmButton: false,
					timer: 3000
				});
			};

			GetDataList($(this).data('request-url'), data, callback);

			$('#saveReportModal').modal('hide');
			e.stopImmediatePropagation();
		});

	});

	//#region Data


	$('#chartSelector').select2({
		placeholder: " - Select chart - ",
		multiple: false
	});


	//#endregion Data

	//#region Relationships

	$("#pnlRelationships").on("click", ".removeRelationship", function () {
		var parent = $(this).parent().parent();
		Swal.fire({
			title: 'Are you sure?',
			text: "You won't be able to revert this!",
			type: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, delete it!'
		}).then((result) => {
			if (result.value) {
				Swal.fire(
					'Deleted!',
					'Your file has been deleted.',
					'success'
				);
				$(parent).remove();
			}
		});
	});



	$("#AddRelationship").click(function () {
		AddRelationship();
	});

	//#region Fields

	$("#pnlFields").on("click", ".removeField", function () {
		var parent = $(this).parent().parent().parent();
		Swal.fire({
			title: 'Are you sure?',
			text: "You won't be able to revert this!",
			type: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, delete it!'
		}).then((result) => {
			if (result.value) {
				Swal.fire(
					'Deleted!',
					'Your file has been deleted.',
					'success'
				);
				$(parent).remove();
				if ($('#pnlFields > div').length == 0) {
					$('#cbFields').hide();
				}
				var fields = [];
				$.each($('#pnlFields > .row'), function (i, option) {
					var fieldsSelect = $(option).find(".fieldSelector");
					var fieldData = fieldsSelect.select2('data')[0];
					if (fieldData) {
						fields.push({ id: fieldData.id, text: fieldData.element.parentElement.value + ' (' + fieldData.text + ')', index: i });
					}
				});
				SetChartValues(fields);
			}
		});
	});




	$("#AddField").click(function () {
		AddField();
	});

	//#endregion Fields

	//#region Filters

	$("#pnlFilters").on("click", ".removeFilter", function () {
		var parent = $(this).parent().parent().parent();
		Swal.fire({
			title: 'Are you sure?',
			text: "You won't be able to revert this!",
			type: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, delete it!'
		}).then((result) => {
			if (result.value) {
				Swal.fire(
					'Deleted!',
					'Your file has been deleted.',
					'success'
				);
				$(parent).remove();
				if ($('#pnlFilters > div').length == 0) {
					$('#cbFilters').hide();
				}
			}
		});
	});


	$("#AddFilter").click(function () {
		AddFilter();
	});

	//#endregion Filters


	//#region Charts

	$("#pnlCharts").on("click", ".removeChart", function () {
		var parent = $(this).parent().parent().parent();
		Swal.fire({
			title: 'Are you sure?',
			text: "You won't be able to revert this!",
			type: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, delete it!'
		}).then((result) => {
			if (result.value) {
				Swal.fire(
					'Deleted!',
					'Your file has been deleted.',
					'success'
				);
				$(parent).remove();
				if ($('#pnlCharts > div').length == 0) {
					$('#cbCharts').hide();
				}
			}
		});
	});

	$("#AddChart").click(function () {
		AddChart($("#chartSelector").val());
	});



	//#endregion Charts

});

$('#runReport').click(function () {
	GenerateData($(this).data('request-url'));
});


function GetReportData() {

	var tables = $('#tableSelect').val();
	var relations = [];
	var fieldsList = [];
	var filtersList = [];


	$('#pnlRelationships > .row').each(function () {

		var primaryKeyTableSelect = $(this).find(".primaryKeyTableSelect");
		var foreignKeyTableSelect = $(this).find(".foreignKeyTableSelect");
		var foreignKeySelect = $(this).find(".foreignKeySelect");

		relations.push({
			"primaryKeyTable": primaryKeyTableSelect.val(),
			"foreignKeyTable": foreignKeyTableSelect.val(),
			"foreignKey": foreignKeySelect.val()
		});
	});

	$('#pnlFields > .row').each(function () {

		var fieldsSelect = $(this).find(".fieldSelector");
		var fieldOperationSelector = $(this).find(".fieldOperationSelector");
		var filterValueSelector = $(this).find(".filterValueSelector");

		fieldsList.push({
			"fieldName": fieldsSelect.val(),
			"fieldAlias": filterValueSelector.val(),
			"aggregateType": fieldOperationSelector.val()
		});

	});

	$('#pnlFilters > .row').each(function () {
		var filterFieldSelector = $(this).find(".filterFieldSelector");
		var filterOperationSelector = $(this).find(".filterOperationSelector");
		var filterValueSelector = $(this).find(".filterValueSelector");

		filtersList.push({
			"fieldName": filterFieldSelector.val(),
			"filterType": filterOperationSelector.val(),
			"value": filterValueSelector.val()
		});
	});

	var charts = [];

	$('#pnlCharts > .row').each(function () {

		var chartType = $(this).find("input[name=chartType]");
		var chartTitle = $(this).find("input[name=chartTitle]");
		var chartFields = [];

		var chartFieldCollection = $(this).find(".chartFieldSelector");

		chartFieldCollection.each(function () {
			chartField = $(this);
			var chartFieldData = chartField.select2('data');
			var fieldType = $(this).parent().find("input[name=fieldType]");
			$.each(chartFieldData, function (i, chartFieldOption) {
				chartFields.push({
					fieldIndex: chartFieldOption.index,
					fieldName: chartFieldOption.id,
					chartFieldType: fieldType.val()
				});
			});
		});

		charts.push({
			chartTitle: chartTitle.val(),
			chartType: chartType.val(),
			dynamicReportChartFields: chartFields
		});
	});

	return {
		"tables": tables,
		"relations": relations,
		"fieldsList": fieldsList,
		"filtersList": filtersList,
		"dynamicReportCharts": charts
	}
}


function GenerateData(graphUrl) {
	var data = GetReportData();

	//Send data and receive model
	$.ajax({
		url: graphUrl,
		content: "application/json; charset=utf-8",
		type: 'POST',
		data: data,
		success: function (result) {
			if (result.error != '') {
				swal({
					position: 'top-end',
					type: 'warning',
					title: result.error,
					showConfirmButton: false,
					timer: 7000
				});

			}
			else if (result.charts) {
				$("#queryResultTable").html('');
				$('#chart-box').html('');
				$.each(result.charts, function (i, chart) {
					if (chart.chartType == 1) {
						var table = $.makeTable(result.data, chart);
						$(table).appendTo("#queryResultTable");
					}
					else if (chart.chartType == 2) {
						var tablePivot = $.makePivotTable(result.data, chart);
						$(tablePivot).appendTo("#queryResultTable");
					}
					else if (chart.chartType == 3) {
						generateChart('pie', result.data, chart, 'chart-box');
					}
					else if (chart.chartType == 4) {
						generateChart('doughnut', result.data, chart, 'chart-box');
					}
					else if (chart.chartType == 5) {
						generateChart('bar', result.data, chart, 'chart-box');
					}
					else if (chart.chartType == 6) {
						generateChart('horizontalBar', result.data, chart, 'chart-box');
					}
					else if (chart.chartType == 7) {
						generateChart('line', result.data, chart, 'chart-box');
					}
				});
			}

		},
		error: function (jqXHR, textStatus, errorThrown) {
			ShowError(jqXHR.status);
		}
	});
}

function GetReportDataById(getUrl, reportId, boxId) {
	$.ajax({
		url: getUrl,
		content: "application/json; charset=utf-8",
		type: 'POST',
		data: { id: reportId },
		success: function (result) {
			if (result.charts) {
				var box = $("#" + boxId);
				box.html('');
				$.each(result.charts, function (i, chart) {
					if (chart.chartType == 1) {
						var table = $.makeTable(result.data, chart);
						$(table).appendTo($(box));
					}
					else if (chart.chartType == 2) {
						var tablePivot = $.makePivotTable(result.data, chart);
						$(tablePivot).appendTo($(box));
					}
					else if (chart.chartType == 3) {
						generateChart('pie', result.data, chart, boxId);
					}
					else if (chart.chartType == 4) {
						generateChart('doughnut', result.data, chart, boxId);
					}
					else if (chart.chartType == 5) {
						generateChart('bar', result.data, chart, boxId);
					}
					else if (chart.chartType == 6) {
						generateChart('horizontalBar', result.data, chart, boxId);
					}
					else if (chart.chartType == 7) {
						generateChart('line', result.data, chart, boxId);
					}
				});
			}

		},
		error: function (jqXHR, textStatus, errorThrown) {
			ShowError(jqXHR.status);
		}
	});
}

$.makeTable = function (mydata, chartDataSource) {
	var resultBox = $('<div class="mt-3 mb-3 row col-10 report-table"></div>');
	var header = $('<h1>' + chartDataSource.chartTitle + '</h1>');
	var table = $('<table class="mb-4" cellspacing="0" cellpadding="0">');
	var tblHeader = "<tr>";
	var columnIndex = 0;
	for (var k in mydata[0]) {
		var c = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.fieldIndex == columnIndex;
		});
		if (c.length > 0) {
			tblHeader += "<th>" + k + "</th>";
		}
		columnIndex++;
	}
	tblHeader += "</tr>";
	$(tblHeader).appendTo(table);
	$.each(mydata, function (index, value) {
		var TableRow = "<tr>";
		var columnIndex = 0;
		$.each(value, function (index, val) {
			var c = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
				return a.fieldIndex == columnIndex;
			});
			if (c.length > 0) {
				TableRow += "<td>" + val + "</td>";
			}
			columnIndex++;
		});
		TableRow += "</tr>";
		$(table).append(TableRow);
	});
	$(resultBox).append(header);
	$(resultBox).append(table);
	return $(resultBox);
};

$.makePivotTable = function (dataArray, chartDataSource) {
	var resultBox = $('<div class="mt-3 mb-3 row col-10 report-table"></div>');
	var header = $('<h1>' + chartDataSource.chartTitle + '</h1>');
	var table = $('<table class="pivot mb-4" cellspacing="0" cellpadding="0">');
	var labels = [];
	var xAxis = [];

	var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
		return a.chartFieldType == 1;
	});

	var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
		return a.chartFieldType == 2;
	});

	var y = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
		return a.chartFieldType == 3;
	});

	$.each(dataArray, function (index, value) {
		labels.push(value[Object.keys(value)[label[0].fieldIndex]]);
	});

	labels = $.unique(labels.sort(function (a, b) {
		if (isNaN(a) || isNaN(b)) {
			return a > b ? 1 : -1;
		}
		return a - b;
	}));

	$.each(dataArray, function (index, value) {
		xAxis.push(parseInt(value[Object.keys(value)[x[0].fieldIndex]]));
	});

	xAxis = $.unique(xAxis.sort(function (a, b) {
		if (isNaN(a) || isNaN(b)) {
			return a > b ? 1 : -1;
		}
		return a - b;
	}));


	var tblHeader = "<tr>";
	var firstHeaderCell = [];
	for (var k in dataArray[label[0].fieldIndex]) firstHeaderCell.push(k);
	tblHeader += "<th class='diag'><span class='inf'>" + firstHeaderCell[label[0].fieldIndex] + "</span><span class='sup'>" + firstHeaderCell[x[0].fieldIndex] + "</span></th>";
	$.each(xAxis, function (index, xValue) {
		tblHeader += "<th>" + xValue + "</th>";
	});
	tblHeader += "</tr>";
	$(tblHeader).appendTo(table);

	$.each(labels, function (index, yValue) {
		var TableRow = "<tr>";
		TableRow += "<td>" + yValue + "</td>";
		$.each(xAxis, function (index, xValue) {
			var arr = $.grep(dataArray, function (a) {
				return a[Object.keys(a)[label[0].fieldIndex]] == yValue && a[Object.keys(a)[x[0].fieldIndex]] == xValue;
			});

			if (arr[0]) {
				TableRow += "<td>" + arr[0][Object.keys(arr[0])[y[0].fieldIndex]] + "</td>";
			}
			else {
				TableRow += "<td> - </td>";
			}
		});

		TableRow += "</tr>";
		$(table).append(TableRow);
	});
	$(resultBox).append(header);
	$(resultBox).append(table);
	return $(resultBox);
};

function generateChart(type, dataArray, chartDataSource, chartBoxId) {
	var chartLabels = [];
	var tableValues = [];
	var colorSets = [];
	var chartData = [];

	var options;

	if (type == 'pie') {

		var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 1;
		});

		var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 2;
		});


		$.each(dataArray, function (index, value) {
			chartLabels.push(value[Object.keys(value)[label[0].fieldIndex]]);
		});

		$.each(dataArray, function (index, value) {
			tableValues.push(value[Object.keys(value)[x[0].fieldIndex]]);
		});


		chartData = {
			labels: chartLabels,
			datasets: [{
				data: tableValues,
				backgroundColor: colorSets,
				borderColor: colorSets,
				borderWidth: 1
			}]
		}

		options = {
			title: {
				display: true,
				text: chartDataSource.chartTitle
			}
		};

	}
	else if (type == "bar") {
		var datasets = [];

		var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 1;
		});

		var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 2;
		});


		$.each(dataArray, function (index, value) {
			datasets.push({
				label: value[Object.keys(value)[label[0].fieldIndex]],
				backgroundColor: getRandomColor(),
				data: [value[Object.keys(value)[x[0].fieldIndex]]],
			});
		});

		chartData = {
			labels: [''],
			datasets: datasets
		}

		options = {
			scaleShowLabels: false,
			scales: {
				yAxes: [{
					scaleLabel: {
						display: true,
						labelString: Object.keys(dataArray[0])[x[0].fieldIndex]
					},
					ticks: {
						beginAtZero: true,
						display: true,
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: Object.keys(dataArray[0])[label[0].fieldIndex]
					},
					ticks: {
						beginAtZero: true,
						display: true,
					}
				}],
			},
			title: {
				display: true,
				text: chartDataSource.chartTitle
			}
		};
	}
	else if (type == "horizontalBar") {
		var datasets = [];

		var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 1;
		});

		var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 2;
		});


		$.each(dataArray, function (index, value) {
			datasets.push({
				label: value[Object.keys(value)[label[0].fieldIndex]],
				backgroundColor: getRandomColor(),
				data: [value[Object.keys(value)[x[0].fieldIndex]]],
			});
		});

		chartData = {
			labels: [''],
			datasets: datasets
		}

		options = {
			scaleShowLabels: false,
			scales: {
				yAxes: [{
					scaleLabel: {
						display: true,
						labelString: Object.keys(dataArray[0])[label[0].fieldIndex]
					},
					ticks: {
						beginAtZero: true,
						display: true,
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: Object.keys(dataArray[0])[x[0].fieldIndex]
					},
					ticks: {
						beginAtZero: true,
						display: true,
					}
				}],
			},
			title: {
				display: true,
				text: chartDataSource.chartTitle
			}
		};
	}
	else if (type == 'line') {
		var datasets = [];
		var xAxis = [];
		var labels = [];

		var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 1;
		});

		var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 2;
		});

		var y = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 3;
		});


		$.each(dataArray, function (index, value) {
			labels.push(value[Object.keys(value)[label[0].fieldIndex]]);
		});

		labels = $.unique(labels.sort(function (a, b) {
			if (isNaN(a) || isNaN(b)) {
				return a > b ? 1 : -1;
			}
			return a - b;
		}));

		$.each(dataArray, function (index, value) {
			xAxis.push(parseInt(value[Object.keys(value)[x[0].fieldIndex]]));
		});

		xAxis = $.unique(xAxis.sort(function (a, b) {
			if (isNaN(a) || isNaN(b)) {
				return a > b ? 1 : -1;
			}
			return a - b;
		}));


		$.each(labels, function (index, yValue) {
			var ds = [];
			$.each(xAxis, function (index, xValue) {
				var arr = $.grep(dataArray, function (a) {
					return a[Object.keys(a)[label[0].fieldIndex]] == yValue && a[Object.keys(a)[x[0].fieldIndex]] == xValue;
				});
				if (arr[0]) {
					ds.push(arr[0][Object.keys(arr[0])[y[0].fieldIndex]]);
				}
				else {
					ds.push(0);
				}
			});
			datasets.push({
				label: yValue,
				borderColor: getRandomColor(),
				data: ds,
				lineTension: 0,
				fill: false,
				backgroundColor: 'transparent',
				pointStyle: 'rectRounded'
			});
		});

		chartData = {
			labels: xAxis,
			datasets: datasets
		};

		options = {
			scaleShowLabels: false,
			scales: {
				yAxes: [{
					scaleLabel: {
						display: true,
						labelString: [Object.keys(dataArray[0])[y[0].fieldIndex]]
					},
					ticks: {
						beginAtZero: true,
						display: true
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: [Object.keys(dataArray[0])[x[0].fieldIndex]]
					},
					ticks: {
						beginAtZero: true,
						display: true,
					}
				}],
			},
			title: {
				display: true,
				text: chartDataSource.chartTitle
			}
		};
	}
	else if (type == 'doughnut') {

		var label = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 1;
		});

		var x = $.grep(chartDataSource.dynamicReportChartFields, function (a) {
			return a.chartFieldType == 2;
		});


		$.each(dataArray, function (index, value) {
			chartLabels.push(value[Object.keys(value)[label[0].fieldIndex]]);
		});

		$.each(dataArray, function (index, value) {
			tableValues.push(value[Object.keys(value)[x[0].fieldIndex]]);
		});


		chartData = {
			labels: chartLabels,
			datasets: [{
				data: tableValues,
				backgroundColor: colorSets,
				borderColor: colorSets,
				borderWidth: 1
			}]
		}

		options = {
			title: {
				display: true,
				text: chartDataSource.chartTitle
			}
		};

	}

	tableValues.forEach(function () {
		colorSets.push(getRandomColor());
	});



	//Change for colors
	if (type == 'line') {
		colorSets = [
			'rgba(255, 99, 132, 0.2)',
			'rgba(54, 162, 235, 0.2)',
			'rgba(255, 206, 86, 0.2)',
			'rgba(75, 192, 192, 0.2)',
			'rgba(153, 102, 255, 0.2)',
			'rgba(255, 159, 64, 0.2)'
		];
	}


	$('#' + chartBoxId).append($('<canvas class="col-10" id="myChart' + type + '" style="width:1000px; height:278px"></canvas>'));

	var canvas = document.getElementById("myChart" + type);
	var ctx = canvas.getContext('2d');

	new Chart(ctx, {
		type: type,
		data: chartData,
		options: options
	});
}