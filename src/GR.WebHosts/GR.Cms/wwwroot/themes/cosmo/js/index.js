const localizer = new Localizer();

$("#headerContainer").replaceWith($(".headContainer"));

const menuBlock = $("#navbarNavAltMarkup .main-nav");
loadAsync("/Menu/GetMenus?menuId=b02f6702-1bfe-4fdb-8f7a-a86447620b7e").then(menus => {
	if (menus.is_success) {
		//<span class="sr-only">(current)</span>
		$.each(menus.result, (i, item) => {
			const block = `<a href="${item.href}" translate="${item.translate}" class="nav-item nav-link py-1 px-3">${item.name}</a>`;
			menuBlock.append(block);
		});
		window.forceTranslate("#navbarNavAltMarkup");
	}
	$(".preloader").fadeOut();
});

//user Section
const userSection = $("#userSection");
new Notificator().getCurrentUser().then(user => {
	if (user.is_success) {
		userSection.replaceWith($(`<div class="navbar-nav user-nav align-items-center">
		<div class="nav-item user-logedin dropdown show">
            <a href="#" id="userLogedinDropdown" data-toggle="dropdown" class="nav-link weight-400 dropdown-toggle p-0 d-flex align-items-center" aria-expanded="true">
				<img src="/Users/GetImage?id=${user.result.id}" class="mr-2 rounded" width="28">${user.result.userName}
			</a>
            <div class="dropdown-menu dropdown-menu-right" aria-labelledby="userLogedinDropdown">
                <a class="dropdown-item" href="/Home">${window.translate("dashboard")}</a>
                <a class="dropdown-item" href="/Users/Profile">${window.translate("my_profile")}</a>
                <div class="dropdown-divider"></div>
                <a class="dropdown-item sa-logout" href="#">${window.translate("logout")}</a>
            </div>
        </div>
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

//Include chat
(function (w, d) {
	w.HelpCrunch = function () { w.HelpCrunch.q.push(arguments) }; w.HelpCrunch.q = [];
	function r() { var s = document.createElement('script'); s.async = 1; s.type = 'text/javascript'; s.src = 'https://widget.helpcrunch.com/'; (d.body || d.head).appendChild(s) };
	if (w.attachEvent) { w.attachEvent('onload', r) } else { w.addEventListener('load', r, false) }
})(window, document);
HelpCrunch('init', 'iso27001expert', {
	applicationId: 1,
	applicationSecret: 'GqwtGJkruYiB8bbRmISXaHSl1DuhRdmJpXrIiAr+8omlkxJa5ef7FzgkwQuJecAvfX/MzGLSPK6pAOsEzybCzQ=='
});

HelpCrunch('showChatWidget');

$(function () {
	let index = 0;
	const helpCrunchTimer = setInterval(function () {
		try {
			const helpCrunchContainer = $($(`iframe[name='helpcrunch-iframe']`).get(0).contentDocument);
			if (helpCrunchContainer) {
				helpCrunchContainer.on("click", function () {
					setTimeout(() => {
						helpCrunchContainer.find("#helpcrunch-container.helpcrunch-chat-fadein .helpcrunch-chat").css("background", "#0540b5");
					}, 200);
				});

				helpCrunchContainer.find(".helpcrunch-widget-type-icon-label").css("background", "#0540b5");
				helpCrunchContainer.find(".helpcrunch-widget-icon-block").css("background", "#0540b5");
				helpCrunchContainer.find(".helpcrunch-widget-type-icon-triangle").css("border", "#0540b5");
				helpCrunchContainer.find(".helpcrunch-widget-type-icon-text").html(window.translate('iso_chat_what_is_your_question'));
				$(`iframe[name='helpcrunch-iframe']`).css("display", "block");
				if (index == 3) clearInterval(helpCrunchTimer);
				index++;
				if ($(window).width() <= 768) {
					helpCrunchContainer.find('.helpcrunch-widget-type-icon-label').hide();
				}
				$(window).resize(function () {
					if ($(window).width() <= 768) {
						helpCrunchContainer.find('.helpcrunch-widget-type-icon-label').hide();
					}
					else {
						helpCrunchContainer.find('.helpcrunch-widget-type-icon-label').show();
					}
				});
			}
		}
		catch (e) { }
	}, 300);
});

$(function () {
	const btn = $(".scroll-to-top");

	$(window).scroll(function () {
		if ($(window).scrollTop() > 300) {
			btn.fadeIn();
		} else {
			btn.fadeOut();
		}
		if ($(window).scrollTop() > 80) {
			$('.page-header').addClass('no-padding');
			$('.sponsors-collapse').addClass('slideup');
			$('.navbar-collapse').collapse('hide');
		} else {
			$('.page-header').removeClass('no-padding');
			$('.sponsors-collapse').removeClass('slideup');
		}
	});

	btn.on('click', function (e) {
		e.preventDefault();
		$('html, body').animate({ scrollTop: 0 }, '300');
	});

	$(document).ready(function () {
		window.forceTranslate().then(() => {
			replaceIso();
			let btnWidth = 80;
			$.each($('.user-nav .btn'), function () {
				const thisWidth = $(this).width();
				if (thisWidth > btnWidth) {
					btnWidth = thisWidth + 24;
				}
			});
			$('.user-nav .btn').css('min-width', btnWidth);
		});
		window.forceTranslatePlaceHolders()
		replaceIso();
	});

	String.prototype.replaceAll = function (search, replacement) {
		var target = this;
		return target.replace(new RegExp(search, 'g'), replacement);
	};

	function replaceIso() {
		$.each($('.iso-text'), function () {
			let str = $(this);
			if (!$(this).find('.color-blue-fw-4400').length > 0) {
				str.html(
					str.text().replaceAll('ISO 27001', '<span><i><span class="color-blue fw-400">ISO</span> 27001</i></span>')
				);
			}
		});
	}

	window.forceTranslatePlaceHolders = function (selector = null) {
		return new Promise((resolve, reject) => {
			try {
				const ctx = (!selector)
					? document.getElementsByTagName('*')
					: document.querySelector(selector).getElementsByTagName('*');
				const translations = Array.prototype.filter.call(ctx,
					function (el) {
						return el.getAttribute('translate-placeholder') != null && !el.hasAttribute("translated-placeholder");
					}
				);
				const trans = window.translations();
				$.each(translations,
					function (index, item) {
						let key = $(item).attr("translate-placeholder");
						if (key != "none" && key) {
							const translation = trans[key];
							if (translation) {
								$(item).attr("placeholder", translation);
								$(item).attr("translated-placeholder", "");
							} else {
								const message = `Key: ${key} is not translated!`;
								console.warn(message);
								localStorage.removeItem("hasLoadedTranslations");
							}
						}
					});
			} catch (e) {
				//ignore
			}
			resolve();
		});
	};
});