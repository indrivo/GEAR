var st = new ST();

function Form() {
	this.row = st.newGuid();
	this.column = st.newGuid();
	this.fieldCount = 0;
	this.data = {
		"id": st.newGuid(),
		"settings": {},
		"stages": {
			"2c8079d5-048f-4baa-92a8-5907414f8eed": {
				"id": "2c8079d5-048f-4baa-92a8-5907414f8eed",
				"settings": {},
				"rows": [
					this.row
				]
			}
		},
		"rows": {
			[this.row]: {
				"columns": [
					this.column
				],
				"id": this.row,
				"config": {
					"fieldset": false,
					"legend": "",
					"inputGroup": false
				},
				"attrs": {
					"className": "f-row"
				}
			}
		},
		"columns": {
			[this.column]: {
				"fields": [
				],
				"id": this.column,
				"config": {
					"width": "100%"
				},
				"className": []
			}
		},
		"fields": {
		}
	};
};


/*
* Constructor
*/
Form.prototype.constructor = Form;

/**
 * Add field to json
 * @param {any} field  Field data
 * @param {any} id Field id
 */
Form.prototype.addField = function (field, id) {
	this.data.fields[id] = field[id];
	this.data.columns[this.column].fields.push(id);
	this.fieldCount++;
};

/**
 * Generate form json
 * @param {any} data Json with fields
 */
Form.prototype.generateJsonForm = function (data) {
	const fields = data.result;

	for (let field in fields) {
		if (fields.hasOwnProperty(field)) {
			const model = {
				name: fields[field].name,
				fieldTypeId: fields[field].id
			};

			switch (fields[field].dataType) {
				case "nvarchar":
					{
						this.pushText(model);
					};
					break;
				case "bool":
					{
						this.pushCheckBox(model);
					};
					break;
				case "decimal":
					{
						this.pushNumber(model);
					};
					break;
				case "int32":
					{
						this.pushNumber(model);
					};
					break;
				case "bytes":
					{
						//console.log(fields[field].dataType);
					};
					break;
				case "date":
					{
						this.pushDate(model);
					};
					break;
				case "datetime":
					{
						this.pushDate(model);
					};
					break;
				case "uniqueidentifier":
					{
						this.pushSelect(model);
					};
					break;
			}
		}
	}
};
/**
 * Push textBox field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushText = function (model) {
	const fieldId = st.newGuid();
	const field = {
		[fieldId]: {
			"tag": "input",
			"attrs": {
				"type": "text",
				"required": false,
				"className": ""
			},
			"config": {
				"disabledAttrs": ["type"],
				"label": model.name,
			},
			"meta": {
				"group": "common",
				"icon": "text-input",
				"id": "text-input"
			},
			"fMap": "attrs.value",
			"id": fieldId,
			"tableFeldId": model.fieldTypeId
		}
	};
	this.addField(field, fieldId);
};
/**
 * Push number field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushNumber = function (model) {
	const fieldId = st.newGuid();
	const field = {
		[fieldId]: {
			"tag": "input",
			"attrs": {
				"type": "number",
				"required": false,
				"className": ""
			},
			"config": {
				"label": model.name,
				"disabledAttrs": [
					"type"
				]
			},
			"meta": {
				"group": "common",
				"icon": "hash",
				"id": "number"
			},
			"fMap": "attrs.value",
			"id": fieldId,
			"tableFeldId": model.fieldTypeId
		}
	};
	this.addField(field, fieldId);
};

/**
 * Push textarea field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushTextarea = function (model) {

};
/**
 * Push dropdown field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushSelect = function (model) {
	const fieldId = st.newGuid();
	const field = {
		[fieldId]: {
			"tag": "select",
			"config": {
				"label": model.name
			},
			"attrs": {
				"required": false,
				"className": ""
			},
			"meta": {
				"group": "common",
				"icon": "select",
				"id": "select"
			},
			"options": [
				{
					"label": "Option 1",
					"value": "option-1",
					"selected": false
				}
			],
			"id": fieldId,
			"tableFeldId": model.fieldTypeId
		}
	};
	this.addField(field, fieldId);
};
/**
 * Push checkbox field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushCheckBox = function (model) {
	const fieldId = st.newGuid();
	const field = {
		[fieldId]: {
			"tag": "input",
			"attrs": {
				"type": "checkbox",
				"required": false
			},
			"config": {
				"label": model.name,
				"disabledAttrs": [
					"type"
				]
			},
			"meta": {
				"group": "common",
				"icon": "checkbox",
				"id": "checkbox"
			},
			"options": [
				{
					"label": model.name,
					"value": model.name,
					"selected": true
				}
			],
			"id": fieldId,
			"tableFeldId": model.fieldTypeId
		}
	};
	this.addField(field, fieldId);
};
/**
 * Push date field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushDate = function (model) {
	const fieldId = st.newGuid();
	const field = {
		[fieldId]: {
			"tag": "input",
			"attrs": {
				"type": "date",
				"required": false,
				"className": ""
			},
			"config": {
				"disabledAttrs": [
					"type"
				],
				"label": model.name
			},
			"meta": {
				"group": "common",
				"icon": "calendar",
				"id": "date-input"
			},
			"id": fieldId,
			"tableFeldId": model.fieldTypeId
		}
	};
	this.addField(field, fieldId);
};
/**
 * Get current json  for render
 * @returns {any} Json
 */
Form.prototype.getJson = function () {
	return this.data;
};
/**
 * Get current json as string for render
 * @returns {any} Json
 */
Form.prototype.get = function () {
	return JSON.stringify(this.data);
};
/**
 * Print json like a string in console
 */
Form.prototype.print = function () {
	console.log(JSON.stringify(this.data));
};
/**
 * Print Json in console
 */
Form.prototype.printJson = function () {
	console.log(this.data);
};
/**
 * Get options for formeo render
 * @param {any} containerSelector Selector
 * @returns {any} Formeo options
 */
Form.prototype.getOptions = function (containerSelector) {
	const container = document.querySelector(containerSelector);
	const formeoOpts = {
		container: container,
		//allowEdit: true,
		controls: {
			sortable: false,
			groupOrder: [
				"common",
				"html"
			],
			elements: [
				{
					tag: 'input',
					options: [{
						label: 'Yes',
						value: 'Yes',
						selected: false
					}, {
						label: 'No',
						value: 'No',
						selected: false
					}],
					attrs: {
						type: 'radio',
						CaptionWidth: '125px'
					},
					config: {
						label: 'Radio'
					},
					meta: {
						group: 'common',
						icon: 'radio-group',
						id: 'radio-new'
					},

				}
			],
			elementOrder: {
				common: [
					"button",
					"checkbox",
					"date-input",
					"hidden",
					"upload",
					"number",
					"radio",
					"select",
					"text-input",
					"textarea"
				]
			}
		},
		events: {
			// onUpdate: console.log,
			// onSave: console.log

		},
		svgSprite: '/assets/images/form_icons.svg',
		// debug: true,
		sessionStorage: true,
		editPanelOrder: ["attrs", "options"]
	};
	return formeoOpts;
};
/**
 * Get form by id
 * @param {any} id Form id
 * @returns {any} Response from server
 */
Form.prototype.getFormFronServer = function (id) {
	var response = null;
	$.ajax({
		url: "/api/Form/GetForm",
		data: { id: id },
		method: "get",
		async: false,
		success: function (data) {
			if (data !== null) {
				response = data;
			}
		},
		error: function (error) {
			response = false;
		}
	});
	return response;
};

/**
 * Get from url
 * @param {any} name Name of url parameter
 * @returns {any} Value of parameter
 */
Form.prototype.getFromUrl = function (name) {
	const url = location.href;
	const parsedUrl = new URL(url);
	const id = parsedUrl.searchParams.get(name);
	return id;
};

/**
 * Render form
 * @param {any} data Json with configs
 * @param {any} target Selector where render form
 */
Form.prototype.render = function (data, target) {
	try {
		const formeo = new window.Formeo(this.getOptions("#temp"), JSON.stringify(data));
		var renderContainer = document.querySelector(target);

		document.querySelector("body").onload = evt => {
			$(document).ready(function () {
				formeo.render(renderContainer);
			});
		};
	} catch (exp) {
		console.log(exp);
	}
};

/**
 * Clean json for null Proprieties
 * @param {any} obj Object for clean
 * @returns {any} The cleaned json
 */
Form.prototype.cleanJson = function (obj) {
	const removeEmpty = (obj) =>
		Object.entries(obj).forEach(([key, val]) => {
			if (val && typeof val === "object") removeEmpty(val);
			else if (val == null) delete obj[key];
		});
	Object.keys(obj).forEach(function (key) {
		if (obj[key] && typeof obj[key] === "object") removeEmpty(obj[key]);
		else if (obj[key] == null) delete obj[key];
	});
	return obj;
};


/**
 * Format json and return it
 * @param {any} json The json
 * @param {any} textarea Selector to place
 */
Form.prototype.formatJSON = function (json, textarea) {
	var nl;
	if (textarea) {
		nl = "&#13;&#10;";
	} else {
		nl = "<br>";
	}
	const tab = "&#160;&#160;&#160;&#160;";
	var ret = "";
	var numquotes = 0;
	var betweenquotes = false;
	var firstquote = false;
	for (let i = 0; i < json.length; i++) {
		const c = json[i];
		if (c == '"') {
			numquotes++;
			if ((numquotes + 2) % 2 == 1) {
				betweenquotes = true;
			} else {
				betweenquotes = false;
			}
			if ((numquotes + 3) % 4 == 0) {
				firstquote = true;
			} else {
				firstquote = false;
			}
		}

		if (c == '[' && !betweenquotes) {
			ret += c;
			ret += nl;
			continue;
		}
		if (c == '{' && !betweenquotes) {
			ret += tab;
			ret += c;
			ret += nl;
			continue;
		}
		if (c == '"' && firstquote) {
			ret += tab + tab;
			ret += c;
			continue;
		} else if (c == '"' && !firstquote) {
			ret += c;
			continue;
		}
		if (c == ',' && !betweenquotes) {
			ret += c;
			ret += nl;
			continue;
		}
		if (c == '}' && !betweenquotes) {
			ret += nl;
			ret += tab;
			ret += c;
			continue;
		}
		if (c == ']' && !betweenquotes) {
			ret += nl;
			ret += c;
			continue;
		}
		ret += c;
	}
	return ret;
};