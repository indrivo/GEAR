const blockId = "#meetingBlock";
const block = $(blockId);

const spinner = new TemplateManager().render("template_svgSpinner", {
    id: new ST().newGuid()
});

block.html(spinner);

const meetingId = new ST().getParamFormUrl("meetingId");

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

    //Inject builder
    builder = new TableInlineEdit();

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
                        isLink: false,
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
                        isLink: false,
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
                        isLink: false,
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
                    $("#history_back")
                        .parent()
                        .find("h1")
                        .append(` ${dbResult.result.code}`);

                    //store globaly meeting
                    this.meeting = dbResult.result;

                    const filters = [{ parameter: "MeetingId", value: this.meetingId }];
                    this.db.getAllWhereWithIncludesAsync(this.entities.meetingParticipants, filters).then(dbParticipants => {
                        if (dbParticipants.is_success) {
                            const participants =
                                dbParticipants.result
                                    .map(x => `${x.personIdReference.name} ${x.personIdReference.lastName}`)
                                    .join(", ");

                            const obj = Object.assign(dbResult.result, {
                                tabs: this.accordionItems
                            });
                            obj.participants = participants.length === 0
                                ? window.translate("iso_no_participants")
                                : participants;

                            const content = this.templateManager.render("template_template_meeting_details", obj, {
                                obj: obj,
                                spinner: spinner
                            });
                            block.html(content);

                            this.bindEventsToTextarea();
                            this.bindEventsToCollapseActions();
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
        const isLoadedAttrName = "is-loaded";
        const scope = this;
        $(".collapse-binder").on("click", function () {
            const ctx = $(this);
            const key = ctx.data("key");
            const container = $(ctx.data("target"));
            const state = ctx.attr(isLoadedAttrName);
            if (state === "true") return;
            switch (key) {
                //Show objectives
                case "reachingGoals": {
                    const dtoId = "#render_f3cb8b17-01a2-439b-938c-820835c77deb";
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

                    scope.builder.createAndRenderTable({
                        target: container,
                        selector: dtoId,
                        dbViewModel: "aa47eaf4-5b96-42a6-8387-72adb53ce713",
                        builderConfiguration: conf
                    });
                } break;

                //Show kpi's
                case "observation": {
                    const dtoId = "#render_a7402009-1442-4d05-b62e-eb3d911269bf";
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

                    scope.builder.createAndRenderTable({
                        target: container,
                        selector: dtoId,
                        dbViewModel: "815544b9-5c25-40bb-b320-47723ca38653",
                        builderConfiguration: conf
                    });
                } break;

                //Show actions
                case "nonconformities": {
                    const dtoId = "#render_a7402009-1442-4d05-b62e-eb3d91126876";
                    const conf = {
                        showActionsColumn: false,
                        ajax: {
                            url: "/PageRender/LoadPagedData",
                            type: "POST",
                            data: {
                                filters: [
                                    { parameter: "Source", searchValue: "5b79aaa8-807e-4e87-99d1-dee520653b39" }]
                            }
                        }
                    };

                    scope.builder.createAndRenderTable({
                        target: container,
                        selector: dtoId,
                        dbViewModel: "f845d6a0-648c-4050-afe1-ef8909bb91dd",
                        builderConfiguration: conf
                    });
                } break;

                //Show actions for audit
                case "auditResults": {
                    const dtoId = "#render_a7402009-1442-4d05-b62e-eb3d91126812";
                    const conf = {
                        showActionsColumn: false,
                        ajax: {
                            url: "/PageRender/LoadPagedData",
                            type: "POST",
                            data: {
                                filters: [
                                    { parameter: "Source", searchValue: "53b83573-7bf0-479e-9598-03469c9e1ae9" }]
                            }
                        }
                    };

                    scope.builder.createAndRenderTable({
                        target: container,
                        selector: dtoId,
                        dbViewModel: "f845d6a0-648c-4050-afe1-ef8909bb91dd",
                        builderConfiguration: conf
                    });
                } break;
            }

            ctx.attr(isLoadedAttrName, "true");
        });
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