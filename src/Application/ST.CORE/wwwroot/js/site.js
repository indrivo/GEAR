//Delete row from Jquery Table
function DeleteData(object) {
	swal({
		title: object.alertText,
		text: object.alertText,
		type: object.type,
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: object.confirmButtonText
	}).then((result) => {
		if (result.value) {
			$.ajax({
				url: `${object.urlForDelete}`,
				type: "post",
				dataType: "json",
				contentType: "application/x-www-form-urlencoded; charset=utf-8",
				data: {
					__RequestVerificationToken: window.getTokenAntiForgery(),
					id: object.rowId
				},
				success: function(data) {
					if (data.success) {
						const oTable = $(`${object.tableId}`).DataTable();
						oTable.draw();
						swal("Deleted!", object.message, "success");
					} else {
						swal("Fail!", data.message, "error");
					}
				},
				error: function() {
					swal("Fail!", object.onServerNoResponse, "error");
				}
			});
		}
	});
}






//------------------------------------------------------------------------------------//
//								Dynamic tree for ISO Standart
//------------------------------------------------------------------------------------//


function loadTree(uri, data = null, type = "get") {
	try {
		const url = new URL(location.href);
		uri = `${url.origin}${uri}`;

		const req = $.ajax({
			url: uri,
			type: type,
			data: data,
			async: false
		});
		return JSON.parse(req.responseText);
	} catch (exp) {
		console.log(exp);
		return null;
	}
}

function getTree(standartEntityId, categoryEntityId, requirementEntityId) {
	const data = loadTree(`/PageRender/GetTreeData?standartEntityId=${standartEntityId}&categoryEntityId=${categoryEntityId}?requirementEntityId=${requirementEntityId}`);
	console.log(data);
	const tree = [
		{
			dataId: "Guid",
			text: "ISO 270001",
			color: "#55ce63",
			selectable: true,
			state: {
				checked: false,
				disabled: false,
				expanded: false,
				selected: false
			},
			nodes: [
				{
					text: "Categorie 1",
					nodes: [
						{
							text: "Sub 1",
							nodes: [
								{
									selectable: false,
									text: `<span>Cerinta 1</span>
												<div style="margin-left: 60%" class="btn-group" role="group" aria-label="Action buttons">
													<a class="btn btn-success btn-sm" href="#">Actions</a>
													<a class="btn btn-info btn-sm" href="#">KPI</a>
													<a href="#" class='btn btn-danger btn-sm'>Goals</a>
												</div>`
								},
								{
									selectable: false,
									text: `<span>Cerinta 2</span>
												<div style="margin-left: 60%" class="btn-group" role="group" aria-label="Action buttons">
													<a class="btn btn-success btn-sm" href="#">Actions</a>
													<a class="btn btn-info btn-sm" href="#">KPI</a>
													<a href="#" class='btn btn-danger btn-sm'>Goals</a>
												</div>`
								},
							]
						},
						{
							text: "Sub 2"
						}
					]
				},
				{
					text: "Categorie 2"
				}
			]
		}
	];
	return tree;
}

$(document).ready(function() {
	let trees = $(".custom-tree-iso");
	$.each(trees, function(index, data) {

		if (!(location.href.indexOf("about:blank") !== -1)) {
			const standartId = $(data).attr("db-tree-standart");
			const categoryId = $(data).attr("db-tree-category");
			const requirementId = $(data).attr("db-tree-requirement");


			$(data).treeview({
				levels: 1,
				selectedBackColor: "#03a9f3",
				onhoverColor: "rgba(0, 0, 0, 0.05)",
				expandIcon: "ti-plus",
				collapseIcon: "ti-minus",
				nodeIcon: "",
				data: getTree(standartId, categoryId, requirementId)
			});

			$(data).on("nodeExpanded",
				function (event, node) {
					console.log(node);
					//$(".custom-tree-iso").treeview(true).addNode({ text: "OK" }, node, 0);
				});
		}
	});
});
