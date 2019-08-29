const blockId = "#meetingBlock";
const block = $(blockId);

const meetingId = "79155b6a-a187-43f8-b087-374384367b12"; //helper.getParamFormUrl("meetingId");

$(() => {
	new IsoMeetings(meetingId);
});


class IsoMeetings {
	//Inject data injector
	db = new DataInjector();

	//Inject template manager
	templateManager = new TemplateManager();

	//Inject toast notifier
	toast = new ToastNotifier();

	//Inject helper
	helper = new ST();

	//constructor
	constructor(meetingId) {
		this.templateManager.registerTemplate("template_template_meeting_children_accordion");
		this.meetingId = meetingId;
		if (!meetingId) return toast.notify({ heading: "Meeting not found" });
		this.init();
	}

	//tab configuration
	accordionItems = [
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_meeting_action_state"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_actions_from_previous_revisions"),
					label: window.translate("iso_meeting_action_state"),
					key: "actionStatus",
					hasButton: false
				}
			]
		},
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_internal_and_external_aspects_relevant"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_changes_in_internal_and_external_aspects"),
					label: window.translate("iso_changes_in_internal_and_external_aspects"),
					key: "aspectChanges",
					hasButton: false
				}
			]
		},
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_reactions_on_information_security_performance"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("nonconformities_and_corrective_actions"),
					label: window.translate("nonconformities_and_corrective_actions"),
					key: "nonconformities",
					hasButton: true,
					button: {
						isLink: true,
						buttonText: window.translate("show_actions"),
						link: ""
					}
				},
				{
					id: this.helper.newGuid(),
					name: window.translate("surveillance_and_measurement_results"),
					label: window.translate("surveillance_and_measurement_results"),
					key: "observation",
					hasButton: true,
					button: {
						isLink: true,
						buttonText: `${window.translate("show")} ${window.translate("iso_kpi")}`,
						link: ""
					}
				},
				{
					id: this.helper.newGuid(),
					name: window.translate("audit_results"),
					label: window.translate("audit_results"),
					key: "auditResults",
					hasButton: true,
					button: {
						isLink: true,
						buttonText: window.translate("show_actions"),
						link: ""
					}
				},
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_achieving_information_security_objectives"),
					label: window.translate("iso_achieving_information_security_objectives"),
					key: "reachingGoals",
					hasButton: true,
					button: {
						isLink: false,
						buttonText: `${window.translate("show")} ${window.translate("iso_objectives")}`
					}
				},
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_trends_in_main_indicators_of_information_security"),
					label: window.translate("iso_trends_in_main_indicators_of_information_security"),
					key: "trand",
					hasButton: true,
					button: {
						isLink: false,
						buttonText: `${window.translate("show")} ${window.translate("iso_trends")}`
					}
				}
			]
		},
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_stakeholder_feedback"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_stakeholder_feedback"),
					label: window.translate("iso_stakeholder_feedback"),
					key: "feedback",
					hasButton: false

				}
			]
		},
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_the_result_of_the_risk_assessment"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_the_result_of_the_risk_assessment"),
					label: window.translate("iso_stakeholder_feedback"),
					key: "evaluationResult",
					hasButton: false
				}
			]
		},
		{
			id: this.helper.newGuid(),
			name: window.translate("iso_opportunities_for_continuous_improvement"),
			hasChildNodes: true,
			items: [
				{
					id: this.helper.newGuid(),
					name: window.translate("iso_opportunities_for_continuous_improvement"),
					label: window.translate("iso_opportunities_for_continuous_improvement"),
					key: "opportunities",
					hasButton: true,
					button: {
						buttonText: window.translate("show_actions"),
						isLink: false
					}
				}
			]
		}
	];

	entities = {
		meetings: "Meeting",
		meetingParticipants: "MeetingParticipant"
	};

	/*
	 *Db meeting
	 */
	meeting = undefined;

	/*
	 * Init component
	 */
	init() {
		const spinner = this.templateManager.render("template_svgSpinner", {
			id: this.helper.newGuid()
		});
		this.db.getByIdWithIncludesAsync(this.entities.meetings, this.meetingId)
			.then(dbResult => {
				if (dbResult.is_success) {
					//store globaly meeting
					this.meeting = dbResult.result;

					const filters = [{ parameter: "MeetingId", value: this.meetingId }];
					this.db.getAllWhereWithIncludesAsync(this.entities.meetingParticipants, filters).then(dbParticipants => {
						if (dbParticipants.is_success) {
							const participants =
								dbParticipants.result
									.map(x => `${x.personIdReference.name} ${x.personIdReference.lastName}`)
									.join(", ");

							const obj = Object.assign({}, dbResult.result);
							obj.participants = participants;
							const content = this.templateManager.render("template_template_meeting_details", obj);
							block.html(content);


							//temp
							$.templates("secondLvl", "#secondLvlCollapse");
							var template = $.templates("#rootCollapse");
							const htmlOutput = template.render(this.accordionItems, {
								obj: obj,
								spinner: spinner
							});

							$("#test").html(htmlOutput);

							this.bindEventsToTextarea();
							this.bindEventsToCollapseActions();

							//end temp
							window.forceTranslate(blockId);
						} else {
							this.toast.notifyErrorList(result.error_keys);
						}
					}).catch(e => {
						this.toast.notify({ heading: "Fail" });
					});
				} else {
					this.toast.notifyErrorList(result.error_keys);
				}
			}).catch(err => {
				this.toast.notify({ heading: "Fail" });
			});
	}

	/*
	 * bind events
	 */
	bindEventsToTextarea() {
		const scope = this;
		$(".textarea-binding").on("blur", function () {
			const ctx = $(this);
			scope.update({
				key: ctx.data("key"),
				value: ctx.val()
			});
		});
	}

	/*
	 * Bind events
	 */
	bindEventsToCollapseActions() {
		const scope = this;
		$(".collapse-binder").on("click", function () {
			const ctx = $(this);
			const key = ctx.data("key");
			const container = $(ctx.data("target"));

			switch (key) {
				case "reachingGoals": {
					const dtoId = "#render_f3cb8b17-01a2-439b-938c-820835c77deb";
					container.html(scope.getReachingGoalsTable());

					const conf = {
						showActionsColumn: false,
						ajax: {
							url: "/PageRender/LoadPagedData",
							type: "POST",
							data: {
								filters: []
							}
						}
					};

					const jDto = $(dtoId)
					const tBuilder = new TableBuilder(conf);
					tBuilder.init(jDto.get(0));
					new TableInlineEdit().init(dtoId);
					jDto
						.on("preInit.dt", function () {
							const headConf = new IsoTableHeadActions().getConfiguration();
							const content = tManager.render("template_headListActions", headConf);
							const selector = $("div.CustomTableHeadBar");
							selector.html(content);
							$('.table-search').keyup(function () {
								const oTable = $(this).closest(".card").find(".dynamic-table").DataTable();
								oTable.search($(this).val()).draw();
							});
							//Delete multiple rows
							$(".deleteMultipleRows").on("click", function () {
								const cTable = $(this).closest(".card").find(tableSelector);
								if (cTable) {
									if (typeof TableBuilder !== 'undefined') {
										new TableBuilder().deleteSelectedRowsHandler(cTable.DataTable());
									}
								}
							});

							$(".add_new_inline").on("click", function () {
								new TableInlineEdit().addNewHandler(this);
							});

							//Items on page
							$(".tablePaginationView a").on("click", function () {
								const ctx = $(this);
								const onPageValue = ctx.data("page");
								const onPageText = ctx.text();
								ctx.closest(".dropdown").find(".page-size").html(`(${onPageText})`);
								const table = ctx.closest(".card").find(tableSelector).DataTable();
								table.page.len(onPageValue).draw();
							});

							//hide columns
							$(".hidden-columns-event").click(function () {
								console.log(tableSelector);
								const tables = $(this).closest(".card").find(tableSelector);
								if (tables.length === 0) return;
								new TableColumnsVisibility().toggleRightListSideBar(`#${tables[0].id}`);
								$("#hiddenColumnsModal").modal();
							});
							window.forceTranslate("div.CustomTableHeadBar");
						});
				} break;
			}
		});
	}

	getReachingGoalsTable() {
		return `  <div class="card">
                <div class="card-body"><table id="render_f3cb8b17-01a2-439b-938c-820835c77deb" db-viewmodel="aa47eaf4-5b96-42a6-8387-72adb53ce713"
                class="dynamic-table table table-striped table-bordered"></table></div></div>`;
	}

	/**
	 * update metadata
	 * @param {any} data
	 */
	update(data = { key: "", value: "" }) {
		//update value
		this.meeting[data.key] = data.value;
		this.db.updateAsync(this.entities.meetings, this.meeting).then(response => {
			if (response.is_success) {
				this.toast.notify({ heading: "Info", text: window.translate("system_inline_saved"), icon: "success" });
			} else {
				this.toast.notifyErrorList(response.error_keys);
			}
		}).catch(e => {
			console.warn(e);
		});
	}
}