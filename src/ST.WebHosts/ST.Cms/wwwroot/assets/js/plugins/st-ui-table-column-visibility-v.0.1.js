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

function getVisibility(id) {
	const cookie = getCookie(`_list_${id}`);
	const visibleItems = [];
	const hiddenItems = [];
	if (cookie) {
		const data = JSON.parse(cookie);

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

$(".table")
	.on("preInit.dt",
		function () {
			const cols = getVisibility(`#${$(this).attr("id")}`);
			$(`#${$(this).attr("id")}`).DataTable().columns(cols.visibledItems).visible(true);
			$(`#${$(this).attr("id")}`).DataTable().columns(cols.hiddenItems).visible(false);

			$("div.CustomizeColumns")
				.html(
					`<div class="col-md-2" style="margin-left: -1em;">
												<a data-id="#${$(this)[0].id
					}" style="margin-bottom: 0.5em;" class="list-side-toggle toggle-columns btn btn-primary btn-sm" href="#">${window.translate("columns-visibility")}</a></div>`);
			$(".list-side-toggle").click(function () {
				toggleRightListSideBar($(this).attr("data-id"));
			});
		});

function IsChecked(state) {
	if (state) return "checked";
	return "";
}


function toggleRightListSideBar(id) {
	try {
		const cols = $(id).DataTable().settings()[0].aoColumns;
		var items = "";
		const cookie = getCookie(`_list_${id}`);
		let display = null;
		if (cookie) {
			display = JSON.parse(cookie);
		}

		$.each(cols,
			function (index, data) {
				let vis = "checked";
				if (display) {
					vis = IsChecked(display.values[data.idx]);
				}
				items += `<li class="list-group-item">
							<div class="checkbox checkbox-info">
							<input type="checkbox" ${vis} data-table="${id}" id="_check_${data.idx
					}" class="complete-activity-trigger vis-check" data-id="${
					data.idx}">
					<label  for="_check_${data.idx}">${data.sTitle}</label>
					</div>
				</li>`;
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
		$(".list-sidebar-central .slimscrollright .r-panel-body").html(container);

		$("#selAllCols").on("click", function () {
			dataStateChange(this, true);
		});

		$("#deselAllCols").on("click", function () {
			dataStateChange(this, false);
		});

		function dataStateChange(ref, state) {
			const inputs = $($(ref)
				.parent()
				.parent()
				.parent()
				.children()[1])
				.find("input[type=checkbox]");

			for (let input = 0; input < inputs.length; input++) {
				inputs[input].checked = state;
				dataChanged(inputs[input]);
			}
		}

		$(".vis-check").change(function () {
			dataChanged(this);
		});
	} catch (error) {
		console.log(error);
	}

	function dataChanged(ref) {
		const checked = $(ref).is(":checked");
		const idd = $(ref).attr("data-id");
		const tabled = $(ref).attr("data-table");
		const cookie = getCookie(`_list_${tabled}`);

		if (cookie) {
			const data = JSON.parse(cookie);
			data.values[idd] = checked;
			data.columns[idd] = idd;

			setCookie(`_list_${tabled}`, JSON.stringify(data), 9999);
		} else {
			const nrCols = $(id).DataTable().settings()[0].aoColumns.length;
			const newCookie = {
				columns: [],
				values: []
			};
			for (let i = 0; i < nrCols; i++) {
				newCookie.values[i] = i;
				newCookie.columns[i] = true;
			}
			newCookie.values[idd] = checked;
			newCookie.columns[idd] = idd;
			setCookie(`_list_${tabled}`, JSON.stringify(newCookie), 9999);
		}
		$(tabled).DataTable().columns([idd]).visible(checked);
	}

	$(".list-sidebar-central").slideDown(50);
	$(".list-sidebar-central").toggleClass("shw-rside");
}