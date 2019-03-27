//Declarations
let renderContainer = document.querySelector('.render-form');
var fr = new Form();
var s = new ST();
let editing = true;
let toggleEdit = document.getElementById('renderForm');
let viewDataBtn = document.getElementById('viewData');
let SaveDataBtn = document.getElementById('saveData');
let resetDemo = document.getElementById('reloadBtn');

//Events
$(document).ready(function () {
	Init();
});

//Functions
function Init() {
	Promise.all([s.getTemplate("preview.html")])
		.then(function (values) {
			CreateTemplate(values);
			const id = getTableId();
			Request(id);
		})
		.catch(function (err) {
			console.log(err);
		});
}


function CreateTemplate(values) {
	$.templates("preview", values[0]);
}

function getTableId() {
	const url = location.href;
	const parsedUrl = new URL(url);
	return parsedUrl.searchParams.get("tableId");
}

function getFormTypeId() {
	const url = location.href;
	const parsedUrl = new URL(url);
	const id = parsedUrl.searchParams.get("formType");
	return id;
}

function Request(id) {
	$.ajax({
		url: "/api/form/GetTableFields",
		method: "get",
		data: { tableId: id },
		success: function (data) {
			Populate(data);
			$.when(fr.generateJsonForm(data)).then(function () {
				StartFormEditor();
			});
		},
		error: function (error) {
			console.log(error);
		}
	});
}

function Populate(data) {
	const content = $.render.preview(data);
	$("#preview_place").html(content);
}

function StartFormEditor() {
	//fr.printJson();
	const formeo = new window.Formeo(fr.getOptions('.build-form'), fr.get());
	document.querySelector("#RenderForm").onclick = evt => {
		if (!editing) {
			formeo.render(renderContainer);
		}

		return editing = !editing;
	};

	document.querySelector("#EditForm").onclick = evt => {
		return editing = !editing;
	};
	resetDemo.onclick = function () {
		window.sessionStorage.removeItem('formData');
		location.reload();
	};

	viewDataBtn.onclick = evt => {
		console.log(formeo.formData);
	};
	$("#form").on("submit", function (e) {
		$.ajax({
			url: "/api/Form/CreateNewForm",
			method: "post",
			data: {
				form: JSON.parse(formeo.formData),
				tableId: getTableId(),
				formTypeId: getFormTypeId(),
				name: $("#form_name").val(),
				description: $("#form_description").val(),
				postUrl: $("#form_postUrl").val(),
				redirectUrl: $("#form_redirectUrl").val()
			},
			success: function (data) {
				if (data !== null) {
					if (data.is_success) {
						location.href = "/Form";
					}
					else {
						alert("Fail! View console.log");
						console.log(data);
					}
				}
			},
			error: function (error) {
				console.log(error);
			}
		});
	});
	SaveDataBtn.onclick = evt => {
		const formObj = $("#form_name");
		const formName = formObj.val();
		if (formName === "") {
			formObj.focus();
			return;
		}
		fr.data = JSON.parse(formeo.formData);
		$("#form").submit();
	};
}
