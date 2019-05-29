// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Events requires jQuery')
}

//Delete row from Jquery Table
function DeleteData(object) {
	swal({
		title: object.alertText,
		text: object.alertText,
		type: object.type,
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: object.confirmButtonText,
		cancelButtonText: window.translate("cancel")
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: `${object.urlForDelete}`,
				type: "post",
				dataType: "json",
				contentType: "application/x-www-form-urlencoded; charset=utf-8",
				data: {
					__RequestVerificationToken: window.getTokenAntiForgery(),
					id: object.rowId
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

//------------------------------------------------------------------------------------//
//								Random color
//------------------------------------------------------------------------------------//
window.getRandomColor = function () {
	const letters = '0123456789ABCDEF';
	let color = '#';
	for (var i = 0; i < 6; i++) {
		color += letters[Math.floor(Math.random() * 16)];
	}
	return color;
};


//------------------------------------------------------------------------------------//
//								Templates
//------------------------------------------------------------------------------------//

function TemplateManager() { }

/**
 * Get template from server
 * @param {any} identifierName
 */
TemplateManager.prototype.getTemplate = function (identifierName) {
	//const template = localStorage.getItem(identifierName);
	//if (template) return template;
	const serverTemplate = load("/Templates/GetTemplateByIdentifier",
		{
			identifier: identifierName
		});
	if (serverTemplate) {
		if (serverTemplate.is_success) {
			const temp = serverTemplate.result;
			localStorage.setItem(identifierName, temp);
			return temp;
		} else {
			console.log(serverTemplate);
		}
	}
	return "";
}

/**
 * Remove template from storage by template identifier
 * @param {any} identifier
 */
TemplateManager.prototype.removeTemplate = function (identifier) {
	localStorage.removeItem(identifier);
}

/**
 * Register template into
 * @param {any} identifier
 */
TemplateManager.prototype.registerTemplate = function (identifier) {
	$.templates(identifier, this.getTemplate(identifier));
}

/**
 * Render template and return content
 * @param {any} identifier
 * @param {any} data
 */
TemplateManager.prototype.render = function (identifier, data, helpers) {
	$.views.settings.allowCode(true);
	this.registerTemplate(identifier);
	return $.render[identifier](data, helpers);
}

//------------------------------------------------------------------------------------//
//								Translations
//------------------------------------------------------------------------------------//

//Get translations from storage
window.translations = function () {
	const cached = localStorage.getItem("translations");
	let trans = {};
	if (!cached) {
		trans = load("/Localization/GetTranslationsForCurrentLanguage");
		localStorage.setItem("translations", JSON.stringify(trans));
	} else {
		trans = JSON.parse(cached);
	}
	window.localTranslations = trans;
	return trans;
}


window.translate = function (key) {
	if (window.localTranslations) {
		return window.localTranslations[key];
	}
	const trans = window.translations();
	return trans[key];
}


//$(document).ajaxComplete(function (event, xhr, settings) {
//	window.forceTranslate();
//});


//On page load translate all
$(document).ready(function () {
	window.forceTranslate();
});

//Translate page content
window.forceTranslate = function () {
	new Promise((resolve, reject) => {
		const translations = Array.prototype.filter.call(
			document.getElementsByTagName('*'),
			function (el) {
				return el.getAttribute('translate') != null && !el.hasAttribute("translated");
			}
		);
		const trans = window.translations();
		$.each(translations,
			function (index, item) {
				let key = $(item).attr("translate");
				$(item).text(trans[key]);
				$(item).attr("translated", "");
			});
		resolve();
	});
};

//------------------------------------------------------------------------------------//
//								Dynamic tree for ISO Standard
//------------------------------------------------------------------------------------//


function loadTree(uri, data = null, type = "get") {
	try {
		const url = new URL(location.href);
		uri = `${url.origin}${uri}`;

		const req = $.ajax({
			url: uri,
			type: type,
			data: data,
			async: false
		});
		return JSON.parse(req.responseText);
	} catch (exp) {
		console.log(exp);
		return null;
	}
}

$(document).ready(function () {
	//find all trees in page
	let trees = $(".custom-tree-iso");
	$.each(trees, function (index, tree) {

		//Check if is not in edit mode
		if (!(location.href.indexOf("about:blank") !== -1)) {
			const standardId = $(tree).attr("db-tree-standard");
			const categoryId = $(tree).attr("db-tree-category");
			const requirementId = $(tree).attr("db-tree-requirement");
			const data = loadTree(`/IsoStandard/GetTreeData?standardEntityId=${standardId}&&categoryEntityId=${categoryId}&&requirementEntityId=${requirementId}`);
			if (data.is_success) {
				const templateManager = new TemplateManager();
				$.templates("IsoCategory", templateManager.getTemplate("template_category.html"));
				$.templates("IsoRequirements", templateManager.getTemplate("template_requirements.html"));
				$.templates("IsoTree", templateManager.getTemplate("template_standard.html"));
				const content = $.render["IsoTree"](data);
				$(tree).html(content);
			}
		}
	});
});


//------------------------------------------------------------------------------------//
//								External Connections
//------------------------------------------------------------------------------------//


var connection = new signalR.HubConnectionBuilder()
	.withUrl("/rtn", signalR.HttpTransportType.LongPolling)
	.build();


//On receive email
connection.on("SendClientEmailNotification", (...data) => {
	$("#notificationEmailAlarm").show();
	var create = CreateEmailNotification(data, data[4], "");
	$("#emailNotificationsContent").prepend(create);
	var count = $("#emailNotificationsContent").find("a").length;
	$("#emailNotificationsCounter").html(count);
});


//On receive notification
connection.on("SendClientNotification", (notification) => {
	performNotification(notification);
});


function performNotification(notification) {
	$("#notificationAlarm").show();
	const template = createNotification(notification);
	$("#notificationList").prepend(template);
}

//Start connection
connection.start();

$(document).ready(function () {
	startUserConnection();
	loadEmails();
	loadNotifications();
	$("#clearNotificationsEvent").on("click",
		function () {
			$("#notificationList").html(null);
			$("#notificationAlarm").hide();
			const notificator = new Notificator();
			notificator.clearNotificationsOnCurrentUser();
		});
});


function startUserConnection() {
	const notificator = new Notificator();
	const user = notificator.getCurrentUser();
	connection.invoke("OnLoad", user.result.id)
		.catch(err => console.error(err.toString()));
}


/**
 * Template creator for email
 * @param {any} arr
 * @param {any} userId
 * @param {any} emailnotId
 * @param {any} d
 */
function CreateEmailNotification(arr, userId, emailNotId, d = null) {
	const date = d != null ? d : new Date();
	let hours = `${date.getHours()}:${date.getMinutes()}`;

	const header = `
		<div class="user-img">
			<img src="/users/getimage?id=${userId}" alt="${arr[2]}" class="img-circle">
			<span class="profile-status online pull-right"></span>
		</div>`;
	const content = `
		<div class="mail-contnet">
			<h5>${arr[3]}</h5>
			<span class="mail-desc">${arr[0]}</span>
			<span class="time">${hours}</span>
		</div>`;
	const newEmailNotification = `<a href='/email/getmessagebyid?id=${emailNotId}'>${header}${content}</a>`;
	return newEmailNotification;
}
/**
 * Create notification by template
 * @param {any} n
 */
function createNotification(n) {
	const content = `
		<div class="mail-contnet">
			<h5>${n.subject}</h5> <span class="mail-desc">${n.content}</span>
				<span class="time">
					${n.created}
				</span>
		</div>`;
	const block = `
		<a notId="${n.id}" href="#">
			<div class="btn btn-danger btn-circle"><i class="fa fa-link"></i></div>
			${content}
		</a>`;
	return block;
}

/**
 * Show notifications on page load
 */
function loadNotifications() {
	const notificator = new Notificator();
	const notificationList = notificator.getAllNotifications();
	if (notificationList.is_success) {
		for (let notification in notificationList.result) {
			performNotification(notificationList.result[notification]);
		}
	}
}


function loadEmails() {
	const notificator = new Notificator();
	const st = new ST();
	var m = $(".notification-items");
	const response = notificator.getFolders();
	if (response != null) {
		if (response.is_success) {
			var folders = response.result.values;
			const f = folders.find((e) => e.Name === "Inbox");
			emailNotifications(f.Id);
			const uri = `/Email?folderId=${f.Id}`;
			$("#SeeAllEmails").attr("href", uri);
			Promise.all([st.getTemplate("folders_layout.html")])
				.then(function (values) {
					$.templates("items", values[0]);
					const content = $.render["items"](folders);
					m.html(content);
					$("#right_menu").html(content);
					m.find("a").on("click", function () {
						const folderId = $(this).attr("folderid");
						if (folderId != undefined) {
							window.location.href = `/Email?folderId=${folderId}`;
						}
					});
				})
				.catch(function (err) {
					console.log(err);
				});
		}
	}
}

/**
 * Show email notifications
 * @param {any} folderId
 */
function emailNotifications(folderId) {
	const notificator = new Notificator();
	const data = notificator.getUnreadMessages(folderId);
	if (data) {
		if (data.is_success) {
			const all = data.result.notifications.values;
			for (const e in all) {
				const arr = [all[e].Subject, all[e].Message, all[e].Author.Email, all[e].Author.UserName];

				const userId = all[e].Author.Id;
				const create = CreateEmailNotification(arr, userId, all[e].Id, all[e].Created);
				$("#emailNotificationsContent").prepend(create);
				const count = $("#emailNotificationsContent").find("a").length;
				$("#emailNotificationsCounter").html(count);
			}
			if (all.length > 0) $("#notificationEmailAlarm").show();
		}
	}
}

//------------------------------------------------------------------------------------//
//								Notificator
//------------------------------------------------------------------------------------//
// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Events requires jQuery');
}

$(document).ready(function () {
	$(".sa-logout").on("click", function () {
		localStorage.removeItem("current_user");
	});
});


function Notificator() { }
/**
 * Constructor
 */
Notificator.prototype.constructor = Notificator;

Notificator.prototype.origin = function () {
	const uri = new URL(location.href);
	return uri.origin;
};

/**
 * Send notification to list of users
 * @param {any} data Data with fields for send a notification
 */
Notificator.prototype.sendNotification = function (data) {
	$.ajax({
		url: "",
		data: {},
		method: "post",
		success: function (data) {

		},
		error: function (error) {
			console.log(error);
		}
	});
};
/**
 * Get folders
 *@returns {any} Folders data
 */
Notificator.prototype.getFolders = function () {
	const settings = localStorage.getItem("settings");
	if (settings) {
		const d = JSON.parse(settings);
		if (d.hasOwnProperty("email")) {
			return d.email.folders;
		} else {
			d.email = {
				folders: loadEmailFolders()
			};
			localStorage.setItem("settings", JSON.stringify(d));
		}
	} else {
		const f = loadEmailFolders();
		const s = {
			email: {
				folders: f
			}
		};
		localStorage.setItem("settings", JSON.stringify(s));
		return f;
	}
};


function loadEmailFolders() {
	var data = null;
	$.ajax({
		url: `/api/Email/GetFolders`,
		method: "get",
		async: false,
		success: function (response) {
			data = response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return data;
}


/**
 * Get all notifications
 *@returns {any} Folders data
 */
Notificator.prototype.getAllNotifications = function () {
	var data = null;
	var userId = null;
	const req = this.getCurrentUser();

	if (req.is_success) {
		userId = req.result.id;
	}
	$.ajax({
		url: `${this.origin()}/api/Notifications/GetNotificationsByUserId`,
		method: "get",
		data: {
			userId: userId
		},
		async: false,
		success: function (response) {
			data = response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return data;
};


/**
 * Clear all notifications by user id
 *@returns {any} nothing
 */
Notificator.prototype.clearNotificationsOnCurrentUser = function () {
	var data = null;
	var userId = null;
	const req = this.getCurrentUser();

	if (req.is_success) {
		userId = req.result.id;
	}
	$.ajax({
		url: `${this.origin()}/api/Notifications/ClearAllByUserId`,
		method: "post",
		data: {
			userId: userId
		},
		async: false,
		success: function (response) {
			data = response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return data;
};


/**
 * Mark as read notification
 * @param {any} notificationId The notification id on read
 */
Notificator.prototype.markAsRead = function (notificationId) {
	$.ajax({
		url: `${this.origin()}/api/Notifications/MarkAsRead`,
		method: "post",
		data: {
			notificationId: notificationId
		},
		success: function (data) {

		},
		error: function (error) {
			console.log(error);
		}
	});
};

/**
 * Get list By folder id
 * @param {any} folderId The folder id
 * @param {any} page The number of page
 * @returns {any} List with notifications
 */
Notificator.prototype.getListByFolderId = function (folderId, page) {
	var response = null;
	$.ajax({
		url: "/api/Email/GetListByFolderId",
		async: false,
		data: {
			folderId: folderId,
			page: page
		},
		method: "get",
		success: function (data) {
			response = data;
			return response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return response;
};


Notificator.prototype.getUnreadMessages = function (folderId) {
	var response = null;
	$.ajax({
		url: "/api/Email/GetUnreadListByFolderId",
		async: false,
		data: {
			folderId: folderId
		},
		method: "get",
		success: function (data) {
			response = data;
			return response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return response;
};

/**
 * Mark as read notification
 * @param {any} notificationId The notification id on read
 */
Notificator.prototype.markAsRead = function (notificationId) {
	$.ajax({
		url: "/api/Email/MarkAsRead",
		method: "post",
		data: {
			notificationId: notificationId
		},
		success: function (data) {

		},
		error: function (error) {
			console.log(error);
		}
	});
};

/**
 * Move Notification to another folder
 * @param {any} notificationId The id of notification
 * @param {any} folderId The folder where the notification are moved
 */
Notificator.prototype.moveTofolder = function (notificationId, folderId) {
	$.ajax({
		url: "/api/Email/MoveToFolder",
		method: "post",
		data: {
			notificationId: notificationId,
			folderId: folderId
		},
		success: function (data) {
			if (data != null) {
				if (data.is_success) {
					window.location.reload();
				}
			}
		},
		error: function (error) {
			console.log(error);
		}
	});
};

/**
 * Request with ajax
 * @param {any} data Ajax parameters
 */
Notificator.prototype.sendRequest = function (data) {
	$.ajax({
		url: data.url,
		method: data.method,
		async: data.async,
		data: data.data,
		success: data.success,
		error: data.success
	});
};
/**
* Get user by id
* @param {any} userId The user id
* @returns {any} User data
*/
Notificator.prototype.getUser = function (userId) {
	var response = null;
	$.ajax({
		url: "/api/Users/GetUserById",
		method: "get",
		data: {
			userId: userId
		},
		async: false,
		success: function (data) {
			response = data;
			return response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return response;
};

/**
* Get current user
* @param {any} userId The user id
* @returns {any} User data
*/
Notificator.prototype.getCurrentUser = function () {
	const userData = localStorage.getItem("current_user");
	if (userData) {
		const parsedUserData = JSON.parse(userData);
		const created = new Date(parsedUserData.created).getTime();
		const now = new Date().getTime();
		const diff = now - created;
		if (diff > 0 && diff < 50000) {
			return parsedUserData;
		}
	}

	var response = null;
	$.ajax({
		url: `${this.origin()}/Account/GetCurrentUser`,
		method: "get",
		async: false,
		success: function (data) {
			data.created = new Date();
			response = data;
			if (response.is_success)
				localStorage.setItem("current_user", JSON.stringify(data));
			return response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return response;
};


/**
* Get count of notifications by folder id
* @param {any} folderId The folder id
* @returns {any} Notifications count
*/
Notificator.prototype.countNotificationsByFolderId = function (folderId) {
	var response = 0;
	$.ajax({
		url: "/api/Email/GetCountbyFolderId",
		async: false,
		data: {
			folderId: folderId
		},
		method: "get",
		success: function (data) {
			if (data != null) {
				if (data.is_success) {
					response = data.result;
				}
			}
			return response;
		},
		error: function (error) {
			console.log(error);
		}
	});
	return response;
};

Notificator.prototype.restore = function (notificationId) {
	$.ajax({
		url: "/api/Email/RestoreFromTrash",
		method: "post",
		data: {
			notificationId: notificationId
		},
		success: function (data) {
			if (data != null) {
				if (data.is_success) {
					window.location.reload();
				}
			}
		},
		error: function (error) {
			console.log(error);
		}
	});
};



//Actions
function ST() { }
/**
 * Constructor
 */
ST.prototype.constructor = ST;

/**
 * Paginate list
 * @param {any} selector Selector for new pagination
 * @param {any} page Current page
 * @param {any} perPage For page
 * @param {any} total Total items
 * @param {any} url Url for get items
 */
ST.prototype.pagination = function (selector, page = 1, perPage = 10, total = 0, url) {
	$(selector).pagination({
		items: total,
		itemsOnPage: perPage,
		cssStyle: 'light-theme',
		onInit: function () { },
		onPageClick: function (pageNumber, event) {
			var origin = location.origin;
			var href = location.href;
			var parsedUrl = new URL(href);
			var curr = parsedUrl.searchParams.get("page");
			if (curr != pageNumber)
				window.location.href = url + "?page=" + pageNumber + "&perPage=" + perPage;
		}
	});
	$(selector).pagination('selectPage', page);
};

/**
 * Wait by specify time and call function
 * @param {any} seconds Time for wait
 * @param {any} callback The function for call
 * @returns {any} Activate events
 */
ST.prototype.wait = function (seconds, callback) {
	return window.setTimeout(callback, seconds);
};


/**
 * Serialize to json form fields
 * @param {any} form The selector of form
 * @returns {json} The resulted json after serialize
 */
ST.prototype.serializeToJson = function (form) {
	var serializer = form.serializeArray();
	var _string = '{';
	for (var ix in serializer) {
		var row = serializer[ix];
		_string += '"' + row.name + '":"' + row.value + '",';
	}
	var end = _string.length - 1;
	_string = _string.substr(0, end);
	_string += '}';
	return JSON.parse(_string);
};

/**
 * Populate form fields
 * @param {any} frm The selector of form
 * @param {any} data Json for form populate
 */
ST.prototype.populateForm = function (frm, data) {
	$.each(data, function (key, value) {
		var $ctrl = $('[name=' + key + ']', frm)
		if ($ctrl.is('select')) {
			$("option", $ctrl).each(function () {
				if (this.value === value) {
					this.selected = "selected"
				}
			});
		}
		if ($ctrl.is('textarea')) {
			$ctrl.html(value);
		}
		else {
			switch ($ctrl.attr("type")) {
				case "text": case "hidden": case "number":
					$ctrl.val(value);
					break;
				case "radio": case "checkbox":
					$ctrl.each(function () {
						if ($(this).attr('value') === value) {
							$(this).attr("checked", value);
						}
					});
					break;
			}
		}
	});
};

/**
 * Get template from server
 * @param {any} relPath Path of template
 * @returns {string} Return template
 */
ST.prototype.getTemplate = function (relPath) {
	return new Promise(function (resolve, reject) {
		$.ajax({
			url: "/StaticFile/GetJRenderTemplate",
			data: {
				relPath: relPath
			},
			success: function (data) {
				if (data) {
					resolve(data);
				} else {
					reject(new Error("TemplateData Invalid!!!"));
				}
			},
			error: function (err) {
				reject(err);
			}
		});
	});
};


/**
 * Generate new Guid
 * @returns {guid} Return new generate guid
 */
ST.prototype.newGuid = function () {
	var result = '';
	var hexcodes = "0123456789abcdef".split("");

	for (var index = 0; index < 32; index++) {
		var value = Math.floor(Math.random() * 16);

		switch (index) {
			case 8:
				result += '-';
				break;
			case 12:
				value = 4;
				result += '-';
				break;
			case 16:
				value = value & 3 | 8;
				result += '-';
				break;
			case 20:
				result += '-';
				break;
		}
		result += hexcodes[value];
	}
	return result;
};

$(document).ready(function () {
	$('.ui.st_menu').dropdown();
});







$(function () {
	"use strict";
	$(function () {
		$(".preloader").fadeOut()
	}), jQuery(document).on("click", ".mega-dropdown", function (i) {
		i.stopPropagation()
	});
	var i = function () {
		var i = window.innerWidth > 0 ? window.innerWidth : this.screen.width,
			e = 70;
		1170 > i ? ($("body").addClass("mini-sidebar"), $(".navbar-brand span").hide(), $(".scroll-sidebar, .slimScrollDiv").css("overflow-x", "visible").parent().css("overflow", "visible"), $(".sidebartoggler i").addClass("ti-menu")) : ($("body").removeClass("mini-sidebar"), $(".navbar-brand span").show(), $(".sidebartoggler i").removeClass("ti-menu"));
		var s = (window.innerHeight > 0 ? window.innerHeight : this.screen.height) - 1;
		s -= e, 1 > s && (s = 1), s > e && $(".page-wrapper").css("min-height", s + "px")
	};
	$(window).ready(i), $(window).on("resize", i), $(".sidebartoggler").on("click",
		function () {
			$("body").hasClass("mini-sidebar")
				? ($("body").trigger("resize"),
					$(".scroll-sidebar, .slimScrollDiv").css("overflow", "hidden").parent()
						.css("overflow", "visible"), $("body").removeClass("mini-sidebar"),
					$(".navbar-brand span").show(), $(".sidebartoggler i").addClass("ti-menu"))
				: ($("body").trigger("resize"), $(".scroll-sidebar, .slimScrollDiv").css("overflow-x", "visible")
					.parent()
					.css("overflow", "visible"), $("body").addClass("mini-sidebar"), $(".navbar-brand span")
						.hide(),
					$(".sidebartoggler i").removeClass("ti-menu"))
		}), $(".fix-header .topbar").stick_in_parent({}), $(".nav-toggler").click(function () {
			$("body").toggleClass("show-sidebar"), $(".nav-toggler i").toggleClass("ti-menu"), $(".nav-toggler i")
				.addClass("ti-close")
		}), $(".sidebartoggler").on("click",
			function () {
				$(".sidebartoggler i").toggleClass("ti-menu")
			}), $(".right-side-toggle").click(function () {
				$(".right-sidebar-central").slideDown(50), $(".right-sidebar-central").toggleClass("shw-rside")
			}), $(function () {
				for (var i = window.location,
					e = $("ul#sidebarnav a").filter(function () {
						return this.href == i
					}).addClass("active").parent().addClass("active"); ;
				) {
					if (!e.is("li")) break;
					e = e.parent().addClass("in").parent().addClass("active")
				}
			}), $(function () {
				$('[data-toggle="tooltip"]').tooltip()
			}), $(function () {
				$('[data-toggle="popover"]').popover()
			}), $(function () {
				$("#sidebarnav").metisMenu()
			}), $(".scroll-sidebar").slimScroll({
				position: "left",
				size: "5px",
				height: "100%",
				color: "#dcdcdc"
			}), $(".message-center").slimScroll({
				position: "right",
				size: "5px",
				color: "#dcdcdc"
			}), $(".aboutscroll").slimScroll({
				position: "right",
				size: "5px",
				height: "80",
				color: "#dcdcdc"
			}), $(".message-scroll").slimScroll({
				position: "right",
				size: "5px",
				height: "570",
				color: "#dcdcdc"
			}), $(".chat-box").slimScroll({
				position: "right",
				size: "5px",
				height: "470",
				color: "#dcdcdc"
			}), $(".slimscrollright").slimScroll({
				height: "100%",
				position: "right",
				size: "5px",
				color: "#dcdcdc"
			}), $("body").trigger("resize"), $(".list-task li label").click(function () {
				$(this).toggleClass("task-done")
			}), $("#to-recover").on("click",
				function () {
					$("#loginform").slideUp(), $("#recoverform").fadeIn()
				}), $(document).on("click",
					".card-actions a",
					function (i) {
						i.preventDefault(), $(this).hasClass("btn-close") && $(this).parent().parent().parent().fadeOut()
					}),
		function (i, e, s) {
			var o = '[data-perform="card-collapse"]';
			i(o).each(function () {
				var e = i(this),
					s = e.closest(".card"),
					o = s.find(".card-block"),
					r = {
						toggle: !1
					};
				o.length ||
					(o = s.children(".card-heading").nextAll().wrapAll("<div/>").parent().addClass("card-block"), r =
						{}), o.collapse(r).on("hide.bs.collapse",
							function () {
								e.children("i").removeClass("ti-minus").addClass("ti-plus")
							}).on("show.bs.collapse",
								function () {
									e.children("i").removeClass("ti-plus").addClass("ti-minus")
								})
			}), i(s).on("click",
				o,
				function (e) {
					e.preventDefault();
					var s = i(this).closest(".card"),
						o = s.find(".card-block");
					o.collapse("toggle")
				})
		}(jQuery, window, document);



	(function (global, factory) {
		if (typeof define === "function" && define.amd) {
			define(['jquery'], factory);
		} else if (typeof exports !== "undefined") {
			factory(require('jquery'));
		} else {
			var mod = {
				exports: {}
			};
			factory(global.jquery);
			global.metisMenu = mod.exports;
		}
	})(this, function (_jquery) {
		'use strict';

		var _jquery2 = _interopRequireDefault(_jquery);

		function _interopRequireDefault(obj) {
			return obj && obj.__esModule ? obj : {
				default: obj
			};
		}

		var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
			return typeof obj;
		} : function (obj) {
			return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
		};

		function _classCallCheck(instance, Constructor) {
			if (!(instance instanceof Constructor)) {
				throw new TypeError("Cannot call a class as a function");
			}
		}

		var Util = function ($) {
			var transition = false;

			var TransitionEndEvent = {
				WebkitTransition: 'webkitTransitionEnd',
				MozTransition: 'transitionend',
				OTransition: 'oTransitionEnd otransitionend',
				transition: 'transitionend'
			};

			function getSpecialTransitionEndEvent() {
				return {
					bindType: transition.end,
					delegateType: transition.end,
					handle: function handle(event) {
						if ($(event.target).is(this)) {
							return event.handleObj.handler.apply(this, arguments);
						}
						return undefined;
					}
				};
			}

			function transitionEndTest() {
				if (window.QUnit) {
					return false;
				}

				var el = document.createElement('mm');

				for (var name in TransitionEndEvent) {
					if (el.style[name] !== undefined) {
						return {
							end: TransitionEndEvent[name]
						};
					}
				}

				return false;
			}

			function transitionEndEmulator(duration) {
				var _this2 = this;

				var called = false;

				$(this).one(Util.TRANSITION_END, function () {
					called = true;
				});

				setTimeout(function () {
					if (!called) {
						Util.triggerTransitionEnd(_this2);
					}
				}, duration);

				return this;
			}

			function setTransitionEndSupport() {
				transition = transitionEndTest();
				$.fn.emulateTransitionEnd = transitionEndEmulator;

				if (Util.supportsTransitionEnd()) {
					$.event.special[Util.TRANSITION_END] = getSpecialTransitionEndEvent();
				}
			}

			var Util = {
				TRANSITION_END: 'mmTransitionEnd',

				triggerTransitionEnd: function triggerTransitionEnd(element) {
					$(element).trigger(transition.end);
				},
				supportsTransitionEnd: function supportsTransitionEnd() {
					return Boolean(transition);
				}
			};

			setTransitionEndSupport();

			return Util;
		}(jQuery);

		var MetisMenu = function ($) {

			var NAME = 'metisMenu';
			var DATA_KEY = 'metisMenu';
			var EVENT_KEY = '.' + DATA_KEY;
			var DATA_API_KEY = '.data-api';
			var JQUERY_NO_CONFLICT = $.fn[NAME];
			var TRANSITION_DURATION = 350;

			var Default = {
				toggle: true,
				preventDefault: true,
				activeClass: 'active',
				collapseClass: 'collapse',
				collapseInClass: 'in',
				collapsingClass: 'collapsing',
				triggerElement: 'a',
				parentTrigger: 'li',
				subMenu: 'ul'
			};

			var Event = {
				SHOW: 'show' + EVENT_KEY,
				SHOWN: 'shown' + EVENT_KEY,
				HIDE: 'hide' + EVENT_KEY,
				HIDDEN: 'hidden' + EVENT_KEY,
				CLICK_DATA_API: 'click' + EVENT_KEY + DATA_API_KEY
			};

			var MetisMenu = function () {
				function MetisMenu(element, config) {
					_classCallCheck(this, MetisMenu);

					this._element = element;
					this._config = this._getConfig(config);
					this._transitioning = null;

					this.init();
				}

				MetisMenu.prototype.init = function init() {
					var self = this;
					$(this._element).find(this._config.parentTrigger + '.' + this._config.activeClass).has(this._config.subMenu).children(this._config.subMenu).attr('aria-expanded', true).addClass(this._config.collapseClass + ' ' + this._config.collapseInClass);

					$(this._element).find(this._config.parentTrigger).not('.' + this._config.activeClass).has(this._config.subMenu).children(this._config.subMenu).attr('aria-expanded', false).addClass(this._config.collapseClass);

					$(this._element).find(this._config.parentTrigger).has(this._config.subMenu).children(this._config.triggerElement).on(Event.CLICK_DATA_API, function (e) {
						var _this = $(this);
						var _parent = _this.parent(self._config.parentTrigger);
						var _siblings = _parent.siblings(self._config.parentTrigger).children(self._config.triggerElement);
						var _list = _parent.children(self._config.subMenu);
						if (self._config.preventDefault) {
							e.preventDefault();
						}
						if (_this.attr('aria-disabled') === 'true') {
							return;
						}
						if (_parent.hasClass(self._config.activeClass)) {
							_this.attr('aria-expanded', false);
							self._hide(_list);
						} else {
							self._show(_list);
							_this.attr('aria-expanded', true);
							if (self._config.toggle) {
								_siblings.attr('aria-expanded', false);
							}
						}

						if (self._config.onTransitionStart) {
							self._config.onTransitionStart(e);
						}
					});
				};

				MetisMenu.prototype._show = function _show(element) {
					if (this._transitioning || $(element).hasClass(this._config.collapsingClass)) {
						return;
					}
					var _this = this;
					var _el = $(element);

					var startEvent = $.Event(Event.SHOW);
					_el.trigger(startEvent);

					if (startEvent.isDefaultPrevented()) {
						return;
					}

					_el.parent(this._config.parentTrigger).addClass(this._config.activeClass);

					if (this._config.toggle) {
						this._hide(_el.parent(this._config.parentTrigger).siblings().children(this._config.subMenu + '.' + this._config.collapseInClass).attr('aria-expanded', false));
					}

					_el.removeClass(this._config.collapseClass).addClass(this._config.collapsingClass).height(0);

					this.setTransitioning(true);

					var complete = function complete() {

						_el.removeClass(_this._config.collapsingClass).addClass(_this._config.collapseClass + ' ' + _this._config.collapseInClass).height('').attr('aria-expanded', true);

						_this.setTransitioning(false);

						_el.trigger(Event.SHOWN);
					};

					if (!Util.supportsTransitionEnd()) {
						complete();
						return;
					}

					_el.height(_el[0].scrollHeight).one(Util.TRANSITION_END, complete).emulateTransitionEnd(TRANSITION_DURATION);
				};

				MetisMenu.prototype._hide = function _hide(element) {

					if (this._transitioning || !$(element).hasClass(this._config.collapseInClass)) {
						return;
					}
					var _this = this;
					var _el = $(element);

					var startEvent = $.Event(Event.HIDE);
					_el.trigger(startEvent);

					if (startEvent.isDefaultPrevented()) {
						return;
					}

					_el.parent(this._config.parentTrigger).removeClass(this._config.activeClass);
					_el.height(_el.height())[0].offsetHeight;

					_el.addClass(this._config.collapsingClass).removeClass(this._config.collapseClass).removeClass(this._config.collapseInClass);

					this.setTransitioning(true);

					var complete = function complete() {
						if (_this._transitioning && _this._config.onTransitionEnd) {
							_this._config.onTransitionEnd();
						}

						_this.setTransitioning(false);
						_el.trigger(Event.HIDDEN);

						_el.removeClass(_this._config.collapsingClass).addClass(_this._config.collapseClass).attr('aria-expanded', false);
					};

					if (!Util.supportsTransitionEnd()) {
						complete();
						return;
					}

					_el.height() == 0 || _el.css('display') == 'none' ? complete() : _el.height(0).one(Util.TRANSITION_END, complete).emulateTransitionEnd(TRANSITION_DURATION);
				};

				MetisMenu.prototype.setTransitioning = function setTransitioning(isTransitioning) {
					this._transitioning = isTransitioning;
				};

				MetisMenu.prototype.dispose = function dispose() {
					$.removeData(this._element, DATA_KEY);

					$(this._element).find(this._config.parentTrigger).has(this._config.subMenu).children(this._config.triggerElement).off('click');

					this._transitioning = null;
					this._config = null;
					this._element = null;
				};

				MetisMenu.prototype._getConfig = function _getConfig(config) {
					config = $.extend({}, Default, config);
					return config;
				};

				MetisMenu._jQueryInterface = function _jQueryInterface(config) {
					return this.each(function () {
						var $this = $(this);
						var data = $this.data(DATA_KEY);
						var _config = $.extend({}, Default, $this.data(), (typeof config === 'undefined' ? 'undefined' : _typeof(config)) === 'object' && config);

						if (!data && /dispose/.test(config)) {
							this.dispose();
						}

						if (!data) {
							data = new MetisMenu(this, _config);
							$this.data(DATA_KEY, data);
						}

						if (typeof config === 'string') {
							if (data[config] === undefined) {
								throw new Error('No method named "' + config + '"');
							}
							data[config]();
						}
					});
				};

				return MetisMenu;
			}();

			/**
			 * ------------------------------------------------------------------------
			 * jQuery
			 * ------------------------------------------------------------------------
			 */

			$.fn[NAME] = MetisMenu._jQueryInterface;
			$.fn[NAME].Constructor = MetisMenu;
			$.fn[NAME].noConflict = function () {
				$.fn[NAME] = JQUERY_NO_CONFLICT;
				return MetisMenu._jQueryInterface;
			};
			return MetisMenu;
		}(jQuery);
	});





	!function (t) { "use strict"; function e(t) { return null !== t && t === t.window } function n(t) { return e(t) ? t : 9 === t.nodeType && t.defaultView } function a(t) { var e, a, i = { top: 0, left: 0 }, o = t && t.ownerDocument; return e = o.documentElement, "undefined" != typeof t.getBoundingClientRect && (i = t.getBoundingClientRect()), a = n(o), { top: i.top + a.pageYOffset - e.clientTop, left: i.left + a.pageXOffset - e.clientLeft } } function i(t) { var e = ""; for (var n in t) t.hasOwnProperty(n) && (e += n + ":" + t[n] + ";"); return e } function o(t) { if (d.allowEvent(t) === !1) return null; for (var e = null, n = t.target || t.srcElement; null !== n.parentElement;) { if (!(n instanceof SVGElement || -1 === n.className.indexOf("waves-effect"))) { e = n; break } if (n.classList.contains("waves-effect")) { e = n; break } n = n.parentElement } return e } function r(e) { var n = o(e); null !== n && (c.show(e, n), "ontouchstart" in t && (n.addEventListener("touchend", c.hide, !1), n.addEventListener("touchcancel", c.hide, !1)), n.addEventListener("mouseup", c.hide, !1), n.addEventListener("mouseleave", c.hide, !1)) } var s = s || {}, u = document.querySelectorAll.bind(document), c = { duration: 750, show: function (t, e) { if (2 === t.button) return !1; var n = e || this, o = document.createElement("div"); o.className = "waves-ripple", n.appendChild(o); var r = a(n), s = t.pageY - r.top, u = t.pageX - r.left, d = "scale(" + n.clientWidth / 100 * 10 + ")"; "touches" in t && (s = t.touches[0].pageY - r.top, u = t.touches[0].pageX - r.left), o.setAttribute("data-hold", Date.now()), o.setAttribute("data-scale", d), o.setAttribute("data-x", u), o.setAttribute("data-y", s); var l = { top: s + "px", left: u + "px" }; o.className = o.className + " waves-notransition", o.setAttribute("style", i(l)), o.className = o.className.replace("waves-notransition", ""), l["-webkit-transform"] = d, l["-moz-transform"] = d, l["-ms-transform"] = d, l["-o-transform"] = d, l.transform = d, l.opacity = "1", l["-webkit-transition-duration"] = c.duration + "ms", l["-moz-transition-duration"] = c.duration + "ms", l["-o-transition-duration"] = c.duration + "ms", l["transition-duration"] = c.duration + "ms", l["-webkit-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["-moz-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["-o-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", o.setAttribute("style", i(l)) }, hide: function (t) { d.touchup(t); var e = this, n = (1.4 * e.clientWidth, null), a = e.getElementsByClassName("waves-ripple"); if (!(a.length > 0)) return !1; n = a[a.length - 1]; var o = n.getAttribute("data-x"), r = n.getAttribute("data-y"), s = n.getAttribute("data-scale"), u = Date.now() - Number(n.getAttribute("data-hold")), l = 350 - u; 0 > l && (l = 0), setTimeout(function () { var t = { top: r + "px", left: o + "px", opacity: "0", "-webkit-transition-duration": c.duration + "ms", "-moz-transition-duration": c.duration + "ms", "-o-transition-duration": c.duration + "ms", "transition-duration": c.duration + "ms", "-webkit-transform": s, "-moz-transform": s, "-ms-transform": s, "-o-transform": s, transform: s }; n.setAttribute("style", i(t)), setTimeout(function () { try { e.removeChild(n) } catch (t) { return !1 } }, c.duration) }, l) }, wrapInput: function (t) { for (var e = 0; e < t.length; e++) { var n = t[e]; if ("input" === n.tagName.toLowerCase()) { var a = n.parentNode; if ("i" === a.tagName.toLowerCase() && -1 !== a.className.indexOf("waves-effect")) continue; var i = document.createElement("i"); i.className = n.className + " waves-input-wrapper"; var o = n.getAttribute("style"); o || (o = ""), i.setAttribute("style", o), n.className = "waves-button-input", n.removeAttribute("style"), a.replaceChild(i, n), i.appendChild(n) } } } }, d = { touches: 0, allowEvent: function (t) { var e = !0; return "touchstart" === t.type ? d.touches += 1 : "touchend" === t.type || "touchcancel" === t.type ? setTimeout(function () { d.touches > 0 && (d.touches -= 1) }, 500) : "mousedown" === t.type && d.touches > 0 && (e = !1), e }, touchup: function (t) { d.allowEvent(t) } }; s.displayEffect = function (e) { e = e || {}, "duration" in e && (c.duration = e.duration), c.wrapInput(u(".waves-effect")), "ontouchstart" in t && document.body.addEventListener("touchstart", r, !1), document.body.addEventListener("mousedown", r, !1) }, s.attach = function (e) { "input" === e.tagName.toLowerCase() && (c.wrapInput([e]), e = e.parentElement), "ontouchstart" in t && e.addEventListener("touchstart", r, !1), e.addEventListener("mousedown", r, !1) }, t.Waves = s, document.addEventListener("DOMContentLoaded", function () { s.displayEffect() }, !1) }(window);
});