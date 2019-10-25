
class Calendar {
    ajaxRequest(requestUrl, requestType, requestData) {
        const baseUrl = '/api/Calendar';
        return new Promise((resolve, reject) => {
            $.ajax({
                url: baseUrl + requestUrl,
                type: requestType,
                data: requestData,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else if (!data.is_success) {
                        reject(data.error_keys);
                    } else {
                        resolve(data);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    addEvent(event = {
        title: '',
        details: '',
        location: '',
        startDate: '',
        endDate: '',
        priority: '',
        members: []
    }) {
        const requestUrl = '/AddEvent';
        return this.ajaxRequest(requestUrl, 'post', event);
    }

    updateEvent(event = {
        id: '',
        title: '',
        details: '',
        location: '',
        startDate: '',
        endDate: '',
        priority: '',
        members: []
    }) {
        const requestUrl = '/UpdateEvent';
        return this.ajaxRequest(requestUrl, 'post', event);
    }

    changeMemberEventAcceptance(event = {
        eventId: '',
        memberId: '',
        acceptance: ''
    }) {
        const requestUrl = '/ChangeMemberEventAcceptance';
        return this.ajaxRequest(requestUrl, 'post', event);
    }

    getAllEventsOrganizedByMe() {
        const requestUrl = '/GetAllEventsOrganizedByMe';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getOrganizationUserEvents(config = {
        userId: '',
        startDate: '',
        endDate: ''
    }) {
        const requestUrl = '/GetOrganizationUserEvents';
        return this.ajaxRequest(requestUrl, 'get', config);
    }

    getMyEvents(config = {
        startDate: '',
        endDate: ''
    }) {
        const requestUrl = '/GetMyEvents';
        return this.ajaxRequest(requestUrl, 'get', config);
    }

    getUserEventsByTimeLine(config = {
        userId: '',
        timeLineType: 'month',
        origin: '',
        expandDayPrecision: 0
    }) {
        const requestUrl = '/GetUserEventsByTimeLine';
        return this.ajaxRequest(requestUrl, 'get', config);
    }

    getEventById(config = {
        eventId: '',
    }) {
        const requestUrl = '/GetEventById';
        return this.ajaxRequest(requestUrl, 'get', config);
    }

    getHelpers() {
        const requestUrl = '/Helpers';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getOrganizationUsers() {
        const requestUrl = '/GetOrganizationUsers';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    deletePermanently(config = {
        eventId: ''
    }) {
        const requestUrl = '/DeletePermanently';
        return this.ajaxRequest(requestUrl, 'delete', config);
    }

    deleteLogically(config = {
        eventId: ''
    }) {
        const requestUrl = '/DeleteLogically';
        return this.ajaxRequest(requestUrl, 'delete', config);
    }

    restore(config = {
        eventId: ''
    }) {
        const requestUrl = '/Restore';
        return this.ajaxRequest(requestUrl, 'delete', config);
    }

    helpers = {};
    loadHelpers() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.helpers.length > 0) {
                resolve(scope.helpers);
            } else {
                scope.getHelpers().then(result => {
                    scope.helpers = result;
                    resolve(scope.helpers);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        })
    }
}