/*
 * Load data from server as json
 */
function load(uri, data = null, type = "get") {
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

/*
 *	Get scripts of page by page param id
 */
function getScripts() {
    const u = new URL(location.href);
    const def = ["/lib/jquery/dist/jquery.js", "/lib/tether/dist/js/tether.min.js",
        "/lib/bootstrap/dist/js/bootstrap.bundle.min.js", "/lib/jquery-ui/jquery-ui.min.js",
        "/lib/datatables/media/js/jquery.dataTables.min.js", "/lib/datatables/media/js/dataTables.bootstrap4.min.js",
        "/lib/jquery-slimscroll/jquery.slimscroll.js", "/js/waves.js", "/lib/sweetalert2/dist/sweetalert2.min.js",
        "/js/sidebarmenu.js", "/lib/select2/dist/js/select2.full.min.js",
        "/lib/sticky-kit/jquery.sticky-kit.js", "/assets/js/actions.js", "/js/site.js",
        "/assets/js/actions.js", "/js/custom.js", "/assets/js/notifications/notificator.js",
        "/lib/jsrender/jsrender.min.js", "/lib/jsviews/jsviews.min.js",
        "/js/signalr/dist/browser/signalr.js", "/assets/js/builder/after-load.js"];

    let pageId = u.searchParams.get("pageId");

    if (!pageId) {
        try {
            pageId = temp.layout.id;
        }
        catch (e) {
            console.log(e);
        }
    }

    if (!pageId) {
        const scr = load("/PageRender/GetScripts", {
            pageId: pageId
        });
        //scr.push(`/PageRender/GetPageScript?pageId=${pageId}`);
        scr.push("/assets/js/builder/after-load.js");

        return scr;
    }
    return def;
}

/*
 * Load styles of page by param id
 */
function getStyles() {
    const u = new URL(location.href);
    let pageId = u.searchParams.get("pageId");

    if (!pageId) {
        try {
            pageId = temp.layout.id;
        }
        catch (e) {
            console.log(e);
        }
    }

    const scr = load("/PageRender/GetStyles", {
        pageId: pageId
    });
    return scr;
}

const scripts = getScripts();

/*
 * Register new plugin for grape js
 */
grapesjs.plugins.add('gjs-dynamic-entities', (editor, options) => {
    const trans = load("/PageRender/GetTranslations");
    const iframe = editor.Canvas.getFrameEl();
    const form = new Form();
    const modal = editor.Modal;

    let translations = [];

    for (let item in trans) {
        translations.push({
            name: trans[item],
            value: item
        });
    }

    //Add translate traits for default type

    var defaultType = editor.DomComponents.getType("default");
    var _initialize = defaultType.model.prototype.initialize;
    defaultType.model.prototype.initialize = function () {
        _initialize.apply(this, arguments);

        this.get("traits").add(
            {
                type: 'select',
                label: 'Translate',
                name: 'translate',
                options: translations
            }
        );
    };

    const entities = load("/PageRender/GetEntities");
    const blocks = load("/PageRender/GetBlocks");
    const forms = load("/PageRender/GetForms");

    var domComps = editor.DomComponents;
    var dType = domComps.getType('default');
    var dModel = dType.model;
    var dView = dType.view;

    //------------------------------------------------------------------------------------//
    //								Define trait types
    //------------------------------------------------------------------------------------//

	/*
	 * Define new trait type
	 */
    editor.TraitManager.addType('preview_form', {
        events: {
            "click": function () {
                const formId = this.target.attributes.attributes["db-form"];
                var mdlDialog = document.querySelector('.gjs-mdl-dialog');
                mdlDialog.className += ' ' + mdlClass;
                modal.setTitle('Preview form');

                const data = form.getFormFronServer(formId);
                if (data.is_success) {
                    const json = form.cleanJson(data.result);

                    try {
                        let formeo = new window.Formeo(
                            {
                                allowEdit: false
                            },
                            JSON.stringify(json));
                        let container = document.createElement("div");

                        const renderContainer = container;

                        $(function () {
                            formeo.render(renderContainer);
                            modal.setContent(container);
                            modal.open();
                        });
                    } catch (exp) {
                        console.log(exp);
                    }
                }
            }
        },
        getInputEl: function () {
            if (!this.inputEl) {
                var button = document.createElement('button');
                button.innerHTML = "Preview form";
                button.setAttribute("class", "btn btn-primary");
                button.setAttribute("style", "width: 100%;");
                this.inputEl = button;
            }
            return this.inputEl;
        }
    });

    //------------------------------------------------------------------------------------//
    //								Define custom traits
    //------------------------------------------------------------------------------------//

    //add custom traits
    domComps.addType('input', {
        model: dModel.extend({
            defaults: Object.assign({}, dModel.prototype.defaults, {
                traits: [
                    // strings are automatically converted to text types
                    'name',
                    'placeholder',
                    {
                        type: 'select',
                        label: 'Type',
                        name: 'type',
                        options: [
                            { value: 'text', name: 'Text' },
                            { value: 'email', name: 'Email' },
                            { value: 'password', name: 'Password' },
                            { value: 'number', name: 'Number' },
                        ]
                    }, {
                        type: 'checkbox',
                        label: 'Required',
                        name: 'required',
                    }]
            })
        }, {
            isComponent: function (el) {
                if (el.tagName === 'INPUT') {
                    return { type: 'input' };
                }
            }
        }),

        view: dView
    });

    //add list settings
    domComps.addType('Dynamic List', {
        model: dModel.extend({
            defaults: Object.assign({}, dModel.prototype.defaults, {
                traits: [
                    // strings are automatically converted to text types
                    'name',
                    {
                        type: 'select',
                        label: 'Entity',
                        name: 'db-entity',
                        options: entities.map(function (data) {
                            let obj = {};
                            obj.name = data.name;
                            obj.value = data.id;
                            return obj;
                        })
                    }]
            })
        }, {
            isComponent: function (el) {
                if (el.tagName === 'TABLE') {
                    return { type: 'Dynamic List' };
                }
            }
        }),

        view: dView
    });

    //add form settings
    domComps.addType('Dynamic Form', {
        model: dModel.extend({
            defaults: Object.assign({}, dModel.prototype.defaults, {
                traits: [
                    // strings are automatically converted to text types
                    'name',
                    {
                        type: 'select',
                        label: 'Form',
                        name: 'db-form',
                        options: forms.map(function (data) {
                            let obj = {};
                            obj.name = data.name;
                            obj.value = data.id;
                            return obj;
                        })
                    },
                    {
                        type: "preview_form",
                        label: "Action",
                        name: "preview"
                    }]
            })
        }, {
            isComponent: function (el) {
                if (el.tagName === 'FORM') {
                    return { type: 'Dynamic Form' };
                }
            }
        }),

        view: dView
    });

    //------------------------------------------------------------------------------------//
    //								Define custom blocks
    //------------------------------------------------------------------------------------//

    var blockManager = editor.BlockManager;

    //Register user blocks
    $.each(blocks,
        function (index, block) {
            blockManager.add('custom-' + block.blockName + '-block', {
                label: block.blockName,
                category: block.category,
                content: block.html,
                attributes: {
                    class: block.icon
                }
            });
        });

    //Dynamic form
    blockManager.add('custom-form-block', {
        label: 'Dynamic Form',
        type: "Dynamic Form",
        category: "Dynamic Entities",
        content: `
		 <div class="row">
                    <div class="col-lg-6">
                        <form class="form" style="margin-left: 1em !important">
                                    <div class="form-group">
                                        <label for="exampleInputuname">User Name</label>
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="ti-user"></i></div>
                                            <input type="text" class="form-control" id="exampleInputuname" placeholder="Username">
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="checkbox checkbox-success">
                                            <input id="checkbox1" type="checkbox">
                                            <label for="checkbox1"> Remember me </label>
                                        </div>
                                    </div>
                                    <button type="submit" class="btn btn-success waves-effect waves-light m-r-10">Submit</button>
                                    <button type="submit" class="btn btn-inverse waves-effect waves-light">Cancel</button>
                                </form>
                    </div>
			`,
        attributes: {
            class: "fa fa-align-justify"
        }
    });

    //Dynamic list
    blockManager.add('custom-list-block', {
        label: 'Dynamic List',
        type: "Dynamic List",
        category: "Dynamic Entities",
        content: `
		<div class="table-responsive">
			<div class="mt-2">
				<div class="d-flex">
					<div class="mr-auto">
						<div class="form-group">
							<a asp-action="Create" class="btn btn-primary btn-sm">
								<i class="mdi mdi-circle-edit-outline mr-2" aria-hidden="true"></i>Add new
							</a>
							<small>New item will be added.</small>
						</div>
					</div>
				</div>
			</div>
			<table class="table table-striped table-bordered" id="pageTable">
				<thead>
					<tr>
						<th><span>Name</span></th>
						<th><span>Description</span></th>
						<th><span>Created</span></th>
						<th><span>Changed</span></th>
						<th><span>Author</span></th>
						<th><span>Deleted</span></th>
						<th><span>Actions</span></th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>Samuel Jackson</td>
						<td>Our description</td>
						<td>18/12/12</td>
						<td>12/12/12</td>
						<td>system</td>
						<td>false</td>
						<td>Actions</td>
					</tr>
				</tbody>
			</table>
		</div>
			`,
        attributes: {
            class: "fa fa-align-justify"
        }
    });
});