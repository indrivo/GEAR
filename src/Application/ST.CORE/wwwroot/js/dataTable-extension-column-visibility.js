window.setCookie = function setCookie(cname, cvalue, exdays) {
	const d = new Date();
	d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
	const expires = "expires=" + d.toUTCString();
	document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

window.getCookie = function getCookie(cname) {
	const name = cname + "=";
	const decodedCookie = decodeURIComponent(document.cookie);
	const ca = decodedCookie.split(";");
	for (let i = 0; i < ca.length; i++) {
		let c = ca[i];
		while (c.charAt(0) == " ") {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function getVisibility(id) {
	const cookie = getCookie(`_list_${id}`);
	const visibledItems = [];
	const hiddenItems = [];
	if (cookie) {
		const data = JSON.parse(cookie);

		for (let i = 0; i < data.values.length; i++) {
			if (data.values[i]) visibledItems.push(i);
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
					}" style="margin-bottom: 0.5em;" class="list-side-toggle btn btn-primary btn-sm" href="#">Visibility of fields</a></div>`);
			$(".list-side-toggle").click(function () {
				toggleRightListSideBar($(this).attr("data-id"));
			});
		});


$(".dynamic-table")
	.on("draw.dt", function (e, settings, json) {
		$(".inline-edit").on("click", function () {
			const viewModelId = $(this).attr("data-viewmodel");
			const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);
			console.log(viewModel);
			const columns = $(this).parent().parent().parent().find(".data-cell");
			for (let i = 0; i < columns.length; i++) {				
				const columnId = $(columns[i]).attr("data-column-id");
				const fieldData = viewModel.result.filter(obj => {
					return obj.columnId === columnId;
				});
				if (fieldData.length > 0) {
					const viewModelId = $(columns[i]).attr("data-viewmodel");
					const cellId = $(columns[i]).attr("data-id");
				}
			}
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
					<a id="selAllCols" href="#">Select All</a>
				</div>
			<div class="col-md-6">
				<a id="deselAllCols" href="#">Deselect All</a>
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