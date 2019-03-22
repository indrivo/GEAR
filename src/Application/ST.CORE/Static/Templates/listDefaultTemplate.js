$(document).ready(function ($) {
	const tableId = "#render_#ListId";
	if ($.fn.DataTable.isDataTable(tableId)) {
		$(tableId).dataTable().fnDestroy();
		$(tableId).dataTable().empty();
	}

	const viewmodelData = load("/PageRender/GetViewModelById?viewModelId=#ViewModelId");
	const renderColumns = [];
	if (viewmodelData.is_success) {
		if (viewmodelData.result.viewModelFields.length > 0) {
			const columns = $("#render_#ListId thead tr");
			columns.html(null);
			let rows = "";
			$.each(viewmodelData.result.viewModelFields, function (index, column) {
				rows += `<th translate='${column.translate}'>${column.name}</th>`;
				renderColumns.push({
					data: null,
					"render": function (data, type, row, meta) {
						return `<div class="data-cell" data-viewmodel="#ViewModelId" data-id="${row.id}" data-column-id="${column.id}">${eval(column.template)}</div>`;
					}
				});
			});
			rows += "<th>Actions</th>";
			columns.html(rows);
			renderColumns.push({
				data: null,
				"render": function (data, type, row, meta) {
					return `<div class="btn-group" role="group" aria-label="Action buttons">
									<a data-viewmodel="${viewmodelData.result.id}" class="inline-edit btn btn-warning btn-sm" href="#">Edit inline</a>
									<a class="btn btn-info btn-sm" href="${location.href}?entityId=${row.id}">Edit</a>
									${row.isDeleted ? `<a href="#" class='btn btn-danger btn-sm' onclick=restoreItem('${row.id}'); >Restore</a>` :
							`<a href="#" class='btn btn-danger btn-sm' onclick=deleteItem('${row.id}'); >Delete</a>`}
									</div>`;
				}
			});
		}
	}

	$(tableId).DataTable({
		"language": {
			"url": `http://cdn.datatables.net/plug-ins/1.10.19/i18n/${window.getCookie("language")}.json`
		},
		dom: '<"CustomizeColumns">lBfrtip',
		"processing": true, // for show progress bar
		"serverSide": true, // for process server side
		"filter": true, // this is for disable filter (search box)
		"orderMulti": false, // for disable multiple column at once
		"destroy": true,
		"ajax": {
			"url": "/PageRender/LoadPagedData",
			"type": "POST",
			"data": {
				"viewModelId": "#ViewModelId",
				"tableId": "#EntityId"
			}
		},
		"columns": renderColumns
	});
});

function restoreItem(rowId) {
	const object = {
		alertTitle: "Restore this #EntityName?",
		alertText: "Are you sure that you want to restore this #EntityName?",
		confirmButtonText: "Yes, restore it!",
		rowId: rowId,
		tableId: "#render_#ListId",
		urlForDelete: "/PageRender/RestoreItemFromDynamicEntity",
		type: "warning",
		onDeleteSuccess: "#EntityName has been restored.",
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
					entityId: "#EntityId"
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

function deleteItem(rowId) {
	const object = {
		alertTitle: "Delete this #EntityName?",
		alertText: "Are you sure that you want to leave this #EntityName?",
		confirmButtonText: "Yes, delete it!",
		rowId: rowId,
		tableId: "#render_#ListId",
		urlForDelete: "/PageRender/DeleteItemFromDynamicEntity",
		type: "warning",
		onDeleteSuccess: "#EntityName has been deleted.",
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
					entityId: "#EntityId"
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