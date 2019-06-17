/* Table column visibility plugin
 * A plugin for hide and show table columns
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Table column visibility require JQuery');
}

function TableColumnsVisibility() {

}

$(document).ready(function () {
	$(".table")
		.on("preInit.dt", function () {
			new TableColumnsVisibility().init(this);
		});
});

TableColumnsVisibility.prototype.constructor = TableColumnsVisibility;


TableColumnsVisibility.prototype.getVisibility = function (id) {
	const cacheValue = localStorage.getItem(`_list_${id}`);
	const visibleItems = [];
	const hiddenItems = [];
	if (cacheValue) {
		const data = JSON.parse(cacheValue);

		for (let i = 0; i < data.values.length; i++) {
			if (data.values[i]) visibleItems.push(i);
			else hiddenItems.push(i);
		}
	}

	return {
		visibledItems: hiddenItems,
		hiddenItems: hiddenItems
	};
}

TableColumnsVisibility.prototype.renderTemplate = function (ctx) {
	return `<a data-id="#${$(ctx)[0].id
		}" style="margin-bottom: 0.5em;" class="list-side-toggle toggle-columns btn btn-primary btn-sm" href="#">${window.translate("columns-visibility")}</a>`;
};

TableColumnsVisibility.prototype.init = function (ctx) {
	const cols = this.getVisibility(`#${$(ctx).attr("id")}`);
	$(`#${$(ctx).attr("id")}`).DataTable().columns(cols.visibledItems).visible(true);
	$(`#${$(ctx).attr("id")}`).DataTable().columns(cols.hiddenItems).visible(false);
	const template = this.renderTemplate(ctx);

	$("div.CustomizeColumns").html(template);
	this.registerInitEvents();
};

TableColumnsVisibility.prototype.registerInitEvents = function () {
	$(".list-side-toggle").click(function () {
		new TableColumnsVisibility().toggleRightListSideBar($(this).attr("data-id"));
	});
};

function IsChecked(state) {
	if (state) return "checked";
	return "";
}


TableColumnsVisibility.prototype.modalContainer = ".list-sidebar-central .slimscrollright .r-panel-body";

TableColumnsVisibility.prototype.renderCheckBox = function (data, id, vis) {
	return `<div class="checkbox checkbox-info">
							<input type="checkbox" ${vis} data-table="${id}" id="_check_${data.idx
		}" class="complete-activity-trigger vis-check" data-id="${
		data.idx}">
					<label  for="_check_${data.idx}">${data.sTitle}</label>
					</div>`;
};

TableColumnsVisibility.prototype.toggleRightListSideBar = function (id) {
	try {
		const cols = $(id).DataTable().settings()[0].aoColumns;
		var items = "";
		const cacheValue = localStorage.getItem(`_list_${id}`);
		let display = null;
		if (cacheValue) {
			display = JSON.parse(cacheValue);
		}

		$.each(cols,
			function (index, data) {
				let vis = "checked";
				if (display) {
					vis = IsChecked(display.values[data.idx]);
				}
				items += `<li class="list-group-item">${new TableColumnsVisibility().renderCheckBox(data, id, vis)}</li>`;
			});

		const container =
			`<div class="row">
				<div class="col-md-6">
					<a id="selAllCols" href="#">${window.translate("select_all")}</a>
				</div>
			<div class="col-md-6">
				<a id="deselAllCols" href="#">${window.translate("deselect_all")}</a>
				</div>
			</div><div class="to-do-widget"><ul class="todo-list list-group m-b-0">${
			items}</ul</div>`;
		$(new TableColumnsVisibility().modalContainer).html(container);

		$("#selAllCols").on("click", function () {
			new TableColumnsVisibility().dataStateChange(this, true, id);
		});

		$("#deselAllCols").on("click", function () {
			new TableColumnsVisibility().dataStateChange(this, false, id);
		});

		$(".vis-check").change(function () {
			new TableColumnsVisibility().dataChanged(this, id);
		});
	} catch (error) {
		console.log(error);
	}

	$(".list-sidebar-central").slideDown(50);
	$(".list-sidebar-central").toggleClass("shw-rside");
}


TableColumnsVisibility.prototype.dataStateChange = function (ref, state, id) {
	const inputs = $($(ref)
		.parent()
		.parent()
		.parent()
		.children()[1])
		.find("input[type=checkbox]");

	for (let input = 0; input < inputs.length; input++) {
		inputs[input].checked = state;
		new TableColumnsVisibility().dataChanged(inputs[input], id);
	}
}


TableColumnsVisibility.prototype.dataChanged = function (ref, id) {
	const checked = $(ref).is(":checked");
	const idd = $(ref).attr("data-id");
	const tabled = $(ref).attr("data-table");
	const cacheValue = localStorage.getItem(`_list_${tabled}`);

	if (cacheValue) {
		const data = JSON.parse(cacheValue);
		data.values[idd] = checked;
		data.columns[idd] = idd;

		localStorage.setItem(`_list_${tabled}`, JSON.stringify(data), 9999);
	} else {
		const nrCols = $(id).DataTable().settings()[0].aoColumns.length;
		const newCacheValue = {
			columns: [],
			values: []
		};
		for (let i = 0; i < nrCols; i++) {
			newCacheValue.values[i] = i;
			newCacheValue.columns[i] = true;
		}
		newCacheValue.values[idd] = checked;
		newCacheValue.columns[idd] = idd;
		localStorage.setItem(`_list_${tabled}`, JSON.stringify(newCacheValue), 9999);
	}
	$(tabled).DataTable().columns([idd]).visible(checked);
};