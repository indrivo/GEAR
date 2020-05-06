/* Notification plugin
 * A plugin for get and send notifications
 *
 * v1.0.0
 *
 * License: MIT Indrivo
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('Notification plugin require JQuery');
}

//------------------------------------------------------------------------------------//
//								Notificator
//------------------------------------------------------------------------------------//

function Notificator() {
    this.user = null;
}

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
    $("#notificationList .notifications").prepend(template);
    this.registerOpenNotificationEvent();
};

/**
 * Append to container
 * @param {any} notification
 */
Notificator.prototype.appendNotificationToContainer = function (notification) {
    const template = this.createNotificationBodyContainer(notification);
    $("#notificationList .notifications").append(template);
};

/**
 * Create and get notification body
 * @param {any} n
 */
Notificator.prototype.createNotificationBodyContainer = function (notification) {
    const content = `
		<div class="mail-contnet">
			<h5>${notification.subject}</h5> <span class="mail-desc">${notification.content}</span>
				<span class="time">
					${notification.created} 
				</span>
		</div>`;
    const block = `
		<a class="notification-item" data-notification-id="${notification.id}" href="javascript:void(0)">
			<div class="btn btn-danger btn-circle"><i class="fa fa-link"></i></div>
			${content}
		</a>`;
    return block;
};


Notificator.prototype.registerOpenNotificationEvent = function () {

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
 * Get all notifications
 *@returns {any} Folders data
 */
Notificator.prototype.getAllNotifications = function () {
    return this.getCurrentUser().then(req => {
        let userId = undefined;
        if (req.is_success) {
            userId = req.result.id;
        } else {
            return req.error_keys;
        }
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `${this.origin()}/api/Notifications/GetNotificationsByUserId`,
                method: "get",
                data: {
                    userId: userId
                },
                success: function (data) {
                    resolve(data);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    }).catch(err => {
        console.warn(err);
    });
};

/**
 * Get all notifications
 * @param {any} page
 * @param {any} perPage
 * @param {any} onlyUnread
 */
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
 * Get notification by id
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
    return new Promise((resolve, reject) => {
        const userData = localStorage.getItem("current_user");
        if (userData) {
            const parsedUserData = JSON.parse(userData);
            const created = new Date(parsedUserData.created).getTime();
            const now = new Date().getTime();
            const diff = now - created;
            if (diff > 0 && diff < 50000) {
                return resolve(parsedUserData);
            }
        }
        $.ajax({
            url: `${this.origin()}/Account/GetCurrentUser`,
            method: "get",

            success: function (data) {
                data.created = new Date();
                if (data.is_success)
                    localStorage.setItem("current_user", JSON.stringify(data));
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};

/**
 * On int
 */
Notificator.prototype.init = function () {
    const self = this;
    self.getAllNotifications(1, 5, true).then(data => {
        if (!data) return;
        if (data.is_success) {
            $.each(data.result.notifications, (i, notification) => {
                self.addNewNotificationToContainer(notification);
            });
            const loaderClassString = 'notification-loader';
            const loaderClass = `.${loaderClassString}`;
            $('#notificationList').find(loaderClass).fadeOut();
            setTimeout(function () { $('#notificationList').find(loaderClass).remove(); }, 400);

            $("#clearNotificationsEvent").on("click",
                function () {
                    $("#notificationList .notifications").html(null);
                    $("#notificationAlarm").hide();
                    self.clearNotificationsOnCurrentUser();
                });
        }
    });
};

/**
 * On receive notification
 * @param {any} notification
 */
Notificator.prototype.onReceiveNotification = function (notification) {
    const date = new Date(notification.created);
    notification.created = moment(date).format("DD.MM.YYYY hh:mm:ss A");
    this.addNewNotificationToContainer(notification);
};

/**
 * Ajax request
 * @param {any} requestUrl
 * @param {any} requestType
 * @param {any} requestData
 */
Notificator.prototype.ajaxRequest = (requestUrl, requestType, requestData) => {
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


//------------------------------------------------------------------------------------//
//								External Connections
//------------------------------------------------------------------------------------//

$(document).ready(function () {
    const notificator = new Notificator();

    //Disable on login page
    if (location.href.indexOf("Account/Login") !== -1) return;
    if (typeof signalR === 'undefined') return;
    initExternalConnections();

    function initExternalConnections() {
        const connPromise = new Promise((resolve, reject) => {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl("/rtn", { transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling })
                .build();
            //Time for one connection
            //connection.serverTimeoutInMilliseconds = 1000 * 60 * 10;
            resolve(connection);
        });

        connPromise.then(connection => {

            //On receive notification
            connection.on("SendClientNotification",
                (notification) => {
                    notificator.onReceiveNotification(notification);
                });

            //Start connection
            connection.start().then(() => {
                $(window).bind("beforeunload",
                    function () {
                        connection.stop();
                    });
            });
        }).catch(function (err) {
            //On error
        });

        //init module
        connPromise.then(() => {
            notificator.init();
        });
    };
});