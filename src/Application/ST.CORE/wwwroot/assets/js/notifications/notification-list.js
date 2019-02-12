let notificator = new Notificator();
let st = new ST();
let menu = $("#notification-menu");
let list = $("#notifications");
let loader_menu = $("#loader_menu");


$.views.helpers({
	getFields: function (object) {
		let key, value;
		const fieldsArray = [];
		for (key in object) {
			if (object.hasOwnProperty(key)) {
				value = object[key];

				fieldsArray.push({
					key: key,
					value: value
				});
			}
		}
		return fieldsArray;
	},
	notTrash: function () {
		const active = menu.find("li.active").contents();
		if (active[0].nodeValue.toString().indexOf("Trash") != -1) {
			return false;
		}
		else {
			return true;
		}
	},
	isTrash: function () {
		const active = menu.find("li.active").contents();
		if (active[0].nodeValue.toString().indexOf("Trash") != -1) {
			return true;
		}
		else {
			return false;
		}
	},
	isread: function (data) {
		if (data.hasOwnProperty("IsRead")) {
			return data.IsRead;
		}
	}
});

$(document).ready(function () {
	$(function () {
		const response = notificator.getFolders();
		if (response.is_success) {
			const data = response.result.values;
			Promise.all([st.getTemplate("notifications/folders.html")])
				.then(function (values) {
					$.templates("menu", values[0]);
					for (let i = 0; i < data.length; i++) {
						data[i] = menuItemStyle(data[i]);
					}
					const content = $.render["menu"](data);
					menu.html(content);
					loader_menu.hide();
					const url = location.href;
					const parsedUrl = new URL(url);
					const folder = parsedUrl.searchParams.get("folderId");
					if (folder != undefined) {
						changeFolder(folder);
					}
					else {
						changeFolder(data[0].Id);
					}

					menu.find("li a").on("click", function () {
						changeFolder($(this).attr("folderId"));
					});
				})
				.catch(function (err) {
					console.log(err);
				});
		}
	});

	/**
	 * * Set icon and color
	 * @param {any} data
	 */
	function menuItemStyle(data) {
		switch (data.Name) {
			case "Sent":
				{
					data.Icon = "send";
					data.ColorType = "warning";
				}
				break;
			case "Inbox":
				{
					data.Icon = "gmail";
					data.ColorType = "success";
				}
				break;
			case "Trash":
				{
					data.Icon = "delete";
					data.ColorType = "danger";
				}
				break;
		}
		return data;
	}

	function changeFolder(folderId) {
		menu.find("li").removeClass("active");
		menu.find(`li a[folderId='${folderId}']`).parent().addClass("active");
		const list = notificator.getListByFolderId(folderId, 1);
		if (list != null) {
			if (list.is_success) {
				populate(list);
				$("#allNot").html(list.result.count);
				$("#not_pagination").pagination({
					items: list.result.count,
					itemsOnPage: 10,
					cssStyle: "light-theme",
					onInit: function () { },
					onPageClick: function (pageNumber, event) {
						const req = notificator.getListByFolderId(folderId, pageNumber);
						if (req != null) {
							if (req.is_success) {
								populate(req);
							}
						}
					}
				});
				$("#not_pagination").pagination("selectPage", 1);
			}
		}
	}


	function populate(data) {
		Promise.all([st.getTemplate("notifications/notifications.html")])
			.then(function (values) {
				$.templates("list", values[0]);
				const content = $.render["list"](data);
				list.html(content);
				$(function () {
					$(".data").click(function () {
						const message = $(this).parent().parent().find("td.Message").html();
						const subject = $(this).parent().parent().find("td.Subject").html();
						$("#message").html(message);
						$("#subject").html(subject);
						$(".notification").modal('show');
						const notId = $(this).parent()
							.parent()
							.find("td.id")
							.attr("notId");
						notificator.markAsRead(notId);
					});
					$(".notification").modal({
						closable: true
					});
				});
				$(function () {
					$(".trash").on("click", function () {
						const trashId = menu.find("a:contains('Trash')").attr("folderId");
						const id = $(this).parent()
							.parent().find("td.id").attr("notId");
						notificator.moveTofolder(id, trashId);
					});
					$(".restore").on("click", function () {
						const id = $(this).parent()
							.parent().find("td.id").attr("notId");
						notificator.restore(id);
					});
					//On message click redirect to message
					$(".data-message").on("click",
						function () {
							const id = $(this).attr("data-id");
							location.href = `/email/getmessagebyid?id=${id}`;
						});

					$(".rightclick").on("contextmenu  ", function (event) {
						event.preventDefault();
						const row = $(this);
						let idl = undefined;
						const childers = row[0].children;
						for (let i = 0; i < childers.length; i++) {
							if (childers[i].className == 'id') {
								idl = childers[i].attributes[1].nodeValue;
							}
						}
						const menu = $("#menurightclick");
						menu.attr("notification", idl);
						const x = event.pageX;
						const y = event.pageY - 120;
						menu.css({ top: y + "px", left: x + "px" });
						menu.show(100);

						$("#menu_close").on("click", function () {
							menu.hide();
						});
						$("#right_menu").find("a").on("click", function (event) {
							const folderid = $(this).attr("folderid");
							const notId = menu.attr("notification");
							notificator.moveTofolder(notId, folderid);
							$(this).unbind("click");
						});

					});
				});
			})
			.catch(function (err) {
				console.log(err);
			});
	}
});