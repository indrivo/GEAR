if (tempDiagram.length > 0) {
    tempDiagram = tempDiagram.replace(/&lt;/g, "<");
    tempDiagram = tempDiagram.replace(/&amp;/g, "&");
    tempDiagram = tempDiagram.replace(/&gt;/g, ">");
    tempDiagram = tempDiagram.replace(/&quot;/g, "\"");
    tempDiagram = tempDiagram.replace(/&apos;/g, "'");
}

const initialDiagram = tempDiagram.length > 0 ? tempDiagram : '<?xml version="1.0" encoding="UTF-8"?>' +
    '<bpmn:definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ' +
    'xmlns:bpmn="http://www.omg.org/spec/BPMN/20100524/MODEL" ' +
    'xmlns:bpmndi="http://www.omg.org/spec/BPMN/20100524/DI" ' +
    'xmlns:dc="http://www.omg.org/spec/DD/20100524/DC" ' +
    'targetNamespace="http://bpmn.io/schema/bpmn" ' +
    'id="Definitions_1">' +
    '<bpmn:process id="Process_1" isExecutable="false">' +
    '<bpmn:startEvent id="StartEvent_1"/>' +
    '</bpmn:process>' +
    '<bpmndi:BPMNDiagram id="BPMNDiagram_1">' +
    '<bpmndi:BPMNPlane id="BPMNPlane_1" bpmnElement="Process_1">' +
    '<bpmndi:BPMNShape id="_BPMNShape_StartEvent_2" bpmnElement="StartEvent_1">' +
    '<dc:Bounds height="36.0" width="36.0" x="173.0" y="102.0"/>' +
    '</bpmndi:BPMNShape>' +
    '</bpmndi:BPMNPlane>' +
    '</bpmndi:BPMNDiagram>' +
    '</bpmn:definitions>';

var viewer = new BpmnJS({
    container: '#canvas'
});

var editor = CodeMirror.fromTextArea(document.getElementById("code"),
    {
        lineNumbers: false,
        lineWrapping: true,
        mode: "xml",
        keyMap: "sublime",
        autoCloseBrackets: true,
        matchBrackets: true,
        showCursorWhenSelecting: true,
        theme: "monokai",
        tabSize: 2,
        autorefresh: true
    });

editor.setSize(null, 1000);

viewer.importXML(initialDiagram,
    function (err) {
        if (err) {
            console.log('error rendering', err);
        } else {
            $(".bjs-powered-by img").attr("src", "/images/logo-icon.png");
            $(".bjs-powered-by").attr("href", "#");
            $(".bjs-powered-by").removeClass("bjs-powered-by").addClass("power-by-indrivo");
            $(".power-by-indrivo").on("click", function () {
                swal("Info", "Power of Soft-Tehnica :D", "");
            });


            viewer.invoke(function (elementRegistry, modeling) {
                var elements = elementRegistry;
                //console.log(elements);
            });

            var eventBus = viewer.get('eventBus');

            // you may hook into any of the following events
            var events = [
                'element.hover',
                'element.out',
                'element.click',
                'element.dblclick',
                'element.mousedown',
                'element.mouseup'
            ];

            events.forEach(function (event) {
                eventBus.on(event, function (e) {
                    if (e.originalEvent.type == "click") {
                        //console.log(e);
                    }
                    const isOpen = storage.isOpenSideBar();
                    if (isOpen) {
                        if (e.originalEvent.type == "click" || e.originalEvent.type == "dblclick") {
                            loadContent(e);
                        }
                    }
                    else {
                        if (e.originalEvent.type == "dblclick") {
                            loadContent(e);
                        }
                    }
                });
            });
        }
    });

// save diagram on button click
const saveButton = document.querySelector('#finalyzeAction');

saveButton.addEventListener('click',
    function () {
        // get the diagram contents
        viewer.saveXML({ format: true },
            function (err, xml) {
                if (err) {
                    console.error('diagram save failed', err);
                } else {
                    document.getElementById("Diagram").value = xml;
                    document.getElementById("DiagramSettings").value = JSON.stringify(storage.elements);
                    document.getElementById("processCreate").submit();
                }
            });
    });
const tabActivate = document.querySelector('#tabActivate');

tabActivate.addEventListener("click", function () {
    if (storage.isOpenSideBar()) {
        toggleRightSideBar();
    }
    viewer.saveXML({ format: true },
        function (err, xml) {
            if (err) {
                console.error('diagram save failed', err);
            } else {
                editor.focus();
                editor.setValue(xml);
                editor.refresh();
            }
        });
});


$(document).ready(function () {
    $(".right-side-toggle-bpm").click(toggleRightSideBar);
});

function toggleRightSideBar() {
    $(".right-sidebar-bpm").slideDown(50);
    $(".right-sidebar-bpm").toggleClass("shw-rside-bpm");
}

const configs = load("/Process/GetBpmConfig");

const storage = {
    currentId: "",
    isOpenSideBar: function () {
        return $(".right-sidebar-bpm").hasClass("shw-rside-bpm");
    },
    settings: {
        getConfigFor: function (elementType) {
            const match = this.elementsConfigurations.filter((x) => {
                return x.Id == elementType;
            });
            if (match.length >= 1) {
                return match[0];
            }
            return null;
        },
        elementsConfigurations: configs
    },
    isIdRegistered: function (id) {
        const match = this.elements.filter((x) => {
            return x.id == id;
        });
        if (match.length >= 1) {
            return true;
        }
        return false;
    },
    elements: tempDiagram.length > 0 ? JSON.parse(tempSettings) : []
};

function loadContent(e) {
    console.log(e);
    $("#sidebar-title").html(`Element Id: <code>${e.element.id}</code>`);
    if (storage.currentId != e.element.id) {
        const settings = storage.settings.getConfigFor(e.element.type);
        if (settings != null) {
            createPanelTabs(settings.Tabs, e);
        }
        else {
            clearPanelTabs();
            $("#bpm-sidebar-tabs-head").html("<h6>No settings available</h6>");
        }
    }

    if (storage.currentId === e.element.id) {
        toggleRightSideBar();
    }

    storage.currentId = e.element.id;
}

function clearPanelTabs() {
    $("#bpm-sidebar-tabs-head").html(null);
    $("#bpm-sidebar-tabs-body").html(null);
}

function getVal(element, e) {
    if (storage.isIdRegistered(e.element.id)) {
        const obj = storage.elements.find((x) => { return x.id == e.element.id });
        let response = obj[element.FormName];
        return response != undefined ? obj[element.FormName] : "";
    }
    return "";
}

function loadTabContent(tab, e) {
    if (tab.Sections.length == 0) return "<h6>No settings available</h6>";
    let container = "";
    $.each(tab.Sections, function (index, section) {
        container += `<h3>${section.Name}</h3>`;
        if (section.Elements.length > 0) {
            $.each(section.Elements, function (i, element) {
                let val = getVal(element, e);
                container += `<label style="padding-right: 3em">${element.Name}</label>`;
                switch (element.Type) {
                    case "label": {
                        container += `<code>${e.element[element.LoadValue]}</code>`;
                    } break;
                    case "textarea": {
                        container += `<textarea data-id="${e.element.id}" name="${element.FormName}" class="form-control bpm-serializable">${val}</textarea>`;
                    } break;
                    case "input": {
                        container += `<input data-id="${e.element.id}" name="${element.FormName}" value="${val}" type="text" class="form-control bpm-serializable" />`;
                    } break;
                    case "color": {
                        container += `<input data-id="${e.element.id}" name="${element.FormName}" value="${val}" type="color" class="form-control bpm-serializable-color" />`;
                    } break;
                    case "select": {
                        let c = document.createElement("select");
                        c.setAttribute("class", "form-control bpm-serializable");
                        c.setAttribute("data-id", e.element.id);
                        c.setAttribute("name", element.FormName);
                        if (element.hasOwnProperty("Settings")) {
                            const req = load(element.Settings.Url);
                            $.each(req, function (index, item) {
                                let opt = document.createElement("option");
                                opt.value = item[element.Settings.Params.Value];
                                opt.innerHTML = item[element.Settings.Params.Name];
                                if (item[element.Settings.Params.Value] == val) {
                                    opt.setAttribute("selected", "selected");
                                }
                                c.appendChild(opt);
                            });
                        }
                        container += c.outerHTML;
                    } break;
                }
                container += "<br>";
            });
        }
        else {
            container += "<h6>No settings available</h6>";
        }
    });
    return container;
}

function createPanelTabs(tabs, e) {
    clearPanelTabs();
    $.each(tabs, function (index, tab) {
        let header = document.createElement("li");
        header.setAttribute("class", "nav-item");
        let ha = document.createElement("a");
        if (index == 0) ha.setAttribute("class", "nav-link active");
        else ha.setAttribute("class", "nav-link");

        ha.setAttribute("data-toggle", "tab");
        ha.setAttribute("href", `#target_${index}`);
        ha.setAttribute("role", "tab");
        ha.innerHTML = `<span class="hidden-sm-up"><i class="ti-home"></i>
											</span> <span class="hidden-xs-down">${tab.Name}</span>`;
        header.appendChild(ha);

        let body = document.createElement("div");
        body.setAttribute("id", `target_${index}`);
        body.setAttribute("role", "tabpanel");
        body.innerHTML = loadTabContent(tab, e);
        if (index == 0) body.setAttribute("class", "tab-pane p-20 active");
        else body.setAttribute("class", "tab-pane p-20");

        $("#bpm-sidebar-tabs-head").append(header);
        $("#bpm-sidebar-tabs-body").append(body);
        $(".bpm-serializable").on("change", onChangeValue);
        $(".bpm-serializable-color").on("input", onChangeValue);
    });
}


function onChangeValue() {
    const id = $(this).attr("data-id");
    const name = $(this).attr("name");
    if (!storage.isIdRegistered(id)) {
        storage.elements.push({
            id: id
        });
    }
    const v = this.value;
    $.each(storage.elements, function () {
        if (this.id == id) {
            viewer.invoke(function (elementRegistry, modeling) {
                let elem = elementRegistry.get(id);
                let updateObj = {};
                updateObj[name] = v;
                if (name != "color") {
                    modeling.updateProperties(elem, updateObj);
                }

                if (name == "color") {
                    var elementsToColor = [];
                    elementsToColor.push(elem);
                    console.log(modeling);
                    modeling.setColor(elementsToColor, {
                        stroke: 'black',
                        fill: v
                    });
                }
            });
            this[name] = v;
        }
    });
}
