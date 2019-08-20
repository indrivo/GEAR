const blockId = "#meetingBlock";
const block = $(blockId);
const db = new DataInjector();
const toast = new ToastNotifier();
const templateManager = new TemplateManager();
const helper = new ST();
//window.translate("key");
templateManager.registerTemplate("template_template_meeting_children_accordion");
$(() => {
	const meetingId = "79155b6a-a187-43f8-b087-374384367b12"; //helper.getParamFormUrl("meetingId");
	if (!meetingId) return toast.notify({ heading: "Meeting not found" });
	db.getByIdWithIncludesAsync("Meeting", meetingId)
		.then(dbResult => {
			console.log(dbResult);
			if (dbResult.is_success) {
				const filters = [{ parameter: "MeetingId", value: meetingId }];
				db.getAllWhereWithIncludesAsync("MeetingParticipant", filters).then(dbParticipants => {
					if (dbParticipants.is_success) {
						const participants =
							dbParticipants.result
								.map(x => `${x.personIdReference.name} ${x.personIdReference.lastName}`)
								.join(", ");

						const obj = Object.assign({}, dbResult.result);
						obj.participants = participants;
						const content = templateManager.render("template_template_meeting_details", obj);
						block.html(content);
						window.forceTranslate(blockId);
					} else {
						toast.notifyErrorList(result.error_keys);
					}
				}).catch(e => {
					toast.notify({ heading: "Fail" });
				});
			} else {
				toast.notifyErrorList(result.error_keys);
			}
		}).catch(err => {
			toast.notify({ heading: "Fail" });
		});
});