const localizer = new Localizer();

$("#headerContainer").replaceWith($(".headContainer"));

const menuBlock = $("#navbarNavAltMarkup div:first-child");
loadAsync("/PageRender/GetMenus?menuId=b02f6702-1bfe-4fdb-8f7a-a86447620b7e").then(menus => {
	if (menus.is_success) {
		//<span class="sr-only">(current)</span>
		$.each(menus.result, (i, item) => {
			const block = `<a href="${item.href}" translate="${item.translate}" class="nav-item nav-link py-1 px-3">${item.name}</a>`;
			menuBlock.append(block);
		});
		window.forceTranslate("#navbarNavAltMarkup");
	}
});

//user Section
const userSection = $("#userSection");
new Notificator().getCurrentUser().then(user => {
	if (user.is_success) {
		userSection.replaceWith($(`<div class="navbar-nav user-nav" style="margin-right: -6em; padding-right: 1em;">
        <a class="nav-item nav-link py-1 px-3" href="/home">${window.translate("iso_hello").toUpperFirstLetter()}, ${user.result.userName}</a>
        <a href="#" class="logoff btn btn-outline-primary py-2 ml-2 sa-logout">${window.translate("logout")}</a>
    </div>`));

		//Log Out
		new ST().registerLocalLogout(".sa-logout");
	}
});

$(document).ready(function () {
	const languagesBlock = $("#languages");
	window.loadAsync("/Localization/GetLanguagesAsJson").then(langs => {
		if (!langs) return;
		const currentLanguage = window.getCookie("language");
		let currentIdentifier = localizer.adaptIdentifier(langs.find(x => x.name === currentLanguage).identifier);
		if (currentIdentifier === 'gb') {
			currentIdentifier = 'en';
		}
		const b = $(`<li class="navbar-nav nav-link px-3 dropdown">
			 <a href="javascript:void(0)" class=" nav-link px-3 dropdown-toggle text-uppercase" data-toggle="dropdown">
				${currentIdentifier}
            <span class="caret"></span>
        </a>
		<ul class="dropdown-menu lang-selector">
		</ul>
		</li>`);
		$.each(langs, function (index, lang) {
			const language = `<a href="/Localization/ChangeLanguage?identifier=${lang.identifier}" class="dropdown-item language-event">
							${lang.name}
						</a>`;
			b.find(".lang-selector").append(language);
		});
		languagesBlock.append(b);
		$(".language-event").on("click", function () {
			localStorage.removeItem("hasLoadedTranslations");
		});
	});
});