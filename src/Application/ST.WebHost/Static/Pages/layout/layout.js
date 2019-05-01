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
	$("#appName").text(settings.app.name);
	$("#PageTitle").text(settings.navigation.current);



	//Set user details
	$("#UserName").text(settings.user.userName);
	$("#UserEmail").text(settings.user.email);
	$(".userImage").attr("src", `/Users/GetImage?id=${settings.user.id}`);



	//Set localization config
	let translateIcon = settings.localization.current.identifier;
	if (translateIcon == "en") {
		translateIcon = "gb";
	}
	$("#currentlanguage").addClass(`flag-icon flag-icon-${translateIcon}`);
	const languageBlock = $("#languageRegion");
	function getIdentifier(idt) {
		return (idt == "en") ? "gb" : idt;
	}

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
			title: "Are you sure?",
			text: "You are going to de-authenticate yourself!",
			type: "warning",
			showCancelButton: true,
			confirmButtonColor: "#DD6B55",
			confirmButtonText: "Yes, log out!"
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