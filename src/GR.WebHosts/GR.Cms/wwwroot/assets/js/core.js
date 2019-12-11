// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('Events requires jQuery');
}

const notificator = new Notificator();
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
                success: function (rData) {
                    resolve(rData);
                },
                error: function (err) {
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
 * Notify success message
 * @param {any} title
 * @param {any} message
 */
ToastNotifier.prototype.notifySuccess = function (title, message) {
    this.notify({ icon: "success", heading: title, text: message });
};

/**
 * Notify error message
 * @param {any} title
 * @param {any} message
 */
ToastNotifier.prototype.notifyError = function (title, message) {
    this.notify({ heading: title, text: message });
};

/**
 * Display errors 
 * @param {any} arr
 */
ToastNotifier.prototype.notifyErrorList = function (errors) {
    const scope = this;
    if (!errors) return;
    switch (typeof errors) {
        case "string":
            {
                scope.notify({ heading: "Error", text: errors });
            } break;
        case "object": {
            for (let i = 0; i < errors.length; i++) {
                scope.notify({ heading: "Error", text: errors[i].message });
            }
        } break;
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

window.htmlTemplates = [];

/**
 * Get template from server
 * @param {any} identifierName
 */
TemplateManager.prototype.getTemplate = function (identifierName) {
    //in memory version
    const inMemory = window.htmlTemplates.find(x => x.id === identifierName);
    if (inMemory) return inMemory.value;

    //cache version
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
            window.htmlTemplates.push({
                id: identifierName,
                value: temp
            });
            return temp;
        } else {
            console.log(serverTemplate);
        }
    }
    return "";
}

/**
 * Get template from server
 * @param {any} identifierName
 */
TemplateManager.prototype.getTemplateAsync = function (identifierName) {
    return new Promise((resolve, reject) => {
        //in memory version
        const inMemory = window.htmlTemplates.find(x => x.id === identifierName);
        if (inMemory) resolve(inMemory.value);
        else loadAsync("/Templates/GetTemplateByIdentifier",
            {
                identifier: identifierName
            }).then(serverTemplate => {
                if (serverTemplate) {
                    if (serverTemplate.is_success) {
                        const temp = serverTemplate.result;
                        localStorage.setItem(identifierName, temp);
                        window.htmlTemplates.push({
                            id: identifierName,
                            value: temp
                        });
                        resolve(temp);
                    } else {
                        console.log(serverTemplate);
                    }
                } else resolve("");

            });
    });
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
 * Register template
 * @param {any} identifier
 */
TemplateManager.prototype.registerTemplateAsync = function (identifier) {
    return this.getTemplateAsync(identifier).then(x => {
        $.templates(identifier, x);
    }).catch(e => console.warn(e));
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

TemplateManager.prototype.renderAsync = function (identifier, data, helpers) {
    $.views.settings.allowCode(true);
    return this.registerTemplateAsync(identifier).then(() => {
        return $.render[identifier](data, helpers);
    });
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
            if (index % 100 == 0) {
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
            const message = `Key: ${key} is not translated!`;
            console.warn(message);
            localStorage.removeItem("hasLoadedTranslations");
        }
        return window.localTranslations[key];
    }
    const trans = window.translations();
    return trans[key];
};

//Translate page content
window.forceTranslate = function (selector = null) {
    return new Promise((resolve, reject) => {
        try {
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
                            const message = `Key: ${key} is not translated!`;
                            console.warn(message);
                            localStorage.removeItem("hasLoadedTranslations");
                        }
                    }
                });
        } catch (e) { }
        resolve();
    });
};


function Localizer() {

}

/**
 * adapt identifiers
 * @param {any} idt
 */
Localizer.prototype.adaptIdentifier = function (idt) {
    switch (idt) {
        case "en":
            {
                idt = "gb";
            }
            break;
        case "ja":
            {
                idt = "jp";
            }
            break;
        case "zh":
            {
                idt = "cn";
            }
            break;
        case "uk":
            {
                idt = "ua";
            }
            break;
        case "el":
            {
                idt = "gr";
            }
            break;
    }
    return idt;
};


/**
 * Translate key
 * @param {any} key
 */
Localizer.prototype.translate = function (key) {
    return window.translate(key);
};

//------------------------------------------------------------------------------------//
//								External Connections
//------------------------------------------------------------------------------------//

$(document).ready(function () {
    if (location.href.indexOf("Account/Login") !== -1) return;
    if (typeof signalR === 'undefined') return;
    notificator.getCurrentUser().then(user => {
        initExternalConnections(user);
    }).catch(err => {
        console.warn(err);
    });
});

function initExternalConnections(user) {
    const connPromise = new Promise((resolve, reject) => {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/rtn", signalR.HttpTransportType.LongPolling)
            .build();
        //Time for one connection
        //connection.serverTimeoutInMilliseconds = 1000 * 60 * 10;
        resolve(connection);
    });


    connPromise.then(connection => {

        //On receive notification
        connection.on("SendClientNotification",
            (notification) => {
                const date = new Date(notification.created);
                notification.created = moment(date).format("DD.MM.YYYY");
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
    }).catch(function (err) {
        //On error
    });

    connPromise.then(() => {
        var loadUserNotifications = new Promise((resolve, reject) => {
            loadNotifications();
        });

        Promise.all([loadUserNotifications]).then(function (values) {
            $("#clearNotificationsEvent").on("click",
                function () {
                    $("#notificationList").html(null);
                    $("#notificationAlarm").hide();
                    const notificator = new Notificator();
                    notificator.clearNotificationsOnCurrentUser();
                });
        });
    });
};

/**
 * Show notifications on page load
 */
function loadNotifications() {
    notificator.getAllNotifications().then(notificationList => {
        if (!notificationList) return;
        if (notificationList.is_success) {
            $.each(notificationList.result, (i, notification) => {
                notificator.addNewNotificationToContainer(notification);
            });
        }
    });
}

//------------------------------------------------------------------------------------//
//								Notificator
//------------------------------------------------------------------------------------//
// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('Events requires jQuery');
}


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
    $("#notificationList").prepend(template);
    this.registerOpenNotificationEvent();
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

//Helpers
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
    $.each(data,
        function (key, value) {
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
            } else {
                switch ($ctrl.attr("type")) {
                    case "text":
                    case "hidden":
                    case "color":
                    case "number":
                        $ctrl.val(value);
                        break;
                    case "radio":
                    case "checkbox":
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
 * Check if string is in an uuid format
 * @param {any} str
 */
ST.prototype.isGuid = function (str) {
    const pattern =
        new RegExp(
            "^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$",
            "i");
    return pattern.test(str);
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

/*
 * Remove selected text
*/
ST.prototype.clearSelectedText = function () {
    if (window.getSelection)
        window.getSelection().removeAllRanges();
    else if (document.selection)
        document.selection.empty();
};

/**
 * Rgb to hex
 * @param {any} color
 */
ST.prototype.rgbToHex = function (color) {
    if (!color) return "";
    var bg = color.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);

    function hex(x) {
        return ("0" + parseInt(x).toString(16)).slice(-2);
    }

    return "#" + hex(bg[1]) + hex(bg[2]) + hex(bg[3]);
};

/**
 * Get parameter from url
 * @param {any} key
 * @param {any} url
 */
ST.prototype.getParamFormUrl = function (key, url = window.location.href) {
    const newUrl = new URL(url);
    const searchParams = new URLSearchParams(newUrl.search);
    return searchParams.get(key);
};

/**
 * Local logout
 * @param {any} selector
 */
ST.prototype.registerLocalLogout = function (selector) {
    $(selector).click(function () {
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
                    url: "/Account/LocalLogout",
                    type: "post",
                    dataType: "json",
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (data) {
                        if (data.is_success) {
                            swal("Success!", data.message, "success");
                            localStorage.clear();
                            //window.deleteCookie("language");
                            window.location.href = "/Account/Login";
                        } else {
                            swal("Fail!", data.error_keys[0].message, "error");
                            localStorage.removeItem("current_user");
                        }
                    },
                    error: function () {
                        swal("Fail!", "Server no response!", "error");
                    }
                });
            }
        });
    });
};

String.prototype.toLowerFirstLetter = function () {
    const first = this[0].toLowerCase();
    const res = `${first}${this.slice(1, this.length)}`;
    return res;
}

String.prototype.toUpperFirstLetter = function () {
    const first = this[0].toUpperCase();
    const res = `${first}${this.slice(1, this.length)}`;
    return res;
}


class Validator {
	/**
	 * Validate email
	 * @param {any} email
	 */
    isValidEmail(email) {
        return /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()\.,;\s@\"]+\.{0,1})+([^<>()\.,;:\s@\"]{2,}|[\d\.]+))$/.test(email);
    }
}

//Jquery Extensions
jQuery.fn.extend({
    hasAttr: function (attrName) {
        return this.get(0).hasAttribute(attrName);
    }
});


//Array Extensions
Array.prototype.update = function (predicate, item) {
    try {
        const old = this.find(predicate);
        const index = this.indexOf(old);
        this[index] = item;
    } catch (e) { console.warn(e); }
};
