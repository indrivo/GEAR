class BaseClass {
    get(parameterName = "") {
        return this[parameterName];
    }

    set(parameterName = "", value = "") {
        this[parameterName] = value;
    }
}

class DashboardManager extends BaseClass {
    /**
     * Constructor
     */
    constructor() {
        super();
        //Inject helper
        this.helper = new ST();
        this.toast = new ToastNotifier();

        $(".droppable-row").sortable({ axis: "y" });
        $(".droppable-row").on("sortchange",
            function (event, ui) {
                //console.log(ui);
            });
    }

    /**
     * Init service
     */
    init() {
        const scope = this;
        scope.setConfiguration();
        scope.loadServerConfiguration();
        $(".draggable-row, .draggable-widget").draggable({
            revert: "invalid",
            helper: "clone",
            cursor: "move"
        });

        //drag rows
        $(".droppable-row").droppable({
            accept: ".draggable-row",
            drop: function (event, ui) {
                scope.addRow();
            }
        });

        this.initDraggableWidgets();
    }

    getWidgetIndexesFromMenuItem(menuItem) {
        const menu = $(menuItem).closest(".options-menu");
        const widget = menu.data("extra");
        const widgetId = widget.attr("data-id");
        const row = widget.closest(".draggable-row-injected");
        const rowId = row.attr("data-id");

        return { widgetId, rowId };
    }

    closeMenu(menuItem) {
        $(menuItem).closest(".options-menu").remove();
    }

    getAclConfiguration(widgetId, rowId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/Dashboard/GetWidgetConf",
                data: {
                    rowId: rowId,
                    widgetId: widgetId
                },
                success: function (sResult) {
                    if (sResult.is_success) resolve(sResult.result);
                    else reject(sResult.error_keys);
                },
                error: function (e) {
                    reject(e);
                }
            });
        });
    }

    saveAclConfiguration(widgetId, rowId, conf = []) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/Dashboard/UpdateAclForWidget",
                method: "post",
                data: { widgetId, rowId, conf },
                success: function (sResult) {
                    if (sResult.is_success) resolve(sResult.result);
                    else reject(sResult.error_keys);
                },
                error: function (e) {
                    reject(e);
                }
            });
        });
    }

    getUISettingsForWidget(widgetId, rowId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/Dashboard/GetUISettingsForWidget",
                data: {
                    rowId: rowId,
                    widgetId: widgetId
                },
                success: function (sResult) {
                    if (sResult.is_success) resolve(sResult.result);
                    else reject(sResult.error_keys);
                },
                error: function (e) {
                    reject(e);
                }
            });
        });
    }

    setConfiguration() {
        const scope = this;
        this.set("widgetMenuConf",
            [
                {
                    label: window.translate("system_customization"),
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            const modal = $("#widget-configuration-modal");
                            const { widgetId, rowId } = scope.getWidgetIndexesFromMenuItem(this);
                            const form = $("#configuration-form");
                            scope.getUISettingsForWidget(widgetId, rowId).then(data => {
                                console.log(data);
                                scope.helper.populateForm(form, data);
                            });
                            modal.modal("show");

                            $("#configuration-modal-save").off().on("click", function () {
                                const o = {};

                                const dataForm = form.serializeArray();
                                $.each(dataForm,
                                    (i, j) => {
                                        o[j.name] = j.value;
                                    });

                                $.ajax({
                                    url: "/api/Dashboard/UpdateUISettings",
                                    method: "post",
                                    data: {
                                        rowId, widgetId, uiSettings: o
                                    },
                                    success: function (sResult) {
                                        if (sResult.is_success) {
                                            scope.toast.notifySuccess("Info", "Configuration was saved!");
                                            modal.modal("hide");
                                            form.get(0).reset();
                                        } else {
                                            scope.toast.notifyErrorList(sResult.error_keys);
                                        }
                                    },
                                    error: function (e) {
                                        scope.toast.notifyErrorList(sResult.error_keys);
                                    }
                                });
                            });

                            scope.closeMenu(this);
                        }
                    }
                },
                {
                    label: window.translate("roles"),
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            const { widgetId, rowId } = scope.getWidgetIndexesFromMenuItem(this);
                            scope.getAclConfiguration(widgetId, rowId).then(data => {
                                const container = $("#widget-roles-container");
                                const modal = $("#widget-permissions-modal");
                                container.find("input[type='checkbox']").prop('checked', false);
                                modal.modal("show");

                                $.each(data, (index, item) => {
                                    const el = container.find(`input[data-id='${item.roleId}']`);
                                    el.prop('checked', item.allow);
                                });

                                $("#form-permissions-btn").off().on("click", function () {
                                    const checkedRoles = container.find("input[type='checkbox']");
                                    const roles = [];
                                    $.each(checkedRoles, (index, item) => {
                                        const el = $(item);
                                        const roleId = el.data("id");
                                        const state = el.is(":checked");
                                        roles.push({
                                            roleId, allow: state
                                        });
                                    });
                                    scope.saveAclConfiguration(widgetId, rowId, roles).then(() => {
                                        scope.toast.notifySuccess("Info", "Configuration was saved!");
                                        modal.modal("hide");
                                    }).catch(e => {
                                        scope.toast.notifyErrorList(e);
                                    });
                                });
                            }).catch(e => {
                                console.warn(e);
                            });

                            scope.closeMenu(this);
                        }
                    }
                },
                {
                    label: "Remove",
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            const { widgetId, rowId } = scope.getWidgetIndexesFromMenuItem(this);
                            $.ajax({
                                url: "/api/Dashboard/DeleteMappedWidget",
                                method: "delete",
                                data: {
                                    widgetId: widgetId,
                                    rowId: rowId
                                },
                                success: (sData) => {
                                    if (sData.is_success) {
                                        scope.toast.notify({ icon: "success", heading: "Info", text: "Widget deleted!" });
                                        widget.remove();
                                    } else {
                                        widget.remove();
                                    }
                                },
                                error: (e) => {
                                    scope.toast.notify({ heading: "Server error" });
                                }
                            });

                            scope.closeMenu(this);
                        }
                    }
                },
                {
                    label: "Exit",
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            $(this).closest(".options-menu").remove();
                        }
                    }
                }
            ]);

        this.set("rowMenuConf",
            [
                {
                    label: "Remove",
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            const menu = $(this).closest(".options-menu");
                            const row = menu.data("extra");
                            const rowId = row.attr("data-id");
                            if (rowId) scope.removeRow(rowId);
                            else {
                                row.remove();
                                scope.toast.notify({
                                    heading: "Info",
                                    text: "Row was deleted",
                                    icon: "success"
                                });
                            }
                            $(this).closest(".options-menu").remove();
                        }
                    }
                },
                {
                    label: "Exit",
                    events: ["click"],
                    eventHandlers: {
                        click: function () {
                            $(this).closest(".options-menu").remove();
                        }
                    }
                }
            ]);
    }

    removeRow(rowId = "") {
        const scope = this;
        const row = $(".droppable-row").find(`div[data-id='${rowId}']`);
        $.ajax({
            url: "/api/DashBoard/DeleteRow",
            method: "delete",
            data: {
                rowId: rowId
            },
            success: (data) => {
                if (data.is_success) {
                    scope.toast.notify({
                        heading: "Info",
                        text: "Row was deleted",
                        icon: "success"
                    });
                    row.remove();
                } else {
                    scope.toast.notifyErrorList(data.error_keys);
                }
            },
            error: (e) => {
                scope.toast.notify({
                    heading: "Alert",
                    text: "Server error"
                });
            }
        });
    }

    addRow(id = "", widgets = []) {
        const scope = this;
        const row = $(`<div data-id="${id}" class="row droppable-widget draggable-row-injected"></div>`);
        if (widgets.length > 0) {
            $.each(widgets, (i, widget) => {
                row.append(scope.getWidgetFromTemplate(widget.groupId, widget.id, widget.name));
            });
        }
        $(".droppable-row").append(row);
        row.contextmenu(function (event) {
            if ($(event.target).hasClass("draggable-widget-injected")) return;
            event.preventDefault();
            $(".options-menu").remove();
            const x = event.pageX;
            const y = event.pageY;
            const el = $(scope.getMenuConfiguration("rowMenuConf"));
            el.css("position", "absolute");
            el.css("left", `${x}px`);
            el.css("top", `${y}px`);
            el.menu();
            el.data("extra", $(this));
            $("body").append(el);

            $("*:not(.options-menu)").on("click",
                function () {
                    $(".options-menu").remove();
                    $(this).off("click");
                });
        });
        this.initDraggableWidgets();
    }

    loadServerConfiguration() {
        const scope = this;
        $.ajax({
            url: "/api/Dashboard/GetDashboardRows",
            data: {
                dashboardId: scope.get("dashboardId")
            },
            success: (data) => {
                if (data.is_success) {
                    $.each(data.result,
                        (i, item) => {
                            scope.addRow(item.rowId, item.widgets);
                        });
                    scope.registerDroppableWidgets();
                }
            },
            error: (e) => {
                scope.toast.notify({ heading: e });
            }
        });
    }


    checkForDuplicateWidgetsInRow(row, widgetId) {
        const match = $(row).find(".draggable-widget-injected").filter((index, item) => {
            return $(item).attr("data-id") == widgetId;
        });
        return match.length > 0;
    }

    //init draggable events for widgets
    initDraggableWidgets() {
        const scope = this;
        //drag widgets
        $(".droppable-widget").droppable({
            accept: ".draggable-widget",
            drop: function (event, ui) {
                const source = $(ui.draggable);
                const widgetId = source.data("id");
                const exist = scope.checkForDuplicateWidgetsInRow(this, widgetId);
                if (!exist) {
                    const groupId = source.data("group-id");
                    const widget = scope.getWidgetFromTemplate(groupId, widgetId, source.text());
                    $(this).append(widget);
                    scope.registerDroppableWidgets();
                    $(".droppable-widget").sortable({ axis: "x" }).disableSelection();
                } else {
                    scope.toast.notify({ heading: "A widget can only appear once in a row" });
                }
            }
        });
    }

    getWidgetFromTemplate(groupId, widgetId, text) {
        const widget = $(`<div class="col-md draggable-widget-injected" data-group-id="${groupId}" data-id="${widgetId}">${text}</div>`);
        return widget;
    }

    registerDroppableWidgets() {
        const scope = this;
        const ctx = $(".draggable-widget-injected");
        ctx.draggable({
            revert: "invalid",
            helper: "clone",
            cursor: "move",
            start: function (event, ui) {
                const el = $(ui.helper);
                const width = el.width();
                const cols = el.parent().children().length - 1;
                const maxWidth = width / cols;
                el.css("width", maxWidth);
            }
        });

        ctx.contextmenu(function (event) {
            event.preventDefault();
            $(".options-menu").remove();
            const x = event.pageX;
            const y = event.pageY;
            const el = scope.getMenuConfiguration("widgetMenuConf");
            $(el).css("position", "absolute");
            $(el).css("left", `${x}px`);
            $(el).css("top", `${y}px`);
            $(el).data("extra", $(this));
            $(el).menu();
            $("body").append(el);

            //$("body:not(.options-menu)").on("click",
            //	function () {
            //		$(".options-menu").remove();
            //		$("body:not(.options-menu)").off("click");
            //	});
        });
    }

    getMenuConfiguration(id = "") {
        const scope = this;
        const menuBlock = document.createElement("ul");
        menuBlock.setAttribute("class", "options-menu card");
        const configurations = scope.get(id);
        for (let i = 0; i < configurations.length; i++) {
            const item = document.createElement("li");
            item.innerHTML = configurations[i].label;
            for (let j = 0; j < configurations[i].events.length; j++) {
                const evt = configurations[i].events[j];
                item.addEventListener(evt, configurations[i].eventHandlers[evt]);
            }
            menuBlock.appendChild(item);
        }

        return menuBlock;
    }

    saveAsync() {
        const conf = {
            dashboardId: this.get("dashboardId"),
            rows: []
        };
        const domRows = $(".droppable-row").find(".draggable-row-injected");
        $.each(domRows,
            (index, item) => {
                const jItem = $(item);
                const rowWidgets = jItem.find(".draggable-widget-injected");
                const widgets = [];
                $.each(rowWidgets, (i, widget) => {
                    const jWidget = $(widget);
                    widgets.push({
                        order: i,
                        id: jWidget.attr("data-id"),
                        groupId: jWidget.attr("data-group-id")
                    });
                });
                conf.rows.push({
                    order: index,
                    rowId: jItem.attr("data-id"),
                    widgets: widgets
                });
            });

        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/Dashboard/SaveDashboardConfiguration",
                data: conf,
                method: "post",
                success: (data) => {
                    if (data.is_success) {
                        $.each(data.result,
                            (i, item) => {
                                $(domRows[item.order]).attr("data-id", item.rowId);
                            });
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (err) => {
                    reject(err);
                }
            });
        });
    }
}