/* Data table render
 * A plugin for render tables in dynamic pages
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica(Indrivo) Srl
 * Author: Lupei Nicolae
 */

// Make sure jQuery has been loaded
if (typeof jQuery === "undefined") {
    throw new Error("Data Table plugin require JQuery");
}

//Find and generate tables or generate manually
window.DisableAutoGenerateTableBuilder = false;

//------------------------------------------------------------------------------------//
//								Table select multiple
//------------------------------------------------------------------------------------//

function RenderTableSelect() {
}

RenderTableSelect.prototype.constructor = RenderTableSelect;

RenderTableSelect.prototype.settings = {
    headContent: () => "#",
    classNameText: "select-checkbox",
    select: {
        style: "multi",
        selector: "td:not(.not-selectable):first-child",
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

/*
 * Define new prototype for TableExport
 */
function TableExport() { };
TableExport.constructor = TableExport;
TableExport.prototype.oldExportAction = function (self, e, dt, button, config) {
    if (button[0].className.indexOf("buttons-excel") >= 0) {
        if ($.fn.dataTable.ext.buttons.excelHtml5.available(dt, config)) {
            $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config);
        } else {
            $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
        }
    } else if (button[0].className.indexOf("buttons-print") >= 0) {
        $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
    } else if (button[0].className.indexOf("buttons-csv") >= 0) {
        $.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config);
    } else if (button[0].className.indexOf("buttons-pdf") >= 0) {
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

    dt.one("preXhr",
        function (e, s, data) {
            // Just this once, load all data from the server...
            data.start = 0;
            data.length = 2147483647;

            dt.one("preDraw",
                function (e, settings) {
                    // Call the original action function
                    new TableExport().oldExportAction(self, e, dt, button, config);

                    dt.one("preXhr",
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
class TBuilderInstance {
    constructor() {
        this.instances = [];
    }

    register(instance) {
        this.instances.push(instance);
    }
}

window.TableBuilderInstances = new TBuilderInstance();
class TableBuilder {
    constructor(configuration) {
        this.showActionsColumn = true;
        this.enableColumnFilters = true;
        this.initialFilters = [];
        const scope = this;
        this.ajax = {
            "url": "/PageRender/LoadPagedData",
            "type": "POST",
            "data": function (config) {
                Object.assign(config, scope.configurations.ajaxParameters);
                config.filters = scope.initialFilters.concat(scope.filters);
                return config;
            }
        };
        this.configurations = {
            tableId: undefined,
            tableJqInstance: undefined,
            tableJsInstance: undefined,
            table: undefined,
            ajaxParameters: {
                viewModelId: undefined
            }
        };
        Object.assign(this, configuration);
        this.filters = [];
        this.registerServices();
        window.TableBuilderInstances.register(this);
    }

	/*
	 * Register services
	*/
    registerServices() {
        this.toast = new ToastNotifier();
        this.db = new DataInjector();
    }

	/**
	 * Init
	 * @param {any} tableEl
	 */
    init(tableEl) {
        const scope = this;
        const listRef = $(tableEl);
        const viewmodelId = listRef.attr("db-viewmodel");
        const listId = listRef.attr("id");
        const hasEditPage = tableEl.hasAttribute("data-is-editable");
        const hasInlineEdit = tableEl.hasAttribute("data-is-editable-inline");
        const hasDeleteRestore = tableEl.hasAttribute("data-allow-edit-restore");
        const hasDeleteForever = tableEl.hasAttribute("data-allow-delete-forever");
        const editPageLink = listRef.attr("data-edit-href");
        this.configurations.tableJsInstance = tableEl;
        this.configurations.tableJqInstance = listRef;
        this.configurations.tableId = listId;
        this.configurations.viewmodelId = viewmodelId;

        loadAsync(`/PageRender/GetViewModelById?viewModelId=${viewmodelId}`).then(viewmodelData => {
            scope.configurations.table = viewmodelData.result.tableModel;

            if (viewmodelData.is_success) {
                const gSettings = {
                    viewmodelData: viewmodelData.result,
                    hasEditPage: hasEditPage,
                    hasInlineEdit: hasInlineEdit,
                    hasDeleteRestore: hasDeleteRestore,
                    editPageLink: editPageLink,
                    listId: listId,
                    viewmodelId: viewmodelId,
                    hasDeleteForever: hasDeleteForever
                };
                scope.configurations = Object.assign(scope.configurations, gSettings);
                return gSettings;
            }
            else {
                throw viewmodelData.error_keys;
            }
        }).then(dataX => {
            this.configureTableBody(dataX);

            $(".dynamic-table thead th:not(.no-sort)").on("click", function () {
                const select = $(this).closest("thead").find("th.no-sort").find("input[type='checkbox']");
                const state = select.is(":checked");
                if (select && state) select.get(0).checked = false;
            });
        });
    }

	/**
	 * Configure table body
	 * @param {any} dataX
	 */
    configureTableBody(dataX) {
        const ctx = this;
        const renderColumns = [];
        if (dataX.viewmodelData.viewModelFields.length > 0) {
            const renderTableSelect = new RenderTableSelect();
            const columns = $(`#${dataX.listId} thead`); columns.html(null);
            const tr = document.createElement("tr");
            const th = document.createElement("th");
            th.setAttribute("class", "no-sort");
            th.innerHTML = renderTableSelect.settings.headContent();
            tr.appendChild(th);
            //CheckBox column
            renderColumns.push({
                data: null,
                "render": function (data, type, row, meta) {
                    return renderTableSelect.templateSelect(data, type, row, meta);
                }
            });

            $.each(dataX.viewmodelData.viewModelFields,
                function (index, column) {
                    let colName = column.name;
                    if (column.translate) {
                        colName = window.translate(column.translate);
                    }
                    const htmlCol = document.createElement("th");
                    let isRequired = false;
                    if (column.tableModelFields) {
                        isRequired = !column.tableModelFields.allowNull;
                    }
                    htmlCol.innerHTML = `${colName}${isRequired ? " <span style='color:red'>*</span>" : ""}`;
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

            //Show action columns
            if (this.showActionsColumn) {
                const actionCol = document.createElement("th");
                actionCol.innerHTML = window.translate("list_actions");
                tr.appendChild(actionCol);

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

            columns.append(tr);
            ctx.addColumnFilters(columns);
        }
        ctx.renderTable({
            viewmodelId: dataX.viewmodelId,
            listId: dataX.listId,
            renderColumns: renderColumns
        });
    }

	/**
	 * Add filter
	 * @param {any} filter
	 */
    injectFilter(filter = {
        parmeter: "",
        searchValue: "",
        criteria: "equals"
    }, target = null) {
        const exist = this.filters.find(x => x.parameter == filter.parameter);
        if (!exist) {
            this.filters.push(filter);
            target.css("background", "#2A4B65");
        }
        else if (filter.value) {
            this.filters.update(x => x.parameter == filter.parameter, filter);
            target.css("background", "#2A4B65");
        } else {
            this.filters = this.filters.filter(x => x.parameter != filter.parameter);
            target.css("background", "");
        }

        this.jTableInstance.ajax.reload();
    }

	/**
	 * Get filter by name
	 * @param {any} parameter
	 */
    getFilter(parameter) {
        return this.filters.find(x => x.parameter == parameter);
    }

	/**
	 * Add column filters
	 * @param {any} columnsEl
	 */
    addColumnFilters(columnsEl) {
        const scope = this;
        const rowFilters = $(`<tr style="background: #a4c4fd;  height: 10px"> <th class="no-sort"> </th> </tr>`);
        $.each(this.configurations.viewmodelData.viewModelFields, (i, vField) => {
            rowFilters.append(scope.pushFilterColumn(vField));
        });
        if (scope.showActionsColumn) {
            rowFilters.append(scope.pushFilterColumn({}));
        }
        columnsEl.append(rowFilters);
        $(".sorting").off();
    }

	/**
	 * Add column filter
	 * @param {any} vField
	 */
    pushFilterColumn(vField) {
        const scope = this;
        const el = $(`<th>
				<div class="row">
						<div class="col-md row-filter">
                               <div class="filter">
                                        <i style="cursor: pointer" class="fa fa-filter" aria-hidden="true"></i>
                                    </div>
                                </div>
                            </div>
					</th>`);
        const filterEl = el.find(".fa-filter");
        if (vField.tableModelFields) {
            const fieldName = vField.tableModelFields.name;
            switch (vField.tableModelFields.dataType) {
                case "nvarchar":
                case "int32":
                case "decimal":
                    {
                        filterEl.on("click", function () {
                            const f = scope.getFilter(fieldName);
                            const conf = {
                                id: fieldName,
                                searchBarPlaceholder: "Caută",
                                searchValue: f ? f.searchValue : "",
                                addButtonLabel: "",
                                textEmitEventTimeout: 300
                            };
                            const item = $.Iso.DynamicFilter('text', this, null, null, conf);

                            $(item.dynamicSelect).on('filterValueChange', function (event, arg) {
                                scope.injectFilter({
                                    parameter: fieldName,
                                    criteria: "contains",
                                    searchValue: arg.value
                                }, el);
                            });
                        });
                    }
                    break;
                case "uniqueidentifier":
                    {
                        filterEl.remove();
                        //filterEl.on('click', function (event) {
                        //    const item = $.Iso.dynamicFilter('list',
                        //        event.target,
                        //        [
                        //            {
                        //                id: '1',
                        //                value: 'First Option'
                        //            }
                        //        ],
                        //        null);

                        //    $(item.container).on('selectValueChange',
                        //        (event, arg) => {
                        //        });
                        //});
                    }
                    break;
                case "bool":
                    {
                        filterEl.remove();
                    }
                    break;
                case "datetime":
                case "date":
                    {
                        filterEl.on('click', () => {
                            const picker = $.Iso.DatePicker(filterEl[0], {
                                format: 'dd/mm/yyyy',
                                todayHighlight: true,
                                autoclose: true
                            }, {
                                placement: 'top-start'
                            });

                            // bind to date change
                            $(picker).on('rangeChangeDate', (event) => {
                            });

                            // bind to closing datepicker
                            $(picker).on('closeDatePickerPopper', (event) => {
                                let maxDate = new Date(8640000000000000);
                                let minDate = new Date(-8640000000000000);

                                let start = event.detail.fromDate;
                                let end = event.detail.toDate;
                                if (!start) {
                                    start = minDate.toLocaleDateString();
                                } else start = start.toLocaleDateString();

                                if (!end) {
                                    end = maxDate.toLocaleDateString();
                                } else end = end.toLocaleDateString();

                                scope.injectFilter({
                                    parameter: fieldName,
                                    criteria: "dateRange",
                                    searchValue: `${start},${end}`
                                }, el);
                            });
                        });
                    }
                    break;
            }
        } else {
            filterEl.remove();
        }

        return el;
    }

	/**
	 * Render table
	 * @param {any} data
	 */
    renderTable(data) {
        this.configurations.ajaxParameters.viewModelId = data.viewmodelId;
        const table = this.configurations.tableJqInstance;
        const renderTableSelect = new RenderTableSelect();
        let dtConfig = Object.assign({
            "language": {
                "url": this.translationsJson()
            },
            dom: this.dom,
            buttons: this.buttons,
            columnDefs: [
                {
                    orderable: false,
                    className: renderTableSelect.className(),
                    targets: "no-sort"
                }
            ],
            "order": [[1, "desc"]],
            colReorder: true,
            select: renderTableSelect.settings.select,
            columnSorting: false,
            orderCellsTop: true,
            orderable: false,
            "scrollX": true,
            "scrollCollapse": true,
            "autoWidth": true,
            "processing": true,
            "serverSide": true,
            "filter": true,
            "orderMulti": false,
            "destroy": true,
            "ajax": this.ajax,
            "columns": data.renderColumns,
            "createdRow": (row, data, dataIndex) => this.onRowCreate(row, data, dataIndex),
            "rowCallback": (row, data) => this.rowCallback(row, data),
            "createdCell": (td, cellData, rowData, row, col) => this.createdCell(td, cellData, rowData, row, col),
            "initComplete": (settings, json) => this.onInitComplete(settings, json)
        }, this.dtConfs);

        this.jTableInstance = table.DataTable(dtConfig);
    }
}

/**
 * Restore item
 * @param {any} rowId
 * @param {any} tableId
 * @param {any} viewModelId
 */
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

/**
 * Delete item
 * @param {any} rowId
 * @param {any} tableId
 * @param {any} viewModelId
 */
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
            loadAsync("/InlineEdit/DeleteItemFromDynamicEntity", {
                id: rowId,
                viewModelId: viewModelId
            }, "post").then(data => {
                if (data.is_success) {
                    const oTable = $(`${tableId}`).DataTable();
                    oTable.draw();
                    this.toast.notify({ heading: window.translate("row_deleted"), icon: "success" });
                } else {
                    this.toast.notifyErrorList(data.error_keys);
                }
            }).catch(err => {
                console.log(err);
                this.toast.notify({ heading: window.translate("delete_fail"), text: window.translate("api_not_respond") });
            });
        }
    });
};

/**
 * Delete item forever
 * @param {any} rowId
 * @param {any} tableId
 * @param {any} viewModelId
 */
TableBuilder.prototype.deleteItemForever = function (rowId, tableId, viewModelId) {
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
            loadAsync("/InlineEdit/DeleteItemForeverFromDynamicEntity", {
                id: rowId,
                viewModelId: viewModelId
            }, "post").then(data => {
                if (data.is_success) {
                    const oTable = $(`${tableId}`).DataTable();
                    oTable.draw();
                    this.toast.notify({ heading: window.translate("row_deleted"), icon: "success" });
                } else {
                    this.toast.notifyErrorList(data.error_keys);
                }
            }).catch(err => {
                this.toast.notify({ heading: window.translate("delete_fail"), text: window.translate("api_not_respond") });
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
        console.warn(e);
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
        + this.getTableRowEditActionButton(row, dataX)
        + this.getTableRowDeleteRestoreActionButton(row, dataX)
        + this.getTableDeleteForeverActionButton(row, dataX);
    return container;
}

/**
 * Delete logic action table
 * @param {any} row
 * @param {any} dataX
 */
TableBuilder.prototype.getTableRowDeleteRestoreActionButton = function (row, dataX) {
    return `${dataX.hasDeleteRestore
        ? `${row.isDeleted
            ? `<a href="javascript:void(0)" class='btn restore-item btn-warning btn-sm' onclick="new TableBuilder().restoreItem('${row.id
            }', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')">${window.translate("restore")}</a>`
            : `<a href="javascript:void(0)" class='btn btn-danger btn-sm' onclick="new TableBuilder().deleteItem('${row.id
            }', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')">${window.translate("delete")}</a>`}`
        : ""}`;
};

/**
 * Inline edit action table
 * @param {any} row
 * @param {any} dataX
 */
TableBuilder.prototype.getTableRowInlineActionButton = function (row, dataX) {
    if (row.isDeleted) return "";
    return `${dataX.hasInlineEdit
        ? `	<a data-viewmodel="${dataX.viewmodelData.result.id
        }" class="inline-edit btn btn-warning btn-sm" href="javascript:void(0)">${window.translate("inline_edit")}</a>`
        : ``}`;
};

/**
 * Delete forever action table
 * @param {any} row
 * @param {any} dataX
 */
TableBuilder.prototype.getTableDeleteForeverActionButton = function (row, dataX) {
    return `${dataX.hasDeleteForever
        ? `	<a data-viewmodel="${dataX.viewmodelData.result.id
        }" class="delete-forever btn btn-warning btn-sm" href="javascript:void(0)">Delete forever</a>`
        : ``}`;
};

/**
 * Edit with forms action table
 * @param {any} row
 * @param {any} dataX
 */
TableBuilder.prototype.getTableRowEditActionButton = function (row, dataX) {
    if (row.isDeleted) return "";
    return `${dataX.hasEditPage ? `<a class="btn btn-info btn-sm" href="${dataX.editPageLink}?itemId=${row.id
        }&&listId=${dataX.viewmodelData.result.id}">${window.translate("edit")}</a>`
        : ``}`;
};

/*
 * Delete/Restore selected rows
*/
TableBuilder.prototype.deleteSelectedRows = function () {
    this.deleteSelectedRowsHandler(this);
}

/**
 * Handler for delete/restore items
 * @param {any} ctx
 */
TableBuilder.prototype.deleteSelectedRowsHandler = function (ctx) {
    const tableId = ctx.table().node().id;
    if (!tableId) {
        return this.toast.notify({ heading: "Something did not work" });
    }
    const selected = ctx.rows({ selected: true }).data();
    const viewModelId = $(`#${tableId}`).attr("db-viewmodel");
    if (selected.length > 0) {
        const toDeleteItems = Array.from(selected.filter(x => !x.isDeleted).map(x => {
            return x.id;
        }));

        const toRestoreItems = Array.from(selected.filter(x => x.isDeleted).map(x => {
            return x.id;
        }));

        const promises = [];
        if (toDeleteItems.length > 0) {
            promises.push(this.deleteRestoreApiAsync(ctx, toDeleteItems, viewModelId, true));
        }

        if (toRestoreItems.length > 0) {
            promises.push(this.deleteRestoreApiAsync(ctx, toRestoreItems, viewModelId, false));
        }

        Promise.all(promises).then(x => {
            ctx.ajax.reload();
            this.toast.notify({ heading: window.translate("items_deleted"), icon: "success" });
        }).catch(e => {
            this.toast.notify({ heading: window.translate("fail_delete_items") });
        });
    } else {
        this.toast.notify({ heading: window.translate("delete_no_selected_items"), icon: "warning" });
    }
};

/**
 * Delete items forever
 * @param {any} ctx
 */
TableBuilder.prototype.deleteSelectedRowsPermanentHandler = function (ctx) {
    const scope = this;
    const tableId = ctx.table().node().id;
    if (!tableId) {
        return scope.toast.notify({ heading: "Something did not work" });
    }
    const selected = ctx.rows({ selected: true }).data();

    if (selected.length === 0)
        return this.toast.notify({ heading: window.translate("delete_no_selected_items"), icon: "warning" });

    swal({
        title: window.translate("system_delete_permanent_multiple_entries"),
        text: "",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: window.translate("system_delete_multiple_confirm"),
        cancelButtonText: window.translate("cancel")
    }).then((result) => {
        if (result.value) {
            const viewModelId = $(`#${tableId}`).attr("db-viewmodel");
            if (selected.length > 0) {
                const toDeleteItems = Array.from(selected.map(x => {
                    return x.id;
                }));

                this.deletePermanentApiAsync(ctx, toDeleteItems, viewModelId)
                    .then(() => {
                        ctx.ajax.reload();
                        scope.toast.notify({ heading: window.translate("items_deleted"), icon: "success" });
                    }).catch(e => {
                        ctx.ajax.reload();
                        scope.toast.notifyErrorList(e);
                    });
            } else {
                scope.toast.notify({ heading: window.translate("delete_no_selected_items"), icon: "warning" });
            }
        }
    });
};

/**
 * Delete items from api
 * @param {any} ctx
 * @param {any} data
 * @param {any} viewModelId
 * @param {any} mode
 */
TableBuilder.prototype.deleteRestoreApiAsync = function (ctx, data, viewModelId, mode) {
    return new Promise((resolve, reject) => {
        window.loadAsync("/PageRender/DeleteItemsFromDynamicEntity",
            {
                ids: data,
                viewModelId: viewModelId,
                mode: mode
            },
            "post").then(req => {
                if (req && req.success) {
                    resolve();
                } else {
                    reject();
                }
            }).catch(err => {
                reject();
                console.warn(err);
            });
    });
};

/**
 * Delete items from api
 * @param {any} ctx
 * @param {any} data
 * @param {any} viewModelId
 * @param {any} mode
 */
TableBuilder.prototype.deletePermanentApiAsync = function (ctx, data, viewModelId) {
    return new Promise((resolve, reject) => {
        window.loadAsync("/PageRender/DeletePermanentItemsFromDynamicEntity",
            {
                ids: data,
                viewModelId: viewModelId
            },
            "post").then(req => {
                if (req.is_success) {
                    resolve();
                } else {
                    reject(req.error_keys);
                }
            }).catch(err => {
                reject(err);
            });
    });
};

//Table buttons
TableBuilder.prototype.buttons = [
    {
        extend: "copyHtml5",
        text: '<i class="fa fa-files-o"></i>',
        exportOptions: {
            columns: ":visible"
        },
        className: ""
    },
    {
        extend: "csvHtml5",
        text: '<i class="fa fa-file-text-o"></i>',
        exportOptions: {
            columns: ":visible"
        },
        action: new TableExport().newExportAction
    },
    {
        extend: "excelHtml5",
        text: '<i class="fa fa-file-excel-o"></i>',
        autoFilter: true,
        exportOptions: {
            columns: ":visible",
            search: "applied",
            order: "applied"
        },
        action: new TableExport().newExportAction
    },
    {
        extend: "pdfHtml5",
        text: '<i class="fa fa-file-pdf-o"></i>',
        exportOptions: {
            columns: ":visible"
        },
        action: new TableExport().newExportAction
    },
    {
        extend: "print",
        exportOptions: {
            columns: ":visible"
        },
        action: new TableExport().newExportAction
    },
    {
        text: "Delete selected items",
        action: new TableBuilder().deleteSelectedRows
    }
];

TableBuilder.prototype.createdCell = function (td, cellData, rowData, row, col) {
    //on created cell
};

TableBuilder.prototype.rowCallback = function (row, data) {
    //on callback
};

/**
 * On row create
 * @param {any} row
 * @param {any} data
 * @param {any} dataIndex
 */
TableBuilder.prototype.onRowCreate = function (row, data, dataIndex) {
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

/*
 * JQuery dt dom position
 */
TableBuilder.prototype.dom = '<"CustomizeColumns">lBfrtip';

/**
 * Set your own props to be translated
 */
TableBuilder.prototype.replaceTableSystemTranslations = function () {
    const customReplace = new Array();
    const searialData = JSON.stringify(customReplace);
    return searialData;
};

/*
 * Get translations for jquery dt
 */
TableBuilder.prototype.translationsJson = function () {
    return `${location.origin}/api/LocalizationApi/GetJQueryTableTranslations?language=${window.getCookie("language")}&customReplace=${this.replaceTableSystemTranslations()}`;
};

/*
 * Additional configs for dt
 */
TableBuilder.prototype.dtConfs = {};

/**
 * Handler after init complete
 * @param {any} settings
 * @param {any} json
 */
TableBuilder.prototype.onInitComplete = function (settings, json) {
    //do something after table complete
}

TableBuilder.prototype.appendColumnsBeforeActions = function () {
    return "";
};

$(document).ready(function () {
    if (window.DisableAutoGenerateTableBuilder) return;
    const tables = Array.prototype.filter.call(
        document.getElementsByTagName("table"),
        function (el) {
            return el.getAttribute("db-viewmodel") != null;
        }
    );

    $.each(tables,
        function (index, tableEl) {
            const conf = {
                showActionsColumn: false
            };
            const builder = new TableBuilder(conf);

            builder.init(tableEl);
        });
});