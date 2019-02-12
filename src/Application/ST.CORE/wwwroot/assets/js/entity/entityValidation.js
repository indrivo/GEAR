var fields = $("#renderedFields").find("div.field");
var validations = {
	Name: {
		identifier: "Name",
		rules: [
			{
				type: "empty"
			}
		]
	}
};
function getValue(name) {
	var value = $("#formValidate").find("div input[name='" + name + "']").val();
	console.log(value);
	return value;
}

$.fn.form.settings.rules.validateScale = function (value) {
	var v = parseInt(getValue("Configurations[1].Value"));
	var m = false;
	if (parseInt(value) < 6) {
		if (v >= parseInt(value)) {
			m = true;
		}
	}
	return m;
};



for (var field in fields) {
	var data = fields[field];
	var childs = data.children;
	if (childs != undefined && childs.length > 0) {
		if (childs[0] != undefined && childs[0].tagName == "P") {
			var type = childs[0].childNodes[0].nodeValue;
			var name = childs[1].attributes[0].nodeValue;
			console.log(type);
			switch (type) {
				case "MaxValue": {
					validations[name] = {
						identifier: name,
						rules: [

						]
					}
				} break;
				case "Precision": {
					validations[name] = {
						identifier: name,
						rules: [
							{
								type: "integer[1..38]",
								prompt: 'Value must be between 1 and 38'
							}
						]
					}
				} break;
				case "Scale": {
					validations[name] = {
						identifier: name,
						rules: [
							{
								type: "validateScale",
								prompt: 'Value must be lower Precision and must be between 1 and 6'
							},
							{
								type: "empty"
							}
						]
					}
				} break;
				case "DefaultValue": {

				} break;
				case "ContentLen": {
					validations[name] = {
						identifier: name,
						rules: [
							{
								type: "integer[1..4000]",
								prompt: 'Value must be between 1 and 4000'
							}
						]
					}
				} break;
				case "ForeingTable": {

				} break;
				//image || file
				case "Extensions": {

				} break;
				case "Folder": {

				} break;
				case "MaxWidth": {

				} break;
				case "MinWidth": {

				} break;
				case "MaxHeight": {

				} break;
				case "MinHeight": {

				} break;
				case "MaxSize": {

				}
			}
		}
	}
}

$('#formValidate').form({
	inline: true,
	on: 'blur',
	fields: validations
});