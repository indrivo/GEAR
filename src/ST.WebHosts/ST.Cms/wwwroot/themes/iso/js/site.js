"use strict";

/************************************************
					Customize system theme js
************************************************/

const settings = JSON.parse(localStorage.getItem("settings"));

const tManager = new TemplateManager();

//Override hide column
$(".table")
	.on("preInit.dt", function () {
		const content = tManager.render("template_headListActions", "");
		const selector = $("div.CustomTableHeadBar");
		selector.html(content);
		window.forceTranslate("div.CustomTableHeadBar");
	});
TableColumnsVisibility.prototype.modalContainer = "#hiddenColumnsModal * .modal-body";

TableColumnsVisibility.prototype.renderCheckBox = function (data, id, vis) {
	const title = (data.targets === "no-sort") ? "#" : data.sTitle;
	return `<div class="custom-control custom-checkbox">
            	<input type="checkbox" ${vis} data-table="${id}" id="_check_${data.idx}" class="custom-control-input vis-check" data-id="${data.idx}" required />
              <label class="custom-control-label" for="_check_${data.idx}">${title}</label>
          </div>`;
};

TableColumnsVisibility.prototype.init = function (ctx) {
	const cols = this.getVisibility(`#${$(ctx).attr("id")}`);
	$(`#${$(ctx).attr("id")}`).DataTable().columns(cols.visibledItems).visible(true);
	$(`#${$(ctx).attr("id")}`).DataTable().columns(cols.hiddenItems).visible(false);
	$(".hidden-columns-event").attr("data-id", `#${$(ctx)[0].id}`);
	$('.table-search').keyup(function () {
		const oTable = $(this).closest(".card").find(".dynamic-table").DataTable();
		oTable.search($(this).val()).draw();
	})
	this.registerInitEvents();
};

TableColumnsVisibility.prototype.registerInitEvents = function () {

	//Delete multiple rows
	$(".deleteMultipleRows").on("click", function () {
		const cTable = $(this).closest(".card").find(".dynamic-table");
		if (cTable) {
			if (typeof TableBuilder !== 'undefined') {
				new TableBuilder().deleteSelectedRowsHandler(cTable.DataTable());
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
		const table = ctx.closest(".card").find(".dynamic-table").DataTable();
		table.page.len(onPageValue).draw();
	});

	//hide columns
	$(".hidden-columns-event").click(function () {
		new TableColumnsVisibility().toggleRightListSideBar($(this).attr("data-id"));
		$("#hiddenColumnsModal").modal();
	});
};
if (typeof TableBuilder !== 'undefined') {
	//Override table select
	TableBuilder.prototype.dom = '<"CustomTableHeadBar">rtip';
	RenderTableSelect.prototype.settings.classNameText = 'no-sort';
	RenderTableSelect.prototype.settings.select.selector = "td:not(.not-selectable):first-child .checkbox-container";
	RenderTableSelect.prototype.selectHandler = function (context) {
		const row = $(context).closest('tr');
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
                                    <input type="checkbox" onchange="${handler}" class="custom-control-input" id="_select${id}"
                                           required>
                                    <label class="custom-control-label" for="_select${id}"></label>
                                </div>
                            </div>`;
	};

	RenderTableSelect.prototype.settings.headContent = function () {
		const id = st.newGuid();
		return new RenderTableSelect().selectTemplateCommom(id, "new RenderTableSelect().selectHeadHandler(this)");
	}.call();

	RenderTableSelect.prototype.templateSelect = function (data, type, row, meta) {
		const id = st.newGuid();
		return new RenderTableSelect().selectTemplateCommom(id, "new RenderTableSelect().selectHandler(this)");
	};

	//Table actions
	TableBuilder.prototype.getTableRowDeleteRestoreActionButton = function (row, dataX) {
		return `${dataX.hasDeleteRestore
			? `${row.isDeleted
				? `<a title="${window.translate("restore")}" href="javascript:void(0)" onclick="new TableBuilder().restoreItem('${row.id
				}', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')"><i class="material-icons">restore</i></a>`
				: `<a title="${window.translate("delete")}" href="javascript:void(0)" onclick="new TableBuilder().deleteItem('${row.id
				}', '#${dataX.listId}', '${dataX.viewmodelData.result.id}')"><i class="material-icons">delete</i></a>`}`
			: ``}`;
	};


	TableBuilder.prototype.getTableRowInlineActionButton = function (row, dataX) {
		if (row.isDeleted) return "";
		return `${dataX.hasInlineEdit
			? `	<a title="${window.translate("edit")}" class="inline-edit" data-viewmodel="${dataX.viewmodelData.result.id
			}" href="javascript:void(0)"><i class="material-icons">edit</i></a>`
			: ``}`;
	};


	TableBuilder.prototype.getTableRowEditActionButton = function (row, dataX) {
		if (row.isDeleted) return "";
		return `${dataX.hasEditPage ? `<a href="${dataX.editPageLink}?itemId=${row.id
			}&&listId=${dataX.viewmodelData.result.id}"><i class="material-icons">edit</i></a>`
			: ``}`;
	};

	TableBuilder.prototype.replaceTableSystemTranslations = function () {
		const customReplace = new Array();
		customReplace.push({ Key: "sProcessing", Value: `<div class="col-md"><div class="lds-dual-ring"></div></div>` });
		customReplace.push({ Key: "processing", Value: `<div class="col-md"><div class="lds-dual-ring"></div></div>` });
		const searialData = JSON.stringify(customReplace);
		return searialData;
	};
}



//override inline edit templates
if (typeof TableInlineEdit !== 'undefined') {
	TableInlineEdit.prototype.toggleVisibilityColumnsButton = function (ctx, state) {
		return;
	};

	TableInlineEdit.prototype.renderActiveInlineButton = function (ctx) {
		ctx.find("i").html("check");
	};

	TableInlineEdit.prototype.getActionsOnAdd = function () {
		const template = `<div class="btn-group" role="group" aria-label="Action buttons">
							<a href="javascript:void(0)" class='add-new-item'><i class="material-icons">check</i></a>
							<a href="javascript:void(0)" class='cancel-new-item'><i class="material-icons">cancel</i></a>
						</div>`;
		return template;
	};
}

//override notification populate container
Notificator.prototype.addNewNotificationToContainer = function (notification) {
	const _ = $("#notificationAlarm");
	if (!_.hasClass("notification"))
		_.addClass("notification");
	const template = this.createNotificationBodyContainer(notification);
	$("#notificationList").prepend(template);
	this.registerOpenNotificationEvent();
}

Notificator.prototype.createNotificationBodyContainer = function (n) {
	const block = `
		<a data-notification-id="${n.id}" href="#" class="notification-item dropdown-item py-3 border-bottom">
            <p><small>${n.subject}</small></p>
            <p class="text-muted mb-1"><small>${n.content}</small></p>
            <p class="text-muted mb-1"><small>${n.created}</small></p>
		</a>`;
	return block;
}

function getIdentifier(idt) {
	switch (idt) {
		case "en": {
			idt = "gb";
		} break;
		case "ja": {
			idt = "jp";
		} break;
		case "zh": {
			idt = "cn";
		} break;
		case "uk": {
			idt = "ua";
		} break;
		case "el": {
			idt = "gr";
		} break;
	}
	return idt;
}

function makeMenuActive(target) {
	if (target) {
		const last = target.closest("ul").closest("li");
		last.addClass("open");
		if (target.closest("nav").length !== 0)
			makeMenuActive(last);
	}
}

$(document).ready(function () {
	//Log Out
	$('.sa-logout').click(function () {
		swal({
			title: window.translate("confirm_log_out_question"),
			text: window.translate("log_out_message"),
			type: "warning",
			showCancelButton: true,
			confirmButtonColor: "#DD6B55",
			confirmButtonText: window.translate("confirm_logout"),
			cancelButtonText: window.translate("cancel")
		}).then((result) => {
			if (result.value) {
				$.ajax({
					url: '/Account/LocalLogout',
					type: "post",
					dataType: "json",
					contentType: "application/x-www-form-urlencoded; charset=utf-8",
					success: function (data) {
						if (data.success) {

							swal("Success!", data.message, "success");
							window.location.href = '/Account/Login';
						} else {
							swal("Fail!", data.message, "error");
						}
					},
					error: function () {
						swal("Fail!", "Server no response!", "error");
					}
				});
			};
		});
	});

	//Menu render promise
	const loadMenusPromise = new Promise((resolve, reject) => {
		const menus = load("/PageRender/GetMenus");
		resolve(menus);
	});

	loadMenusPromise.then(menus => {
		const renderMenuContainer = $("#left-nav-bar");
		if (menus.is_success) {
			const content = tManager.render("template_RenderIsoMenuItem.html", menus.result, {
				host: location.origin
			});
			renderMenuContainer.html(content);
			let route = location.href;
			if (route[route.length - 1] === "#") {
				route = route.substr(0, route.length - 1);
			}
			renderMenuContainer.find(`a[href='${route}']`)
				.parent()
				.addClass("active");
			makeMenuActive(renderMenuContainer.find(`a[href='${route}']`));
			window.forceTranslate("#left-nav-bar");
		}
	});


	//Localization promise
	var localizationPromise = new Promise((resolve, reject) => {
		//Set localization config
		let translateIcon = getIdentifier(settings.localization.current.identifier);
		$("#currentlanguage").addClass(`flag-icon flag-icon-${translateIcon}`);
		const languageBlock = $("#languageRegion");
		resolve(languageBlock);
	});

	localizationPromise.then(languageBlock => {
		$.each(settings.localization.languages, function (index, lang) {
			const language = `<a href="/Localization/ChangeLanguage?identifier=${lang.identifier}" class="dropdown-item language-event">
							<i class="flag-icon flag-icon-${getIdentifier(lang.identifier)}"></i> ${lang.name}
						</a>`;
			languageBlock.append(language);
		});
	});

	localizationPromise.then(() => {
		$(".language-event").on("click", function () {
			localStorage.removeItem("hasLoadedTranslations");
		});
	});

	//Emails promise
	var emailPromise = new Promise((resolve, reject) => {
		const notificator = new Notificator();
		const response = notificator.getFolders();
		if (response) resolve(response);
	});

	emailPromise.then(response => {
		if (response.is_success) {
			var folders = response.result.values;
			const f = folders.find((e) => e.Name === "Inbox");
			const uri = `/Email?folderId=${f.Id}`;
			$("#SeeAllEmails").attr("href", uri);

			const content = tManager.render("template_folders_layout.html", folders);
			var m = $(".notification-items");
			m.html(content);
			$("#right_menu").html(content);
			m.find("a").on("click", function () {
				const folderId = $(this).attr("folderid");
				if (folderId != undefined) {
					window.location.href = `/Email?folderId=${folderId}`;
				}
			});
		}
	});

	Promise.all([loadMenusPromise, localizationPromise, emailPromise]).then(function (values) {
		window.forceTranslate();
	});
});

/************************************************
					End Custom js
************************************************/


/************************************************
Page Pre Loader Removal After Page Load
************************************************/

var PreLoader;

$(window).on("load", function () {

	$('.loader-wrapper').not('.incomponent').fadeOut(1000, function () {
		PreLoader = $(this).detach();
	});

});

/************************************************
End Pre Loader Removal After Page Load
************************************************/


/*!
  * FreakPixels v1.1.0 (http://freakpixels.com/)
  * Copyright 2011-2018 The FreakPixels Authors 
  * Licensed under MIT    
  */


"use strict";

/************************************************
Page Pre Loader Removal After Page Load
************************************************/

var PreLoader;

$(window).on("load", function () {

	$('.loader-wrapper').not('.incomponent').fadeOut(1000, function () {
		PreLoader = $(this).detach();
	});

});






/* Dom Ready */
(function ($) {

	"use strict";


	/* Initialize Tooltip */
	$('[data-toggle="tooltip"]').tooltip();



	/* Initialize Popover */
	$('[data-toggle="popover"]').popover();



	/* Initialize Lightbox */
	$('body').delegate('[data-toggle="lightbox"]', 'click', function (event) {
		event.preventDefault();
		$(this).ekkoLightbox();
	});




	/************************************************
	Append Preloader (use in ajax call)
	************************************************/
	$('body').delegate('.append-preloader', 'click', function () {

		$(PreLoader).show();
		$('body').append(PreLoader);
		setTimeout(function () {

			$('.loader-wrapper').fadeOut(1000, function () {
				PreLoader = $(this).detach();
			});

		}, 3000);

	});


	/************************************************
	Toggle Preloader in card or box
	************************************************/
	$('body').delegate('[data-toggle="loader"]', 'click', function () {

		var target = $(this).attr('data-target');
		$('#' + target).show();

	});



	/************************************************
	Toggle Sidebar Nav
	************************************************/
	$('body').delegate('.toggle-sidebar', 'click', function () {
		$('.sidebar').toggleClass('collapsed');

		if (localStorage.getItem("asideMode") === 'collapsed') {
			localStorage.setItem("asideMode", 'expanded')
		} else {
			localStorage.setItem("asideMode", 'collapsed')
		}
		return false;
	});

	var p;
	$('body').delegate('.hide-sidebar', 'click', function () {
		if (p) {
			p.prependTo(".wrapper");
			p = null;
		} else {
			p = $(".sidebar").detach();
		}
	});

	$.fn.setAsideMode = function () {
		if (localStorage.getItem("asideMode") === null) {

		}
		else if (localStorage.getItem("asideMode") === 'collapsed') {
			$('.sidebar').addClass('collapsed');
		}
		else {
			$('.sidebar').removeClass('collapsed');
		}
	}
	if ($(window).width() > 768) {
		$.fn.setAsideMode();
	}






	/************************************************
Sidebar Nav Accordion
************************************************/
	$('body').delegate('.navigation li:has(.sub-nav) > a', 'click', function () {
		/*$('.navigation li').removeClass('open');*/
		$(this).siblings('.sub-nav').slideToggle();
		$(this).parent().toggleClass('open');
		return false;
	});




	/************************************************
	Sidebar Colapsed state submenu position
	************************************************/
	$('body').delegate('.navigation ul li:has(.sub-nav)', 'mouseover', function () {

		if ($(".sidebar").hasClass("collapsed")) {

			var $menuItem = $(this),
				$submenuWrapper = $('> .sub-nav', $menuItem);

			// grab the menu item's position relative to its positioned parent
			var menuItemPos = $menuItem.position();

			// place the submenu in the correct position relevant to the menu item
			$submenuWrapper.css({
				top: menuItemPos.top,
				left: menuItemPos.left + $menuItem.outerWidth()
			});
		}

	});




	/************************************************
	Toggle Controls on small devices
	************************************************/
	$('body').delegate('.toggle-controls', 'click', function () {
		$('.controls-wrapper').toggle().toggleClass('d-none');
	});






	/************************************************
	Toast Messages
	************************************************/
	$('body').delegate('[data-toggle="toast"]', 'click', function () {

		var dataAlignment = $(this).attr('data-alignment');
		var dataPlacement = $(this).attr('data-placement');
		var dataContent = $(this).attr('data-content');
		var dataStyle = $(this).attr('data-style');


		if ($('.toast.' + dataAlignment + '-' + dataPlacement).length) {

			$('.toast.' + dataAlignment + '-' + dataPlacement).append('<div class="alert alert-dismissible fade show alert-' + dataStyle + ' "> ' + dataContent + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div>');

		}
		else {
			$('body').append('<div class="toast ' + dataAlignment + '-' + dataPlacement + '"> <div class="alert alert-dismissible fade show alert-' + dataStyle + ' "> ' + dataContent + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div> </div>');
		}

	});





	/**************************************
	Chosen Form Control
	**************************************/
	$('.form-control-chosen').chosen({
		allow_single_deselect: true,
		width: '100%'
	});
	$('.form-control-chosen-required').chosen({
		allow_single_deselect: false,
		width: '100%'
	});
	$('.form-control-chosen-search-threshold-100').chosen({
		allow_single_deselect: true,
		disable_search_threshold: 100,
		width: '100%'
	});
	$('.form-control-chosen-optgroup').chosen({
		width: '100%'
	});
	$(function () {
		$('[title="clickable_optgroup"]').addClass('chosen-container-optgroup-clickable');
	});
	$(document).delegate('[title="clickable_optgroup"] .group-result', 'click', function () {
		var unselected = $(this).nextUntil('.group-result').not('.result-selected');
		if (unselected.length) {
			unselected.trigger('mouseup');
		} else {
			$(this).nextUntil('.group-result').each(function () {
				$('a.search-choice-close[data-option-array-index="' + $(this).data('option-array-index') + '"]').trigger('click');
			});
		}
	});






	/*****************************************
	Themer Changer with local storage
	*****************************************/

	$.fn.removeClassStartingWith = function (filter) {
		$(this).removeClass(function (index, className) {
			return (className.match(new RegExp("\\S*" + filter + "\\S*", 'g')) || []).join(' ')
		});
		return this;
	};


	$('body').delegate('.theme-changer', 'click', function () {
		var primaryColor = $(this).attr('primary-color');
		var sidebarBg = $(this).attr('sidebar-bg');
		var logoBg = $(this).attr('logo-bg');
		var headerBg = $(this).attr('header-bg');

		localStorage.setItem("primaryColor", primaryColor);
		localStorage.setItem("sidebarBg", sidebarBg);
		localStorage.setItem("logoBg", logoBg);
		localStorage.setItem("headerBg", headerBg);

		$.fn.setThemeTone(primaryColor);
	});



	$.fn.setThemeTone = function (primaryColor) {

		if (localStorage.getItem("primaryColor") === null) {

		}
		else {

			/* SIDEBAR */
			if (localStorage.getItem("sidebarBg") === "light") {
				$('.sidebar ').addClass('sidebar-light');
			}
			else {
				$('.sidebar').removeClass('sidebar-light');
			}



			/* PRIMARY COLOR */
			if (localStorage.getItem("primaryColor") === 'primary') {
				document.documentElement.style.setProperty('--theme-colors-primary', '#4B89FC');
			} else {
				var colorCode = getComputedStyle(document.body).getPropertyValue('--theme-colors-' + localStorage.getItem("primaryColor"));
				document.documentElement.style.setProperty('--theme-colors-primary', colorCode);
			}


			/* LOGO */
			if (localStorage.getItem("logoBg") === 'white' || localStorage.getItem("logoBg") === 'light') {
				$('.sidebar .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-dark').addClass('navbar-light bg-' + localStorage.getItem("logoBg"));
			} else {
				$('.sidebar .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-light').addClass('navbar-dark bg-' + localStorage.getItem("logoBg"));
			}



			/* HEADER */
			if (localStorage.getItem("headerBg") === "light" || localStorage.getItem("headerBg") === "white") {
				$('.header .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-dark').addClass('navbar-light bg-' + localStorage.getItem("headerBg"));
			}
			else {
				$('.header .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-light').addClass('navbar-dark bg-' + localStorage.getItem("headerBg"));
			}

		}



	}

	$.fn.setThemeTone();

})(jQuery);





/*****************************************
Full Screen Toggle
*****************************************/
function toggleFullScreen() {
	if ((document.fullScreenElement && document.fullScreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
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





