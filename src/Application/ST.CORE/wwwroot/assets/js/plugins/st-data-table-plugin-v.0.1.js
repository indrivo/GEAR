/* Data table render
 * A plugin for render tables in dynamic pages
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Data Table plugin require JQuery');
}

function restoreItem(rowId, tableId, viewModelId) {
	const object = {
		alertTitle: window.translate("restore_alert"),
		alertText: "",
		confirmButtonText: "Yes, restore it!",
		rowId: rowId,
		tableId: tableId,
		urlForDelete: "/PageRender/RestoreItemFromDynamicEntity",
		type: "warning",
		onDeleteSuccess: "Item has been restored.",
		onDeleteFail: "Something wrong",
		onServerNoResponse: "Api not respond or not have permissions."
	};

	swal({
		title: object.alertTitle,
		text: object.alertText,
		type: object.type,
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: object.confirmButtonText
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: `${object.urlForDelete}`,
				type: "post",
				data: {
					id: object.rowId,
					viewModelId: viewModelId
				},
				success: function (data) {
					if (data.success) {
						const oTable = $(`${object.tableId}`).DataTable();
						oTable.draw();
						swal("Restored!", object.message, "success");
					} else {
						swal("Fail!", data.message, "error");
					}
				},
				error: function () {
					swal("Fail!", object.onServerNoResponse, "error");
				}
			});
		}
	});
}

function deleteItem(rowId, tableId, viewModelId) {
	const object = {
		alertTitle: "Delete this item?",
		alertText: "Are you sure that you want to leave this item?",
		confirmButtonText: "Yes, delete it!",
		rowId: rowId,
		tableId: tableId,
		urlForDelete: "/PageRender/DeleteItemFromDynamicEntity",
		type: "warning",
		onDeleteSuccess: "Item has been deleted.",
		onDeleteFail: "Something wrong",
		onServerNoResponse: "Api not respond or not have permissions."
	};

	swal({
		title: object.alertText,
		text: object.alertText,
		type: object.type,
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: object.confirmButtonText
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: `${object.urlForDelete}`,
				type: "post",
				data: {
					id: object.rowId,
					viewModelId: viewModelId
				},
				success: function (data) {
					if (data.success) {
						const oTable = $(`${object.tableId}`).DataTable();
						oTable.draw();
						swal("Deleted!", object.message, "success");
					} else {
						swal("Fail!", data.message, "error");
					}
				},
				error: function () {
					swal("Fail!", object.onServerNoResponse, "error");
				}
			});
		}
	});
}

var tables = Array.prototype.filter.call(
	document.getElementsByTagName('table'),
	function (el) {
		return el.getAttribute("db-viewmodel") != null;
	}
);

const form = new Form();

/**
 * Render table cell
 * @param {any} column
 */
function renderCell(row, column) {
	try {
		return eval(column.template);
	}
	catch (e) {
		return "";
	}
}

$.each(tables,
	function (index, table) {
		const listRef = $(table);
		const viewmodelId = listRef.attr("db-viewmodel");
		const listId = listRef.attr("id");
		const hasEditPage = table.hasAttribute("data-is-editable");
		const hasInlineEdit = table.hasAttribute("data-is-editable-inline");
		const hasDeleteRestore = table.hasAttribute("data-allow-edit-restore");
		const editPageLink = listRef.attr("data-edit-href");
		const tableId = `#${listId}`;
		if ($.fn.DataTable.isDataTable(tableId)) {
			$(tableId).dataTable().fnDestroy();
			$(tableId).dataTable().empty();
		}

		const viewmodelData = load(`/PageRender/GetViewModelById?viewModelId=${viewmodelId}`);
		const renderColumns = [];
		if (viewmodelData.is_success) {
			if (viewmodelData.result.viewModelFields.length > 0) {
				const columns = $(`#${listId} thead tr`);
				columns.html(null);
				let rows = "<th>#</th>";
				//CheckBox column
				renderColumns.push({
					data: null,
					"render": function (data, type, row, meta) {
						return "";
					}
				});

				$.each(viewmodelData.result.viewModelFields,
					function (index, column) {
						rows += `<th translate='${column.translate}'>${column.name}</th>`;
						renderColumns.push({
							data: null,
							"render": function (data, type, row, meta) {
								return `<div class="data-cell" data-viewmodel="${viewmodelId}" data-id="${row.id
									}" data-column-id="${column.id}">${renderCell(row, column)}</div>`;
							}
						});
					});
				rows += `<th>${window.translate("list_actions")}</th>`;
				columns.html(rows);
				renderColumns.push({
					data: null,
					"render": function (data, type, row, meta) {
						return `<div class="btn-group" role="group" aria-label="Action buttons">
										${getRenderRowActions(row, viewmodelData, hasEditPage, hasInlineEdit, editPageLink)}
										${hasDeleteRestore
								? `${row.isDeleted
									? `<a href="#" class='btn restore-item btn-warning btn-sm' onclick="restoreItem('${row.id
									}', '#${listId}', '${viewmodelData.result.id}')">Restore</a>`
									: `<a href="#" class='btn btn-danger btn-sm' onclick="deleteItem('${row.id
									}', '#${listId}', '${viewmodelData.result.id}')">Delete</a>`}`
								: ``}
										</div>`;
					}
				});
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
		function getRenderRowActions(row, viewmodelData, hasEditPage, hasInlineEdit, editPageLink) {
			if (row.isDeleted) return "";
			return `${hasInlineEdit
				? `	<a data-viewmodel="${viewmodelData.result.id
				}" class="inline-edit btn btn-warning btn-sm" href="#">Edit inline</a>`
				: ``}

										${hasEditPage
					? `<a class="btn btn-info btn-sm" href="${editPageLink}?itemId=${row.id
					}&&listId=${viewmodelData.result.id}">Edit</a>`
					: ``}`;
		}


		function deleteSelectedRows() {
			const selected = this.rows({ selected: true }).data();
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
						viewModelId: viewmodelData.result.id
					},
					"post");
				if (req && req.success) {
					this.ajax.reload();
					$.toast({
						heading: 'Items deleted',
						text: ``,
						position: 'bottom-right',
						loaderBg: '#ff6849',
						icon: 'success',
						hideAfter: 3500,
						stack: 6
					});
				} else {
					$.toast({
						heading: "Fail delete items",
						text: "",
						position: 'bottom-right',
						loaderBg: '#ff6849',
						icon: 'error',
						hideAfter: 3500,
						stack: 6
					});
				}
			}
		}

		var oldExportAction = function (self, e, dt, button, config) {
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

		var newExportAction = function (e, dt, button, config) {
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
							oldExportAction(self, e, dt, button, config);

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
		};

		$(tableId).DataTable({
			"language": {
				"url": `http://cdn.datatables.net/plug-ins/1.10.19/i18n/${window.getCookie("language")}.json`
			},
			//rowsGroup: [
			//	0
			//],
			//'createdRow': function(row, data, dataIndex){
			//	// Add COLSPAN attribute
			//	$('td:eq(1)', row).attr('colspan', 5);

			//	// Hide required number of columns
			//	// next to the cell with COLSPAN attribute
			//	$('td:eq(2)', row).css('display', 'none');
			//	$('td:eq(3)', row).css('display', 'none');
			//	$('td:eq(4)', row).css('display', 'none');
			//	$('td:eq(5)', row).css('display', 'none');
			//},
			dom: '<"CustomizeColumns">lBfrtip',
			buttons: [
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
					action: newExportAction
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
					action: newExportAction
				},
				{
					extend: 'pdfHtml5',
					text: '<i class="fa fa-file-pdf-o"></i>',
					exportOptions: {
						columns: ':visible'
					},
					action: newExportAction
				},
				{
					extend: 'print',
					exportOptions: {
						columns: ':visible'
					},
					action: newExportAction
				},
				{
					text: 'Delete selected items',
					action: deleteSelectedRows
				}
			],
			columnDefs: [
				{
					orderable: false,
					className: 'select-checkbox',
					targets: 0
				}
			],
			select: {
				style: 'multi',
				selector: 'td:not(.not-selectable):first-child',
				blurable: true
			},
			"scrollX": true,
			"processing": true, // for show progress bar
			"serverSide": true, // for process server side
			"filter": true, // this is for disable filter (search box)
			"orderMulti": false, // for disable multiple column at once
			"destroy": true,
			"ajax": {
				"url": "/PageRender/LoadPagedData",
				"type": "POST",
				"data": {
					"viewModelId": viewmodelId
				}
			},
			"columns": renderColumns,
			"createdRow": function (row, data, dataIndex) {
				if (data.isDeleted) {
					$(row).addClass("row-deleted");
					$(row).find("td.select-checkbox").find("input").css("display", "none");
					$(row).find("td").addClass("not-selectable");
				}
			},
			"rowCallback": function (row, data) {
				//on callback
			},
			"createdCell": function (td, cellData, rowData, row, col) {
				//on created cell
			}
		});
	});