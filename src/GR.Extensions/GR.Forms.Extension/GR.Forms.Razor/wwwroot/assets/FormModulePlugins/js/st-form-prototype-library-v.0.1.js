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
                "fields": [],
                "id": this.column,
                "config": {
                    "width": "100%"
                },
                "className": []
            }
        },
        "fields": {}
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
                fieldTypeId: this.getAttrForTableField(fields, fields[field].id),
                fieldId: fields[field].id,
                tableId: fields[field].tableId,
                fieldConfigurations: fields[field].tableFieldConfigValues
            };

            switch (fields[field].dataType) {
                case "nvarchar":
                    this.pushText(model);
                    break;
                case "bool":
                    this.pushCheckBox(model);
                    break;
                case "decimal":
                    this.pushNumber(model);
                    break;
                case "int32":
                    this.pushNumber(model);
                    break;
                case "bytes": {
                    //console.log(fields[field].dataType);
                }
                    break;
                case "date":
                    this.pushDate(model);
                    break;
                case "datetime":
                    this.pushDate(model);
                    break;
                case "uniqueidentifier": {
                    if (model.fieldConfigurations && model.fieldConfigurations.length > 0)
                        this.pushSelect(model);
                }
                    break;
            }
        }
    }
};

Form.prototype.getAttrForTableField = function (fields, selectedId) {
    return fields.map(field => {
        return {
            label: field.name,
            value: field.id,
            selected: field.id === selectedId
        };
    });
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
                "className": "",
                "tableFieldId": model.fieldTypeId
            },
            "config": {
                "disabledAttrs": ["type"],
                "label": model.name
            },
            "meta": {
                "group": "common",
                "icon": "text-input",
                "id": "text-input"
            },
            "fMap": "attrs.value",
            "id": fieldId
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
                "className": "",
                "tableFieldId": model.fieldTypeId
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
            "id": fieldId
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


Form.prototype.getReferenceTable = function (conf) {
    const schema = conf.find(x => {
        return x.tableFieldConfig.name === "ForeingSchemaTable";
    });
    const entity = conf.find(x => {
        return x.tableFieldConfig.name === "ForeingTable";
    });

    if (!entity || !schema) return [];
    const refFields = load(`/Form/GetEntityReferenceFields?entityName=${entity.value}&&entitySchema=${schema.value}`);
    return refFields.map(field => {
        return {
            label: field.name,
            value: field.name,
            selected: false
        };
    });
};

function getSelectedValue(arr) {
    for (let f in arr) {
        if (arr[f].selected) {
            return arr[f].value;
        }
    }
    return undefined;
}

Form.prototype.attrsToArray = function (data) {
    const form = data.result;
    const table = load("/Form/GetFormTableReference",
        {
            formId: this.getFromUrl("formId")
        });
    if (!table) return form;
    const tableFields = this.getEntityFields(table.id);
    for (let i in form.fields) {
        if (form.fields.hasOwnProperty(i)) {
            for (let j in form.fields[i].attrs) {
                const sel = form.fields[i].attrs[j];
                if (j === "tableFieldId") {
                    if (tableFields) {
                        const fields = tableFields.map(field => {
                            return {
                                label: field.name,
                                value: field.id,
                                selected: field.id === sel
                            };
                        });
                        form.fields[i].attrs[j] = fields;
                    }
                } else if (j === "fieldReference") {
                    {
                        form.fields[i].attrs[j] =
                            [{
                                label: "default",
                                value: sel,
                                selected: true
                            }];
                    }
                    ;
                }
            }
        }
    }
    return form;
}

/**
 * Required to boolean
 * @param {any} data
 */
Form.prototype.parseRequiredToBoolean = function (data) {
    const form = data.result;
    for (let i in form.fields) {
        if (form.fields.hasOwnProperty(i)) {
            for (let j in form.fields[i].attrs) {
                const sel = form.fields[i].attrs[j];
                if (j === "required") {
                    form.fields[i].attrs[j] = sel === "True";
                }
            }
        }
    }

    return form;
}

/**
 * Attrs of form to single select
 * @param {any} form
 */
Form.prototype.attrsToString = function (form) {
    for (let i in form.fields) {
        if (form.fields.hasOwnProperty(i)) {
            for (let j in form.fields[i].attrs) {
                if (Array.isArray(form.fields[i].attrs[j])) {
                    form.fields[i].attrs[j] = getSelectedValue(form.fields[i].attrs[j]);
                }
            }
        }
    }
    return form;
};

/**
 * Get reference of select control on change table field
 * @param {any} tableId
 * @param {any} fieldId
 */
Form.prototype.getReferenceSelectOnChangeTable = function (tableId, fieldId) {
    return load(`/Form/GetReferenceFields?entityId=${tableId}&&entityFieldId=${fieldId}`);
    ;
};

/**
 * Push dropdown field to form
 * @param {any} model Data of fields
 */
Form.prototype.pushSelect = function (model) {
    const fieldId = st.newGuid();
    const fieldReference = this.getReferenceTable(model.fieldConfigurations);
    const field = {
        [fieldId]: {
            "tag": "select",
            "config": {
                "label": model.name
            },
            "attrs": {
                "required": false,
                "className": "",
                "tableFieldId": model.fieldTypeId,
                "fieldReference": fieldReference
            },
            "meta": {
                "group": "common",
                "icon": "select",
                "id": "custom-select"
            },
            "options": [
                {
                    "label": "No data",
                    "value": "",
                    "selected": false
                }
            ],
            "id": fieldId
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
            "tableFieldId": model.fieldTypeId
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
            "tableFieldId": model.fieldTypeId
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

Form.prototype.getEntityFields = function (tableId) {
    return load("/Form/GetEntityFields", {
        tableId: tableId
    });
};

/**
 * Get options for formeo render
 * @param {any} containerSelector Selector
 * @param {any} tableId Selector
 * @returns {any} Formeo options
 */
Form.prototype.getOptions = function (containerSelector, tableId) {
    const container = document.querySelector(containerSelector);
    const fields = this.getEntityFields(tableId).map(field => {
        return {
            label: field.name,
            value: field.id,
            selected: false
        };
    });

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
                    tag: "select",
                    attrs: {
                        tableFieldId: (() => {
                            const options = fields;
                            return options;
                        })(),
                        fieldReference: []
                    },
                    config: {
                        label: "Data reference select"
                    },
                    options: [],
                    meta: {
                        group: "common",
                        icon: "select",
                        id: "custom-select"
                    }
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
                    "textarea",
                    "custom-select"
                ]
            }
        },
        events: {
            //onUpdate: (evt) => {
            //	this.registerChangeRefEvent(this, tableId);
            //}
            //onSave: console.log

        },
        svgSprite: "/assets/images/form_icons.svg",
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
    const context = this;
    $.ajax({
        url: "/api/Form/GetForm",
        data: {id: id},
        method: "get",
        async: false,
        success: function (data) {
            if (data) {
                if (data.is_success) {
                    data.result = context.parseRequiredToBoolean(data);
                }
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
 * Register change event for table field
 * @param {any} former
 * @param {any} tableId
 */
Form.prototype.registerChangeRefEvent = function (former, tableId) {
    setTimeout(function () {
        $(`#${former.controls.formID}`).find("select[name^=tableFieldId]").on("change", function () {
            changeTableField(this, tableId);
        });
    }, 1000);
};


function changeTableField(context, tableId) {
    console.log($(context));
    const selectedId = $(context).val();
    const fields = fr.getReferenceSelectOnChangeTable(tableId, selectedId);
    const attrRefs = $(context).parent()
        .parent()
        .parent()
        .parent()
        .parent()
        .find("select[name^=fieldReference]");

    attrRefs.html(null);
    if (fields.length > 0) {
        $.each(fields, function () {
            attrRefs.append(new Option(this.name, this.name));
        });
    }
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
 * Check if is edit form
 * @param {any} formRef
 * @param {any} formId
 */
Form.prototype.checkIfIsEditForm = function (formRef, formId) {
    const itemId = this.getFromUrl("itemId");
    //const listId = this.getFromUrl("listId");
    if (itemId) {
        const data = window.load("/Form/GetValuesFormObjectEditInForm",
            {
                formId: formId,
                itemId: itemId
            });
        const context = this;
        if (data && data.is_success) {
            setTimeout(function () {
                context.populateEditForm(formRef, data.result);
            }, 50);

        } else {
            $.toast({
                heading: "Fail to load form",
                text: "",
                position: 'bottom-right',
                loaderBg: '#ff6849',
                icon: 'error',
                hideAfter: 3500,
                stack: 6
            });
        }
    }
}


/**
 * Extract form fields
 * @param {any} place
 */
Form.prototype.extractOnlyReferenceFields = function (place, serialized) {
    const final = {};
    console.log(serialized);
    for (let s in serialized) {
        if (serialized.hasOwnProperty(s) && s) {
            const id = $(`#${place} #${s}`).attr("table-field-id");
            if (!this.isGuid(id)) continue;
            if (id) {
                final[id] = serialized[s];
            } else
                final[s] = serialized[s];
        }
    }

    return final;
}


/**
 * Check if is guid
 * @param {any} value
 */
Form.prototype.isGuid = function (value) {
    return /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(value);
};


/**
 * Populate edit form
 * @param {any} frm
 * @param {any} data
 */
Form.prototype.populateEditForm = function (frm, data) {
    const context = this;
    $(frm).attr("isEdit", true);
    $.each(data, function (key, value) {
        if (context.isGuid(key)) {
            const $ctrl = $('[name=' + key + ']', frm);
            if ($ctrl.is('select')) {
                $("option", $ctrl).each(function () {
                    if (this.value === value) {
                        this.selected = "selected";
                    }
                });
            }
            if ($ctrl.is('textarea')) {
                $ctrl.html(value);
            } else {
                switch ($ctrl.attr("type")) {
                    case "text":
                    case "hidden":
                    case "number":
                        $ctrl.val(value);
                        console.log($ctrl);
                        break;
                    case "radio":
                    case "checkbox":
                        $ctrl.each(function () {
                            if ($(this).attr('value') === value) {
                                $(this).attr("checked", value);
                            }
                        });
                        break;
                }
            }
        } else {
            const input = document.createElement("input");
            input.setAttribute("type", "hidden");
            input.setAttribute("name", key);
            input.setAttribute("value", value);
            document.querySelector(frm).appendChild(input);
        }
    });
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
 * @param {any} json Old json
 * @param {any} textarea data
 * @return {any} result
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

        if (c == "[" && !betweenquotes) {
            ret += c;
            ret += nl;
            continue;
        }
        if (c == "{" && !betweenquotes) {
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
        if (c == "," && !betweenquotes) {
            ret += c;
            ret += nl;
            continue;
        }
        if (c == "}" && !betweenquotes) {
            ret += nl;
            ret += tab;
            ret += c;
            continue;
        }
        if (c == "]" && !betweenquotes) {
            ret += nl;
            ret += c;
            continue;
        }
        ret += c;
    }
    return ret;
};


Form.prototype.populateSelect = function (selects) {
    $.each(selects,
        function (index, select) {
            const fieldId = $(select).attr("table-field-id");
            const reference = $(select).attr("field-reference");
            $(select).html(null);
            const req = load("/PageRender/GetInputSelectValues",
                {
                    fieldId: fieldId
                });
            let identifier = "name";
            ;
            if (reference) {
                try {
                    identifier = reference.charAt(0).toLowerCase() + reference.slice(1);
                } catch (e) {
                    console.log(e);
                }
            }

            try {
                $(select).append(new Option(window.translate("no_value_selected"), ""));
                if (req.is_success) {
                    $.each(req.result,
                        function (index, item) {
                            $(select).append(new Option(item[identifier], item.id));
                        });
                }
            } catch (e) {
                console.log(e);
            }
        });
}