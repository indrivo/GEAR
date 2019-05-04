const menus = load("/PageRender/GetMenus");

const renderMenuContainer = $("#sidebarnav");

const settings = JSON.parse(localStorage.getItem("settings"));

const tManager = new TemplateManager();
//Render menu
if (menus.is_success) {
	const content = tManager.render("template_renderMenu.html", menus.result, {
		host: location.origin
	});
	renderMenuContainer.html(content);
	window.forceTranslate();
}

$(document).ready(function () {
	const notificator = new Notificator();
	const st = new ST();

	var m = $(".notification-items");
	const response = notificator.getFolders();
	if (response != null) {
		if (response.is_success) {
			var folders = response.result.values;
			const f = folders.find((e) => e.Name === "Inbox");
			const uri = `/Email?folderId=${f.Id}`;
			$("#SeeAllEmails").attr("href", uri);

			const content = tManager.render("template_folders_layout.html", folders);
			m.html(content);
			$("#right_menu").html(content);
			m.find("a").on("click", function () {
				const folderId = $(this).attr("folderid");
				if (folderId != undefined) {
					window.location.href = `/Email?folderId=${folderId}`;
				}
			});
		}
	}

	//Set current view and controller
	$("#currentController").html(settings.navigation.controller);
	$("#currentView").html(settings.navigation.view);



	//Set app name
	$("#appName").html(settings.app.name);
	$("#PageTitle").html(settings.navigation.current);



	//Set user details
	$("#UserName").text(settings.user.userName);
	$("#UserEmail").text(settings.user.email);
	$(".userImage").attr("src", `/Users/GetImage?id=${settings.user.id}`);



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
	//Set localization config
	let translateIcon = getIdentifier(settings.localization.current.identifier);
	$("#currentlanguage").addClass(`flag-icon flag-icon-${translateIcon}`);
	const languageBlock = $("#languageRegion");

	$.each(settings.localization.languages, function (index, lang) {
		const language = `<a href="/Localization/ChangeLanguage?identifier=${lang.identifier}" class="dropdown-item language-event">
							<i class="flag-icon flag-icon-${getIdentifier(lang.identifier)}"></i> ${lang.name}
						</a>`;
		languageBlock.append(language);
	});

	$(".language-event").on("click", function () {
		localStorage.removeItem("translations");
	});


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
});