/* Data table render
 * A plugin for render tables in dynamic pages
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica(Indrivo) Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Data Table plugin require JQuery');
}

//------------------------------------------------------------------------------------//
//								Table select multiple
//------------------------------------------------------------------------------------//
window.TBuilder = new TableBuilder();

function RenderTableSelect() {

}

RenderTableSelect.prototype.constructor = RenderTableSelect;

RenderTableSelect.prototype.settings = {
	headContent: "#",
	classNameText: 'select-checkbox',
	select: {
		style: 'multi',
		selector: 'td:not(.not-selectable):first-child',
		blurable: true
	}
};

RenderTableSelect.prototype.className = function () {
	return this.settings.classNameText;
};

RenderTableSelect.prototype.templateSelect = function (data, type, row, meta) {
	return "";
};

//------------------------------------------------------------------------------------//
//								Table Export
//------------------------------------------------------------------------------------//

function TableExport() { };
TableExport.constructor = TableExport;
TableExport.prototype.oldExportAction = function (self, e, dt, button, config) {
	if (button[0].className.indexOf('buttons-excel') >= 0) {
		if ($.fn.dataTable.ext.buttons.excelHtml5.available(dt, config)) {
			$.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config);
		} else {
			$.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
		}
	} else if (button[0].className.indexOf('buttons-print') >= 0) {
		$.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
	} else if (button[0].className.indexOf('buttons-csv') >= 0) {
		$.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config);
	} else if (button[0].className.indexOf('buttons-pdf') >= 0) {
		$.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config);
	}
};

/**
 * Override default export 
 * @param {any} e
 * @param {any} dt
 * @param {any} button
 * @param {any} config
 */
TableExport.prototype.newExportAction = function (e, dt, button, config) {
	var self = this;
	var oldStart = dt.settings()[0]._iDisplayStart;

	dt.one('preXhr',
		function (e, s, data) {
			// Just this once, load all data from the server...
			data.start = 0;
			data.length = 2147483647;

			dt.one('preDraw',
				function (e, settings) {
					// Call the original action function
					new TableExport().oldExportAction(self, e, dt, button, config);

					dt.one('preXhr',
						function (e, s, data) {
							// DataTables thinks the first item displayed is index 0, but we're not drawing that.
							// Set the property to what it was before exporting.
							settings._iDisplayStart = oldStart;
							data.start = oldStart;
						});

					// Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
					setTimeout(dt.ajax.reload, 0);

					// Prevent rendering of the full data to the DOM
					return false;
				});
		});

	// Requery the server with the new one-time export settings
	dt.ajax.reload();
}

//------------------------------------------------------------------------------------//
//								Table Builder
//------------------------------------------------------------------------------------//

function TableBuilder() { };

TableBuilder.constructor = TableBuilder;

TableBuilder.prototype.restoreItem = function (rowId, tableId, viewModelId) {
	swal({
		title: window.translate("restore_alert"),
		text: "",
		type: "warning",
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: window.translate("confirm_query_restore"),
		cancelButtonText: window.translate("cancel")
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: "/PageRender/RestoreItemFromDynamicEntity",
				type: "post",
				data: {
					id: rowId,
					viewModelId: viewModelId
				},
				success: function (data) {
					if (data.success) {
						const oTable = $(`${tableId}`).DataTable();
						oTable.draw();
						swal(window.translate("restored_complete"), "", "success");
					} else {
						swal(window.translate("restore_fail"), data.message, "error");
					}
				},
				error: function () {
					swal(window.translate("restore_fail"), window.translate("api_not_respond"), "error");
				}
			});
		}
	});
}

TableBuilder.prototype.deleteItem = function (rowId, tableId, viewModelId) {
	swal({
		title: window.translate("delete_query_item"),
		text: "",
		type: "warning",
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: window.translate("delete_confirm_query"),
		cancelButtonText: window.translate("cancel")
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: "/PageRender/DeleteItemFromDynamicEntity",
				type: "post",
				data: {
					id: rowId,
					viewModelId: viewModelId
				},
				success: function (data) {
					if (data.success) {
						const oTable = $(`${tableId}`).DataTable();
						oTable.draw();
						swal(window.translate("row_deleted"), "", "success");
					} else {
						swal(window.translate("delete_fail"), data.message, "error");
					}
				},
				error: function () {
					swal(window.translate("delete_fail"), window.translate("api_not_respond"), "error");
				}
			});
		}
	});
};

/**
 * Render table cell
 * @param {any} column
 */
TableBuilder.prototype.renderCell = function (row, column) {
	try {
		return eval(column.template);
	}
	catch (e) {
		return "";
	}
}

/**
 * Get action buttons
 * @param {any} row
 * @param {any} viewmodelData
 * @param {any} hasEditPage
 * @param {any} hasInlineEdit
 * @param {any} editPageLink
 */
TableBuilder.prototype.getRenderRowActions = function (row, dataX) {
	const container = this.getTableRowInlineActionButton(row, dataX)
		//+ this.getTableRowEditActionButton(row, dataX)
		+ this.getTableRowDeleteRestoreActionButton(row, dataX);
	return container;
}

TableBuilder.prototype.getTableRowDeleteRestoreActionButton = function (row, dataX) {
	return `${dataX.hasDeleteRestore
		? `${row.isDeleted
			? `<a href="javascript:void(0)" class='btn restore-item btn-warning btn-sm' onclick="new TableBuilder().restoreItem('${row.id
			}', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')">${window.translate("restore")}</a>`
			: `<a href="javascript:void(0)" class='btn btn-danger btn-sm' onclick="new TableBuilder().deleteItem('${row.id
			}', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')">${window.translate("delete")}</a>`}`
		: ""}`;
};


TableBuilder.prototype.getTableRowInlineActionButton = function (row, dataX) {
	if (row.isDeleted) return "";
	return `${dataX.hasInlineEdit
		? `	<a data-viewmodel="${dataX.viewmodelData.result.id
		}" class="inline-edit btn btn-warning btn-sm" href="javascript:void(0)">${window.translate("inline_edit")}</a>`
		: ``}`;
};


TableBuilder.prototype.getTableRowEditActionButton = function (row, dataX) {
	if (row.isDeleted) return "";
	return `${dataX.hasEditPage ? `<a class="btn btn-info btn-sm" href="${dataX.editPageLink}?itemId=${row.id
		}&&listId=${dataX.viewmodelData.result.id}">${window.translate("edit")}</a>`
		: ``}`;
};


TableBuilder.prototype.deleteSelectedRows = function () {
	window.TBuilder.deleteSelectedRowsHandler(this);
}

TableBuilder.prototype.deleteSelectedRowsHandler = function (ctx) {
	const tableId = ctx.table().node().id;
	if (!tableId) {
		$.toast({
			heading: "Something did not work",
			text: "",
			position: 'top-right',
			loaderBg: '#ff6849',
			icon: 'error',
			hideAfter: 3500,
			stack: 6
		});
		return;
	}
	const selected = ctx.rows({ selected: true }).data();
	const viewModelId = $(`#${tableId}`).attr("db-viewmodel");
	if (selected.length > 0) {
		const ids = selected.map(x => {
			return x.id;
		});
		const data = [];
		for (let i = 0; i < ids.length; i++) {
			data.push(ids[i]);
		}
		const req = load("/PageRender/DeleteItemsFromDynamicEntity",
			{
				ids: data,
				viewModelId: viewModelId
			},
			"post");
		if (req && req.success) {
			ctx.ajax.reload();
			$.toast({
				heading: window.translate("items_deleted"),
				text: ``,
				position: 'top-right',
				loaderBg: '#ff6849',
				icon: 'success',
				hideAfter: 3500,
				stack: 6
			});
		} else {
			$.toast({
				heading: window.translate("fail_delete_items"),
				text: "",
				position: 'top-right',
				loaderBg: '#ff6849',
				icon: 'error',
				hideAfter: 3500,
				stack: 6
			});
		}
	} else {
		$.toast({
			heading: window.translate("delete_no_selected_items"),
			text: "",
			position: 'top-right',
			loaderBg: '#ff6849',
			icon: 'warning',
			hideAfter: 3500,
			stack: 6
		});
	}
};

TableBuilder.prototype.buttons = [
	{
		extend: 'copyHtml5',
		text: '<i class="fa fa-files-o"></i>',
		exportOptions: {
			columns: ':visible'
		},
		className: ""
	},
	{
		extend: 'csvHtml5',
		text: '<i class="fa fa-file-text-o"></i>',
		exportOptions: {
			columns: ':visible'
		},
		action: new TableExport().newExportAction
	},
	{
		extend: 'excelHtml5',
		text: '<i class="fa fa-file-excel-o"></i>',
		autoFilter: true,
		exportOptions: {
			columns: ':visible',
			search: 'applied',
			order: 'applied'
		},
		action: new TableExport().newExportAction
	},
	{
		extend: 'pdfHtml5',
		text: '<i class="fa fa-file-pdf-o"></i>',
		exportOptions: {
			columns: ':visible'
		},
		action: new TableExport().newExportAction
	},
	{
		extend: 'print',
		exportOptions: {
			columns: ':visible'
		},
		action: new TableExport().newExportAction
	},
	{
		text: 'Delete selected items',
		action: new TableBuilder().deleteSelectedRows
	}
];

TableBuilder.prototype.createdCell = function (td, cellData, rowData, row, col) {
	//on created cell
};

TableBuilder.prototype.rowCallback = function (row, data) {
	//on callback
};

TableBuilder.prototype.createdRow = function (row, data, dataIndex) {
	if (data.isDeleted) {
		$(row).addClass("row-deleted");
		$(row).find("td.select-checkbox").find("input").css("display", "none");
		$(row).find("td").addClass("not-selectable");
	}
	// Add COLSPAN attribute
	//	$('td:eq(1)', row).attr('colspan', 5);

	//	// Hide required number of columns
	//	// next to the cell with COLSPAN attribute
	//	$('td:eq(2)', row).css('display', 'none');
	//	$('td:eq(3)', row).css('display', 'none');
	//	$('td:eq(4)', row).css('display', 'none');
	//	$('td:eq(5)', row).css('display', 'none');
};


TableBuilder.prototype.dom = '<"CustomizeColumns">lBfrtip';

TableBuilder.prototype.replaceTableSystemTranslations = function () {
	const customReplace = new Array();
	//customReplace.push({ Key: "propName", Value: "value" });
	const searialData = JSON.stringify(customReplace);
	return searialData;
};

TableBuilder.prototype.translationsJson = function () {
	return `${location.origin}/api/LocalizationApi/GetJQueryTableTranslations?language=${window.getCookie("language")}&customReplace=${this.replaceTableSystemTranslations()}`;
};

TableBuilder.prototype.ajax = {
	"url": "/PageRender/LoadPagedData",
	"type": "POST",
	"data": {}
};

TableBuilder.prototype.renderTable = function (data) {
	const ctx = this;
	const ajax = ctx.ajax;
	ajax.data = {
		"viewModelId": data.viewmodelId
	};

	const tableId = `#${data.listId}`;
	if ($.fn.DataTable.isDataTable(tableId)) {
		$(tableId).dataTable().fnDestroy();
		$(tableId).dataTable().empty();
	}
	const renderTableSelect = new RenderTableSelect();
	$(tableId).DataTable({
		"language": {
			"url": ctx.translationsJson(),
		},
		dom: this.dom,
		buttons: this.buttons,
		columnDefs: [
			{
				orderable: false,
				className: renderTableSelect.className(),
				targets: 'no-sort'
			}
		],
		colReorder: true,
		select: renderTableSelect.settings.select,
		"scrollX": true,
		"scrollCollapse": true,
		"autoWidth": true,
		"processing": true,
		"serverSide": true,
		"filter": true,
		"orderMulti": false,
		"destroy": true,
		"ajax": ajax,
		"columns": data.renderColumns,
		"createdRow": (row, data, dataIndex) => ctx.createdRow(row, data, dataIndex),
		"rowCallback": (row, data) => ctx.rowCallback(row, data),
		"createdCell": (td, cellData, rowData, row, col) => ctx.createdCell(td, cellData, rowData, row, col)
	});
}

TableBuilder.prototype.appendColumnsBeforeActions = function () {
	return "";
};


TableBuilder.prototype.configureTableBody = function (dataX) {
	const ctx = this;
	if (dataX.viewmodelData.is_success) {
		const renderColumns = [];
		if (dataX.viewmodelData.result.viewModelFields.length > 0) {
			const renderTableSelect = new RenderTableSelect();
			const columns = $(`#${dataX.listId} thead`);
			columns.html(null);
			const tr = document.createElement("tr");
			const th = document.createElement("th");
			th.setAttribute("class", "no-sort");
			th.innerHTML = renderTableSelect.settings.headContent;
			tr.appendChild(th);
			//CheckBox column
			renderColumns.push({
				data: null,
				"render": function (data, type, row, meta) {
					return renderTableSelect.templateSelect(data, type, row, meta);
				}
			});

			$.each(dataX.viewmodelData.result.viewModelFields,
				function (index, column) {
					let colName = column.name;
					if (column.translate) {
						colName = window.translate(column.translate);
					}
					const htmlCol = document.createElement("th");
					htmlCol.innerHTML = colName;
					tr.appendChild(htmlCol);
					renderColumns.push({
						config: {
							column: column
						},
						data: null,
						"render": function (data, type, row, meta) {
							const elDiv = document.createElement("div");
							elDiv.setAttribute("class", "data-cell hasTooltip");
							elDiv.setAttribute("data-prop-name", (column.tableModelFields) ? column.tableModelFields.name : "");
							elDiv.setAttribute("data-viewmodel", dataX.viewmodelId);
							elDiv.setAttribute("data-id", row.id);
							elDiv.setAttribute("data-column-id", column.id);
							elDiv.innerHTML = ctx.renderCell(row, column);
							return elDiv.outerHTML;
						}
					});
				});
			//const htmlCol = document.createElement("th");
			//htmlCol.innerHTML = ctx.appendColumnsBeforeActions();
			//tr.appendChild(htmlCol);

			const actionCol = document.createElement("th");
			actionCol.innerHTML = window.translate("list_actions");
			tr.appendChild(actionCol);
			columns.html(tr.outerHTML);
			renderColumns.push({
				data: null,
				"render": function (data, type, row, meta) {
					const elDiv = document.createElement("div");
					elDiv.setAttribute("class", "btn-group");
					elDiv.setAttribute("role", "group");
					elDiv.setAttribute("aria-label", "Action buttons");
					elDiv.innerHTML = ctx.getRenderRowActions(row, dataX);
					return elDiv.outerHTML;
				}
			});
		}
		ctx.renderTable({
			viewmodelId: dataX.viewmodelId,
			listId: dataX.listId,
			renderColumns: renderColumns
		});
	};
};

$(document).ready(function () {
	const tablePromise = new Promise((resolve, reject) => {
		const tables = Array.prototype.filter.call(
			document.getElementsByTagName('table'),
			function (el) {
				return el.getAttribute("db-viewmodel") != null;
			}
		);
		resolve(tables);
	});

	tablePromise.then((tables => {
		$.each(tables,
			function (index, table) {
				const listRef = $(table);
				const viewmodelId = listRef.attr("db-viewmodel");
				const listId = listRef.attr("id");
				const hasEditPage = table.hasAttribute("data-is-editable");
				const hasInlineEdit = table.hasAttribute("data-is-editable-inline");
				const hasDeleteRestore = table.hasAttribute("data-allow-edit-restore");
				const editPageLink = listRef.attr("data-edit-href");

				var viewModelPromise = new Promise((resolve, reject) => {
					const viewmodelData = load(`/PageRender/GetViewModelById?viewModelId=${viewmodelId}`);
					resolve({
						viewmodelData: viewmodelData,
						hasEditPage: hasEditPage,
						hasInlineEdit: hasInlineEdit,
						hasDeleteRestore: hasDeleteRestore,
						editPageLink: editPageLink,
						listId: listId,
						viewmodelId: viewmodelId
					});
				});

				viewModelPromise.then(dataX => {
					new TableBuilder().configureTableBody(dataX);
				});
			});
	}));
});