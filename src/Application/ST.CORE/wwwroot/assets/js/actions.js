// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Events requires jQuery')
}

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
			var origin = location.origin;
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
 * Show notification on body by type
 * @param {any} model Model with fields
 * of new notification
 * work on bootstrap
 */
ST.prototype.notification = function (model) {
	$.notify(
		{
			// options
			icon: model.icon || "glyphicon glyphicon-warning-sign",
			title: model.title,
			message: model.message || "Message",
			url: "#",
			target: "_blank"
		},
		{
			// settings
			element: "body",
			position: null,
			type: model.type || "info",
			allow_dismiss: true,
			newest_on_top: false,
			showProgressbar: false,
			placement: {
				from: "top",
				align: "right"
			},
			offset: 20,
			spacing: 10,
			z_index: 1031,
			delay: 5000,
			timer: 1000,
			url_target: "_blank",
			mouse_over: null,
			animate: {
				enter: "animated fadeInDown",
				exit: "animated fadeOutUp"
			},
			onShow: null,
			onShown: null,
			onClose: null,
			onClosed: null,
			icon_type: "class",
			template:
				'<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
				'<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
				'<span data-notify="icon"></span> ' +
				'<span data-notify="title">{1}</span> ' +
				'<span data-notify="message">{2}</span>' +
				'<div class="progress" data-notify="progressbar">' +
				'<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
				"</div>" +
				'<a href="{3}" target="{4}" data-notify="url"></a>' +
				"</div>"
		}
	);
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
 * Search items on list
 * Work with semantic-ui
 * @param {any} selector Selector for response render
 * @param {any} api Url for get data
 * @param {any} key Key for search
 * @param {any} url Url to change after select item
 */
ST.prototype.search = function (selector, api, key, url) {
	$(selector)
		.search({
			apiSettings: {
				url: api
			},
			fields: {
				results: 'results',
				title: key
			},
			minCharacters: 2,
			onSelect: function (result, response) {
				window.location.href = url + result.id;
				return true;
			}
		});
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
	$.each(data, function (key, value) {
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
		}
		else {
			switch ($ctrl.attr("type")) {
				case "text": case "hidden": case "number":
					$ctrl.val(value);
					break;
				case "radio": case "checkbox":
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

$(document).ready(function () {
	$('.ui.st_menu').dropdown();
});


