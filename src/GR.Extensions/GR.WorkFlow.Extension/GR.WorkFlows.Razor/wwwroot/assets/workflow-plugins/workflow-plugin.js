class stateMachine {
    constructor() {
        const toast = new ToastNotifier();
        this.loadRoles();
        this.loadActions();
    }

    ajaxRequest(requestUrl, requestType, requestData) {
        const baseUrl = '/api/WorkFlowBuilder';
        return new Promise((resolve, reject) => {
            $.ajax({
                url: baseUrl + requestUrl,
                type: requestType,
                data: requestData,
                success: (data) => {
                    if (Array.isArray(data)) {
                        resolve(data);
                    }
                    else {
                        if (data.is_success) {
                            resolve(data.result);
                        } else if (!data.is_success) {
                            reject(data.error_keys);
                        } else {
                            resolve(data);
                        }
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    getAllWorkflows() {
        const requestUrl = '/GetAllWorkflows';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getRolesAllowedToParticipateInWorkflow() {
        const requestUrl = '/GetRolesAllowedToParticipateInWorkflow';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getAllRegisteredActions() {
        const requestUrl = '/GetAllRegisteredActions';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getTransitionById(transitionId) {
        const requestUrl = '/GetTransitionById';
        return this.ajaxRequest(requestUrl, 'get', { transitionId });
    }

    getWorkFlowById(workflowId) {
        const requestUrl = '/GetWorkFlowById';
        return this.ajaxRequest(requestUrl, 'get', { workflowId });
    }

    getStateById(stateId) {
        const requestUrl = '/GetStateById';
        return this.ajaxRequest(requestUrl, 'get', { stateId });
    }

    addNewWorkflow(workflow = {
        name: '',
        description: '',
        enabled: false
    }) {
        const requestUrl = '/AddNewWorkflow';
        return this.ajaxRequest(requestUrl, 'post', workflow);
    }

    addStateToWorkFlow(state = {
        name: '',
        description: '',
        workFlowId: '',
        additionalSettings: {}
    }) {
        const requestUrl = '/AddStateToWorkFlow';
        return this.ajaxRequest(requestUrl, 'post', state);
    }

    createTransition(transition = {
        workFlowId: '',
        name: '',
        fromStateId: '',
        toStateId: ''
    }) {
        const requestUrl = '/CreateTransition';
        return this.ajaxRequest(requestUrl, 'post', transition);
    }

    setStartStateInWorkflow(state = {
        workFlowId: '',
        stateId: ''
    }) {
        const requestUrl = '/SetStartStateInWorkflow';
        return this.ajaxRequest(requestUrl, 'post', state);
    }

    setEndStateInWorkflow(state = {
        workFlowId: '',
        stateId: ''
    }) {
        const requestUrl = '/SetEndStateInWorkflow';
        return this.ajaxRequest(requestUrl, 'post', state);
    }

    addOrUpdateTransitionActions(transition = {
        transitionId: '',
        actionHandlers: ''
    }) {
        const requestUrl = '/AddOrUpdateTransitionActions';
        return this.ajaxRequest(requestUrl, 'post', transition);
    }

    addOrUpdateTransitionAllowedRoles(transition = {
        transitionId: '',
        roles: ''
    }) {
        const requestUrl = '/AddOrUpdateTransitionAllowedRoles';
        return this.ajaxRequest(requestUrl, 'post', transition);
    }

    updateTransitionName(transition = {
        transitionId: '',
        newName: ''
    }) {
        const requestUrl = '/UpdateTransitionName';
        return this.ajaxRequest(requestUrl, 'post', transition);
    }

    enableOrDisableWorkFlow(workflow = {
        workFlowId: '',
        state: false
    }) {
        const requestUrl = '/EnableOrDisableWorkFlow';
        return this.ajaxRequest(requestUrl, 'post', workflow);
    }

    updateWorkFlowAsync(workflow = {
        id: '',
        name: '',
        description: '',
        enabled: false
    }) {
        const requestUrl = '/UpdateWorkFlowAsync';
        return this.ajaxRequest(requestUrl, 'post', workflow);
    }

    updateStateAdditionalSettings(state = {
        stateId: '',
        settings: {}
    }) {
        const requestUrl = '/UpdateStateAdditionalSettings';
        return this.ajaxRequest(requestUrl, 'post', state);
    }

    updateStateGeneralInfo(state = {
        stateId: '',
        newName: '',
        description: '',
    }) {
        const requestUrl = '/UpdateStateGeneralInfo';
        return this.ajaxRequest(requestUrl, 'post', state);
    }

    updateWorkFlowStateAsync(workflow = {
        stateId: '',
        workFlowId: '',
        name: '',
        description: '',
        additionalSettings: {}
    }) {
        const requestUrl = '/UpdateWorkFlowStateAsync';
        return this.ajaxRequest(requestUrl, 'post', workflow);
    }

    removeTransitionById(transitionId) {
        const requestUrl = '/RemoveTransitionById';
        return this.ajaxRequest(requestUrl, 'delete', { transitionId });
    }

    removeState(stateId) {
        const requestUrl = '/RemoveState';
        return this.ajaxRequest(requestUrl, 'delete', { stateId });
    }

    removeWorkflow(workFlowId) {
        const requestUrl = '/RemoveWorkflow';
        return this.ajaxRequest(requestUrl, 'delete', { workFlowId });
    }

    roles = [];

    loadRoles() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.roles.length > 0) {
                resolve(scope.roles);
            } else {
                scope.getRolesAllowedToParticipateInWorkflow().then(result => {
                    scope.roles = result;
                    resolve(scope.roles);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        });
    }

    actions = [];

    loadActions() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.actions.length > 0) {
                resolve(scope.actions);
            } else {
                scope.getAllRegisteredActions().then(result => {
                    scope.actions = result;
                    resolve(scope.actions);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        });
    }
}