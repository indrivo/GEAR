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
    //Log Out
    new ST().registerLocalLogout(".sa-logout");

    //Menu render promise
    var loadMenusPromise = new Promise((resolve, reject) => {
        const menus = load("/Menu/GetMenus");
        resolve(menus);
    });

    loadMenusPromise.then(menus => {
        const renderMenuContainer = $("#sidebarnav");
        if (menus.is_success) {
            const content = tManager.render("template_renderMenu.html", menus.result, {
                host: location.origin
            });
            renderMenuContainer.html(content);
            window.forceTranslate("#sidebarnav");
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

    Promise.all([loadMenusPromise, localizationPromise]).then(function (values) {
        window.forceTranslate();
    });
});