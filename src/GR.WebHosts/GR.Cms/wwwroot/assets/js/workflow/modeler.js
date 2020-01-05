/**
 * bpmn-js-seed
 *
 * This is an example script that loads an embedded diagram file <diagramXML>
 * and opens it using the bpmn-js modeler.
 */
$(document).ready(function (BpmnModeler) {
    // create modeler
    var bpmnModeler = new BpmnModeler({
        container: '#canvas'
    });

    // import function
    function importXML(xml) {
        // import diagram
        bpmnModeler.importXML(xml, function (err) {
            if (err) {
                return console.error('could not import BPMN 2.0 diagram', err);
            }

            const canvas = bpmnModeler.get('canvas');

            // zoom to fit full viewport
            canvas.zoom('fit-viewport');
        });

        // save diagram on button click
        const saveButton = document.querySelector('#save-button');

        saveButton.addEventListener('click', function () {
            // get the diagram contents
            bpmnModeler.saveXML({ format: true }, function (err, xml) {
                if (err) {
                    console.error('diagram save failed', err);
                } else {
                    document.getElementById("Diagram").value = xml;
                }
            });
        });
    }

    // a diagram to display
    //
    // see index-async.js on how to load the diagram asynchronously from a url.
    // (requires a running webserver)
    const initialDiagram = '<?xml version="1.0" encoding="UTF-8"?>' +
        '<bpmn:definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ' +
        'xmlns:bpmn="http://www.omg.org/spec/BPMN/20100524/MODEL" ' +
        'xmlns:bpmndi="http://www.omg.org/spec/BPMN/20100524/DI" ' +
        'xmlns:dc="http://www.omg.org/spec/DD/20100524/DC" ' +
        'targetNamespace="http://bpmn.io/schema/bpmn" ' +
        'id="Definitions_1">' +
        '<bpmn:process id="Process_1" isExecutable="false">' +
        '<bpmn:startEvent id="StartEvent_1"/>' +
        "</bpmn:process>" +
        '<bpmndi:BPMNDiagram id="BPMNDiagram_1">' +
        '<bpmndi:BPMNPlane id="BPMNPlane_1" bpmnElement="Process_1">' +
        '<bpmndi:BPMNShape id="_BPMNShape_StartEvent_2" bpmnElement="StartEvent_1">' +
        '<dc:Bounds height="36.0" width="36.0" x="173.0" y="102.0"/>' +
        '</bpmndi:BPMNShape>' +
        '</bpmndi:BPMNPlane>' +
        '</bpmndi:BPMNDiagram>' +
        '</bpmn:definitions>';
    // import xml
    importXML(initialDiagram);
})(window.BpmnJS);