$(function () {
	const form = new Form();

	const id = form.getFromUrl("formId");

	const data = form.getFormFronServer(id);

	$(this).ready(function () {
		$("#formeo-rendered-0").css("width", "100%");
	});

	if (data != null) {
		if (data.is_success) {
			var json = form.cleanJson(data.result);

			let formeo = new window.Formeo(
				{
					allowEdit: false
				},
				JSON.stringify(json));

			const renderContainer = document.querySelector("#preview_place");

			$(function () {
				formeo.render(renderContainer);
				const selects = Array.prototype.filter.call(
					renderContainer.getElementsByTagName('select'),
					function (el) {
						return el.getAttribute('table-field-id') != null;
					}
				);
				form.populateSelect(selects);
			});

			$("#serializeform").on("click",
				function () {
					const st = new ST();
					const serialized = st.serializeToJson($("#serForm"));
					const final = {};
					for (let s in serialized) {
						if (serialized.hasOwnProperty(s)) {
							const id = $("#prev-" + s).attr("table-field-id");
							if (id != undefined) {
								final[id] = serialized[s];
							} else
								final[s] = serialized[s];
						}
					}
					//const json = form.formatJSON(JSON.stringify(final));

					//$("#textarea").empty();
					//$("#textarea").append(json);
					const str = JSON.stringify(final, null, 4);
					output(syntaxHighlight(str));
				});
		}
	} else {
		alert("fail");
	}
});

function output(inp) {
	$("#jsonPlace").html(inp);
}

function syntaxHighlight(json) {
	json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
	return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
		var cls = 'number';
		if (/^"/.test(match)) {
			if (/:$/.test(match)) {
				cls = 'key';
			} else {
				cls = 'string';
			}
		} else if (/true|false/.test(match)) {
			cls = 'boolean';
		} else if (/null/.test(match)) {
			cls = 'null';
		}
		return '<span class="' + cls + '">' + match + '</span>';
	});
}