//Add Save
cmdm.add("save", function () {
    let html = editor.getHtml();
    const css = editor.getCss();
    const js = editor.getJs();
    const script = document.createElement("script");
    script.innerHtml = js;
    console.log(js);
    console.log(script);
    html += script.innerText;

    console.log(script.innerText);

    const url = new URL(location.href);
    const pageId = url.searchParams.get("pageId");
    $.ajax({
        url: "/Page/Save",
        type: "post",
        data: {
            cssCode: css,
            htmlCode: html,
            pageId: pageId
        },
        error: function (error) {
            var mdlDialog = document.querySelector('.gjs-mdl-dialog');
            mdlDialog.className += ' ' + mdlClass;
            modal.setTitle('Info');
            modal.setContent(
                `
			<center>
				<span style="color: red; font-size: 1.5em;" class='fa fa-close'></span><br>Data fail to save!
			</center>`);
            modal.open();
        },
        success: function (data) {
            if (data.is_success) {
                var mdlDialog = document.querySelector('.gjs-mdl-dialog');
                mdlDialog.className += ' ' + mdlClass;
                modal.setTitle('Info');
                modal.setContent(
                    `
			<center>
				<span style="color: green; font-size: 1.5em;" class='fa fa-check-circle'></span><br>Data has been saved!
			</center>`);
                modal.open();
            }
        }
    });
});

pn.addButton('options', {
    id: 'save',
    className: 'fa fa-save',
    command: function () { editor.runCommand('save') },
    attributes: {
        'title': 'Save',
        'data-tooltip-pos': 'bottom'
    }
});

// Add info command
var mdlClass = 'gjs-mdl-dialog-sm';
var infoContainer = document.getElementById('info-panel');
cmdm.add('open-info', function () {
    var mdlDialog = document.querySelector('.gjs-mdl-dialog');
    mdlDialog.className += ' ' + mdlClass;
    infoContainer.style.display = 'block';
    modal.setTitle('About Page Builder');
    modal.setContent(infoContainer);
    modal.open();
    modal.getModel().once('change:open',
        function () {
            mdlDialog.className = mdlDialog.className.replace(mdlClass, '');
        });
});

pn.addButton('options', {
    id: 'open-info',
    className: 'fa fa-question-circle',
    command: function () { editor.runCommand('open-info') },
    attributes: {
        'title': 'About',
        'data-tooltip-pos': 'bottom'
    }
});

			//$(document).ready(function () {
			// $(document).keydown(function (e) {
			// e.preventDefault();
			// if ((e.which == '115' || e.which == '83') && (e.ctrlKey || e.metaKey) && !(e.altKey)) {
			// editor.runCommand('save');
			// return false;
			// }
			// return true;
			// });
			//});