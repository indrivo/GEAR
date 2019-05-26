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
	email();
	notifications();
	$.ajax({
		url: "/Account/GetCurrentUser",
		success: (data) => {
			if (data.is_success) {
				connection.invoke("OnLoad", data.result.id)
					.catch(err => console.error(err.toString()));
			}
		}
	});
	$("#clearNotificationsEvent").on("click",
		function () {
			$("#notificationList").html(null);
			$("#notificationAlarm").hide();
			const notificator = new Notificator();
			notificator.clearNotificationsOnCurrentUser();
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
function notifications() {
	const notificator = new Notificator();
	const notificationList = notificator.getAllNotifications();
	if (notificationList.is_success) {
		for (let notification in notificationList.result) {
			performNotification(notificationList.result[notification]);
		}
	}
}


function email() {
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