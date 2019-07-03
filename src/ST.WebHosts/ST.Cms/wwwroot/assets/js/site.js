// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Events requires jQuery');
}

//TODO: need to delete this gavno code and write new code
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
//									Ajax requests
//------------------------------------------------------------------------------------//
/**
 * Load data with ajax
 * @param {any} uri
 * @param {any} data
 * @param {any} type
 */
window.load = function (uri, data = null, type = "get") {
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
};

/**
 * Load data with ajax async
 * @param {any} uri
 * @param {any} data
 * @param {any} type
 */
window.loadAsync = function (uri, data = null, type = "get") {
	return new Promise((resolve, reject) => {
		try {
			const url = new URL(location.href);
			uri = `${url.origin}${uri}`;
			$.ajax({
				url: uri,
				type: type,
				data: data,
				success: function(rData) {
					resolve(rData);
				},
				error: function(err) {
					reject(err);
				}
			});
		} catch (exp) {
			reject(exp);
		}
	});
};

//------------------------------------------------------------------------------------//
//								End ajax requests
//------------------------------------------------------------------------------------//

//------------------------------------------------------------------------------------//
//								Toast notifier
//------------------------------------------------------------------------------------//
function ToastNotifier() {

}

ToastNotifier.prototype.constructor = ToastNotifier;

/**
 * Display notification
 * @param {any} conf
 */
ToastNotifier.prototype.notify = (conf) => {
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

/**
 * Display errors 
 * @param {any} arr
 */
ToastNotifier.prototype.notifyErrorList = function(arr) {
	for (let i = 0; i < arr.length; i++) {
		this.notify({ heading: "Error", text: arr[i].message });
	}
};

//------------------------------------------------------------------------------------//
//								End toast notifier
//------------------------------------------------------------------------------------//


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
	const cached = localStorage.getItem("hasLoadedTranslations");
	let trans = {};
	if (!cached) {
		trans = load("/Localization/GetTranslationsForCurrentLanguage");
		let index = 0;
		let step = 1;
		let round = {};
		for (let key in trans) {
			round[key] = trans[key];
			index++;
			if (index % 200 == 0) {
				localStorage.setItem(`translations_${step}`, JSON.stringify(round));
				round = {};
				step++;
			}
		}
		localStorage.setItem(`translations_${step}`, JSON.stringify(round));
		localStorage.setItem("transCollectionCount", step);
		localStorage.setItem("hasLoadedTranslations", "yes")
	} else {
		const count = localStorage.getItem("transCollectionCount");
		for (let i = 1; i <= count; i++) {
			const cacheSerialCollection = localStorage.getItem(`translations_${i}`);
			trans = Object.assign(trans, JSON.parse(cacheSerialCollection));
		}
	}
	window.localTranslations = trans;
	return trans;
}


window.translate = function (key) {
	if (window.localTranslations) {
		if (!window.localTranslations.hasOwnProperty(key)) {
			$.toast({
				heading: `Key: ${key} is not translated!`,
				text: "",
				position: 'top-right',
				loaderBg: '#ff6849',
				icon: 'error',
				hideAfter: 3500,
				stack: 6
			});
			localStorage.removeItem("hasLoadedTranslations");
		}
		return window.localTranslations[key];
	}
	const trans = window.translations();
	return trans[key];
}


//$(document).ajaxComplete(function (event, xhr, settings) {
//	window.forceTranslate();
//});

//Translate page content
window.forceTranslate = function (selector = null) {
	return new Promise((resolve, reject) => {
		var ctx = (!selector)
			? document.getElementsByTagName('*')
			: document.querySelector(selector).getElementsByTagName('*');
		const translations = Array.prototype.filter.call(ctx,
			function (el) {
				return el.getAttribute('translate') != null && !el.hasAttribute("translated");
			}
		);
		const trans = window.translations();
		$.each(translations,
			function (index, item) {
				let key = $(item).attr("translate");
				if (key != "none" && key) {
					const translation = trans[key];
					if (translation) {
						$(item).text(translation);
						$(item).attr("translated", "");
					} else {
						$.toast({
							heading: `Key: ${key} is not translated!`,
							text: "",
							position: 'top-right',
							loaderBg: '#ff6849',
							icon: 'error',
							hideAfter: 3500,
							stack: 6
						});
						localStorage.removeItem("hasLoadedTranslations");
					}
				}
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

$(document).ready(function () {
	if (typeof signalR === 'undefined') return;
	const notificator = new Notificator();
	const user = notificator.getCurrentUser();
	const connPromise = new Promise((resolve, reject) => {
		var connection = new signalR.HubConnectionBuilder()
			.withUrl("/rtn", signalR.HttpTransportType.LongPolling)
			.build();
		//Time for one connection
		connection.serverTimeoutInMilliseconds = 1000 * 60 * 10;
		resolve(connection);
	});


	connPromise.then(connection => {
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
			notificator.addNewNotificationToContainer(notification);
		});

		if (!user.is_success) {
			throw "User not authorized!";
		}
		//Start connection
		connection.start().then(() => {
			if (user && user.is_success) {
				connection.invoke("OnLoad", user.result.id)
					.catch(err => console.error(err.toString()));
				$(window).bind("beforeunload",
					function () {
						connection.stop();
					});
			}
		});
	});

	connPromise.catch(function () {
		//On error
	});

	connPromise.then(() => {
		var loadUserNotifications = new Promise((resolve, reject) => {
			loadNotifications();
		});

		var loadUserEmails = new Promise((resolve, reject) => {
			loadEmails();
		});

		Promise.all([loadUserNotifications, loadUserEmails]).then(function (values) {
			$("#clearNotificationsEvent").on("click",
				function () {
					$("#notificationList").html(null);
					$("#notificationAlarm").hide();
					const notificator = new Notificator();
					notificator.clearNotificationsOnCurrentUser();
				});
		});
	});
});

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
 * Show notifications on page load
 */
function loadNotifications() {
	const notificator = new Notificator();
	const notificationList = notificator.getAllNotifications();
	if (notificationList.is_success) {
		for (let notification in notificationList.result) {
			notificator.addNewNotificationToContainer(notificationList.result[notification]);
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

/**
 * Add new notification to container
 * @param {any} notification
 */
Notificator.prototype.addNewNotificationToContainer = function (notification) {
	$("#notificationAlarm").show();
	const template = this.createNotificationBodyContainer(notification);
	$("#notificationList").prepend(template);
	this.registerOpenNotificationEvent();
};

/**
 * Create and get notification body
 * @param {any} n
 */
Notificator.prototype.createNotificationBodyContainer = function (n) {
	const content = `
		<div class="mail-contnet">
			<h5>${n.subject}</h5> <span class="mail-desc">${n.content}</span>
				<span class="time">
					${n.created}
				</span>
		</div>`;
	const block = `
		<a class="notification-item" data-notification-id="${n.id}" href="#">
			<div class="btn btn-danger btn-circle"><i class="fa fa-link"></i></div>
			${content}
		</a>`;
	return block;
};


Notificator.prototype.registerOpenNotificationEvent = function () {
	$(".notification-item").off("click", this.openNotificationHandler);
	$(".notification-item").on("click", this.openNotificationHandler);
};


Notificator.prototype.openNotificationHandler = function () {
	const notId = $(this).data("notification-id");
	if (!notId) {
		return;
	}
	var notContext = new Notificator();
	const res = notContext.getNotificationById(notId);
	if (res.is_success) {
		Swal.fire(
			res.result.Subject,
			res.result.Content,
			'warning'
		);
	}
};


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
	const settings = localStorage.getItem("email_settings");
	if (settings) {
		const d = JSON.parse(settings);
		if (d.hasOwnProperty("email")) {
			return d.email.folders;
		} else {
			d.email = {
				folders: loadEmailFolders()
			};
			localStorage.setItem("email_settings", JSON.stringify(d));
		}
	} else {
		const f = loadEmailFolders();
		const s = {
			email: {
				folders: f
			}
		};
		localStorage.setItem("email_settings", JSON.stringify(s));
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
 * Get notifcation by id
 * @param {any} notificationId
 */
Notificator.prototype.getNotificationById = function (notificationId) {
	var response = null;
	$.ajax({
		url: "/api/Notifications/GetNotificationById",
		async: false,
		data: {
			notificationId: notificationId
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
			//var origin = location.origin;
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



if (typeof String != 'undefined') {
	String.prototype.toLowerFirstLetter = function () {
		const first = this[0].toLowerCase();
		const res = `${first}${this.slice(1, this.length)}`;
		return res;
	}
}
