!function ($) {
    "use strict";
    const calendar = new Calendar();
    const toast = new ToastNotifier();
    const eventTemplate = $.templates("#eventTemplate");

    var CalendarApp = function () {
        this.$body = $("body"),
            this.$helpers = [],
            this.$calendar = $('#calendar'),
            this.$event = ('#calendar-events div.calendar-events'),
            this.$categoryForm = $('#add-new-event form'),
            this.$extEvents = $('#calendar-events'),
            this.$modal = $('#my-event'),
            this.$saveCategoryBtn = $('.save-category'),
            this.$calendarObj = null
    };

    //popover.prototype.documentMousedown = function (ev) {
    //    // only hide the popover if the click happened outside the popover
    //    // rewrite fullcalendar to do nothing on click outside 
    //};

    /* on drop */
    CalendarApp.prototype.onDrop = function (eventObj, date) {
        let calendarEvent = Object.assign({}, eventObj.data('eventObject'));
        const currentView = this.$calendar.fullCalendar('getView').dateProfile.currentRangeUnit;
        let displayDate = date.clone();
        let requestStartDate = date.clone();
        let requestEndDate = date.clone();

        if (currentView === 'month') {
            const diffDays = displayDate.diff(moment(), 'days');
            if (diffDays === 0) {
                displayDate = moment().local().add(1, 'm');
                requestStartDate = moment().local().add(1, 'm');
                requestEndDate = moment().local().add(61, 'm');
            } else {
                displayDate = displayDate.local().hour(8);
                requestStartDate = requestStartDate.local().hour(8);
                requestEndDate = requestEndDate.local().hour(9);
            }
        } else {
            requestStartDate = requestStartDate.local();
            requestEndDate = requestEndDate.local().add(1, 'h');
        }

        requestStartDate = requestStartDate.local().format('YYYY-MM-DD[T]HH:mm');
        requestEndDate = requestEndDate.local().format('YYYY-MM-DD[T]HH:mm');

        const eventData = {
            title: eventObj.data('eventObject').title,
            details: eventObj.data('event-details'),
            location: eventObj.data('event-location'),
            startDate: requestStartDate,
            endDate: requestEndDate,
            priority: this.$helpers.eventPriority[eventObj.data('event-priority')].systemName,
        }

        if (eventObj.data('event-members').length != 0) {
            eventData.members = eventObj.data('event-members').split(',');
        }

        calendarEvent.start = displayDate;
        calendarEvent.editable = true;
        if (displayDate.isAfter(new Date())) {
            calendar.addEvent(eventData).then(newEventId => {
                calendarEvent.id = newEventId;
                this.$calendar.fullCalendar('renderEvent', calendarEvent, true);
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        } else {
            toast.notify({ text: window.translate("system_calendar_error_date"), icon: "error" });
        }

    },

        /* on click on event */
        CalendarApp.prototype.onEventClick = function (calEvent, jsEvent, view) {
            detailsPopup.show();
        },
        CalendarApp.prototype.enableDrag = function () {
            //init events
            $(this.$event).each(function () {
                // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
                // it doesn't need to have a start or end
                var eventObject = {
                    title: $.trim($(this).find('.draft-event-title').text()) // use the element's text as the event title
                };
                // store the Event Object in the DOM element so we can get to it later
                $(this).data('eventObject', eventObject);
                // make the event draggable using jQuery UI
                $(this).draggable({
                    zIndex: 999,
                    revert: true,      // will cause the event to go back to its
                    revertDuration: 0  //  original position after the drag
                });
            });
        }

    CalendarApp.prototype.eventDrop = function (event, delta, revertFunc) {
        const isAfter = event.start.local().isAfter(moment());
        if (!isAfter) {
            toast.notify({ text: window.translate("system_calendar_error_date"), icon: "error" });
            revertFunc();
        }
        else {
            let endDate = null;

            if (event.end !== null) {
                endDate = event.end;
            } else {
                endDate = event.start;
            }

            const systemEvent = {
                id: event.id,
                title: event.title,
                startDate: event.start.format('YYYY-MM-DD'),
                endDate: endDate.format('YYYY-MM-DD')
            }
            calendar.updateEvent(systemEvent).then();
        }
    }

    CalendarApp.prototype.eventRender = function (event, element, view) {
        const time = event.start.local().format('hha');
        const htmlOutput = eventTemplate.render({
            id: event.id,
            time: time,
            title: event.title,
            editable: event.editable
        });
        element.html(htmlOutput);
    }
    /* Initializing */
    CalendarApp.prototype.init = function (helpers) {
        this.$helpers = helpers;
        this.enableDrag();
        /*  Initialize the calendar  */
        let date = new Date();
        let $this = this;

        let getConfig = {
            userId: settings.user.id,
            timeLyneType: 'month',
            expandDayPrecision: 6
        }

        let events = [];
        return calendar.getUserEventsByTimeLine(getConfig).then(result => {
            $.each(result.events, function () {
                const scope = this;

                let editable = false;

                if (scope.organizerInfo.id === getConfig.userId) {
                    editable = true;
                }

                events.push({
                    id: scope.id,
                    title: scope.title,
                    start: scope.startDate,
                    end: scope.endDate,
                    description: scope.details,
                    editable: editable
                });
            });

            $this.$calendarObj = $this.$calendar.fullCalendar({
                locale: settings.localization.current.identifier,
                slotDuration: '00:15:00', /* If we want to split day time each 15minutes */
                minTime: '08:00:00',
                maxTime: '19:00:00',
                defaultView: 'month',
                handleWindowResize: true,
                fixedWeekCount: false,
                firstDay: 1,
                header: {
                    left: 'month,agendaWeek,agendaDay',
                    center: 'title',
                    right: 'prev,next today'
                },
                events: events,
                eventRender: function (event, element, view) {
                    $this.eventRender(event, element, view);
                },
                height: 800,
                allDaySlot: false,
                editable: true,
                droppable: true, // this allows things to be dropped onto the calendar !!!
                eventLimit: 5, // allow "more" link when too many events
                selectable: true,
                drop: function (date) { $this.onDrop($(this), date); },
                eventDrop: function (event, delta, revertFunc) {
                    $this.eventDrop(event, delta, revertFunc);
                },
                slotLabelFormat: 'h:mma'
            });

            //on new event
            this.$saveCategoryBtn.on('click', function () {
                var categoryName = $this.$categoryForm.find("input[name='category-name']").val();
                var categoryColor = $this.$categoryForm.find("select[name='category-color']").val();
                if (categoryName !== null && categoryName.length != 0) {
                    $this.$extEvents.append('<div class="calendar-events bg-' + categoryColor + '" data-class="bg-' + categoryColor + '" style="position: relative;"><i class="fa fa-move"></i>' + categoryName + '</div>')
                    $this.enableDrag();
                }

            });
        });


    },

        //init CalendarApp
        $.CalendarApp = new CalendarApp, $.CalendarApp.Constructor = CalendarApp

}(window.jQuery),

    //initializing CalendarApp
    function ($) {
        "use strict";
        const calendar = new Calendar();
        const toast = new ToastNotifier();
        let draftEvents = [];

        const templates = {
            eventPopup: $.templates("#eventPopup"),
            eventPopupEdit: $.templates("#eventPopupEdit"),
            teamMember: $.templates("#teamMember"),
            teamMemberEdit: $.templates("#teamMemberEdit"),
            eventDraftList: $.templates("#eventDraftList"),
            editDraftEvent: $.templates("#editDraftEvent"),
            eventDraftPopupEdit: $.templates("#eventDraftPopupEdit"),
        }
        const detailsPopup = $('.details-popup');
        let helpers = [];
        detailsPopup.hide();
        loadDraftEvents();

        $('#toggle-forms').on('click', function () {
            $('#add-event').slideDown();
            $('#edit-event').slideUp();
        });

        $('#add-event').submit(function (e) {
            e.preventDefault();
            const scope = $(this);
            const eventObject = {
                title: scope.find('.event-title').val(),
                details: scope.find('.event-details').val(),
                location: scope.find('.event-location').val(),
                priority: scope.find('.event-priority').val(),
                members: scope.find('.event-members').val()
            }

            const draftObject = Object.assign({}, eventObject);
            draftObject.draftId = uniqueDraftId();

            const htmlOutput = templates.eventDraftList.render(draftObject);
            $('#calendar-events').append(htmlOutput);
            draftEvents.push(draftObject);
            setEventsLocalStorage(draftEvents);
            $.CalendarApp.enableDrag();
            $(this)[0].reset();
            select2Refresh([$('.event-priority'), $('.event-members')]);
            attachActionsDraftEvents();
        });

        $(document).ready(function () {
            loadHelpers();
        });

        function loadDraftEvents() {
            $('#calendar-events').html(null);
            draftEvents = localStorage.getItem("draftEvents");
            draftEvents = JSON.parse(draftEvents);
            $.each(draftEvents, function () {
                const htmlOutput = templates.eventDraftList.render(this);
                $('#calendar-events').append(htmlOutput);
            });
            if (draftEvents === null) {
                draftEvents = [];
            }
            attachActionsDraftEvents();
        }

        function calendarRequest(request) {
            return request.then(result => {
                return result;
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        }

        function loadHelpers() {
            calendarRequest(calendar.getOrganizationUsers()).then(users => {
                const selectMembersConfig = {
                    options: users,
                    selectTarget: $('#add-event .event-members'),
                    selectedValues: [],
                    isSelect2: true
                }
                fillMembersSelect(selectMembersConfig);

                calendarRequest(calendar.getHelpers()).then(data => {
                    helpers = data;
                    helpers.users = users;
                    const selectConfig = {
                        options: helpers.eventPriority,
                        selectTarget: $('#add-event .event-priority'),
                        selectedValues: null,
                        translatable: true,
                        isSelect2: true
                    }
                    fillSelect(selectConfig);
                    initCalendar();
                });
            });
        }

        function fillSelect(selectConfig) {
            selectConfig.selectTarget.html(null);
            $.each(selectConfig.options, function (index, value) {
                let text = value.systemName;
                let defaultSelected = false;
                let selected = false;
                if (selectConfig.translatable) {
                    text = window.translate(value.translationKey);
                }
                if (selectConfig.selectedValues) {
                    if ($.inArray(index.toString(), selectConfig.selectedValues.toString()) != -1) {
                        defaultSelected = true;
                        selected = true;
                    }
                }
                const newOption = new Option(text, index, defaultSelected, selected);
                selectConfig.selectTarget.append(newOption);
            });
            window.forceTranslate(selectConfig.selectTarget);
            if (selectConfig.isSelect2) {
                selectConfig.selectTarget.select2();
            }
        }

        /**
         * /
         * selectMembersConfig {
         *	selectTarget : <DOMElement> //seelct target
         *	options: <array> //array of options,
         *	selectedValues: <array> //array of selected options(id-s),
         *	disabledValues: <array> //array of disabled options(id-s),
         *	isSelect2: <bool> //if select element is select2
         *	additionalNone: <bool> //if true will add additional none value element "Add a guest"
         * }
         */
        function fillMembersSelect(selectMembersConfig) {
            selectMembersConfig.selectTarget.html(null);
            $.each(selectMembersConfig.options, function (index, value) {
                let text = `${value.firstName} ${value.lastName}`;
                let defaultSelected = false;
                let selected = false;
                if (value.firstName === null || value.lastName === null) {
                    text = value.userName
                }
                if (selectMembersConfig.selectedValues) {
                    if ($.inArray(value.id, selectMembersConfig.selectedValues) != -1) {
                        selected = true;
                    }
                }
                const newOption = new Option(text, value.id, defaultSelected, selected);
                selectMembersConfig.selectTarget.append(newOption);

                if (selectMembersConfig.disabledValues) {
                    if ($.inArray(value.id, selectMembersConfig.disabledValues) != -1) {
                        selectMembersConfig.selectTarget.find(`option[value="${value.id}"]`).prop('disabled', true);
                    }
                }
            });
            if (selectMembersConfig.additionalNone) {
                selectMembersConfig.selectTarget.prepend(new Option(window.translate('system_calendar_add_guest'), 'none', true, true));
            }
            if (selectMembersConfig.isSelect2) {
                selectMembersConfig.selectTarget.select2();
            }
        }

        function initCalendar() {
            $.CalendarApp.init(helpers).then(() => {
                $('.calendar-controls').prepend($('.fc-toolbar'));
                $('.fc-today-button').insertAfter($('.fc-prev-button'));
                $('.fc-toolbar').addClass('row align-items-end');
                $('.fc-toolbar .fc-left').addClass('col-md-3 text-left');
                $('.fc-toolbar .fc-center').addClass('col text-left').insertAfter($('.fc-toolbar .fc-left'));
                $('.fc-toolbar .fc-right').addClass('col-auto');
                eventOnClick();
                viewOnClick();
            });
        }

        function eventOnClick() {
            $(document).on('click', function (e) {
                //e.preventDefault();
                let target = $(e.target);

                const isEvent = target.is('.fc-event');
                const isEventElement = target.is('.fc-event *');
                const isPopup = target.is('.details-popup');
                const isPopupElement = target.is('.details-popup *');
                const isPopupAction = target.is('.details-popup .event-edit i') || target.is('.details-popup .event-delete i');
                const isSelect = target.is('.select2-container *');
                const isDatepicker = target.is('.datepicker *') || target.is('.datepicker');
                const isClockpicker = target.is('.clockpicker-popover *') || target.is('.clockpicker-popover');
                const popupCase = isPopup || isPopupElement || isSelect || isDatepicker || isClockpicker;
                if (isEvent) {
                    addPopup(target, 'eventPopup');
                } else if (isEventElement) {
                    addPopup(target.closest('.fc-event'), 'eventPopup');
                } else if (!isPopupAction && popupCase) {
                    return;
                } else {
                    $('.details-popup').data('visibility', 'hidden').hide();
                }
            });
        }

        function viewOnClick() {
            let getConfig = {
                userId: settings.user.id,
                timeLineType: 'month',
                expandDayPrecision: 6
            }

            let originDateDirection = 'next';

            $('.fc-header-toolbar .fc-left button').on('click', function () {
                const currentView = $.CalendarApp.$calendar.fullCalendar('getView');
                getConfig.timeLineType = currentView.dateProfile.currentRangeUnit;
            });

            $('.fc-header-toolbar .fc-prev-button').on('click', function () {
                originDateDirection = 'prev';
                loadEvents(getConfig, originDateDirection);
            });

            $('.fc-header-toolbar .fc-next-button').on('click', function () {
                loadEvents(getConfig, originDateDirection);
            });
        }

        function loadMembersInPopup(membersList, template) {
            const loadPlace = $('.details-popup-members-list');
            loadPlace.html(null);
            if (membersList.length > 0) {
                loadPlace.closest('.details-popup-members').show();
                $.each(membersList, function (index, value) {
                    appendMemberToList(value, loadPlace, template);
                });
            }
            else {
                if (template === 'teamMember') {
                    loadPlace.html(window.translate('system_calendar_no_members'));
                }
            }
            window.forceTranslate($('.details-popup-members-list'));
        }

        function appendMemberToList(value, loadPlace, template) {
            let htmlOutput = null;
            let acceptance = '';
            let outputName = `${value.firstName} ${value.lastName}`;

            switch (value.acceptance) {
                case 0: {
                    acceptance = 'check_circle_outline';
                    break;
                };
                case 1: {
                    acceptance = 'help_outline';
                    break;
                };
                case 2: {
                    acceptance = 'block';
                    break;
                };
            }

            if (value.firstName === null || value.lastName === null) {
                outputName = value.userName;
            }

            const renderConfig = {
                outputName: outputName,
                acceptanceIcon: acceptance
            }

            if (template === 'teamMember') {
                htmlOutput = templates.teamMember.render(value, renderConfig);
            } else {
                htmlOutput = templates.teamMemberEdit.render(value, renderConfig);
            }
            loadPlace.append(htmlOutput);
        }

        function attachEventsPopupActions(target) {
            $('.details-popup .event-edit').on('click', function (e) {
                e.preventDefault();
                $('.details-popup').data('visibility', 'null');
                addPopup(target, 'eventPopupEdit');
            });
            $('.details-popup .event-delete').on('click', function (e) {
                const eventId = $(this).data('event-id');
                e.preventDefault();
                $('.details-popup').data('visibility', 'null');

                calendar.deletePermanently({ eventId }).then(() => {
                    $('.details-popup').hide();
                    $.CalendarApp.$calendar.fullCalendar('removeEvents', [eventId]);
                });
            });
        }

        function addPopup(targetPopup, template) {
            const eventId = targetPopup.children('.fc-content').data('event-id');
            const editable = targetPopup.children('.fc-content').data('event-editable');
            let htmlOutput = null;
            if ($('.details-popup').data('visibility') === `visible-${eventId}`) {
                return;
            }
            calendarRequest(calendar.getEventById({ eventId: eventId })).then(event => {
                let renderHelpers = {
                    startDate: moment(event.startDate, "YYYY-MM-DDTHH:mm:ss").format('MMM D, YYYY'),
                    endDate: moment(event.endDate, "YYYY-MM-DDTHH:mm:ss").format('MMM D, YYYY'),
                    startDateEdit: moment(event.startDate, "YYYY-MM-DDTHH:mm:ss").format('MMM D, YYYY'),
                    endDateEdit: moment(event.endDate, "YYYY-MM-DDTHH:mm:ss").format('MMM D, YYYY'),
                    startTime: moment(event.startDate, "YYYY-MM-DDTHH:mm:ss").format('HH:mm'),
                    endTime: moment(event.endDate, "YYYY-MM-DDTHH:mm:ss").format('HH:mm'),
                    priority: window.translate(helpers.eventPriority[event.priority].translationKey),
                    editable: editable,
                };

                if (template != 'eventPopup') {
                    htmlOutput = templates.eventPopupEdit.render(event, renderHelpers);
                    $('.details-popup').html(htmlOutput);

                    $('.date-inline-editing').datepicker({
                        autoclose: true,
                        format: "M d, yyyy",
                        weekStart: "1",
                        todayHighlight: true,
                        startDate: new Date(),
                        maxViewMode: 2
                    });
                    $('.time-inline-editing').clockpicker();

                    loadMembersInPopup(event.invitedUsers, 'teamMemberEdit');
                    const selectMembersConfig = {
                        options: helpers.users,
                        selectTarget: $('.details-popup .event-members'),
                        selectedValues: [],
                        disabledValues: [],
                        isSelect2: true,
                        additionalNone: true
                    }

                    $.each($('.details-popup .event-member'), function () {
                        const memberId = $(this).data('member-id');
                        selectMembersConfig.options = selectMembersConfig.options.filter(member => member.id !== memberId);
                    });

                    fillMembersSelect(selectMembersConfig);

                    $('.details-popup .event-members').val('none').trigger('change');
                    attachMemberDelete();
                    attachMemberChange();

                } else {
                    htmlOutput = templates.eventPopup.render(event, renderHelpers);
                    $('.details-popup').html(htmlOutput);
                    loadMembersInPopup(event.invitedUsers, 'teamMember');
                    attachEventsPopupActions(targetPopup);
                    attachAcceptanceActions();
                }

                let popper = new Popper(targetPopup, $('.details-popup'), {
                    placement: 'right',
                    arrow: {
                        enabled: true,
                        element: '.popup__arrow'
                    }
                });

                const selectConfig = {
                    options: helpers.eventPriority,
                    selectTarget: $('.details-popup .event-priority'),
                    selectedValues: [event.priority],
                    translatable: true,
                    isSelect2: true
                }

                fillSelect(selectConfig);

                attachSelect2DropdownClass();

                $('.details-popup-priority .event-priority').on('change', function () {
                    const allPriorities = 'priority-0 priority-1 priority-2 priority-3';
                    const priority = $(this).val();
                    $('.details-popup .event-priority-bullet').removeClass(allPriorities).addClass(`priority-${priority}`);
                });

                $('#edit-event').submit(function (e) {
                    e.preventDefault();
                    $('.details-popup').data('visibility', 'hidden').hide();
                    const scope = '#edit-event';
                    const startDateValue = $(`${scope} .event-start-date`).val();
                    const startDate = moment(startDateValue, 'MMM D, YYYY').format('YYYY-MM-DD');
                    const startTime = $(`${scope} .event-start-time`).val();
                    const endDateValue = $(`${scope} .event-end-date`).val();
                    const endDate = moment(endDateValue, 'MMM D, YYYY').format('YYYY-MM-DD');
                    const endTime = $(`${scope} .event-end-time`).val();
                    const event = {
                        id: $(`${scope} .submit-edit-event`).data('event-id'),
                        title: $(`${scope} .event-title`).val(),
                        startDate: `${startDate}T${startTime}`,
                        endDate: `${endDate}T${endTime}`,
                        details: $(`${scope} .event-details`).val(),
                        priority: $(`${scope} .event-priority`).val(),
                        members: []
                    }

                    $.each($('.details-popup .event-member'), function () {
                        const memberId = $(this).data('member-id');
                        event.members.push(memberId);
                    });
                    updateEvent(event);
                });

                window.forceTranslate($('.details-popup'));
                $('.details-popup').data('visibility', `visible-${eventId}`).show();
            });
        }

        function attachSelect2DropdownClass() {
            $('.details-popup .select2').on('click', function () {
                $('.select2-dropdown').addClass('edit-event-select2');
            });
        }

        function attachActionsDraftEvents() {
            $('#calendar-events .calendar-events .draft-event-edit').off().on('click', function () {
                $('#edit-event .form-content').html(null);
                const draftEvent = $(this).closest('.calendar-events');
                const event = {
                    draftId: draftEvent.data('draftid'),
                    title: draftEvent.data('event-title'),
                    details: draftEvent.data('event-details'),
                    location: draftEvent.data('event-location'),
                    priority: draftEvent.data('event-priority'),
                    members: draftEvent.data('event-members').split(',')
                }
                const htmlOutput = templates.editDraftEvent.render(event);
                $('#edit-event .form-content').append(htmlOutput);

                let selectMembersConfig = {
                    options: helpers.users,
                    selectTarget: $('#edit-event .event-members'),
                    selectedValues: [],
                    isSelect2: true
                }

                if (event.members.length > 1) {
                    $.each(event.members, function () {
                        selectMembersConfig.selectedValues.push(this);
                    });
                }

                fillMembersSelect(selectMembersConfig);

                const selectConfig = {
                    options: helpers.eventPriority,
                    selectTarget: $('#edit-event .event-priority'),
                    selectedValues: [event.priority],
                    translatable: true,
                    isSelect2: true
                }
                fillSelect(selectConfig);


                select2Refresh([$('.event-priority'), $('.event-members')]);

                updateFormSubmitJsAction();

                $('#add-event').slideUp();
                $('#edit-event').slideDown();
            });
            $('#calendar-events .calendar-events .draft-event-delete').off().on('click', function () {
                const draftId = $(this).closest('.calendar-events').data('draftid');
                draftEvents = draftEvents.filter(function (event) {
                    return event.draftId !== draftId;
                });
                setEventsLocalStorage(draftEvents);
                $(this).closest('.calendar-events').remove();
            });
        }

        function attachAcceptanceActions() {
            $('.details-popup .respond-event').on('click', function () {
                const eventId = $(this).data('event-id');
                const acceptance = $(this).data('acceptance');
                const memberId = settings.user.id;
                calendar.changeMemberEventAcceptance({
                    eventId,
                    memberId,
                    acceptance
                }).then(() => {
                    toast.notify({ text: window.translate("system_calendar_respond_success"), icon: "success" });
                    $('.details-popup').data('visibility', 'hidden').hide();
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
            });
        }

        function updateFormSubmitJsAction() {
            $('#edit-event').off().submit(function (e) {
                e.preventDefault();
                const scope = $(this);
                const draftObject = {
                    draftId: scope.find('#edit-form-submit').data('draftid'),
                    title: scope.find('.event-title').val(),
                    details: scope.find('.event-details').val(),
                    location: scope.find('.event-location').val(),
                    priority: scope.find('.event-priority').val(),
                    members: scope.find('.event-members').val()
                }

                const htmlOutput = templates.eventDraftList.render(draftObject);
                $(`.calendar-events[data-draftid="${draftObject.draftId}"]`).replaceWith(htmlOutput);

                var foundIndex = draftEvents.findIndex(x => x.draftId == draftObject.draftId);
                draftEvents[foundIndex] = Object.assign({}, draftObject);
                setEventsLocalStorage(draftEvents);

                $.CalendarApp.enableDrag();
                $(this)[0].reset();
                attachActionsDraftEvents();
                scope.slideUp();
            });
        }

        function findObjectByProperty(array, property, propertyValue) {
            return array.filter(obj => { return obj[property] === propertyValue })[0];
        }

        function setEventsLocalStorage(events) {
            localStorage.setItem("draftEvents", JSON.stringify(events));
        }

        function uniqueDraftId() {
            return '_' + Math.random().toString(36).substr(2, 9);
        }

        function select2Refresh(targets) {
            $.each(targets, function () {
                $(this).select2();
            });
        }

        function loadEvents(config, originDirection) {

            let getConfig = config;
            const currentView = $.CalendarApp.$calendar.fullCalendar('getView');
            getConfig.origin = currentView.dateProfile.date.format("MM/DD/YYYY");


            switch (getConfig.timeLineType) {
                case 'week':
                case 'day': {
                    getConfig.expandDayPrecision = 0;
                    break;
                } default: {
                    getConfig.expandDayPrecision = 6;
                }
            }

            calendar.getUserEventsByTimeLine(getConfig).then(result => {
                $.each(result.events, function () {
                    const scope = this;
                    const existingEvent = $.CalendarApp.$calendar.fullCalendar('clientEvents', scope.id);
                    if (existingEvent.length === 0) {
                        let editable = false;
                        if (scope.organizerInfo.id === getConfig.userId) {
                            editable = true;
                        }
                        const event = {
                            id: scope.id,
                            title: scope.title,
                            start: scope.startDate,
                            end: scope.endDate,
                            description: scope.details,
                            editable: editable
                        }
                        $.CalendarApp.$calendar.fullCalendar('renderEvent', event, true);
                    }
                });
            });
        }

        function updateEvent(event) {
            calendar.updateEvent(event).then(() => {
                refreshEvent(event.id);
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        }

        function refreshEvent(eventId) {
            $.CalendarApp.$calendar.fullCalendar('removeEvents', eventId);
            calendar.getEventById({ eventId }).then(event => {
                const eventRender = {
                    id: event.id,
                    title: event.title,
                    start: event.startDate,
                    end: event.endDate,
                    description: event.details,
                    editable: true
                }
                $.CalendarApp.$calendar.fullCalendar('renderEvent', eventRender, true);
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        }

        function attachMemberDelete() {
            $('.details-popup .member-dismiss').off().on('click', function (e) {
                e.stopPropagation();
                const memberId = $(this).closest('.event-member').data('member-id');
                const member = findObjectByProperty(helpers.users, 'id', memberId);
                let memberName = `${member.firstName} ${member.lastName}`;
                if (member.firstName === null || member.lastName === null) {
                    memberName = member.userName
                }
                $('.details-popup .event-members').append(new Option(memberName, member.id));
                $('.details-popup .event-members').select2();
                attachSelect2DropdownClass();
                $(this).closest('.event-member').remove();
            });
        }

        function attachMemberChange() {
            $('.details-popup .event-members').change(function () {
                const memberId = $(this).val();
                if (memberId !== 'none' && memberId !== null) {
                    const value = findObjectByProperty(helpers.users, 'id', memberId);
                    value.acceptance = 1;
                    appendMemberToList(value, $('.details-popup-members-list'), 'teamMemberEdit');
                    attachMemberDelete();

                    $(this).find(`option[value="${memberId}"]`).remove();
                    $(this).val('none').trigger('change');
                    attachSelect2DropdownClass();
                }
            });
        }

    }(window.jQuery);