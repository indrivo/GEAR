/* Table inline edit
 * A plugin for inline edit on dynamic tables
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica(Indrivo) Srl
 * Author: Lupei Nicolae
 */

// Make sure jQuery has been loaded
if (typeof jQuery === "undefined") {
    throw new Error("Inline edit plugin require JQuery");
}

function TableInlineEdit() {
};

$(document).ready(function () {
    new TableInlineEdit().init(".dynamic-table");
});

/**
 * Create and render new table with inline edit
 * @param {any} conf
 */
TableInlineEdit.prototype.createAndRenderTable = function (conf = {
    target: undefined,
    selector: "",
    dbViewModel: undefined,
    builderConfiguration: {
    }
}) {
    if (!conf || !conf.selector || !conf.target) {
        console.warn("Bad configuration!!!");
        return 0;
    }

    const container = document.createElement("div");
    container.setAttribute("class", "card");
    const cBody = document.createElement("div");
    cBody.setAttribute("class", "card-body");
    const table = document.createElement("table");
    table.setAttribute("id", conf.selector.substr(1));
    table.setAttribute("class", "dynamic-table table table-striped table-bordered");
    table.setAttribute("db-viewmodel", conf.dbViewModel);
    const headSection = document.createElement("thead");
    const defaultRow = document.createElement("tr");
    headSection.append(defaultRow);
    table.append(headSection);
    cBody.append(table);
    container.append(cBody);

    $(conf.target).html(container);
    const jDto = $(conf.selector);
    const tBuilder = new TableBuilder(conf.builderConfiguration);
    tBuilder.init(jDto.get(0));
    new TableInlineEdit().init(conf.selector);
    jDto.on("preInit.dt", function () {
        const headConf = new IsoTableHeadActions().getConfiguration();
        const content = tManager.render("template_headListActions", headConf);
        const selector = $("div.CustomTableHeadBar");
        selector.html(content);
        $('.table-search').keyup(function () {
            const oTable = $(this).closest(".card").find(".dynamic-table").DataTable();
            oTable.search($(this).val()).draw();
        });

        //Delete multiple rows
        $(".deleteMultipleRows").on("click", function () {
            const cTable = $(this).closest(".card").find(conf.selector);
            if (cTable) {
                if (typeof TableBuilder !== 'undefined') {
                    tBuilder.deleteSelectedRowsHandler(cTable.DataTable());
                }
            }
        });

        $(".add_new_inline").on("click", function () {
            new TableInlineEdit().addNewHandler(this);
        });

        //Items on page
        $(".tablePaginationView a").on("click", function () {
            const ctx = $(this);
            const onPageValue = ctx.data("page");
            const onPageText = ctx.text();
            ctx.closest(".dropdown").find(".page-size").html(`(${onPageText})`);
            const table = ctx.closest(".card").find(conf.selector).DataTable();
            table.page.len(onPageValue).draw();
        });

        //hide columns
        $(".hidden-columns-event").click(function () {
            const tables = $(this).closest(".card").find(conf.selector);
            if (tables.length === 0) return;
            new TableColumnsVisibility().toggleRightListSideBar(`#${tables[0].id}`);
            $("#hiddenColumnsModal").modal();
        });
        window.forceTranslate("div.CustomTableHeadBar");
    });
};

/**
 * Constructor
 */
TableInlineEdit.prototype.constructor = TableInlineEdit;

/**
 * Db provider
 */
TableInlineEdit.prototype.db = new DataInjector();

/*
 * Toast notifier
 */
TableInlineEdit.prototype.toast = new ToastNotifier();

/**
 * Default container on unknown column type
 */
TableInlineEdit.prototype.defaultNotEditFieldContainer = "";

/**
 * Register inline edit events to JQ dt
 * @param {any} selector
 */
TableInlineEdit.prototype.init = function (selector) {
    $(selector)
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
};

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
    const scope = this;
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
    row.setAttribute(scope.attributeNames.addingInProgressAttr, "false");
    row.setAttribute(scope.attributeNames.validatorAttr, "false");
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
                $(cell).find(".cancel-new-item").on("click", function () {
                    scope.cancelNewItem($(this));
                });
                $(cell).find(".add-new-item").on("click", function () {
                    scope.addNewItem($(this));
                });
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
    this.bindEventsAfterInitInlineEdit(row);
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
 * Cancel new item
 * @param {any} context
 */
TableInlineEdit.prototype.cancelNewItem = function (context) {
    context.off("click");
    context.parent().find(".add-new-item").off("click");
    this.toggleVisibilityColumnsButton(context, false);
    this.cancelTableAddMode(context);
    context.closest("tr").remove();
};

/*
 * Constants
 */
TableInlineEdit.prototype.attributeNames = {
    addingInProgressAttr: "is-adding-in-progress",
    validatorAttr: "is-validation-running"
};

/**
 * Add new item
 * @param {any} context
 */
TableInlineEdit.prototype.addNewItem = function (context) {
    const dTable = context.closest("table").DataTable();
    const rowContext = context.closest("tr");
    const entityName = rowContext.attr("entityName");
    const isValid = this.isValidNewRow(rowContext);
    if (!isValid) {
        return this.toast.notify({ heading: window.translate("system_inline_edit_validate_row") });
    }

    if (rowContext.attr(this.attributeNames.addingInProgressAttr) === "true") return 1;
    rowContext.attr(this.attributeNames.addingInProgressAttr, "true");

    const data = this.getRowDataOnAddMode(rowContext);
    this.db.addAsync(entityName, data).then(req => {
        if (req.is_success) {
            this.toast.notify({ heading: window.translate("system_inline_edit_row_added"), icon: "success" });
            dTable.draw();
            this.toggleVisibilityColumnsButton(context, false);
            context.off("click");
            this.cancelTableAddMode(context);
        } else {
            rowContext.attr(this.attributeNames.addingInProgressAttr, "false");
            this.toast.notify({ heading: req.error_keys[0].message });
            this.toggleVisibilityColumnsButton(context, false);
        }
    }).catch(err => {
        console.warn(err);
    });
};

/**
 * Cancel add mode
 * @param {any} ctx
 */
TableInlineEdit.prototype.cancelTableAddMode = function (ctx) {
    ctx.closest("table").attr("add-mode", "false");
};

/**
 * Validate row
 * @param {any} context
 */
TableInlineEdit.prototype.isValidNewRow = function (context) {
    $(context).attr(this.attributeNames.validatorAttr, "true");
    const els = $(context).find("textarea.data-new");
    let isValid = true;
    $.each(els, function (index, el) {
        const jel = $(el);
        if (jel.hasAttr("data-required")) {
            if (!jel.val()) {
                if (!jel.hasClass("cell-red")) {
                    jel.addClass("cell-red");
                }
                isValid = false;
            }
        }
    });

    const elsDates = context.get(0).querySelectorAll("input.datepicker-control");

    $.each(elsDates,
        (index, el) => {
            const ctx = $(el).parent();
            if (el.hasAttribute("data-required")) {
                if (!el.value) {
                    if (!ctx.hasClass("cell-red")) {
                        ctx.addClass("cell-red");
                    }
                    isValid = false;
                }
            }
        });

    const referenceCells = context.get(0).querySelectorAll("select.data-new");
    $.each(referenceCells,
        (index, el) => {
            const jel = $(el);
            if (jel.hasAttr("data-required")) {
                const input = jel.closest(".data-cell").find(".fire-reference-component");
                if (!jel.val()) {
                    if (!input.hasClass("cell-red")) {
                        input.addClass("cell-red");
                    }
                    isValid = false;
                } else {
                    input.removeClass("cell-red");
                }
            }
        });

    $(context).attr(this.attributeNames.validatorAttr, "false");
    return isValid;
};

/**
 * Get row data on add mode
 * @param {any} context
 */
TableInlineEdit.prototype.getRowDataOnAddMode = (context) => {
    const data = $(context).find(".data-new");
    const obj = {};
    for (let i = 0; i < data.length; i++) {
        const f = $(data[i]);
        const propName = f.attr("data-prop-name");
        switch (f.attr("data-type")) {
            case "nvarchar":
            case "int32":
            case "int":
            case "decimal": {
                obj[propName] = f.val();
            } break;
            case "bool": {
                obj[propName] = f.prop("checked");
            } break;
            case "datetime":
            case "date":
                {
                    const date = f.val();
                    const parsed = moment(date, "DD/MM/YYYY").format("DD.MM.YYYY");
                    if (parsed !== "Invalid date")
                        obj[propName] = parsed;
                } break;
            case "uniqueidentifier":
                {
                    const value = f.val();
                    if (!value) {
                        obj[propName] = "00000000-0000-0000-0000-000000000000";
                    } else {
                        obj[propName] = value;
                    }
                }
                break;
        }
    }
    return obj;
};

/**
 * Do something after cell added
 * @param {any} cell
 */
TableInlineEdit.prototype.onGetNewAddCell = function (cell) {
    //do something
};

/**
 * Return new cell container for field definition
 * by data type and references
 * @param {*} column
 * @param {*} cell
 */
TableInlineEdit.prototype.getAddRowCell = function (column, cell) {
    if (!column.config) {
        cell.innerHTML = this.defaultNotEditFieldContainer;
        return {
            cell: cell,
            entityName: ""
        };
    }
    if (!column.config.column.tableModelFields) {
        cell.innerHTML = this.defaultNotEditFieldContainer;
        return {
            cell: cell,
            entityName: ""
        };
    }

    const cellContent = document.createElement("div");
    cellContent.setAttribute("class", "data-cell");
    const entityName = column.config.column.tableModelFields.table.name;
    const tableId = column.config.column.tableModelFields.table.id;
    const { allowNull, dataType } = column.config.column.tableModelFields;
    const propName = column.config.column.tableModelFields.name;
    const propId = column.config.column.tableModelFields.id;
    const value = "";
    const data = {
        tableId, entityName,
        propId,
        propName, allowNull,
        dataType, value,
        addMode: true, viewModel: column.config.column
    };
    //create ui container element by field data type
    switch (dataType) {
        case "nvarchar":
            {
                const el = this.getTextEditCell(data);
                cellContent.appendChild(el);
                this.onAfterInitAddTextCell(el, data);
            }
            break;
        case "int32":
        case "int":
        case "decimal":
            {
                const el = this.getNumberEditCell(data);
                this.onAfterInitAddNumberCell(el, data);
                cellContent.appendChild(el);
            } break;
        case "bool":
            {
                const el = this.getBooleanEditCell(data);
                this.onAfterInitAddBooleanCell(el, data);
                cellContent.appendChild(el);
            }
            break;
        case "datetime":
        case "date":
            {
                const el = this.getDateEditCell(data);
                this.onAfterInitAddDateCell(el, data);
                cellContent.appendChild(el);
            } break;
        case "uniqueidentifier":
            {
                data.value = false;
                const el = this.getReferenceEditCell(data);
                this.onAfterInitAddReferenceCell(el, data);
                cellContent.appendChild(el);
            }
            break;
    }
    cell.appendChild(cellContent);
    this.onGetNewAddCell(cell);
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
    if (!data.allowNull) {
        el.setAttribute("data-required", "");
    }
    el.innerHTML = data.value;
    el.value = data.value;
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
    if (!data.allowNull) {
        el.setAttribute("data-required", "");
    }
    return el;
};

/**
 * Get cell for boolean fields
 * @param {any} data
 */
TableInlineEdit.prototype.getBooleanEditCell = (data) => {
    const labelIdentifier = `label_${new ST().newGuid()}`;
    const div = document.createElement("div");
    div.setAttribute("class", "checkbox checkbox-success");
    div.style.marginTop = "-1em";
    div.style.marginLeft = "2em";
    const label = document.createElement("label");
    label.setAttribute("for", labelIdentifier);
    const el = document.createElement("input");
    el.setAttribute("class", "inline-update-event data-input");
    el.setAttribute("data-prop-id", data.propId);
    el.setAttribute("data-id", data.cellId);
    el.setAttribute("type", "checkbox");
    el.setAttribute("data-prop-name", data.propName);
    el.setAttribute("data-entity", data.tableId);
    el.setAttribute("data-type", "bool");
    el.setAttribute("id", labelIdentifier);
    el.setAttribute("name", labelIdentifier);
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
    if (!data.allowNull) {
        el.setAttribute("data-required", "");
    }
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
    const data = load(`/InlineEdit/GetRowReferences?entityId=${conf.tableId}&propertyId=${conf.propId}`);
    if (data) {
        if (data.is_success) {
            const entityName = data.result.entityName;
            $.each(data.result.data, function (index, obj) {
                dropdown.options[dropdown.options.length] = new Option(obj.name, obj.id);
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
 * After add cell for in add mode bind ui validations
 * @param {any} el
 */
TableInlineEdit.prototype.onAddedCellBindValidations = function (el) {
    $(el).on("keydown", function () {
        $(this).removeClass("cell-red");
    });

    $(el).on("blur", function () {
        if (this.hasAttribute("data-required")) {
            const ctx = $(this);
            if (!ctx.val()) {
                ctx.addClass("cell-red");
            }
        }
    });
};

/**
 * On after init add text cell
 * @param {any} el
 * @param {any} data
 */
TableInlineEdit.prototype.onAfterInitAddTextCell = function (el, data) {
    el.setAttribute("class", "inline-add-event data-new form-control");
    if (!data.allowNull) {
        el.setAttribute("required", "required");
    }
    this.onAddedCellBindValidations(el);
};

/**
 * On after init number cell
 * @param {any} el
 * @param {any} data
 */
TableInlineEdit.prototype.onAfterInitAddNumberCell = function (el, data) {
    el.setAttribute("class", "inline-add-event data-new form-control");
    if (!data.allowNull) {
        el.setAttribute("required", "required");
    }
    this.onAddedCellBindValidations(el);
};

/**
 * On after init bool cell
 * @param {any} el
 * @param {any} data
 */
TableInlineEdit.prototype.onAfterInitAddBooleanCell = function (el, data) {
    $(el).find("input").attr("class", "inline-update-event data-input custom-control-input");
};

/**
 * On after init date cell
 * @param {any} el
 * @param {any} data
 */
TableInlineEdit.prototype.onAfterInitAddDateCell = function (el, data) {
    el.setAttribute("class", "inline-add-event data-new form-control");
    $(el).on("change", function () { })
        .datepicker({
            format: "dd/mm/yyyy",
            autoclose: true
        });
    $(el).on("change", function () {
        if (!this.hasAttribute("data-required")) return;
        if ($(this).val()) {
            $(this).removeClass("cell-red");
        } else {
            $(this).removeClass("cell-red");
            $(this).addClass("cell-red");
        }
    });
};

/**
 * On after init reference cell
 * @param {any} el
 * @param {any} data
 */
TableInlineEdit.prototype.onAfterInitAddReferenceCell = function (el, data) {
    $(el).find("select").attr("class", "inline-add-event data-new form-control");
};

/**
 * On after init edit cell
 * @param {any} columns
 * @param {any} index
 */
TableInlineEdit.prototype.onAfterInitEditCellDefaultHandler = function (columns, index) {
    const el = $(columns[index]).find(".inline-update-event");
    el.on("blur", onInputEventHandler);
    el.on("keydown change", function () {
        $(this).removeClass("cell-red");
    });
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
            format: "dd/mm/yyyy",
            autoclose: true
        });
    $(columns[index]).find(".inline-update-event").on("change", function () {
        if (!this.hasAttribute("data-required")) return;
        if ($(this).val()) {
            $(this).removeClass("cell-red");
        } else {
            $(this).removeClass("cell-red");
            $(this).addClass("cell-red");
        }
    });
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
TableInlineEdit.prototype.bindEventsAfterInitInlineEdit = function (row) {
    //do something
};

/**
 * On cell value changed
 * @param {any} target
 */
TableInlineEdit.prototype.onEditCellValueChanged = function (target) {
    const targetCtx = $(target);
    if (!targetCtx.hasClass("inline-update-event")) return;
    const rowId = targetCtx.attr("data-id");
    const entityId = targetCtx.attr("data-entity");
    const propertyId = targetCtx.attr("data-prop-id");
    const type = targetCtx.attr("data-type");
    const isRequired = target.hasAttribute("data-required");
    let value = "";
    let displaySuccessText = "";
    let isValid = true;
    switch (type) {
        case "bool":
            {
                value = targetCtx.prop("checked");
                displaySuccessText = `${window.translate("system_inline_edit_select_value_changed")} ${value ? "on" : "off"} checkbox`;
            } break;
        case "uniqueidentifier":
            {
                value = targetCtx.val();
                displaySuccessText = `${window.translate("system_inline_edit_select_value_changed")} : ${targetCtx.find("option:selected").text()}`;
            } break;
        default: {
            value = targetCtx.val();
            if (isRequired) {
                if (!value) {
                    targetCtx.addClass("cell-red");
                    isValid = false;
                }
            }
            const displayValue = value.length > 10 ? `${value.substr(0, 9)} ...` : value;
            displaySuccessText = `${window.translate("system_inline_edit_text_chnaged")} ${displayValue}`;
        } break;
    }

    if (!isValid) return;
    loadAsync(`/InlineEdit/SaveTableCellData`, { entityId, propertyId, rowId, value }, "post").then(req => {
        if (req.is_success) {
            this.displayNotification({ heading: window.translate("system_inline_saved"), text: displaySuccessText, icon: "success" });
        } else {
            this.displayNotification({ heading: req.error_keys[0].message });
        }
    }).catch(err => { console.warn(err) });
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

/*-------------------------------------------------
				End Events
-------------------------------------------------*/

/*-------------------------------------------------
				Event Handlers
-------------------------------------------------*/
TableInlineEdit.prototype.getOnNonRecognizedField = function (columnCtx, confs) {
};

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
    loadAsync(`/InlineEdit/GetViewModelColumnTypes?viewModelId=${viewModelId}`).then(viewModel => {
        const columns = targetCtx.parent().parent().parent().find(".data-cell");
        const table = targetCtx.closest("table").DataTable();
        const row = targetCtx.closest("tr");
        const index = table.row(row).index();
        let obj = table.row(index).data();
        for (let i = 0; i < columns.length; i++) {
            const columnCtx = $(columns[i]);
            const columnId = columnCtx.attr("data-column-id");
            const fieldData = viewModel.result.entityFields.find(x => {
                return x.columnId === columnId;
            });

            const viewModelConfigurations = viewModel.result.viewModelFields.find(x => {
                return x.id === columnId;
            });

            const cellId = columnCtx.attr("data-id");

            if (fieldData) {
                //const viewModelId = $(columns[i]).attr("data-viewmodel");
                const tableId = fieldData.tableId;
                const propId = fieldData.id;
                const propName = fieldData.name;
                const parsedPropName = propName.toLowerFirstLetter();
                const value = obj[parsedPropName];
                const allowNull = fieldData.allowNull;
                let container = value;
                const data = { cellId, tableId, propId, value, propName, allowNull, addMode: false, viewModel: viewModel.result };
                switch (fieldData.dataType) {
                    case "nvarchar":
                        {
                            container = this.getTextEditCell(data);
                            columnCtx.html(container);
                            this.onAfterInitTextEditCell(columns, i);
                        }
                        break;
                    case "int32":
                    case "int":
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
            } else if (viewModelConfigurations.configurations.length > 0) {
                switch (viewModelConfigurations.virtualDataType) {
                    //Many to many
                    case 3:
                        {
                            this.initManyToManyControl({
                                viewModelConfigurations, columnCtx, cellId
                            });
                        }
                        break;
                }
            } else {
                this.getOnNonRecognizedField(columnCtx, viewModelConfigurations);
            }
        }
        targetCtx.on("click", completeEditInlineHandler);
        this.bindEventsAfterInitInlineEdit(row);
    }).catch(err => {
        console.warn(err);
    });
};

/**
 * Get viewmodel field configurations for many to many
 * @param {any} data
 */
TableInlineEdit.prototype.getManyToManyViewModelConfigurations = (data) => {
    const { configurations } = data;
    const sourceEntity = configurations.find(x => x.viewModelFieldCodeId === 2001);
    const sourceSelfParamName = configurations.find(x => x.viewModelFieldCodeId === 1003);
    const sourceRefParamName = configurations.find(x => x.viewModelFieldCodeId === 2003);
    const referenceEntityName = configurations.find(x => x.viewModelFieldCodeId === 1001);
    return { sourceEntity, sourceSelfParamName, sourceRefParamName, referenceEntityName };
};

/**
 * Many to many control
 * @param {any} data
 */
TableInlineEdit.prototype.initManyToManyControl = function (data) {
    const { viewModelConfigurations, columnCtx, cellId } = data;
    const scope = this;
    const mCtx = columnCtx.closest("td");
    const { sourceEntity, sourceSelfParamName, sourceRefParamName, referenceEntityName }
        = scope.getManyToManyViewModelConfigurations(viewModelConfigurations);
    //TODO: create default component for set many to many
};

/**
 * Transform row from edit mode to read mode
 * @param {any} target
 */
TableInlineEdit.prototype.completeInlineEditForRow = function (target) {
    const targetCtx = $(target);
    const table = targetCtx.closest("table").DataTable();
    const row = targetCtx.closest("tr");
    const index = table.row(row).index();
    let obj = table.row(index).data();
    targetCtx.off("click", completeEditInlineHandler);
    const columns = targetCtx.parent().parent().parent().find(".data-cell");

    const viewModelId = $(columns[0]).attr("data-viewmodel");
    loadAsync(`/InlineEdit/GetViewModelColumnTypes?viewModelId=${viewModelId}`).then(viewModel => {
        if (!viewModelId) return;
        if (!viewModel.is_success) return;
        const promises = [];
        for (let i = 0; i < columns.length; i++) {
            const forPromise = new Promise((globalResolve, globalReject) => {
                const columnCtx = $(columns[i]);
                const inspect = columnCtx.find(".data-input");
                const type = inspect.attr("data-type");
                const columnId = inspect.attr("data-prop-id");
                const colId = columnCtx.attr("data-column-id");
                const threads = [];
                const fieldData = viewModel.result.entityFields.find(obj => {
                    return obj.id === columnId;
                });

                const viewModelConfigurations = viewModel.result.viewModelFields.find(x => {
                    return x.id === colId;
                });

                if (!inspect) globalResolve();
                const pr1 = new Promise((pr1Resolve, pr2Reject) => {
                    if (!fieldData) pr1Resolve();
                    const propName = fieldData.name;
                    const parsedPropName = propName.toLowerFirstLetter();

                    const value = inspect.val();

                    switch (type) {
                        case "bool":
                            {
                                obj[parsedPropName] = inspect.prop("checked");
                                pr1Resolve();
                            }
                            break;
                        case "uniqueidentifier":
                            {
                                const refEntity = inspect.attr("data-ref-entity");
                                this.db.getByIdWithIncludesAsync(refEntity, value).then(refObject => {
                                    if (refObject.is_success) {
                                        obj[`${parsedPropName}Reference`] = refObject.result;
                                        obj[parsedPropName] = value;
                                    } else {
                                        this.toast.notifyErrorList(refObject.error_keys);
                                    }
                                    pr1Resolve();
                                }).catch(err => { console.warn(err) });
                            }
                            break;
                        default:
                            {
                                obj[parsedPropName] = value;
                                pr1Resolve();
                            }
                            break;
                    }
                }).then(() => {
                    columnCtx.find(".inline-update-event").off("blur", onInputEventHandler);
                    columnCtx.find(".inline-update-event").off("changed", onInputEventHandler);
                });
                threads.push(pr1);
                const pr2 = new Promise((localResolve, localReject) => {
                    if (viewModelConfigurations) {
                        switch (viewModelConfigurations.virtualDataType) {
                            //Many to many
                            case 3:
                                {
                                    const { sourceEntity, sourceSelfParamName, sourceRefParamName, referenceEntityName } =
                                        this.getManyToManyViewModelConfigurations(viewModelConfigurations);
                                    const filters = [{ parameter: sourceSelfParamName.value, value: obj.id }];
                                    this.db.getAllWhereWithIncludesAsync(sourceEntity.value, filters).then(mResult => {
                                        if (mResult.is_success) {
                                            obj[`${sourceEntity.value.toLowerFirstLetter()}Reference`] = mResult.result;
                                        } else {
                                            this.toast.notifyErrorList(mResult.error_keys);
                                        }
                                        localResolve();
                                    }).catch(err => {
                                        console.warn(err);
                                        localResolve();
                                    });
                                }
                                break;
                            default:
                                localResolve();
                                break;
                        }
                    } else localResolve();
                });
                threads.push(pr2);
                Promise.all(threads).then(() => {
                    globalResolve();
                });
            });

            promises.push(forPromise);
        }
        promises.push(this.forceLoadDependenciesOnEditComplete(obj));
        Promise.all(promises).then(results => {
            row.find("td").unbind();
            const additionalDependencies = results[results.length - 1];
            obj = Object.assign(obj, additionalDependencies);
            const redraw = table.row(index).data(obj).invalidate();
            $(redraw.row(index).nodes()).find(".inline-edit").on("click", inlineEditHandler);
        });
    }).catch(err => {
        console.warn(err);
    });
};

/**
 * Helper for do something on edit complete
 * @param {any} obj
 */
TableInlineEdit.prototype.forceLoadDependenciesOnEditComplete = function (obj) {
    return new Promise((resolve, reject) => {
        //do something
        resolve(obj);
    });
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

TableInlineEdit.prototype.elementOffset = function (el) {
    var rect = el.getBoundingClientRect(),
        scrollLeft = window.pageXOffset || document.documentElement.scrollLeft,
        scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    console.log({ top: rect.top + scrollTop, left: rect.left + scrollLeft });
    return { top: rect.top + scrollTop, left: rect.left + scrollLeft };
};