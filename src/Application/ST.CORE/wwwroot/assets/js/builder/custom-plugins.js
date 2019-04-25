window.load = function (uri, data = null, type = "get") {
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
};
/*
 *	Get scripts of page by page param id
 */
function getScripts() {
	const u = new URL(location.href);
	const def = [
		"/lib/jquery/jquery.min.js",
		"/lib/tether/js/tether.min.js",
		"/lib/twitter-bootstrap/js/bootstrap.bundle.min.js",
		"/lib/jqueryui/jquery-ui.min.js",
		"/lib/datatables/js/jquery.dataTables.min.js",
		"/lib/datatables/js/dataTables.bootstrap4.min.js",
		"/lib/jQuery-slimScroll/jquery.slimscroll.min.js",
		"/lib/limonte-sweetalert2/sweetalert2.min.js",
		"/lib/select2/js/select2.full.min.js",
		"/lib/sticky-kit/sticky-kit.min.js",
		"/js/site.js",
		"/lib/jsrender/jsrender.min.js",
		"/lib/jsviews/jsviews.min.js",
		"/js/signalr/dist/browser/signalr.js",
		"/assets/js/builder/after-load.js"
	];

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
grapesjs.plugins.add("gjs-dynamic-entities", (editor, options) => {
	const trans = load("/Localization/GetTranslationsForCurrentLanguage");
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
	var initialize = defaultType.model.prototype.initialize;
	defaultType.model.prototype.initialize = function () {
		initialize.apply(this, arguments);

		this.get("traits").add(
			{
				type: "select",
				label: "Translate",
				name: "translate",
				options: translations
			}
		);
	};

	const entities = load("/PageRender/GetEntities");
	const blocks = load("/PageRender/GetBlocks");
	const forms = load("/PageRender/GetForms");
	const viewModels = load("/PageRender/GetViewModels");
	const pages = load("/PageRender/GetPages");

	var domComps = editor.DomComponents;
	var dType = domComps.getType("default");
	var dModel = dType.model;
	var dView = dType.view;

	//------------------------------------------------------------------------------------//
	//								Define trait types
	//------------------------------------------------------------------------------------//

	/*
	 * Define new trait type
	 */
	editor.TraitManager.addType("preview_form", {
		events: {
			"click": function () {
				const formId = this.target.attributes.attributes["db-form"];
				const mdlDialog = document.querySelector(".gjs-mdl-dialog");
				mdlDialog.className += " " + mdlClass;
				modal.setTitle("Preview form");

				const data = form.getFormFronServer(formId);
				if (data.is_success) {
					const json = form.cleanJson(data.result);

					try {
						const formeo = new window.Formeo(
							{
								allowEdit: false
							},
							JSON.stringify(json));
						const container = document.createElement("div");

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
				const button = document.createElement("button");
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
	domComps.addType("input", {
		model: dModel.extend({
			defaults: Object.assign({}, dModel.prototype.defaults, {
				traits: [
					// strings are automatically converted to text types
					"name",
					{
						type: "text",
						name: "placeholder"
					},
					{
						type: "select",
						label: "Type",
						name: "type",
						options: [
							{ value: "text", name: "Text" },
							{ value: "email", name: "Email" },
							{ value: "password", name: "Password" },
							{ value: "number", name: "Number" }
						]
					}, {
						type: "checkbox",
						label: "Required",
						name: "required"
					}]
			})
		}, {
				isComponent: function (el) {
					if (el.tagName === "INPUT") {
						return { type: "input" };
					}
				}
			}),

		view: dView
	});

	//add list settings
	domComps.addType("Dynamic List", {
		model: dModel.extend({
			defaults: Object.assign({}, dModel.prototype.defaults, {
				traits: [
					// strings are automatically converted to text types
					"description",
					{
						type: "text",
						label: "List identifier",
						name: "id"
					},
					{
						type: "select",
						label: "View model",
						name: "db-viewmodel",
						options: viewModels.map(function (data) {
							const obj = {};
							obj.name = data.name;
							obj.value = data.id;
							return obj;
						})
					},
					{
						type: "checkbox",
						name: "data-is-editable",
						label: "Has edit option?"
					},
					{
						type: "checkbox",
						name: "data-is-editable-inline",
						label: "Allow edit inline?"
					},
					{
						type: "select",
						label: "Edit Link",
						name: "data-edit-href",
						options: pages.map(function (data) {
							const obj = {};
							obj.name = data.name;
							obj.value = data.id;
							return obj;
						})
					},
					{
						type: "checkbox",
						name: "data-allow-edit-restore",
						label: "Allow delete/restore?"
					},]
			})
		}, {
				isComponent: function (el) {
					if (el.tagName === "TABLE") {
						return { type: "Dynamic List" };
					}
				}
			}),

		view: dView
	});

	//add form settings
	domComps.addType("Dynamic Form", {
		model: dModel.extend({
			defaults: Object.assign({}, dModel.prototype.defaults, {
				traits: [
					// strings are automatically converted to text types
					"name",
					{
						type: "select",
						label: "Form",
						name: "db-form",
						options: forms.map(function (data) {
							const obj = {};
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
					if (el.tagName === "FORM") {
						return { type: "Dynamic Form" };
					}
				}
			}),

		view: dView
	});


	function getTreeOptions() {
		return entities.map(function (data) {
			const obj = {};
			obj.name = data.name;
			obj.value = data.id;
			return obj;
		});
	}


	//add form settings
	domComps.addType("Dynamic Tree", {
		model: dModel.extend({
			defaults: Object.assign({}, dModel.prototype.defaults, {
				traits: [
					// strings are automatically converted to text types
					"name",
					{
						type: "select",
						label: "Standard Entity",
						name: "db-tree-standard",
						options: getTreeOptions()
					},
					{
						type: "select",
						label: "Category Entity",
						name: "db-tree-category",
						options: getTreeOptions()
					},
					{
						type: "select",
						label: "Requirement category",
						name: "db-tree-requirement",
						options: getTreeOptions()
					}
				]
			})
		}, {
				isComponent: function (el) {
					if (el.tagName === "TREE") {
						return { type: "Dynamic Tree" };
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
			blockManager.add("custom-" + block.blockName + "-block", {
				label: block.blockName,
				category: block.category,
				content: block.html,
				attributes: {
					class: block.icon
				}
			});
		});


	//Dynamic form
	blockManager.add("custom-form-block", {
		label: "Dynamic Form",
		type: "Dynamic Form",
		category: "Dynamic Entities",
		content: `
		 <div class="row"> 
                    <div class="col-lg-12">
                        <form class="render-dynamic-form" novalidate action="#" method="post" style="margin-left: 1em !important">
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
		</div>
			`,
		attributes: {
			class: "fa fa-align-justify"
		}
	});


	//Dynamic tree
	blockManager.add("custom-tree-block", {
		label: "ISO Standard Tree Block",
		type: "Dynamic Tree",
		category: "Dynamic Entities",
		content: `
				<tree class="custom-tree-iso col-md-12" style="margin: 0.2em;">Loading tree ...</tree>
			`,
		attributes: {
			class: "fa fa-tree"
		}
	});


	//Dynamic list
	blockManager.add("custom-list-block", {
		label: "Dynamic List",
		type: "Dynamic List",
		category: "Dynamic Entities",
		content: `
<div class="card">
	<div class="card-body">
		<h4 class="card-title">Title</h4>
		<h6 class="card-subtitle">Sub Title</h6>
		<div class="table-responsive">
			<div class="mt-2">
				<div class="d-flex">
					<div class="mr-auto">
						<div class="form-group">
							<a href="#" class="btn btn-primary btn-sm">
								<i class="mdi mdi-circle-edit-outline mr-2" aria-hidden="true"></i>Add
							</a>
							<small>New  will be added.</small>
						</div>
					</div>
				</div>
			</div>
			<table class="dynamic-table table table-striped table-bordered" id="render_" db-viewmodel="">
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
				<tbody></tbody>
			</table>
		</div>
	</div>
</div>
			`,
		attributes: {
			class: "fa fa-align-justify"
		}
	});
});
