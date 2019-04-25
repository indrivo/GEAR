/* Table inline edit
 * A plugin for inline edit
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Inline edit plugin require JQuery');
}

$(".dynamic-table")
	.on("draw.dt", function (e, settings, json) {
		$(".inline-edit").on("click", inlineEdit);
	});


/**
 * Start inline edit for row
 */
function inlineEdit() {
	const context = $(this);
	context.html("Complete");
	context.removeClass("inline-edit");
	context.addClass("inline-complete");
	context.removeClass("btn-warning").addClass("btn-success");
	context.off("click", inlineEdit);

	const viewModelId = $(this).attr("data-viewmodel");
	const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);

	const columns = $(this).parent().parent().parent().find(".data-cell");
	const table = $(this).closest("table").DataTable();
	const row = $(this).closest("tr");
	const index = table.row(row).index();
	var obj = table.row(index).data();
	for (let i = 0; i < columns.length; i++) {
		const columnId = $(columns[i]).attr("data-column-id");
		const fieldData = viewModel.result.filter(obj => {
			return obj.columnId === columnId;
		});
		if (fieldData.length > 0) {
			//const viewModelId = $(columns[i]).attr("data-viewmodel");
			const cellId = $(columns[i]).attr("data-id");
			const tableId = fieldData[0].tableId;
			const propId = fieldData[0].id;
			const propName = fieldData[0].name;
			const parsedPropName = propName[0].toLowerCase() + propName.substr(1, propName.length);
			const value = obj[parsedPropName];
			let container = value;
			switch (fieldData[0].dataType) {
				case "nvarchar":
					{
						const el = document.createElement("input");
						el.setAttribute("class", "inline-update-event data-input form-control");
						el.setAttribute("data-prop-id", propId);
						el.setAttribute("data-id", cellId);
						el.setAttribute("type", "text");
						el.setAttribute("data-entity", tableId);
						el.setAttribute("data-type", "nvarchar");
						el.setAttribute("value", value);

						container = el;
						$(columns[i]).html(container);
						$(columns[i]).find(".inline-update-event").on("blur", onInputEvent);
					}
					break;
				case "int32":
					{
						const el = document.createElement("input");
						el.setAttribute("class", "inline-update-event data-input form-control");
						el.setAttribute("data-prop-id", propId);
						el.setAttribute("data-id", cellId);
						el.setAttribute("type", "number");
						el.setAttribute("data-entity", tableId);
						el.setAttribute("data-type", "int32");
						el.setAttribute("value", value);
						container = el;
						$(columns[i]).html(container);
						$(columns[i]).find(".inline-update-event").on("blur", onInputEvent);
					}
					break;
				case "bool":
					{
						const div = document.createElement("div");
						div.setAttribute("class", "checkbox checkbox-success");
						div.style.marginTop = "-1em";
						div.style.marginLeft = "2em";
						const label = document.createElement("label");
						label.setAttribute("for", "test");
						const el = document.createElement("input");
						el.setAttribute("class", "inline-update-event data-input");
						el.setAttribute("data-prop-id", propId);
						el.setAttribute("data-id", cellId);
						el.setAttribute("type", "checkbox");
						el.setAttribute("data-entity", tableId);
						el.setAttribute("data-type", "bool");
						el.setAttribute("id", "test");
						el.setAttribute("name", "test");
						el.style.maxWidth = "1em";
						if (value) {
							el.setAttribute("checked", "checked");
						}

						div.appendChild(el);
						div.appendChild(label);
						container = div;
						$(columns[i]).html(container);
						$(columns[i]).find(".inline-update-event").on("change", onInputEvent);
					}
					break;
				case "datetime":
				case "date":
					{
						const el = document.createElement("input");
						el.setAttribute("class", "inline-update-event data-input form-control");
						el.setAttribute("data-prop-id", propId);
						el.setAttribute("data-id", cellId);
						el.setAttribute("type", "text");
						el.setAttribute("data-entity", tableId);
						el.setAttribute("data-type", "datetime");
						el.setAttribute("value", value);
						container = el;
						$(columns[i]).html(container);
						$(columns[i]).find(".inline-update-event")
							.on("change", onInputEvent)
							.datepicker({
								format: 'dd/mm/yyyy'
							}).addClass("datepicker");
					}
					break;
				case "uniqueidentifier":
					{
						const div = document.createElement("input-group mb-3");
						const dropdown = document.createElement("select");
						dropdown.setAttribute("class", "inline-update-event data-input form-control");
						dropdown.setAttribute("data-prop-id", propId);
						dropdown.setAttribute("data-id", cellId);
						dropdown.setAttribute("data-entity", tableId);
						dropdown.setAttribute("data-type", "uniqueidentifier");
						dropdown.style.width = "80%";
						dropdown.options[dropdown.options.length] = new Option(window.translate("no_value_selected"), '');
						//Populate dropdown
						const data = load(`/PageRender/GetRowReferences?entityId=${tableId}&propertyId=${propId}`);
						if (data) {
							if (data.is_success) {
								$.each(data.result.data, function (index, obj) {
									dropdown.options[dropdown.options.length] = new Option(obj.Name, obj.Id);
								});
								dropdown.setAttribute("data-ref-entity", data.result.entityName);
							}
							dropdown.value = value;
						}
						div.appendChild(dropdown);
						const addOption = document.createElement("a");
						addOption.setAttribute("class", "btn btn-success");
						addOption.innerHTML = "<i class='fa fa-plus'></i>";
						div.appendChild(addOption);
						container = div;
						$(columns[i]).html(container);
						$(columns[i]).find(".inline-update-event").on("change", onInputEvent);
					}
					break;
			}
		}
	}
	$(this).on("click", completeEditInline);
}

/**
 * Complete inline edit
 */
function completeEditInline() {
	const service = new DataInjector();
	const table = $(this).closest("table").DataTable();
	var row = $(this).closest("tr");
	const index = table.row(row).index();
	var obj = table.row(index).data();
	$(this).off("click", completeEditInline);
	const columns = $(this).parent().parent().parent().find(".data-cell");

	const viewModelId = $(columns[0]).attr("data-viewmodel");
	const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);
	if (!viewModelId) return;
	if (!viewModel.is_success) return;

	for (let i = 0; i < columns.length; i++) {
		const inspect = $(columns[i]).find(".data-input");
		if (inspect) {
			const type = inspect.attr("data-type");
			const columnId = inspect.attr("data-prop-id");

			const fieldData = viewModel.result.filter(obj => {
				return obj.id === columnId;
			});

			if (fieldData.length === 0) continue;
			const propName = fieldData[0].name;
			const parsedPropName = propName[0].toLowerCase() + propName.substr(1, propName.length);

			const value = inspect.val();

			switch (type) {
				case "bool": {
					obj[parsedPropName] = inspect.prop("checked");
				} break;
				case "uniqueidentifier": {
					const refEntity = inspect.attr("data-ref-entity");
					const refObject = service.GetById(refEntity, value);
					if (refObject.is_success) {
						var json = JSON.stringify(refObject.result);
						var newJson = json.replace(/"([\w]+)":/g, function ($0, $1) {
							return ('"' + $1.toLowerCase() + '":');
						});
						var newObj = JSON.parse(newJson);
						obj[`${parsedPropName}Reference`] = newObj;
						obj[parsedPropName] = value;
					}

				} break;
				default: {
					obj[parsedPropName] = value;
				} break;
			}
			$(columns[i]).find(".inline-update-event").off("blur", onInputEvent);
			$(columns[i]).find(".inline-update-event").off("changed", onInputEvent);

		}
	}

	const redraw = table.row(index).data(obj).invalidate();

	$(redraw.row(index).nodes()).find(".inline-edit").on("click", inlineEdit);
}


/**
 * On change event
 */
function onInputEvent() {
	const rowId = $(this).attr("data-id");
	const entityId = $(this).attr("data-entity");
	const propId = $(this).attr("data-prop-id");
	const type = $(this).attr("data-type");
	let value = "";
	switch (type) {
		case "bool":
			{
				value = $(this).prop("checked");
			} break;
		default: {
			value = $(this).val();
		} break;
	}

	const req = load(`/PageRender/SaveTableCellData`,
		{
			entityId: entityId,
			propertyId: propId,
			rowId: rowId,
			value: value
		}, "post");
	if (req.is_success) {
		$.toast({
			heading: 'Data was saved with success',
			text: `You change ${value} value`,
			position: 'bottom-right',
			loaderBg: '#ff6849',
			icon: 'success',
			hideAfter: 3500,
			stack: 6
		});
	} else {
		$.toast({
			heading: req.error_keys[0].message,
			text: "",
			position: 'bottom-right',
			loaderBg: '#ff6849',
			icon: 'error',
			hideAfter: 3500,
			stack: 6
		});
	}
}