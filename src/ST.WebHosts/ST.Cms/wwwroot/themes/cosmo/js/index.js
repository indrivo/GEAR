$("#headerContainer").replaceWith($(".headContainer"));
//block[0].outerHTML = $(".headContainer").map(x => x.outerHTML).join();


const menuBlock = $("#navbarNavAltMarkup div:first-child");
loadAsync("/PageRender/GetMenus?menuId=b02f6702-1bfe-4fdb-8f7a-a86447620b7e").then(menus => {
	if (menus.is_success) {
		//<span class="sr-only">(current)</span>
		$.each(menus.result, (i, item) => {
			const block = `<a href="${item.href}" translate="${item.translate}" class="nav-item nav-link px-3">${item.name}</a>`;
			menuBlock.append(block);
		});
		window.forceTranslate("#navbarNavAltMarkup");
	}
});

//user Section
const userSection = $("#userSection");
new Notificator().getCurrentUser().then(user => {
	if (user.is_success) {
		userSection.replaceWith($(`<div class="navbar-nav ml-auto" style="margin-right: -6em; padding-right: 1em;">
        <a class="nav-item nav-link px-3" href="/home">${window.translate("iso_hello").toUpperFirstLetter()}, ${user.result.userName}</a>
        <a href="#" class="logoff btn btn-secondary px-3  py-2 circle sa-logout">${window.translate("logout")}</a>
    </div>`));

		$(".sa-logout").click(function () {
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
							if (data.success) {

								swal("Success!", data.message, "success");
								window.location.href = "/Account/Login";
							} else {
								swal("Fail!", data.message, "error");
							}
						},
						error: function () {
							swal("Fail!", "Server no response!", "error");
						}
					});
				}
			});
		});
	}
});

$(document).ready(function () {
	const languagesBlock = $("#languages");
	window.loadAsync("/Localization/GetLanguagesAsJson").then(langs => {
		if (!langs) return;
		const b = $(`<li class="navbar-nav nav-link px-3 dropdown">
			 <a href="javascript:void(0)" class=" nav-link px-3 dropdown-toggle" data-toggle="dropdown" style="color:white;">
            ${window.getCookie("language")}
            <span class="caret"></span>
        </a>
		<ul class="dropdown-menu lang-selector">
		</ul>
		</li>`);
		$.each(langs, function (index, lang) {
			const language = `<a href="/Localization/ChangeLanguage?identifier=${lang.identifier}" class="dropdown-item language-event">
							<i class="flag-icon flag-icon-${lang.identifier}"></i> ${lang.name}
						</a>`;
			b.find(".lang-selector").append(language);

		});
		languagesBlock.append(b);
		$(".language-event").on("click", function () {
			localStorage.removeItem("hasLoadedTranslations");
		});
	});
});