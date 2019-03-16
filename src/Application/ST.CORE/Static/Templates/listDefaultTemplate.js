$(document).ready(function ($) {
	const tableId = '.render_#EntityId';
	if ($.fn.DataTable.isDataTable(tableId)) {
		$(tableId).dataTable().fnDestroy();
		$(tableId).dataTable().empty();
	}

	const viewmodelData = load("/PageRender/GetViewModelById?viewModelId=#ViewModelId")
	const renderColumns = [];
	if (viewmodelData.is_success) {
		if (viewmodelData.result.viewModelFields.length > 0) {
			const columns = $(".render_#EntityId thead tr");
			columns.html(null);
			let rows = "";
			$.each(viewmodelData.result.viewModelFields, function (index, column) {
				rows += `<th translate='${column.translate}'>${column.name}</th>`;
				renderColumns.push({
					data: null,
					"render": function (data, type, row, meta) {
						return `${eval(column.template)}`;
					}
				});
			});
			rows += "<th>Actions</th>";
			columns.html(rows);
			renderColumns.push({
				data: null,
				"render": function (data, type, row, meta) {
					return `<div class="btn-group" role="group" aria-label="Action buttons">
									<a class="btn btn-info btn-sm" href="@Url.Action("Edit")?id=${row.id}">Edit</a>
									<button type="button" class='btn btn-danger btn-sm' onclick=createAlert('${row.id
						}'); >Delete</button>
									</div>`;
				}
			});
		}
	}

	$(tableId).DataTable({
		dom: '<"CustomizeColumns">lBfrtip',
		"processing": true, // for show progress bar
		"serverSide": true, // for process server side
		"filter": true, // this is for disable filter (search box)
		"orderMulti": false, // for disable multiple column at once
		"destroy": true,
		"ajax": {
			"url": '/PageRender/LoadPagedData',
			"type": "POST",
			"data": {
				"viewModelId": "#ViewModelId",
				"tableId": "#EntityId"
			}
		},
		"columns": renderColumns
	});
});

function createAlert(rowId) {
	const object = {
		alertTitle: "Delete this #EntityName?",
		alertText: "Are you sure that you want to leave this #EntityName?",
		confirmButtonText: "Yes, delete it!",
		rowId: rowId,
		tableId: ".render_#EntityId",
		urlForDelete: '/PageRender/DeleteItemFromDynamicEntity',
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