// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Events requires jQuery');
}

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
};


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
	var response = null;
	$.ajax({
		url: `${this.origin()}/Account/GetCurrentUser`,
		method: "get",
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