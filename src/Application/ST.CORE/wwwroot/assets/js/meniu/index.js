
//-------------------Global variabiles declaration-----------

var menubarPath = "meniu/index.html",
	listappPath = "meniu/meniulist.html",
	deleteTemplate = "delete.html";

var MenuUrlService = "http://localhost:52390";

var AppsLoaderList = $(".applistloader"),
	BarLoader = $(".barloader");

var MenuAppModel = {
	page: 1,
	perpage: 5,
	total: 0
};

function getTemplate(relPath) {
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
}

var action = function () {
	function init() {
		AppsLoaderList.show();
		$(function () {
			$("#meniupagination").pagination({
				items: MenuAppModel.total,
				itemsOnPage: MenuAppModel.perpage,
				cssStyle: "list-pagination",
				onPageClick: function (pageNumber, event) {
					MenuAppModel.page = pageNumber;
					GetList(MenuAppModel);
				},
				onInit: function () {
					GetList(MenuAppModel);
				}
			});
		});
	}
	function SuccessAjax(data) {
		AppsLoaderList.hide();
		if (data) {
			if (data.is_success) {
				const fields = ["Aplicatia", "Adresa"];
				data.model = fields;
				const html = $.render.listapp(data);
				$("#meniulist").html(html);
				$(function () {
					$("#meniupagination").pagination(
						"updateItems",
						data.pagination.total_count
					);
				});
				GetMenuitems(data.result[0].id);
			}
		}
		AddEvents(MenuAppModel);
	}
	function ErrorAjax(error) {
		console.log(error);
	}

	function GetList(MenuAppModel) {
		$("#meniulist").html(null);
		AppsLoaderList.show();
		$("#meniupagination").hide();
		$.ajax({
			url: MenuUrlService + "/api/Applications/GetByPage",
			method: "get",
			data: {
				page: MenuAppModel.page,
				pageSize: MenuAppModel.perpage
			},
			success: function (data) {
				SuccessAjax(data);
				$("#meniupagination").show();
			},
			error: function (err) {
				ErrorAjax(err);
			}
		});
	}
	function AddEvents(MenuAppModel) {
		$(".appItem").on("click", function () {
			const id = $(this).attr("data");
			GetMenuitems(id);
		});
		$(".delApp").on("click", function () {
			const id = $(this).attr("data");
			deleteApp(id);
		});
	}
	function deleteApp(id) {
		$.ajax({
			url: MenuUrlService + "/api/Applications/Delete/" + id,
			method: "delete",
			success: function (data) {
				if (data) {
					if (data.is_success) {
						const notification = new Notification({
							type: "success",
							message: "Inregistrarea a fost stearsa cu sucess"
						});
						GetList(MenuAppModel);
					} else {
						const notification1 = new Notification({
							type: "danger",
							message: "Inregistrarea nu poate fi stersa!"
						});
					}
				}
			},
			error: function (error) {
				const notification = new Notification({
					type: "danger",
					message: "Inregistrarea nu poate fi stersa!"
				});
				console.log(error);
			}
		});
	}

	function GetMenuitems(id) {
		$("#barManagement").html(null);
		BarLoader.show();
		$.ajax({
			url: MenuUrlService + "/GetMenuItems",
			data: {
				applicationId: id
			},
			method: "get",
			success: function (data) {
				const result = {};
				result.data = data;
				const html = $.render.menubar(result);
				$("#barManagement").html(html);
				BarLoader.hide();
			},
			error: function (error) {
				BarLoader.hide();
			}
		});
		AddEventsMenu();
	}

	function AddEventsMenu() {
		$(".AddItemMenu").on("click", function () {
			const AppId = $(this).parent().parent().attr("applicationId");
			$("#AddMenuModal").modal("show");
			$("#MenuItemForm1").find("input[type=hidden]").val(AppId);
		});
		$("#addMenuItem1S").on("click", function () {
			var form = $("#MenuItemForm1");
			const data = form.serializeArray();
			console.log(data);
			$.ajax({
				url: MenuUrlService + "/AddMenuItem",
				method: "post",
				data: serializeToJson(data),
				dataType: "json",
				contentType: "application/json",
				success: function (data) {
					if (data) {
						if (data.is_success) {
							const notification = new Notification({
								type: "success",
								message: "Item adaugat!"
							});
							const appId = $("#menujs").attr("applicationId");
							GetMenuitems(appId);
							form[0].reset();
						}
						else {
							new Notification({
								type: "danger",
								message: "Itemul nu poate fi adaugat!"
							});
						}
					}
				},
				error: function (error) {
					console.log(error);
				}
			});
		});
		$(".delMenuItem").on("click", function () {
			const id = $(this).parent().find("a").attr("data");
			const todelete = $(this).parent().find("a").html();
			const model = {
				todelete: todelete,
				modalId: "DeleteModalItemMenu",
				selector: "delItem",
				id: id
			};
			const deleteTemplate = $.render.delete(model);
			$("#placeDeleteModal").html(deleteTemplate);
			$("#DeleteModalItemMenu").modal("show");
			$("#delItem").on("click", function () {
				const id = $(this).attr("data");
				$.ajax({
					url: MenuUrlService + "/Delete/" + id,
					method: "delete",
					success: function (data) {
						try {
							if (data.is_success) {
								new Notification({
									type: "success",
									message: "Item sters!"
								});
								const appId = $("#menujs").attr("applicationId");
								GetMenuitems(appId);
								$("#DeleteModalItemMenu").modal("hide");
							}
						}
						catch (e) {
							//on error
						}
					},
					error: function (error) {
						console.log(error);
					}
				});
			});
		});
	}

	function AddMenuItem() {

	}

	return {
		init: init
	};
};
var Manager = action();
$(document).ready(function () {
	if (location.href.toLowerCase().indexOf("menu") !== -1) {
		Promise.all([getTemplate(menubarPath), getTemplate(listappPath), getTemplate(deleteTemplate)])
			.then(function (values) {
				$.templates("menubar", values[0]);
				$.templates("listapp", values[1]);
				$.templates("delete", values[2]);
				Manager.init();
			})
			.catch(function (err) {
				console.log(err);
			});
	}

	//Add event + app

	$("#addAppSubmit").on("click", function () {
		var form = $("#AppForm");
		const data = form.serializeArray();
		$.ajax({
			url: MenuUrlService + "/api/Applications/AddApplication",
			method: "post",
			dataType: "json",
			contentType: "application/json",
			data: serializeToJson(data),
			success: function (data) {
				if (data) {
					if (data.is_success) {
						$("#AddAppModal").modal("hide");
						Manager.init();
						form[0].reset();
						new Notification({
							type: "success",
							message: "Aplicatia a fost adaugata!"
						});
					}
				}
			},
			error: function (error) {
				console.log(error);
			}
		});
	});
});
