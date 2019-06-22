/* Table inline edit
 * A plugin for inline edit on dynamic tables 
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === "undefined") {
	throw new Error("Inline edit plugin require JQuery");
}

function TableInlineEdit() {

};

/**
 * Constructor
 */
TableInlineEdit.prototype.constructor = TableInlineEdit;

/**
 * Default container on unknown column type 
 */
TableInlineEdit.prototype.defaultNotEditFieldContainer = "-";

/**
 * Get actions on add
 */
TableInlineEdit.prototype.getActionsOnAdd = function () {
	const template = `<div class="btn-group" role="group" aria-label="Action buttons">
						<a href="javascript:void(0)" class='btn add-new-item btn-success btn-sm'><i class="fa fa-check"></i></a>
						<a ref="javascript:void(0)" class='btn cancel-new-item btn-danger btn-sm'><i class="fa fa-close"></i></a>
					  </div>`;
	return template;
};


TableInlineEdit.prototype.addNewHandler = function (ctx, jdt = null) {
	const card = $(ctx).closest(".card");
	const dto = card.find(".dynamic-table");
	if (!jdt) {
		jdt = dto.DataTable();
	}
	//if (dto.attr("add-mode") === "true") {
	//	return this.displayNotification({ heading: window.translate("system_inline_edit_add_fail") });
	//}
	const row = document.createElement("tr");
	row.setAttribute("isNew", "true");
	const columns = jdt.columns().context[0].aoColumns;
	for (let i in columns) {
		//Ignore hidden column
		if (!columns[i].bVisible) continue;
		let cell = document.createElement("td");
		if (columns[i].targets === "no-sort") {
			cell.innerHTML = this.defaultNotEditFieldContainer;
		} else
			if (columns[i].sTitle === window.translate("list_actions")) {
				cell.innerHTML = this.getActionsOnAdd();
				$(cell).find(".cancel-new-item").on("click", cancelNewItem);
				$(cell).find(".add-new-item").on("click", addNewItem);
			}
			else {
				const newCell = this.getAddRowCell(columns[i], cell);
				cell = newCell.cell;
				if (newCell.entityName)
					row.setAttribute("entityName", newCell.entityName);
			}

		row.appendChild(cell);
	}
	dto.attr("add-mode", "true");
	$("tbody", dto).prepend(row);
	new TableInlineEdit().bindEventsAfterInitInlineEdit();
	return this.toggleVisibilityColumnsButton(ctx, true);
};


/**
 * Toggle button disable state
 * @param {*} context 
 * @param {*} state 
 */
TableInlineEdit.prototype.toggleVisibilityColumnsButton = function (ctx, state) {
	if (state) {
		ctx.closest(".card")
			.find(".CustomizeColumns")
			.find(".toggle-columns")
			.addClass("disabled");
	} else {
		ctx.closest(".card")
			.find(".CustomizeColumns")
			.find(".toggle-columns")
			.removeClass("disabled");
	}
}


/**
 * Cancel add new item
 */
function cancelNewItem() {
	const context = $(this);
	context.off("click", cancelNewItem);
	context.parent().find(".add-new-item").off("click", addNewItem);
	new TableInlineEdit().toggleVisibilityColumnsButton(context, false);
	cancelTableAddMode(context);
	context.closest("tr").remove();
}

function addNewItem() {
	const context = $(this);
	const dTable = context.closest("table").DataTable();
	const rowContext = context.closest("tr");
	const entityName = rowContext.attr("entityName");
	const data = getRowData(rowContext);
	const dataContext = new DataInjector();
	const req = dataContext.Add(entityName, data);
	if (req.is_success) {
		$.toast({
			heading: "New row added",
			text: `info`,
			position: "top-right",
			loaderBg: "#ff6849",
			icon: "success",
			hideAfter: 3500,
			stack: 6
		});
		dTable.draw();
		new TableInlineEdit().toggleVisibilityColumnsButton(context, false);
		context.off("click", addNewItem);
		cancelTableAddMode(context);
	}
	else {
		$.toast({
			heading: req.error_keys[0].message,
			text: "",
			position: "top-right",
			loaderBg: "#ff6849",
			icon: "error",
			hideAfter: 3500,
			stack: 6
		});
		new TableInlineEdit().toggleVisibilityColumnsButton(context, false);
	}

	//TODO: Finish add inline edit
}

function cancelTableAddMode(ctx) {
	ctx.closest("table").attr("add-mode", "false");
}

function getRowData(context) {
	const data = $(context).find(".data-new");
	const obj = {};
	for (let i = 0; i < data.length; i++) {
		const f = $(data[i]);
		switch (f.attr("data-type")) {
			case "nvarchar":
			case "datetime":
			case "date":
			case "int32":
			case "decimal": {
				obj[f.attr("data-prop-name")] = f.val();
			} break;
			case "bool": {
				obj[f.attr("data-prop-name")] = f.prop("checked");
			} break;
		}
	}
	return obj;
}


/**
 * Return new cell container for field definition 
 * by data type and references
 * @param {*} column 
 * @param {*} cell 
 */
TableInlineEdit.prototype.getAddRowCell = function (column, cell) {
	if (!column.config.column.tableModelFields) {
		cell.innerHTML = this.defaultNotEditFieldContainer;
		return {
			cell: cell,
			entityName: ""
		};
	}
	$(cell).addClass("expandable-cell");
	const cellContent = document.createElement("div");
	cellContent.setAttribute("class", "data-cell");
	const entityName = column.config.column.tableModelFields.table.name;
	const tableId = column.config.column.tableModelFields.table.id;
	const { allowNull, dataType } = column.config.column.tableModelFields;
	const propName = column.config.column.tableModelFields.name;
	const propId = column.config.column.tableModelFields.id;
	const value = "";
	const data = { tableId, entityName, propId, propName, allowNull, dataType, value };
	console.log(data);
	//create ui container element by field data type
	switch (dataType) {
		case "nvarchar":
			{
				const el = this.getTextEditCell(data);
				el.setAttribute("class", "inline-add-event data-new form-control");
				if (!allowNull) {
					el.setAttribute("required", "required");
				}

				cellContent.appendChild(el);
			}
			break;
		case "int32":
		case "decimal":
			{
				const el = this.getNumberEditCell(data);
				el.setAttribute("class", "inline-add-event data-new form-control");
				if (!allowNull) {
					el.setAttribute("required", "required");
				}

				cellContent.appendChild(el);
			} break;
		case "bool":
			{
				const div = document.createElement("div");
				div.setAttribute("class", "checkbox checkbox-success");
				div.style.marginTop = "-1em";
				div.style.marginLeft = "2em";
				const label = document.createElement("label");
				label.setAttribute("for", "test");
				const el = document.createElement("input");
				el.setAttribute("class", "inline-add-event");
				el.setAttribute("data-prop-id", propId);
				el.setAttribute("data-prop-name", propName);
				el.setAttribute("type", "checkbox");
				el.setAttribute("data-entity", tableId);
				el.setAttribute("data-type", "bool");
				el.setAttribute("id", "test");
				el.setAttribute("name", "test");
				el.style.maxWidth = "1em";

				div.appendChild(el);
				div.appendChild(label);
				cellContent.appendChild(div);
			}
			break;
		case "datetime":
		case "date":
			{
				const el = this.getDateEditCell(data);
				el.setAttribute("class", "inline-add-event data-new form-control");
				cellContent.appendChild(el);
				$(columns[i]).html(container);
				$(columns[i]).find(".inline-add-event")
					.on("change", function () { })
					.datepicker({
						format: "dd/mm/yyyy"
					}).addClass("datepicker");
			} break;
		case "uniqueidentifier":
			{
				const div = document.createElement("div");
				div.setAttribute("class", "input-group mb-3");
				const dropdown = document.createElement("select");
				dropdown.setAttribute("class", "inline-add-event data-new form-control");
				dropdown.setAttribute("data-prop-id", propId);
				dropdown.setAttribute("data-prop-name", propName);
				dropdown.setAttribute("data-entity", tableId);
				dropdown.setAttribute("data-type", "uniqueidentifier");
				dropdown.options[dropdown.options.length] = new Option(window.translate("no_value_selected"), "");
				//Populate dropdown
				const data = load(`/PageRender/GetRowReferences?entityId=${tableId}&propertyId=${propId}`);
				if (data) {
					if (data.is_success) {
						$.each(data.result.data, function (index, obj) {
							dropdown.options[dropdown.options.length] = new Option(obj.Name, obj.Id);
						});
						dropdown.setAttribute("data-ref-entity", data.result.entityName);
					}
				}
				div.appendChild(dropdown);
				const addOptionDiv = document.createElement("div");
				addOptionDiv.setAttribute("class", "input-group-append");
				const addOption = document.createElement("a");
				addOption.setAttribute("class", "btn btn-success");
				const plus = document.createElement("span");
				plus.setAttribute("class", "fa fa-plus");
				plus.style.color = "white";
				addOption.appendChild(plus);
				addOption.addEventListener("click", addNewToReferenceHandler);
				addOptionDiv.appendChild(addOption);
				div.appendChild(addOptionDiv);
				cellContent.appendChild(div);
			}
			break;
	}
	cell.appendChild(cellContent);

	return {
		cell: cell,
		entityName: entityName
	};
}

TableInlineEdit.prototype.renderActiveInlineButton = function (ctx) {
	ctx.html("Complete");
	ctx.removeClass("btn-warning").addClass("btn-success");
};

/*-------------------------------------------------
				Get inline edit cells
-------------------------------------------------*/
/**
 * Get cell for textual fields
 * @param {any} data
 */
TableInlineEdit.prototype.getTextEditCell = (data) => {
	const el = document.createElement("textarea");
	el.setAttribute("class", "inline-update-event data-input form-control");
	el.setAttribute("data-prop-id", data.propId);
	el.setAttribute("data-id", data.cellId);
	el.setAttribute("type", "text");
	el.setAttribute("data-entity", data.tableId);
	el.setAttribute("data-prop-name", data.propName);
	el.setAttribute("data-type", "nvarchar");
	el.innerHTML = data.value;
	return el;
};

/**
 * Get cell for number fields
 * @param {any} data
 */
TableInlineEdit.prototype.getNumberEditCell = (data) => {
	const el = document.createElement("input");
	el.setAttribute("class", "inline-update-event data-input form-control");
	el.setAttribute("data-prop-id", data.propId);
	el.setAttribute("data-id", data.cellId);
	el.setAttribute("type", "number");
	el.setAttribute("data-entity", data.tableId);
	el.setAttribute("data-prop-name", data.propName);
	el.setAttribute("data-type", "int32");
	el.setAttribute("value", data.value);
	return el;
};

/**
 * Get cell for boolean fields
 * @param {any} data
 */
TableInlineEdit.prototype.getBooleanEditCell = (data) => {
	const div = document.createElement("div");
	div.setAttribute("class", "checkbox checkbox-success");
	div.style.marginTop = "-1em";
	div.style.marginLeft = "2em";
	const label = document.createElement("label");
	label.setAttribute("for", "test");
	const el = document.createElement("input");
	el.setAttribute("class", "inline-update-event data-input");
	el.setAttribute("data-prop-id", data.propId);
	el.setAttribute("data-id", data.cellId);
	el.setAttribute("type", "checkbox");
	el.setAttribute("data-prop-name", data.propName);
	el.setAttribute("data-entity", data.tableId);
	el.setAttribute("data-type", "bool");
	el.setAttribute("id", "test");
	el.setAttribute("name", "test");
	el.style.maxWidth = "1em";
	if (data.value) {
		el.setAttribute("checked", "checked");
	}

	div.appendChild(el);
	div.appendChild(label);

	return div;
};
/**
 * Get date edit cell
 * @param {any} data
 */
TableInlineEdit.prototype.getDateEditCell = (data) => {
	const el = document.createElement("input");
	el.setAttribute("class", "inline-update-event data-input form-control");
	el.setAttribute("data-prop-id", data.propId);
	el.setAttribute("data-id", data.cellId);
	el.setAttribute("data-prop-name", data.propName);
	el.setAttribute("type", "text");
	el.setAttribute("data-entity", data.tableId);
	el.setAttribute("data-type", "datetime");
	el.setAttribute("value", data.value);
	return el;
};

/**
 * Get reference edit cell
 * @param {any} conf
 */
TableInlineEdit.prototype.getReferenceEditCell = (conf) => {
	const div = document.createElement("div");
	div.setAttribute("class", "input-group mb-3");
	const dropdown = document.createElement("select");
	dropdown.setAttribute("class", "inline-update-event data-input form-control");
	dropdown.setAttribute("data-prop-id", conf.propId);
	dropdown.setAttribute("data-prop-name", conf.propName);
	dropdown.setAttribute("data-id", conf.cellId);
	dropdown.setAttribute("data-entity", conf.tableId);
	dropdown.setAttribute("data-type", "uniqueidentifier");
	dropdown.options[dropdown.options.length] = new Option(window.translate("no_value_selected"), "");
	//Populate dropdown
	const data = load(`/PageRender/GetRowReferences?entityId=${conf.tableId}&propertyId=${conf.propId}`);
	if (data) {
		if (data.is_success) {
			const entityName = data.result.entityName;
			let key = "Name";
			if (entityName === "Users") {
				key = "UserName";
			}
			$.each(data.result.data, function (index, obj) {
				dropdown.options[dropdown.options.length] = new Option(obj[key], obj.Id);
			});
			dropdown.setAttribute("data-ref-entity", entityName);
		}
		dropdown.value = conf.value;
	}
	div.appendChild(dropdown);
	const addOptionDiv = document.createElement("div");
	addOptionDiv.setAttribute("class", "input-group-append");
	const addOption = document.createElement("a");
	addOption.setAttribute("class", "btn btn-success");
	const plus = document.createElement("span");
	plus.setAttribute("class", "fa fa-plus");
	plus.style.color = "white";
	addOption.appendChild(plus);
	addOption.addEventListener("click", addNewToReferenceHandler);
	addOptionDiv.appendChild(addOption);
	div.appendChild(addOptionDiv);
	return div;
};

/*-------------------------------------------------
				End Get inline cells
-------------------------------------------------*/


/*-------------------------------------------------
				Bind events to inline cells
-------------------------------------------------*/

/**
 * On after init edit cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitEditCellDefaultHandler = function (columns, index) {
	$(columns[index]).find(".inline-update-event").on("blur", onInputEventHandler);
};

/**
 * On after init text cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitTextEditCell = function (columns, index) {
	this.onAfterInitEditCellDefaultHandler(columns, index);
};

/**
 * On after init number cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitNumberEditCell = function (columns, index) {
	this.onAfterInitEditCellDefaultHandler(columns, index);
};

/**
 * On after init boolean cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitBooleanEditCell = function (columns, index) {
	this.onAfterInitEditCellDefaultHandler(columns, index);
};

/**
 * On after init date time cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitDateEditCell = function (columns, index) {
	$(columns[index]).find(".inline-update-event")
		.on("change", onInputEventHandler)
		.datepicker({
			format: "dd/mm/yyyy"
		}).addClass("datepicker");
};

/**
 * On after init reference cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitReferenceCell = function (columns, index) {
	$(columns[index]).find(".inline-update-event").on("change", onInputEventHandler);
};

/**
 * Bind events after row is ready to edit inline
 */
TableInlineEdit.prototype.bindEventsAfterInitInlineEdit = function () {
	try {
		// ReSharper disable once ConstructorCallNotUsed
		new $.Iso.InlineEditingCells();
	} catch (e) { };
};

/**
 * On cell value changed
 * @param {any} target
 */
TableInlineEdit.prototype.onEditCellValueChanged = function (target) {
	const targetCtx = $(target);
	const rowId = targetCtx.attr("data-id");
	const entityId = targetCtx.attr("data-entity");
	const propertyId = targetCtx.attr("data-prop-id");
	const type = targetCtx.attr("data-type");
	let value = "";
	let displaySuccessText = "";
	switch (type) {
		case "bool":
			{
				value = targetCtx.prop("checked");
				displaySuccessText = `You turned ${value ? "on" : "off"} checkbox`;
			} break;
		case "uniqueidentifier":
			{
				value = targetCtx.val();
				displaySuccessText = `Was selected : ${targetCtx.find("option:selected").text()}`;
			} break;
		default: {
			value = targetCtx.val();
			displaySuccessText = `You change ${value} value`;
		} break;
	}

	const req = load(`/PageRender/SaveTableCellData`, { entityId, propertyId, rowId, value }, "post");
	if (req.is_success) {
		this.displayNotification({ heading: window.translate("system_inline_saved"), text: displaySuccessText, icon: "success" });
	} else {
		this.displayNotification({ heading: req.error_keys[0].message });
	}
};
/*-------------------------------------------------
				End Bind events to inline cells
-------------------------------------------------*/

/*-------------------------------------------------
				Events
-------------------------------------------------*/
/**
 * Start inline edit for row
 */
function inlineEditHandler() {
	new TableInlineEdit().initInlineEditForRow(this);
}

/**
 * Complete inline edit handler
 */
function completeEditInlineHandler() {
	new TableInlineEdit().completeInlineEditForRow(this);
}

/**
 * On change value event for edit cell
 */
function onInputEventHandler() {
	new TableInlineEdit().onEditCellValueChanged(this);
}

/**
 * On add new obj to reference
 */
function addNewToReferenceHandler() {
	new TableInlineEdit().addNewDataToReference(this);
}

$(".dynamic-table")
	.on("draw.dt", function (e, settings, json) {
		$(".inline-edit").off("click", inlineEditHandler);
		$(".inline-edit").on("click", inlineEditHandler);
		const table = $(this).DataTable();
		const buttons = table.buttons();
		let match = false;
		for (let i = 0; i < buttons.length; i++) {
			if (buttons[i].node.innerHTML.indexOf("fa-plus") != -1) {
				match = true;
			}
		}

		if (!match) {
			table.button().add(0, {
				action: function (e, dt, button, config) {
					new TableInlineEdit().addNewHandler(button, dt);
				},
				text: '<i class="fa fa-plus"></i>'
			});
		}
	});

/*-------------------------------------------------
				End Events
-------------------------------------------------*/




/*-------------------------------------------------
				Event Handlers
-------------------------------------------------*/
/**
 * Transform row in inline edit mode
 * @param {any} target
 */
TableInlineEdit.prototype.initInlineEditForRow = function (target) {
	const targetCtx = $(target);
	this.renderActiveInlineButton(targetCtx);
	targetCtx.removeClass("inline-edit");
	targetCtx.addClass("inline-complete");
	targetCtx.off("click", inlineEditHandler);

	const viewModelId = targetCtx.attr("data-viewmodel");
	const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);

	const columns = targetCtx.parent().parent().parent().find(".data-cell");
	const table = targetCtx.closest("table").DataTable();
	const row = targetCtx.closest("tr");
	const index = table.row(row).index();
	var obj = table.row(index).data();
	for (let i = 0; i < columns.length; i++) {
		const columnCtx = $(columns[i]);
		const columnId = columnCtx.attr("data-column-id");
		const fieldData = viewModel.result.filter(obj => {
			return obj.columnId === columnId;
		});
		if (fieldData.length > 0) {
			//const viewModelId = $(columns[i]).attr("data-viewmodel");
			const cellId = columnCtx.attr("data-id");
			columnCtx.parent().addClass("expandable-cell");
			const tableId = fieldData[0].tableId;
			const propId = fieldData[0].id;
			const propName = fieldData[0].name;
			const parsedPropName = propName[0].toLowerCase() + propName.substr(1, propName.length);
			const value = obj[parsedPropName];
			let container = value;
			const data = { cellId, tableId, propId, value, propName };
			switch (fieldData[0].dataType) {
				case "nvarchar":
					{
						container = this.getTextEditCell(data);
						columnCtx.html(container);
						this.onAfterInitTextEditCell(columns, i);
					}
					break;
				case "int32":
				case "decimal":
					{
						container = this.getNumberEditCell(data);
						columnCtx.html(container);
						this.onAfterInitNumberEditCell(columns, i);
					}
					break;
				case "bool":
					{
						container = this.getBooleanEditCell(data);
						columnCtx.html(container);
						this.onAfterInitBooleanEditCell(columns, i);
					}
					break;
				case "datetime":
				case "date":
					{
						container = this.getDateEditCell(data);
						columnCtx.html(container);
						this.onAfterInitDateEditCell(columns, i);
					}
					break;
				case "uniqueidentifier":
					{
						container = this.getReferenceEditCell(data);
						columnCtx.html(container);
						this.onAfterInitReferenceCell(columns, i);
					}
					break;
			}
		}
	}
	targetCtx.on("click", completeEditInlineHandler);
	this.bindEventsAfterInitInlineEdit();
};

/**
 * Transform row from edit mode to read mode
 * @param {any} target
 */
TableInlineEdit.prototype.completeInlineEditForRow = function (target) {
	const service = new DataInjector();
	const targetCtx = $(target);
	const table = targetCtx.closest("table").DataTable();
	var row = targetCtx.closest("tr");
	const index = table.row(row).index();
	var obj = table.row(index).data();
	$(this).off("click", completeEditInlineHandler);
	const columns = targetCtx.parent().parent().parent().find(".data-cell");

	const viewModelId = $(columns[0]).attr("data-viewmodel");
	const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);
	if (!viewModelId) return;
	if (!viewModel.is_success) return;

	for (let i = 0; i < columns.length; i++) {
		const columnCtx = $(columns[i]);
		const inspect = columnCtx.find(".data-input");
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
			columnCtx.find(".inline-update-event").off("blur", onInputEventHandler);
			columnCtx.find(".inline-update-event").off("changed", onInputEventHandler);
		}
	}

	const redraw = table.row(index).data(obj).invalidate();

	$(redraw.row(index).nodes()).find(".inline-edit").on("click", inlineEditHandler);
};

/**
 * Add new data to reference entity
 * @param {any} target
 */
TableInlineEdit.prototype.addNewDataToReference = function (target) {
	const targetCtx = $(target);
	var dropdown = targetCtx.parent().parent().find("select");
	const entityName = dropdown.attr("data-ref-entity");
	if (!entityName) {
		return this.displayNotification({ heading: "No reference!" });
	}

	swal({
		title: `Add new ${entityName}`,
		html:
			'<input id="add_new_ref" class="swal2-input">',
		showCancelButton: true,
		preConfirm: function () {
			return new Promise(function (resolve) {
				resolve([
					$("#add_new_ref").val()
				]);
			});
		},
		onOpen: function () {
			$("#add_new_ref").focus();
		}
	}).then(function (result) {
		if (result.value) {
			const context = new DataInjector();
			const obj = {
				name: result.value[0]
			};
			const res = context.Add(entityName, obj);
			if (res) {
				if (res.is_success) {
					const newId = res.result;
					const detail = context.GetById(entityName, newId);
					if (detail.is_success) {
						dropdown.append(new Option(detail.result.Name, newId));
					}
					this.displayNotification({ heading: "Info", text: `A new item has been added`, icon: "success" });
				} else {
					this.displayNotification({ heading: res.error_keys[0].message });
				}
			} else {
				this.displayNotification({ heading: "Fail to save data" });
			}
		}
	});
	return null;
};

/**
 * Display notification
 * @param {any} conf
 */
TableInlineEdit.prototype.displayNotification = (conf) => {
	const settings = {
		heading: "",
		text: "",
		position: "top-right",
		loaderBg: "#ff6849",
		icon: "error",
		hideAfter: 2500,
		stack: 6
	};
	Object.assign(settings, conf);
	$.toast(settings);
};

/*-------------------------------------------------
				End Event Handlers
-------------------------------------------------*/