const settings = JSON.parse(localStorage.getItem("settings"));

const tManager = new TemplateManager();

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

$(document).ready(function () {
	//Menu render promise
	var loadMenusPromise = new Promise((resolve, reject) => {
		const menus = load("/PageRender/GetMenus");
		resolve(menus);
	});

	loadMenusPromise.then(menus => {
		const renderMenuContainer = $("#sidebarnav");
		if (menus.is_success) {
			const content = tManager.render("template_renderMenu.html", menus.result, {
				host: location.origin
			});
			renderMenuContainer.html(content);
			window.forceTranslate();
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
});