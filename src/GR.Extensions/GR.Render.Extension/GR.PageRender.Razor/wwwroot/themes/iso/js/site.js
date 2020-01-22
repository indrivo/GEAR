var PreLoader;

$(window).on("load",
    function () {
        moment.locale(settings.localization.current.identifier);
        $(".loader-wrapper").not(".incomponent").fadeOut(400,
            function () {
                PreLoader = $(this).detach();
            });
    });

/* Dom Ready */
(function ($) {
    const $body = $("body");

    /* Initialize Tooltip */
    $('[data-toggle="tooltip"]').tooltip();

    /* Initialize Popover */
    $('[data-toggle="popover"]').popover();

    /* Initialize Lightbox */
    $body.delegate('[data-toggle="lightbox"]',
        "click",
        function (event) {
            event.preventDefault();
            $(this).ekkoLightbox();
        });

	/************************************************
	 Append Preloader (use in ajax call)
	 ************************************************/
    $body.delegate(".append-preloader",
        "click",
        function () {
            $(PreLoader).show();
            $body.append(PreLoader);
            setTimeout(function () {
                $(".loader-wrapper").fadeOut(200,
                    function () {
                        PreLoader = $(this).detach();
                    });
            },
                1000);
        });

    $(".logo-holder").click(function () {
        location.href = "/Home";
    });

	/************************************************
	 Toggle Preloader in card or box
	 ************************************************/
    $body.delegate('[data-toggle="loader"]',
        "click",
        function () {
            var target = $(this).attr("data-target");
            $("#" + target).show();
        });

	/************************************************
	 Toggle Sidebar Nav
	 ************************************************/

    // $(".sidebar.collapsed li.open .sub-nav").css({ "display": "none" });

    var activeMenuItem = $(".sidebar li.active").parent().parent();
    activeMenuItem.css({ 'background-color': 'rgba(0, 0, 0, 0.15)' });

    $body.delegate('.toggle-sidebar', 'click', function () {
        $('.sidebar').toggleClass('collapsed');

        if (localStorage.getItem("asideMode") === 'collapsed') {
            localStorage.setItem("asideMode", 'expanded')
        } else {
            localStorage.setItem("asideMode", 'collapsed')
        }
        return false;
    });

    var p;
    $body.delegate('.hide-sidebar', 'click', function () {
        if (p) {
            p.prependTo(".wrapper");
            p = null;
        } else {
            p = $(".sidebar").detach();
        }
    });

    $.fn.setAsideMode = function () {
        if (localStorage.getItem("asideMode") === null) {
        } else if (localStorage.getItem("asideMode") === 'collapsed') {
            $('.sidebar').addClass('collapsed');
        } else {
            $('.sidebar').removeClass('collapsed');
        }
    };
    if ($(window).width() > 768) {
        $.fn.setAsideMode();
    }

    /************************************************
     Sidebar Nav Accordion
     ************************************************/
    $body.on('click', '.navigation li:has(.sub-nav) > a', function () {
        /*$('.navigation li').removeClass('open');*/
        $(this).siblings('.sub-nav').slideToggle();
        $(this).parent().toggleClass('open');
        return false;
    });

	/************************************************
     Sidebar Colapsed state submenu position
	 ************************************************/
    $body.find('.navigation ul li:has(.sub-nav)').on('mouseover', function () {
        activeMenuItem.addClass('open');

        $('.sidebar.collapsed .navigation li.open').find('.sub-nav').css({ "display": "block" });

        if ($(".sidebar").hasClass("collapsed")) {
            const $menuItem = $(this),
                $submenuWrapper = $('> .sub-nav', $menuItem);
            // grab the menu item's position relative to its positioned parent
            const menuItemPos = $menuItem.position();

            // place the submenu in the correct position relevant to the menu item
            $submenuWrapper.css({
                top: menuItemPos.top,
                left: menuItemPos.left + $menuItem.outerWidth()
            });
        }
    });

	/************************************************
     On mouseleave collapse in menu items
	 ************************************************/
    $body.find('.sidebar.collapsed .navigation').on('mouseleave', function () {
        $('.sidebar .navigation li.active').parents('li').last().addClass('active');

        $(".sidebar.collapsed .navigation > #left-nav-bar > li.open").each(function (index) {
            $(this).find('.sub-nav').first().css({ "display": "none" });
        });

    });
    $body.find('.sidebar.collapsed .navigation').on('mouseover', function () {
        activeMenuItem.addClass('open');

        $(".sidebar.collapsed .navigation li.open").each(function (index) {
            $(this).find('.sub-nav').first().css({ "display": "block" });
        });
    });

	/************************************************
	 Toggle Controls on small devices
	 ************************************************/
    $body.delegate(".toggle-controls",
        "click",
        function () {
            $(".controls-wrapper").toggle().toggleClass("d-none");
        });

	/************************************************
	 Toast Messages
	 ************************************************/
    $body.delegate('[data-toggle="toast"]',
        "click",
        function () {
            var dataAlignment = $(this).attr("data-alignment");
            var dataPlacement = $(this).attr("data-placement");
            var dataContent = $(this).attr("data-content");
            var dataStyle = $(this).attr("data-style");

            if ($(".toast." + dataAlignment + "-" + dataPlacement).length) {
                $(".toast." + dataAlignment + "-" + dataPlacement).append(
                    '<div class="alert alert-dismissible fade show alert-' +
                    dataStyle +
                    ' "> ' +
                    dataContent +
                    '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div>');
            } else {
                $body.append('<div class="toast ' +
                    dataAlignment +
                    "-" +
                    dataPlacement +
                    '"> <div class="alert alert-dismissible fade show alert-' +
                    dataStyle +
                    ' "> ' +
                    dataContent +
                    '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div> </div>');
            }
        });

	/**************************************
	 Chosen Form Control
	 **************************************/
    $(".form-control-chosen").chosen({
        allow_single_deselect: true,
        width: "100%"
    });
    $(".form-control-chosen-required").chosen({
        allow_single_deselect: false,
        width: "100%"
    });
    $(".form-control-chosen-search-threshold-100").chosen({
        allow_single_deselect: true,
        disable_search_threshold: 100,
        width: "100%"
    });
    $(".form-control-chosen-optgroup").chosen({
        width: "100%"
    });
    $(function () {
        $('[title="clickable_optgroup"]').addClass("chosen-container-optgroup-clickable");
    });
    $(document).delegate('[title="clickable_optgroup"] .group-result',
        "click",
        function () {
            var unselected = $(this).nextUntil(".group-result").not(".result-selected");
            if (unselected.length) {
                unselected.trigger("mouseup");
            } else {
                $(this).nextUntil(".group-result").each(function () {
                    $('a.search-choice-close[data-option-array-index="' + $(this).data("option-array-index") + '"]')
                        .trigger("click");
                });
            }
        });

	/*****************************************
	 Themer Changer with local storage
	 *****************************************/

    $.fn.removeClassStartingWith = function (filter) {
        $(this).removeClass(function (index, className) {
            return (className.match(new RegExp("\\S*" + filter + "\\S*", "g")) || []).join(" ")
        });
        return this;
    };

    $body.delegate(".theme-changer",
        "click",
        function () {
            var primaryColor = $(this).attr("primary-color");
            var sidebarBg = $(this).attr("sidebar-bg");
            var logoBg = $(this).attr("logo-bg");
            var headerBg = $(this).attr("header-bg");

            localStorage.setItem("primaryColor", primaryColor);
            localStorage.setItem("sidebarBg", sidebarBg);
            localStorage.setItem("logoBg", logoBg);
            localStorage.setItem("headerBg", headerBg);

            $.fn.setThemeTone(primaryColor);
        });

    $.fn.setThemeTone = function (primaryColor) {
        if (localStorage.getItem("primaryColor") === null) {
        } else {
            /* SIDEBAR */
            if (localStorage.getItem("sidebarBg") === "light") {
                $(".sidebar ").addClass("sidebar-light");
            } else {
                $(".sidebar").removeClass("sidebar-light");
            }

            /* PRIMARY COLOR */
            if (localStorage.getItem("primaryColor") === "primary") {
                document.documentElement.style.setProperty("--theme-colors-primary", "#4B89FC");
            } else {
                var colorCode = getComputedStyle(document.body)
                    .getPropertyValue("--theme-colors-" + localStorage.getItem("primaryColor"));
                document.documentElement.style.setProperty("--theme-colors-primary", colorCode);
            }

            /* LOGO */
            if (localStorage.getItem("logoBg") === "white" || localStorage.getItem("logoBg") === "light") {
                $(".sidebar .navbar").removeClassStartingWith("bg").removeClassStartingWith("navbar-dark")
                    .addClass("navbar-light bg-" + localStorage.getItem("logoBg"));
            } else {
                $(".sidebar .navbar").removeClassStartingWith("bg").removeClassStartingWith("navbar-light")
                    .addClass("navbar-dark bg-" + localStorage.getItem("logoBg"));
            }

            /* HEADER */
            if (localStorage.getItem("headerBg") === "light" || localStorage.getItem("headerBg") === "white") {
                $(".header .navbar").removeClassStartingWith("bg").removeClassStartingWith("navbar-dark")
                    .addClass("navbar-light bg-" + localStorage.getItem("headerBg"));
            } else {
                $(".header .navbar").removeClassStartingWith("bg").removeClassStartingWith("navbar-light")
                    .addClass("navbar-dark bg-" + localStorage.getItem("headerBg"));
            }
        }
    };

    $.fn.setThemeTone();
})(jQuery);

/*****************************************
 Full Screen Toggle
 *****************************************/
function toggleFullScreen() {
    if ((document.fullScreenElement && document.fullScreenElement !== null) ||
        (!document.mozFullScreen && !document.webkitIsFullScreen)) {
        if (document.documentElement.requestFullScreen) {
            document.documentElement.requestFullScreen();
        } else if (document.documentElement.mozRequestFullScreen) {
            document.documentElement.mozRequestFullScreen();
        } else if (document.documentElement.webkitRequestFullScreen) {
            document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
        }
    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
    }
}

/************************************************
			Customize system theme js
************************************************/

const settings = JSON.parse(localStorage.getItem("settings"));

const tManager = new TemplateManager();

function IsoTableHeadActions() {
    this.settings = {
        show: true,
        enabledOptions: {
            add: true,
            deleteRestore: true,
            deletePermanent: true,
            hiddenColumns: true,
            rows: true
        },
        actions: {
            add: {
                class: "add_new_inline btn btn-outline-primary mr-2",
                translate: "add"
            }
        },
        customPreActions: [],
        customPostActions: []
    };
}

/*
 * Constructor
 */
IsoTableHeadActions.prototype.constructor = IsoTableHeadActions;

/*
 * Get configurations
 */
IsoTableHeadActions.prototype.getConfiguration = function () {
    return this;
};

/*
 * text cell position
 */
function getOffset(el) {
    const rect = el.getBoundingClientRect();
    return {
        left: rect.left + window.scrollX,
        top: rect.top + window.scrollY
    };
}

// function changeTextCellPosition() {
//     $(this).parent().focusout(function () {
//         $(this).css("left", "");
//         $(this).css("top", "");
//     });

//     const expandCell = $(this).parent();
//     const pos = new TableInlineEdit().elementOffset($(this).parent().get(0));
//     const docHeight = $(document).height();
//     const docWidth = $(document).width();
//     const hPercent = pos.top * 100 / docHeight;
//     const diffH = docHeight - pos.top;
//     const textareaWidth = $(expandCell).innerWidth();

//     console.log(textareaWidth);

//     const navBarWidth = $(".navigation").width();
//     pos.left -= navBarWidth;
//     const wPercent = pos.left * 100 / docWidth;
//     //const diffW = docWidth - pos.left;

//     if (hPercent > 72 && hPercent < 75) {
//         expandCell.css("top", `${pos.top - diffH}px`);
//     } else if (hPercent > 80) {
//         expandCell.css("top", `${pos.top - diffH
//             // - 240
//             }px`);
//     }

//     if (wPercent > 70) {
//         expandCell.css("left", `${docWidth - navBarWidth - textareaWidth
//             // * 2
//             }px`);
//             console.log('wPercent > 70');

//     }
//     // getOffset(element).left;
//     // console.log(left);
// }


function changeTextCellPosition() {
    $(this).focusout(function () {
        $(this).css("left", "");
        $(this).css("top", "");
    });

    const expandCell = $(this);
    const pos = new TableInlineEdit().elementOffset($(this).get(0));
    const docHeight = $(document).height();
    const docWidth = $(document).width();
    const hPercent = pos.top * 100 / docHeight;
    const diffH = docHeight - pos.top;
    const textareaWidth = $(expandCell).innerWidth();


    const navBarWidth = $(".navigation").width();
    pos.left -= navBarWidth;
    const wPercent = pos.left * 100 / docWidth;

    if (hPercent > 72 && hPercent < 75) {
        expandCell.css("top", `${pos.top - diffH}px`);
    } else if (hPercent > 80) {
        expandCell.css("top", `${pos.top - diffH
            }px`);
    }

    if (wPercent > 70) {
        expandCell.css("left", `${docWidth - navBarWidth - textareaWidth
            }px`);

    }

}

$(".table")
    .on("preInit.dt",
        function () {
            if (!$(this).hasClass("dynamic-table")) return;
            const conf = new IsoTableHeadActions().getConfiguration();
            const domTableId = $(this).attr("id");
            const tableBuilderInstance = window.TableBuilderInstances.instances.find(x => x.configurations.listId == domTableId);
            const entityId = tableBuilderInstance.configurations.viewmodelData.tableModelId;
            $.ajax({
                url: "/api/EntitySecurity/GetEntityPermissionsForCurrentUser",
                data: { entityId },
                async: false,
                success: function (response) {
                    if (!response.is_success) {
                        tableBuilderInstance.toast.notifyErrorList(response.error_keys);
                        return;
                    }
                    const permissions = response.result;
                    const fullControl = permissions.includes("FullControl");
                    if (!fullControl) {
                        conf.settings.enabledOptions.add = permissions.includes("Write");
                        conf.settings.enabledOptions.deletePermanent = permi0ssions.includes("DeletePermanent");
                        conf.settings.enabledOptions.deleteRestore = permissions.includes("Delete") | permissions.includes("Restore");
                    }

                    //Risk company matrix
                    if ($(this).attr("db-viewmodel") === "8d42136d-eed5-4cdf-ae6c-424e2986ebf5") {
                        conf.settings.actions.add.class = "add-matrix btn btn-outline-primary mr-2";
                    }
                    const content = tManager.render("template_headListActions", conf);
                    const selector = $("div.CustomTableHeadBar");
                    selector.html(content);
                    selector.find(".add-matrix").on("click", riskMatrixCreate);
                    window.forceTranslate("div.CustomTableHeadBar");
                },
                error: function () { }
            });
        });

function riskMatrixCreate() {
    const scope = this;
    const db = new DataInjector();
    const helper = new ST();
    db.getAllWhereWithIncludesAsync("CompanySettings").then(x => {
        if (x.is_success) {
            const settings = x.result.find(p => p.parameterReference.code === 0);
            if (!settings) {
                return;
            }
            if (settings.value == 1) {
                //template
                const template = x.result.find(p => p.parameterReference.code === 2);
                if (!template) {
                    return;
                }
                db.getByIdWithIncludesAsync("CommonRiskMatrixTemplate", template.value).then(common => {
                    if (common.is_success) {
                        const matrix = common.result;
                        matrix.id = helper.newGuid();
                        db.addAsync("CompanyRiskMatrix", matrix).then(h => {
                            if (h.is_success) {
                                const promises = [];
                                promises.push(new Promise((resolve, reject) => {
                                    const filters = [{ parameter: "TemplateId", value: template.value }];
                                    db.getAllWhereNoIncludesAsync("MatrixImpactDefinition", filters).then(y => {
                                        if (y.is_success) {
                                            if (y.result.length < parseInt(common.result.impactUnitScale))
                                                reject(
                                                    "All the details in the template can not be added because the template has not been completely set up");
                                            else
                                                resolve(y);
                                        } else {
                                            reject("Configuration error, try contact administrator");
                                        }
                                    });
                                }));
                                promises.push(new Promise((resolve, reject) => {
                                    const filters = [{ parameter: "MatrixId", value: template.value }];
                                    db.getAllWhereNoIncludesAsync("MatrixCellValues", filters).then(y => {
                                        if (y.is_success) {
                                            if (y.result.length < parseInt(common.result.impactUnitScale))
                                                reject(
                                                    "All the details in the template can not be added because the template has not been completely set up");
                                            else
                                                resolve(y);
                                        } else {
                                            reject("Configuration error, try contact administrator");
                                        }
                                    });
                                }));

                                Promise.all(promises).then(res => {
                                    const addPromises = [];
                                    addPromises.push(new Promise((resolve, reject) => {
                                        const data = res[0].result;
                                        for (let i = 0; i < data.length; i++) {
                                            data[i].templateId = matrix.id;
                                        }
                                        db.addRangeAsync("CompanyMatrixImpactDefinition", data).then(r => {
                                            if (r.result.length > 0) {
                                                resolve();
                                            }
                                        });
                                    }));

                                    addPromises.push(new Promise((resolve, reject) => {
                                        const data = res[1].result;
                                        for (let i = 0; i < data.length; i++) {
                                            data[i].matrixId = matrix.id;
                                        }
                                        db.addRangeAsync("CompanyMatrixCellValues", data).then(r => {
                                            if (r.result.length > 0) {
                                                resolve();
                                            }
                                        });
                                    }));

                                    Promise.all(addPromises).then(y => {
                                        location.href = `/edit-company-matrix?templateId=${matrix.id}`;
                                    }).catch(e => {
                                        alert(e);
                                        location.reload();
                                    });
                                }).catch(e => {
                                    alert(e);
                                    location.reload();
                                });
                            }
                        });
                    } else {
                        console.warn(common.error_keys);
                    }
                });
            } else if (settings.value == 2) {
                new TableInlineEdit().addNewHandler(scope);
            }
        } else {
            console.warn(x.error_keys);
        }
    });
}

/***********************************************
			Override table column visibility
************************************************/
if (typeof TableColumnsVisibility !== "undefined") {
    //Container what store column visibility control
    TableColumnsVisibility.prototype.modalContainer = "#hiddenColumnsModal * .modal-body";

	/**
	 * Trigger handler then columns visibility are changed
	 * @param {any} source
	 */
    TableColumnsVisibility.prototype.onColumnsVisibilityStateChanged = function (source) {
        const jqSource = $(source);
        const nodeName = source.nodeName;
        switch (nodeName) {
            case "INPUT":
                {
                    const tableIdentifier = jqSource.data("table");
                    $(tableIdentifier).DataTable().draw();
                }
                break;
            case "A":
                {
                    const tableIdentifier = jqSource.closest(".modal-body")
                        .find("ul")
                        .find("li:first-child")
                        .find("input")
                        .data("table");
                    $(tableIdentifier).DataTable().draw();
                }
                break;
        }
    };

	/**
	 * Render checkbox for column visibility
	 * @param {any} data
	 * @param {any} id
	 * @param {any} vis
	 */
    TableColumnsVisibility.prototype.renderCheckBox = function (data, id, vis) {
        const title = (data.targets === "no-sort") ? "#" : data.sTitle;
        return `<div class="custom-control custom-checkbox">
            	<input type="checkbox" ${vis} data-table="${id}" id="_check_${data.idx
            }" class="custom-control-input vis-check" data-id="${data.idx}" required />
              <label class="custom-control-label" for="_check_${data.idx}">${title}</label>
          </div>`;
    };

	/**
	 * Init column visibility control
	 * @param {any} ctx
	 */
    TableColumnsVisibility.prototype.init = function (ctx) {
        const tableId = `#${$(ctx).attr("id")}`;
        const table = $(tableId);
        const dto = table.DataTable();
        if (table.hasClass("dynamic-table")) {
            const cols = this.getVisibility(tableId);
            dto.columns(cols.visibledItems).visible(true);
            dto.columns(cols.hiddenItems).visible(false);
            $(".hidden-columns-event").attr("data-id", `#${$(ctx)[0].id}`);
            this.registerInitEvents();
        } else {
            const template = this.renderTemplate(ctx);
            $("div.CustomizeColumns").html(template);
            $(".list-side-toggle").click(function () {
                new TableColumnsVisibility().toggleRightListSideBar($(this).attr("data-id"));
                $("#hiddenColumnsModal").modal();
            });
        }
    };

	/*
	 * Register events for control initialization
	*/
    TableColumnsVisibility.prototype.registerInitEvents = function () {
        $(".table-search").keyup(function () {
            const oTable = $(this).closest(".card").find(".dynamic-table").DataTable();
            oTable.search($(this).val()).draw();
        });

        //Delete multiple rows
        $(".deleteMultipleRows").on("click",
            function () {
                const cTable = $(this).closest(".card").find(".dynamic-table");
                if (cTable) {
                    if (typeof TableBuilder !== "undefined") {
                        new TableBuilder().deleteSelectedRowsHandler(cTable.DataTable());
                    }
                }
            });

        $(".delete-multiple-rows-permanent").on("click",
            function () {
                const cTable = $(this).closest(".card").find(".dynamic-table");
                if (cTable) {
                    if (typeof TableBuilder !== "undefined") {
                        new TableBuilder().deleteSelectedRowsPermanentHandler(cTable.DataTable());
                    }
                }
            });

        $(".add_new_inline").on("click",
            function () {
                new TableInlineEdit().addNewHandler(this);
            });

        //Items on page
        $(".tablePaginationView a").on("click",
            function () {
                const ctx = $(this);
                const onPageValue = ctx.data("page");
                const onPageText = ctx.text();
                ctx.closest(".dropdown").find(".page-size").html(`(${onPageText})`);
                const table = ctx.closest(".card").find(".dynamic-table").DataTable();
                table.page.len(onPageValue).draw();
            });

        //hide columns
        $(".hidden-columns-event").click(function () {
            new TableColumnsVisibility().toggleRightListSideBar($(this).attr("data-id"));
            $("#hiddenColumnsModal").modal();
        });
    };
}

/***********************************************
			Override TableBuilder
************************************************/
if (typeof TableBuilder !== "undefined") {
    //Override table select
    TableBuilder.prototype.dom = '<"CustomTableHeadBar">rtip';
    RenderTableSelect.prototype.settings.classNameText = "no-sort";
    RenderTableSelect.prototype.settings.select.selector = "td:not(.not-selectable):first-child .checkbox-container";
    //Table buttons
    TableBuilder.prototype.buttons = [];

    RenderTableSelect.prototype.selectHandler = function (context) {
        const row = $(context).closest("tr");
        const table = row.closest("table").DataTable();
        if (row.hasClass("selected")) {
            table.row(row).deselect();
        } else {
            table.row(row).select();
        }
    };

    RenderTableSelect.prototype.selectHeadHandler = function (context) {
        const table = $(context).closest(".card").find(".dynamic-table");
        const dTable = table.DataTable();
        const rows = table.find("tbody tr");
        const isChecked = $(context).prop("checked");
        rows.each((index, item) => {
            const input = $(item).find("td div.checkbox-container").find("input");
            if (isChecked) {
                dTable.row(item).select();
            } else {
                dTable.row(item).deselect();
            }
            if (input) input.prop("checked", isChecked);
        });
    };

    RenderTableSelect.prototype.selectTemplateCommom = function (id, handler) {
        return `<div class="checkbox-container">
                                <div class="custom-control custom-checkbox">
                                    <input type="checkbox" onchange="${handler
            }" class="custom-control-input" id="_select_${id}"
                                           required>
                                    <label class="custom-control-label" for="_select_${id}"></label>
                                </div>
                            </div>`;
    };

    RenderTableSelect.prototype.settings.headContent = () => {
        const id = st.newGuid();
        return new RenderTableSelect().selectTemplateCommom(id, "new RenderTableSelect().selectHeadHandler(this)");
    };

    RenderTableSelect.prototype.templateSelect = function (data, type, row, meta) {
        const id = st.newGuid();
        return new RenderTableSelect().selectTemplateCommom(id, "new RenderTableSelect().selectHandler(this)");
    };

    //Table actions
    TableBuilder.prototype.getTableRowDeleteRestoreActionButton = function (row, dataX) {
        return `${dataX.hasDeleteRestore
            ? `${row.isDeleted
                ? `<a title="${window.translate("restore")
                }" href="javascript:void(0)" onclick="new TableBuilder().restoreItem('${row.id
                }', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')"><i class="material-icons">restore</i></a>`
                : `<a title="${window.translate("delete")
                }" href="javascript:void(0)" onclick="new TableBuilder().deleteItem('${row.id
                }', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')"><i class="material-icons">delete</i></a>`}`
            : ``}`;
    };

    TableBuilder.prototype.getTableRowInlineActionButton = function (row, dataX) {
        if (row.isDeleted) return "";
        return `${dataX.hasInlineEdit
            ? `	<a title="${window.translate("edit")}" class="inline-edit" data-viewmodel="${dataX.viewmodelData.result
                .id
            }" href="javascript:void(0)"><i class="material-icons">edit</i></a>`
            : ``}`;
    };

    TableBuilder.prototype.getTableDeleteForeverActionButton = function (row, dataX) {
        return `${dataX.hasDeleteForever
            ? `	<a onclick="new TableBuilder().deleteItemForever('${row.id
            }', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')" data-viewmodel="${dataX.viewmodelData.result.id
            }" class="delete-forever" href="javascript:void(0)" title="Delete forever"><i class="material-icons">delete_forever</i></a>`
            : ``}`;
    };

    //Rewrite actions for table
    TableBuilder.prototype.getTableRowEditActionButton = function (row, dataX) {
        if (row.isDeleted) return "";
        return `${dataX.hasEditPage
            ? `<a href="${dataX.editPageLink}?itemId=${row.id
            }&&listId=${dataX.viewmodelData.result.id}"><i class="material-icons">edit</i></a>`
            : ``}`;
    };

    //Rewrite jq dt translations
    TableBuilder.prototype.replaceTableSystemTranslations = function () {
        const customReplace = new Array();
        customReplace.push({ Key: "sProcessing", Value: `<div class="col-md lds-dual-ring"></div>` });
        //customReplace.push({ Key: "processing", Value: `<div class="col-md lds-dual-ring"></div>` });
        const serialData = JSON.stringify(customReplace);
        return serialData;
    };

	/**
 * On table init complete
 * @param {any} settings
 * @param {any} json
 */
    TableBuilder.prototype.onInitComplete = function (settings, json) {
        if (this.configurations.table.name === "CommonRiskMatrixTemplate" ||
            this.configurations.table.name === "CompanyRiskMatrix") return;
        new TableInlineEdit().addNewHandler($(settings.nTable).parent());
        // The second argument is not required
        this.configurations.overflowIndicator = $.Iso.OverflowIndicator($($(settings.nTable)), { trigger: "focus" });
    };
}

/***********************************************
			Override inline edit templates
************************************************/
if (typeof TableInlineEdit !== "undefined") {
    TableInlineEdit.prototype.toggleVisibilityColumnsButton = function (ctx, state) {
        return;
    };

    TableInlineEdit.prototype.renderActiveInlineButton = function (ctx) {
        //ctx.find("i").html("check");
    };

    //Set actions for table
    TableInlineEdit.prototype.getActionsOnAdd = function () {
        const template = `<div class="btn-group" role="group" aria-label="Action buttons">
							<a href="javascript:void(0)" class='add-new-item'><i class="material-icons">check</i></a>
							<a href="javascript:void(0)" class='cancel-new-item'><i class="material-icons">cancel</i></a>
						</div>`;
        return template;
    };

    //On add new cell for inline edit
    TableInlineEdit.prototype.onGetNewAddCell = function (cell) {
        const ctx = $(cell);
        ctx.addClass("expandable-cell");
        ctx.find("div:first-child").addClass("hasTooltip");
    };

	/**
 * On after init reference cell
 * @param {any} el
 * @param {any} data
 */
    TableInlineEdit.prototype.onAfterInitAddReferenceCell = function (el, data) {
        const scope = this;
        const select = $(el).find("select");
        select.attr("class", "inline-add-event data-new form-control");
        $(el).find("select.inline-add-event").on("change",
            function () {
                scope.addNewItem($(this));
            });
    };

	/**
 * On after init text cell
 * @param {any} columns
 * @param {any} index
 */
    TableInlineEdit.prototype.onAfterInitTextEditCell = function (columns, index) {
        this.onAfterInitEditCellDefaultHandler(columns, index);
        const columnCtx = $(columns[index]);
        const expandCell = columnCtx.parent();
        expandCell.addClass("expandable-cell");
        columnCtx.on("click", changeTextCellPosition);
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
        $(el).parent().on("click", changeTextCellPosition);
        this.onAddedCellBindValidations(el);
    };

    TableInlineEdit.prototype.defaultNotEditFieldContainer = "<div class='text-center'>-</div>";

	/**
 * On row create
 * @param {any} row
 * @param {any} data
 * @param {any} dataIndex
 */
    TableBuilder.prototype.onRowCreate = function (row, data, dataIndex) {
        const scope = this;
        if (data.isDeleted) {
            $(row).addClass("row-deleted");
            $(row).find("td.select-checkbox").find("input").css("display", "none");
            $(row).find("td").addClass("not-selectable");
        }
        const rowScope = $(row);
        rowScope.attr("data-viewmodel", scope.configurations.viewmodelId);
        rowScope.unbind();
        rowScope.on("dblclick",
            function () {
                new ST().clearSelectedText();
                new TableInlineEdit().initInlineEditForRow(this);
            });
    };

    //Restyle inline edit controls
    TableInlineEdit.prototype.getBooleanEditCell = (data) => {
        const labelIdentifier = `label_${new ST().newGuid()}`;
        const container = document.createElement("div");
        container.setAttribute("class", "checkbox-container pt-2 pb-2");
        const div = document.createElement("div");
        div.setAttribute("class", "custom-control custom-checkbox");
        const label = document.createElement("label");
        label.setAttribute("class", "custom-control-label");
        label.setAttribute("for", labelIdentifier);

        const el = document.createElement("input");
        el.setAttribute("class", "inline-update-event data-input custom-control-input");
        el.setAttribute("data-prop-id", data.propId);
        el.setAttribute("data-id", data.cellId);
        el.setAttribute("type", "checkbox");
        el.setAttribute("data-prop-name", data.propName);
        el.setAttribute("data-entity", data.tableId);
        el.setAttribute("data-type", "bool");
        el.setAttribute("id", labelIdentifier);
        el.setAttribute("name", labelIdentifier);

        if (data.value) {
            el.setAttribute("checked", "checked");
        }

        div.appendChild(el);
        div.appendChild(label);
        container.appendChild(div);
        $(container).closest(".data-cell").addClass("text-center");
        return container;
    };

	/**
	 * Get reference edit cell
	 * @param {any} conf
	 */
    TableInlineEdit.prototype.getReferenceEditCell = function (conf) {
        const gScope = this;
        const div = document.createElement("div");
        const dropdown = document.createElement("select");
        dropdown.setAttribute("class", "inline-update-event data-input form-control");
        dropdown.setAttribute("data-prop-id", conf.propId);
        dropdown.setAttribute("data-prop-name", conf.propName);
        dropdown.setAttribute("data-id", conf.cellId);
        dropdown.setAttribute("data-entity", conf.tableId);
        dropdown.setAttribute("data-type", "uniqueidentifier");
        dropdown.style.display = "none";
        dropdown.options[dropdown.options.length] = new Option(window.translate("no_value_selected"), "");
        if (!conf.allowNull) {
            dropdown.setAttribute("data-required", "");
        }
        const container = document.createElement("div");
        container.setAttribute("class",
            "fire-reference-component input-group outline-control br-none inline-editing-input");
        const el = document.createElement("input");
        el.setAttribute("class", "form-control virtual-el-reference");
        el.setAttribute("type", "text");
        el.setAttribute("readonly", "");
        container.appendChild(el);
        const decorator = document.createElement("div");
        decorator.classList = ["input-group-append"];
        const grSpan = document.createElement("span");
        grSpan.classList = ["input-group-text"];
        const icon = document.createElement("span");
        icon.classList = ["material-icons"];
        icon.innerHTML = "keyboard_arrow_down";
        grSpan.appendChild(icon);
        decorator.appendChild(grSpan);
        container.appendChild(decorator);
        gScope.populateColumnDropdown(conf, dropdown, div, el);
        div.appendChild(dropdown);
        div.appendChild(container);
        return div;
    };

	/**
	 * Populate column dropdown
	 * @param {any} conf
	 */
    TableInlineEdit.prototype.populateColumnDropdown = function (conf, dropdown, div, el) {
        const gScope = this;
        //Populate dropdown
        return loadAsync(`/InlineEdit/GetRowReferences?entityId=${conf.tableId}&propertyId=${conf.propId}`).then(data => {
            if (data) {
                if (data.is_success) {
                    const entityName = data.result.entityName;
                    data.result.data.unshift({ id: null, name: window.translate("no_value_selected") });
                    $.each(data.result.data,
                        function (index, obj) {
                            if (obj.id === conf.value) {
                                el.value = obj.name;
                            }
                            dropdown.options[dropdown.options.length] = new Option(obj.name, obj.id);
                        });
                    dropdown.setAttribute("data-ref-entity", entityName);
                    const items = data.result.data.map(x => {
                        return {
                            id: x.id,
                            value: x.name
                        };
                    });
                    gScope.attachEventsToSelect(div, items, entityName, dropdown);
                }
                dropdown.value = conf.value;
            }
        });
    };

	/**
	 * Attach events
	 */
    TableInlineEdit.prototype.attachEventsToSelect = function (div, items, entityName, dropdown) {
        const gScope = this;
        $($(div).find(".fire-reference-component")).on("click",
            function (event) {
                if (event.originalEvent.detail > 1) return;
                const cellCtx = this;
                const item = $.Iso.dynamicFilter("list",
                    event.target,
                    items,
                    {
                        create: function (value) {
                            return new Promise((resolve, reject) => {
                                gScope.db.addAsync(entityName, { name: value }).then(response => {
                                    if (response.is_success) {
                                        dropdown.options[dropdown.options.length] =
                                            new Option(value, response.result);
                                        const successMessage =
                                            `${window.translate("system_record")} ${value} ${window
                                                .translate("system_record_added_into")} ${entityName}`;
                                        gScope.toast.notify({ heading: successMessage, icon: "success" });
                                        resolve(response.result);
                                    } else {
                                        reject();
                                        gScope.toast.notifyErrorList(response.error_keys);
                                    }
                                });
                            });
                        },
                        update: function (obj) {
                            return new Promise((resolve, reject) => {
                                gScope.db.getByIdWithIncludesAsync(entityName, obj.id).then(x => {
                                    if (x.is_success) {
                                        const newObj = x.result;
                                        newObj.name = obj.value;
                                        gScope.db.updateAsync(entityName, newObj).then(y => {
                                            if (y.is_success) {
                                                gScope.toast.notify({
                                                    heading: window.translate("system_entry_updaded"),
                                                    icon: "success"
                                                });
                                                resolve();
                                            } else {
                                                gScope.toast.notifyErrorList(y.error_keys);
                                                reject();
                                            }
                                        }).catch(err => {
                                            reject(err);
                                        });
                                    } else {
                                        gScope.toast.notify({
                                            heading: window.translate("system_data_no_item_found")
                                        });
                                    }
                                }).catch(err => {
                                    reject(err);
                                });
                            });
                        },
                        delete: function (obj) {
                            return new Promise((resolve, reject) => {
                                const params = [{ parameter: "Id", value: obj.id }];
                                gScope.db.deletePermanentWhereAsync(entityName, params).then(x => {
                                    if (x.is_success) {
                                        gScope.toast.notify({
                                            heading: window.translate("system_data_record_deleted"),
                                            icon: "success"
                                        });
                                        resolve();
                                    } else {
                                        gScope.toast.notifyErrorList(x.error_keys);
                                        reject();
                                    }
                                }).catch(err => {
                                    reject(err);
                                });
                            });
                        }
                    },
                    {
                        entity: entityName,
                        ctx: cellCtx,
                        items: items,
                        searchBarPlaceholder: window.translate("system_search_add"),
                        addButtonLabel: window.translate("add")
                    },
                    { placement: "bottom-auto" });

                $(item.container).on("selectValueChange",
                    (event, arg) => {
                        const { ctx, entity, items } = arg.options;
                        //const exist = items.find(x => x.id === arg.value);
                        $(dropdown).val(arg.value);
                        $(dropdown).trigger("change");
                        gScope.db.getByIdWithIncludesAsync(entity, arg.value).then(x => {
                            if (x.is_success) {
                                let param = "name";
                                if (entity == "Users") {
                                    param = "userName";
                                }
                                if (!x.result) {
                                    $(ctx).find(".virtual-el-reference").val(window.translate("no_value_selected"));
                                } else
                                    $(ctx).find(".virtual-el-reference").val(x.result[param]);
                            } else {
                                gScope.toast.notifyErrorList(x.error_keys);
                            }
                        });
                    });
            });
    };

	/**
	 * Get date edit cell
	 * @param {any} data
	 */
    TableInlineEdit.prototype.getDateEditCell = (data) => {
        const container = document.createElement("div");
        container.setAttribute("class", "input-group outline-control br-none inline-editing-input");
        const el = document.createElement("input");
        el.setAttribute("class", "inline-update-event datepicker-control data-input form-control");
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
        container.appendChild(el);
        const decorator = document.createElement("div");
        decorator.classList = ["input-group-append"];
        const grSpan = document.createElement("span");
        grSpan.classList = ["input-group-text"];
        const icon = document.createElement("span");
        icon.classList = ["material-icons"];
        icon.innerHTML = "calendar_today";
        grSpan.appendChild(icon);
        decorator.appendChild(grSpan);
        container.appendChild(decorator);
        return container;
    };

	/**
 * On after init date cell
 * @param {any} el
 * @param {any} data
 */
    TableInlineEdit.prototype.onAfterInitAddDateCell = function (el, data) {
        const input = $(el).find(".inline-update-event");
        input.get(0).setAttribute("class", "inline-add-event data-new form-control datepicker-control");
        input.on("change", function () { })
            .datepicker({
                format: "dd/mm/yyyy",
                autoclose: true
            });
        input.on("change",
            function () {
                if (!this.hasAttribute("data-required")) return;
                if ($(this).val()) {
                    $(this).parent().removeClass("cell-red");
                } else {
                    $(this).parent().removeClass("cell-red").addClass("cell-red");
                }
            });
    };

	/**
	 * Rewrite add new line
	 * @param {any} ctx
	 * @param {any} jdt
	 */
    TableInlineEdit.prototype.addNewHandler = function (ctx, jdt = null) {
        const scope = this;
        const card = $(ctx).closest(".card");
        const dto = card.find(".dynamic-table");
        if (!jdt) {
            jdt = dto.DataTable();
        }
        const activeNewRows = dto.find("tbody tr[isNew='true']");
        if (activeNewRows.length > 0) {
            return this.displayNotification({
                heading: window.translate("system_inline_edit_add_fail"),
                icon: "warning"
            });
        }
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
            } else {
                const newCell = this.getAddRowCell(columns[i], cell);
                cell = newCell.cell;
                if (newCell.entityName)
                    row.setAttribute("entityName", newCell.entityName);
                $(cell).find("textarea.inline-add-event, input.inline-add-event").on("blur",
                    function () {
                        scope.addNewItem($(this));
                    });
            }

            row.appendChild(cell);
        }
        dto.attr("data-add-mode", "true");
        const tBody = $("tbody", dto);
        const haveEmptyRow = tBody.find(".dataTables_empty");
        if (haveEmptyRow) {
            $(haveEmptyRow).closest("tr").remove();
        }
        tBody.prepend(row);
        this.bindEventsAfterInitInlineEdit(row);
        return this.toggleVisibilityColumnsButton(ctx, true);
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

        const viewModelId = targetCtx.attr("data-viewmodel");
        loadAsync(`/InlineEdit/GetViewModelColumnTypes?viewModelId=${viewModelId}`).then(viewModel => {
            const dt = targetCtx.closest("table");
            const table = dt.DataTable();
            const row = targetCtx;
            const columns = row.find(".data-cell");
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
                    const tableId = fieldData.tableId;
                    const propId = fieldData.id;
                    const propName = fieldData.name;
                    const parsedPropName = propName.toLowerFirstLetter();
                    const value = obj[parsedPropName];
                    const allowNull = fieldData.allowNull;
                    let container = value;
                    const data = {
                        cellId,
                        tableId,
                        propId,
                        value,
                        propName,
                        allowNull,
                        addMode: false,
                        viewModel: viewModel.result
                    };
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
                                    viewModelConfigurations,
                                    columnCtx,
                                    cellId
                                });
                            }
                            break;
                    }
                } else {
                    this.getOnNonRecognizedField(columnCtx, viewModelConfigurations);
                }
            }
            this.bindEventsAfterInitInlineEdit(row);
        }).catch(err => {
            console.warn(err);
        });
    };

	/**
 * Transform row from edit mode to read mode
 * @param {any} target
 */
    TableInlineEdit.prototype.completeInlineEditForRow = function (target) {
        const targetCtx = $(target);
        const htTable = targetCtx.closest("table");
        const table = htTable.DataTable();
        let row = targetCtx.closest("tr");
        const index = table.row(row).index();
        let obj = table.row(index).data();
        targetCtx.off("click", completeEditInlineHandler);
        const columns = targetCtx.parent().parent().parent().find(".data-cell");
        let viewModelId = htTable.attr("db-viewmodel");
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
                                        const {
                                            sourceEntity,
                                            sourceSelfParamName,
                                            sourceRefParamName,
                                            referenceEntityName
                                        } =
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
                $(redraw.row(index).nodes()).unbind();
                $(redraw.row(index).nodes()).on("dblclick",
                    function () {
                        new TableInlineEdit().initInlineEditForRow(this);
                    });
                $.Iso.OverflowIndicator(htTable, { trigger: "focus" });
            });
        }).catch(err => {
            console.warn(err);
        });
    };

    //bind events after inline edit was started for row
    TableInlineEdit.prototype.bindEventsAfterInitInlineEdit = function (row) {
        try {
            // ReSharper disable once ConstructorCallNotUsed
            new $.Iso.InlineEditingCells();
        } catch (e) {
        }
        const ctx = $(row);
        ctx.unbind();
        if (!ctx.get(0).hasAttribute("isnew")) {
            row.on("dblclick",
                function (e) {
                    e.preventDefault();
                    new ST().clearSelectedText();
                    $(this).unbind();
                    new TableInlineEdit().completeInlineEditForRow(this);
                });
        }
    };

	/**
	 * Many to many control
	 * @param {any} data
	 */
    TableInlineEdit.prototype.initManyToManyControl = function (data) {
        const { viewModelConfigurations, columnCtx, cellId } = data;
        const scope = this;
        const mCtx = columnCtx.closest("td");
        const { sourceEntity, sourceSelfParamName, sourceRefParamName, referenceEntityName } =
            scope.getManyToManyViewModelConfigurations(viewModelConfigurations);
        mCtx.on("click",
            function () {
                if (event.detail > 1) return;
                const promiseArr = [];
                promiseArr.push(scope.db.getAllWhereWithIncludesAsync(referenceEntityName.value));
                promiseArr.push(scope.db.getAllWhereWithIncludesAsync(sourceEntity.value,
                    [{ parameter: sourceSelfParamName.value, value: cellId }]));
                //get data
                Promise.all(promiseArr).then(pResult => {
                    const rAll = pResult[0];
                    const rSelected = pResult[1];
                    if (!rAll.is_success || !rSelected.is_success) {
                        return scope.displayNotification({ heading: window.translate("system_something_went_wrong") });
                    }

                    const dItems = rAll.result.map(x => {
                        const e = rSelected.result.find(y =>
                            y[sourceRefParamName.value.toString().toLowerFirstLetter()] === x.id);
                        const o = {
                            id: x.id,
                            value: x.name,
                            checked: e ? true : false
                        };
                        return o;
                    });

                    const multiSelectItem = $.Iso.dynamicFilter("multi-select",
                        mCtx,
                        dItems,
                        null,
                        {
                            sourceEntity,
                            sourceSelfParamName,
                            sourceRefParamName,
                            referenceEntityName,
                            recordId: cellId,
                            searchBarPlaceholder: window.translate("system_search")
                        });

                    $(multiSelectItem.container).on("selectValueChange",
                        (event, arg) => {
                            const { id, checked } = arg.value.changedValue;
                            const {
                                recordId,
                                sourceEntity,
                                sourceSelfParamName,
                                sourceRefParamName,
                                referenceEntityName
                            } = arg.options;
                            if (checked) {
                                const addO = {};
                                addO[sourceSelfParamName.value] = recordId;
                                addO[sourceRefParamName.value] = id;
                                scope.db.addAsync(sourceEntity.value, addO).then(addResult => {
                                    if (addResult.is_success) {
                                        scope.toast.notify({
                                            heading: window.translate("system_inline_saved"),
                                            icon: "success"
                                        });
                                    } else {
                                        scope.toast.notifyErrorList(addResult.error_keys);
                                    }
                                }).catch(err => {
                                    console.warn(err);
                                });
                            } else {
                                const deleteFilters = [
                                    { parameter: sourceSelfParamName.value, value: recordId },
                                    { parameter: sourceRefParamName.value, value: id }
                                ];
                                scope.db.deletePermanentWhereAsync(sourceEntity.value, deleteFilters).then(
                                    deleteResult => {
                                        if (deleteResult.is_success) {
                                            scope.toast.notify({
                                                heading: window.translate("system_inline_saved"),
                                                icon: "success"
                                            });
                                        } else {
                                            scope.toast.notifyErrorList(deleteResult.error_keys);
                                        }
                                    }).catch(err => err);
                            }
                        });
                }).catch(err => {
                    console.warn(err);
                });
            });
    };
}

/***********************************************
			Override notificator
************************************************/
if (typeof Notificator !== "undefined") {
    const noNotifications = `<p id="noNotifications" class="text-muted p-3 m-0">${window.translate('system_notificator_no_notifications')}</p>`;

    const ajaxRequest = (requestUrl, requestType, requestData) => {
        const baseUrl = '/api/Notifications';
        return new Promise((resolve, reject) => {
            $.ajax({
                url: baseUrl + requestUrl,
                type: requestType,
                data: requestData,
                success: (data) => {
                    if (Array.isArray(data)) {
                        resolve(data);
                    }
                    else {
                        if (data.is_success) {
                            resolve(data.result);
                        } else if (!data.is_success) {
                            reject(data.error_keys);
                        } else {
                            resolve(data);
                        }
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    const markAsRead = notificationId => {
        const requestUrl = '/MarkAsRead';
        return ajaxRequest(requestUrl, 'post', { notificationId });
    }

    const loaderClassString = 'notification-loader';
    const loaderClass = `.${loaderClassString}`;

    const addLoader = elementDOM => {
        const loadermarkup = `<div class="${loaderClassString}">
			<div class="${loaderClassString}-body justify-content-center align-items-center">
			<div class="lds-dual-ring"></div>
				</div>
			</div>`;
        elementDOM.append(loadermarkup);
        elementDOM.find(loaderClass).fadeIn();
    }

    const removeLoader = elementDOM => {
        elementDOM.find(loaderClass).fadeOut();
        setTimeout(function () { elementDOM.find(loaderClass).remove(); }, 400);
    }

    const getNotificationsFromDropdown = () => {
        let notifications = [];
        $('#notificationList .notification-item').each(function () {
            notifications.push($(this).data('notification-id'));
        });
        return notifications;
    }

    let notificationsPage = 2;
    let stopGetNotifications = false;

    addLoader($('#notificationList'));

    $(document).click(() => {
        $("#notificationList").collapse('hide');
    });
    $('#notificationDropdown').click(() => {
        $("#notificationList").collapse('toggle');
    });
    $('#notificationList').click(e => {
        e.stopPropagation();
    });
    $('#notificationList .clear-all').click(() => {
        $('#notificationList .notifications').hide(500);
        setTimeout(function () {
            const markNotifications = getNotificationsFromDropdown();
            $('#notificationList .notifications').html(noNotifications).slideDown(100);
            $.each(markNotifications, function () {
                markAsRead(this);
            });
        }, 500);
    });

    $('#notificationList .show-more').click(() => {
        if (!stopGetNotifications) {
            addLoader($('#notificationList'));
            Notificator.prototype.getAllNotifications(notificationsPage, 5, true).then(data => {
                if (!data) return;
                if (data.is_success) {
                    if (data.result.notifications.length > 0) {
                        notificationsPage++;
                    }
                    else {
                        $('#notificationList .show-more').hide();
                        stopGetNotifications = true;
                    }
                    $.each(data.result.notifications, (i, notification) => {
                        Notificator.prototype.addNewNotificationToContainer(notification);
                    });
                }
                $('#noNotifications').remove();
                removeLoader($('#notificationList'));
            }).catch(e => {
                new ToastNotifier().notifyErrorList(e);
            });
        }
    });

    //override notification populate container
    Notificator.prototype.addNewNotificationToContainer = function (notification) {
        const _ = $("#notificationAlarm");
        if (!_.hasClass("notification"))
            _.addClass("notification");
        const template = this.createNotificationBodyContainer(notification);
        const target = $("#notificationList .notifications");
        $("#noNotifications").hide();
        target.append(template);
        $(`.notification-item[data-notification-id="${notification.id}"]`).click(function () {
            if (!$(this).find('.notification-content').hasClass('slide-open')) {
                $(this).find('.notification-content').slideDown().addClass('slide-open');
            }
            else {
                $(this).find('.notification-content').slideUp().removeClass('slide-open');
            }
            $(this).siblings('.notification-item').find('.notification-content').slideUp().removeClass('slide-open');
        });
        this.registerOpenNotificationEvent();
        $(`.notification-item[data-notification-id="${notification.id}"] .delete-notification`).click(() => {
            $(`.notification-item[data-notification-id="${notification.id}"]`).hide(500);
            setTimeout(function () {
                markAsRead(notification.id).then(() => {
                    $(`.notification-item[data-notification-id="${notification.id}"]`).remove();
                }).catch(e => {
                    new ToastNotifier().notifyErrorList(e);
                    $(`.notification-item[data-notification-id="${notification.id}"]`).show();
                });
            }, 500);
        });
    }

    Notificator.prototype.createNotificationBodyContainer = function (n) {
        const block = `
			<div data-notification-id="${n.id}" 
                class="notification-item dropdown-item border-bottom position-relative" style="cursor: pointer;">
				<div class="notification-body py-1">
					<p class="mb-0"><small>${n.subject}</small></p>
					<div class="mb-0 notification-content" style="display:none">${n.content}</div>
					<p class="text-muted mb-0"><small>${moment(n.created, "DD.MM.YYYY hh:mm:ss A").from(new Date())}</small></p>
					<span class="delete-notification">
						<i class="material-icons">close</i>
					</span>
				</div>
			</div>`;
        return block;
    }

    Notificator.prototype.getAllNotifications = function (page, perPage, onlyUnread) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `${this.origin()}/api/Notifications/GetUserNotificationsWithPagination`,
                method: "get",
                data: {
                    page,
                    perPage,
                    onlyUnread
                },
                success: function (data) {
                    resolve(data);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    };
}
else console.warn("Notification plugin not injected");

/***********************************************
			Override DataInjector
************************************************/
if (typeof DataInjector !== "undefined") {
	/**
	 * Add async
	 * @param {any} entityName
	 * @param {any} object
	 */
    DataInjector.prototype.addAsync = function (entityName, object) {
        const promises = [];
        const entityCodeFormats = [
            { name: "Objective", code: 1000, propName: "Code" },
            { name: "Asset", code: 1001, propName: "Code" },
            { name: "Risk", code: 1002, propName: "Code" },
            { name: "KPI", code: 1003, propName: "Code" },
            { name: "InternalAudit", code: 1004, propName: "Code" },
            { name: "Meeting", code: 1005, propName: "Code" },
            { name: "NomInterestedParty", code: 1006, propName: "Code" },
            { name: "NomLocation", code: 1007, propName: "Code" }
        ];
        const search = entityCodeFormats.find(x => x.name.toLowerCase() === entityName.toLowerCase());
        if (search) {
            const pr = new Promise((resolve, reject) => {
                this.getAllWhereWithIncludesAsync("CompanySettings").then(x => {
                    if (x.is_success) {
                        const config = x.result.find(y => y.parameterReference.code === search.code);
                        if (config) {
                            let format = config.value;
                            const d = new Date();
                            format = format.replace(/{Year}/g, d.getFullYear());
                            format = format.replace(/{Month}/g, d.getMonth() < 10 ? `0${d.getMonth()}` : d.getMonth());
                            format = format.replace(/{Day}/g, d.getDate() < 10 ? `0${d.getDate()}` : d.getDate());
                            this.countAsync(entityName).then(g => {
                                if (g.is_success) {
                                    format = format.replace(/{NextIndex}/g, g.result + 1);
                                    object[search.propName] = format;
                                    resolve();
                                }
                            });
                        } else {
                            reject(window.translate("iso_code_format_not_configured"));
                        }
                    }
                });
            });
            promises.push(pr);
        }

        return new Promise((resolve, reject) => {
            Promise.all(promises).then(x => {
                const dataParams = JSON.stringify({
                    entityName: entityName,
                    object: JSON.stringify(object)
                });
                $.ajax({
                    url: `/api/DataInjector/AddAsync`,
                    data: dataParams,
                    method: "post",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (error) {
                        reject(error);
                    }
                });
            }).catch(e => {
                new ToastNotifier().notify({ heading: e });
            });
        });
    };
}

function makeMenuActive(target) {
    if (target) {
        const last = target.closest("ul").closest("li");
        const a = last.find("a:first-child span.nav-item-text").first();
        if (a.text()) {
            $(".breadcrumb").prepend(`<li class="breadcrumb-item">${a.text()}</li>`);
        }
        last.addClass("active");
        $('.navigation').on('mouseover', function () {
            //last.addClass("open");
        });
        if (target.closest("nav").length !== 0)
            makeMenuActive(last);
    }
}

$(document).ready(function () {
    window.forceTranslate();
    //Log Out
    new ST().registerLocalLogout(".sa-logout");

    //Menu render promise
    const loadMenusPromise = loadAsync("/Menu/GetMenus");

    loadMenusPromise.then(menus => {
        const renderMenuContainer = $("#left-nav-bar");
        if (menus.is_success) {
            const content = tManager.render("template_RenderIsoMenuItem.html",
                menus.result,
                {
                    host: location.origin
                });
            renderMenuContainer.html(content);
            window.forceTranslate("#left-nav-bar");
            let route = location.href;
            if (route[route.length - 1] === "#") {
                route = route.substr(0, route.length - 1);
            }
            const activeMenu = renderMenuContainer.find(`a[href='${route}']`);
            activeMenu.parent()
                .addClass("active");
            $(".breadcrumb").html(null)
                .prepend(`<li class="breadcrumb-item active" aria-current="page">${activeMenu.find("span.nav-item-text")
                    .text()}</li>`);
            makeMenuActive(activeMenu);
            if (history.length > 2) {
                $("#history_back").css("display", "block");
            }
        }
    });

    const locHelper = new Localizer();

    //Localization promise
    const localizationPromise = new Promise((resolve, reject) => {
        //Set localization config
        const translateIcon = locHelper.adaptIdentifier(settings.localization.current.identifier);
        $("#currentlanguage").addClass(`flag-icon flag-icon-${translateIcon}`);
        const languageBlock = $("#languageRegion");
        resolve(languageBlock);
    });

    localizationPromise.then(languageBlock => {
        $.each(settings.localization.languages,
            function (index, lang) {
                const language =
                    `<a href="/Localization/ChangeLanguage?identifier=${lang.identifier
                    }" class="dropdown-item language-event">
							<i class="flag-icon flag-icon-${locHelper.adaptIdentifier(lang.identifier)}"></i> ${lang.name}
						</a>`;
                languageBlock.append(language);
            });
    });

    localizationPromise.then(() => {
        $(".language-event").on("click",
            function () {
                localStorage.removeItem("hasLoadedTranslations");
            });
    });

    Promise.all([loadMenusPromise, localizationPromise]).then(function (values) {
        window.forceTranslate();
    });




    //$("body").append($(`<a target="_blank" href="/cart" class="buynow-btn btn btn-success text-white"><span class="material-icons mr-2 align-middle text-white">shopping_cart</span> <span class="text">View Cart</span></a>`));
});
$('body').on('DOMNodeInserted', '.dataTables_scrollBody', function () {
    const slider = document.querySelector('.dataTables_scrollBody');
    let isDown = false;
    let startX;
    let scrollLeft;

    slider.addEventListener('mousedown', (e) => {
        isDown = true;
        slider.classList.add('active');
        startX = e.pageX - slider.offsetLeft;
        scrollLeft = slider.scrollLeft;
    });
    slider.addEventListener('mouseleave', () => {
        isDown = false;
        slider.classList.remove('active');
    });
    slider.addEventListener('mouseup', () => {
        isDown = false;
        slider.classList.remove('active');
    });
    slider.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault();
        const x = e.pageX - slider.offsetLeft;
        const walk = (x - startX) * 3; //scroll-fast
        slider.scrollLeft = scrollLeft - walk;
    });
});

function openColorPicker(event) {
    if (event.preventDefault) {
        event.preventDefault();
        event.stopPropagation();
    }
    else {
        event.returnValue = false;
    }
    const inputId = event.target.id;
    const inputElem = $(`#${inputId}`);
    inputElem.addClass('d-block');

    inputElem.spectrum({
        preferredFormat: "hex",
        showPalette: true,
        showButtons: false,
        showSelectionPalette: true,
        selectionPalette: [],
        localStorageKey: 'colorPickerPallette',
        maxSelectionSize: 10,
        showInput: true,
        showAlpha: true,
        color: inputElem.val(),
        move: function () {
            const color = inputElem.spectrum("get").toHexString();
            inputElem.val(color);
        },
        change: function () {
            const color = inputElem.spectrum("get").toHexString();
            inputElem.css('background-color', color);
            inputElem.val(color);
        }
    });

    inputElem.siblings('.sp-replacer').ready(() => {
        inputElem.siblings('.sp-replacer').attr('style', `opacity: 0;
			width: 100%;
			height: 47px;
			padding: 0;
			top: -46px;
			position: relative;`);
        inputElem.spectrum("show");
    });

    inputElem
}

/************************************************
					End Custom js
************************************************/