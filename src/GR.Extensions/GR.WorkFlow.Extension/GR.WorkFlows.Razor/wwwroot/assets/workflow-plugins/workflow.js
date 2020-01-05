!function ($) {
    "use strict";
    const stateMachineManager = new stateMachine();
    const toast = new ToastNotifier();
    const connectionModal = $.templates("#coonection-modal");
    const editStateModal = $.templates("#edit-state-modal");
    const editWorkflowModal = $.templates("#edit-workflow-modal");
    const workflowTemplate = $.templates("#workflow-template");
    const workflowContainerTemplate = $.templates("#workflow-container-template");
    const stateTemplate = $.templates("#state-template");
    const checkboxDeleteTemplate = $.templates("#checkboxDeleteTemplate");
    const checkboxDeleteTemplateContract = $.templates("#checkboxDeleteTemplateContract");
    const modal = $('#state-machine-modal');
    let loadedWorkflow = JSON.parse(localStorage.getItem("workflow"));

    const getWorkflowContracts = id => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/WorkFlowExecutor/GetWorkflowContracts',
                type: 'get',
                data: { workFLowId: id },
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

    const removeEntityContractToWorkFlow = (entityName, workFLowId) => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/WorkFlowExecutor/RemoveEntityContractToWorkFlow',
                type: 'delete',
                data: {
                    entityName,
                    workFLowId
                },
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

    const findObjectsByProperties = (array, properties) => {
        return array.filter(obj => {
            let response = true;
            properties.forEach(property => {
                const propertyFound = obj[property.name] === property.value;
                response = response && propertyFound;
            });
            return response;
        });
    }

    const setDataLocalStorage = (name, data = {}) => {
        localStorage.setItem(name, JSON.stringify(data));
    }

    const loadHelpers = () => {
        const rolesPromise = stateMachineManager.loadRoles();
        const actionsPromise = stateMachineManager.loadActions();
        return Promise.all([rolesPromise, actionsPromise]).then(data => {
            return {
                roles: data[0],
                actions: data[1]
            }
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const openConfirmationModal = (entity = {
        id: null,
        title: '',
        type: '',
    }) => {
        const submitBtn = $('.modal-confirm-delete #submit-delete');
        $('#deleteConfirmModal .workflow-title').html(entity.title);
        submitBtn.data('entity', entity);
        submitBtn.on('click', function () {
            switch (entity.type) {
                case 'workflow': {
                    deleteWorkflow(entity.id);
                }
            }

            $('#deleteConfirmModal').modal('hide');
        });

        $('#deleteConfirmModal').modal('show');
    }

    const addLoader = elementDOM => {
        const loadermarkup = `<div class="st-mch-loader">
										<div class="state-machine-loader justify-content-center align-items-center">
										<div class="lds-dual-ring"></div>
											</div>
										</div>`;
        elementDOM.append(loadermarkup);
        elementDOM.find('.st-mch-loader').fadeIn();
    }

    const removeLoader = elementDOM => {
        elementDOM.find('.st-mch-loader').fadeOut();
        setTimeout(function () { elementDOM.find('.st-mch-loader').remove(); }, 3000);
    }

    const openStateModal = stateObj => {
        stateMachineManager.getStateById(stateObj.id).then(state => {
            const htmlOutput = editStateModal.render(state);
            modal.html(htmlOutput);
            attachStateModalActions(state);
            modal.modal('show');
        })
    }

    const attachStateModalActions = (state) => {
        const form = $('#edit-state');
        const checkboxes = $('.start-end-state .custom-checkbox');
        const sendStartEndState = {
            start: state.isStartState,
            end: state.isEndState
        }

        $.each(checkboxes, function (i, checkbox) {
            $(checkbox).find('.custom-control-input').change(function () {
                if ($(this).is(':checked')) {
                    $(checkbox).siblings('.custom-checkbox').addClass('unavailable').prop("checked", false);

                    if ($(this).attr('id') === 'transition_start_state') {
                        sendStartEndState.start = true;
                    }
                    else {
                        sendStartEndState.start = false;
                    }

                    if ($(this).attr('id') === 'transition_end_state') {
                        sendStartEndState.end = true;
                    }
                    else {
                        sendStartEndState.end = false;
                    }
                }
                else {
                    $(checkbox).siblings('.custom-checkbox').removeClass('unavailable');
                }
            });
        });

        form.submit(function (e) {
            e.preventDefault();
            const statetoSend = {
                stateId: state.id,
                newName: form.find('#edit-state-title').val(),
                description: form.find('#edit-state-description').val()
            }
            const requestState = {
                workflowId: state.workFlowId,
                stateId: state.id
            }

            const updateState = () => {
                stateMachineManager.updateStateGeneralInfo(statetoSend).then(() => {
                    modal.modal('hide');
                    refreshWorkflowState(state.id);
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
            }

            if (sendStartEndState.start) {
                stateMachineManager.setStartStateInWorkflow(requestState).then(() => {
                    updateState();
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
            } else if (sendStartEndState.end) {
                stateMachineManager.setEndStateInWorkflow(requestState).then(() => {
                    updateState();
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
            }
            else {
                updateState();
            }
        });

    }

    const refreshWorkflowState = (id) => {
        stateMachineManager.getStateById(id).then(state => {
            const stateElement = $(`.w[data-state-id="${id}"]`);
            stateElement.find('.state-name').text(state.name);
            if (state.isStartState) {
                $('.w').data('isstart', 'false');
                stateElement.data('isstart', 'true');
            } else if (state.isEndState) {
                $('.w').data('isend', 'false');
                stateElement.data('isend', 'true');
            }
            updateStateBoxColors();
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const updateStateBoxColors = () => {
        $('.w .ep').css('background-color', '#ffa500');
        $.each($('.w'), function () {
            if ($(this).data('isstart') == 'true' || $(this).data('isstart') == true) {
                $(this).find('.ep').css('background-color', '#009010');
            }
            else if ($(this).data('isend') == 'true' || $(this).data('isend') == true) {
                $(this).find('.ep').css('background-color', '#FF0000');
            }
        });
    }

    const loadWorkflowStates = workflow => {
        const states = workflow.states;
        $(`.jtk-canvas#${workflow.id}`).html(null);
        $.each(states, function (index, state) {
            appendStateToWorkflow(workflow.id, state);
        });
        initjsPlumb(workflow.id);
    }

    const loadWorkflowTransitions = (id, instance) => {
        stateMachineManager.getWorkFlowById(id).then(workflow => {
            if (workflow.transitions) {
                const transitions = workflow.transitions;
                $.each(transitions, function (index, transition) {
                    instance.connect({
                        id: transition.id,
                        source: transition.fromState.id,
                        target: transition.toState.id,
                        type: "basic",
                        parameters: {
                            'transition': transition
                        }
                    });
                });
                removeLoader($('#state-machine'));
            }
        });
    }

    const getSelectedRolesActionsFromTransition = () => {
        let roles = [];
        modal.find('.roles input:checked').each(function () {
            roles.push($(this).attr('id'));
        });
        let actions = [];
        modal.find('.actions input:checked').each(function () {
            actions.push($(this).attr('id'));
        });
        return {
            roles,
            actions
        }
    }

    const deleteConnection = id => {
        stateMachineManager.removeTransitionById(id).then().catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const openEditModal = id => {
        stateMachineManager.getWorkFlowById(id).then(workflow => {
            const htmlOutput = editWorkflowModal.render(workflow, { id });
            modal.html(htmlOutput);
            $.each(workflow.states, function () {
                appendStateToWorkflowModal(this);
            });
            getWorkflowContracts(id).then(contracts => {
                $.each(contracts, function () {
                    appendContractToWorkflowModal(this);
                });
            });
            attachEditModalActions(id);
            modal.modal('show');
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const attachEditModalActions = id => {
        $('#add-new-state-button').click(function (e) {
            e.preventDefault();
            const state = {
                name: $('#add-new-state-name').val(),
                workflowId: id,
                description: '',
                additionalSettings: JSON.stringify({
                    x: "0px",
                    y: "0px"
                })
            }
            createState(state).then(() => {
                $('#add-new-state-name').val(null);
            });
        });
        $('#add-new-contract-button').click(function (e) {
            e.preventDefault();
            const contract = {
                entityName: $('#add-new-contract-name').val(),
                workflowId: id
            }
            createContract(contract).then(result => {
                $('#add-new-contract-name').val(null);
                contract.id = result;
                appendContractToWorkflowModal(contract);
            });
        });
        $('#edit-workflow').submit(function (e) {
            e.preventDefault();
            const workflow = {
                id,
                name: $('#edit-workflow-title').val(),
                description: $('#edit-workflow-description').val(),
                enabled: $('#edit-workflow-active').is(':checked')
            }
            let refreshCanvas = false;
            if (loadedWorkflow.id === id) {
                refreshCanvas = true;
            }
            updateWorkflow(workflow, refreshCanvas).then(() => modal.modal('hide'));
        });
    }

    const createState = (state = {
        name: '',
        description: '',
        workflowId: '',
        additionalSettings: {
            x: 0,
            y: 0
        }
    }) => {
        return stateMachineManager.addStateToWorkFlow(state).then(stateId => {
            state.id = stateId;
            appendStateToWorkflowModal(state);
            appendStateToWorkflow(state.workflowId, state);
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const createContract = (contract = {
        entityName: '',
        workflowId: ''
    }) => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/WorkFlowExecutor/RegisterEntityContractToWorkFlow',
                type: 'post',
                data: contract,
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

    const appendContractToWorkflowModal = contract => {
        const htmlOutput = checkboxDeleteTemplateContract.render(contract);
        modal.find('.contracts-list').append(htmlOutput);
        attachContractFromModalActions(contract.id);
    }

    const attachContractFromModalActions = id => {
        const scope = $(`.contract-item[data-contract-id="${id}"]`);
        const workflowId = scope.closest('.modal-dialog').data('workflow-id');
        const entityName = scope.data('entity-name');
        scope.find('.contract-delete').click(function (e) {
            e.preventDefault(e);
            scope.addClass('unavailable');
            removeEntityContractToWorkFlow(entityName, workflowId).then(() => {
                scope.remove();
                toast.notify({ text: window.translate("system_state_machine_deleted_success"), icon: "success" });
            }).catch(e => {
                scope.removeClass('unavailable');
                toast.notifyErrorList(e);
            });
        });
    }

    const appendStateToWorkflow = (workflowId, state) => {
        const aSets = state.additionalSettings;
        let helpers = {
            x: "0px",
            y: "0px",
            bgColor: '#ffa500'
        };
        if (aSets && aSets.x) {
            helpers.x = aSets.x;
        }
        if (aSets && aSets.y) {
            helpers.y = aSets.y;
        }
        if (state.isStartState) {
            helpers.bgColor = '#009010';
        }
        if (state.isEndState) {
            helpers.bgColor = '#FF0000';
        }
        const htmlOutput = stateTemplate.render(state, helpers);
        $(`.jtk-canvas[data-workflow-id="${workflowId}"]`).append(htmlOutput);
        $(`.w[data-state-id="${state.id}"] .state-edit`).click(() => {
            openStateModal(state);
        });
    }

    const appendStateToWorkflowModal = state => {
        const htmlOutput = checkboxDeleteTemplate.render(state);
        modal.find('.states-list').append(htmlOutput);
        attachStateFromModalActions(state.id);
    }

    const attachStateFromModalActions = id => {
        const scope = $(`.state-item[data-state-id="${id}"]`);
        const workflowId = scope.closest('.modal-dialog').data('workflow-id');
        scope.find('.state-delete').click(function (e) {
            e.preventDefault(e);
            scope.addClass('unavailable');
            stateMachineManager.removeState(id).then(() => {
                scope.remove();
                if (loadedWorkflow == workflowId) {
                    loadWorkflow(workflowId);
                }
                toast.notify({ text: window.translate("system_state_machine_deleted_success"), icon: "success" });
            }).catch(e => {
                scope.removeClass('unavailable');
                toast.notifyErrorList(e);
            });
        });
    }

    const loadWorkflow = id => {
        $('#state-machine').html(null);
        addLoader($('#state-machine'));
        const workflowlistItem = $(`.workflow[data-workflow-id="${id}"]`);
        workflowlistItem.addClass('active-workflow');
        workflowlistItem.siblings('.workflow').removeClass('active-workflow');
        stateMachineManager.getWorkFlowById(id).then(workflow => {
            workflow.id = id;
            const htmlOutput = workflowContainerTemplate.render(workflow);
            $('#state-machine').append(htmlOutput);
            if (workflow.states.length > 0) {
                loadWorkflowStates(workflow);
            }
            else {
                removeLoader($('#state-machine'));
            }
            setDataLocalStorage('workflow', { id });
            loadedWorkflow = id;
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const addNewWorkflow = workflow => {
        return stateMachineManager.addNewWorkflow(workflow).then(workflowId => {
            workflow.id = workflowId;
            appendWorkflowToList(workflow);
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const updateWorkflow = (workflow, refreshCanvas = false) => {
        return stateMachineManager.updateWorkFlowAsync(workflow).then(() => {
            if (refreshCanvas) {
                loadWorkflow(workflow.id);
            }
            $(`#${workflow.id}.workflow .custom-control-label`).text(workflow.name);
            modal.modal('hide');
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const attachWorkflowItemActions = id => {
        const scope = $(`#${id}.workflow`);
        scope.find('.workflow-load').on('click', () => loadWorkflow(id));
        scope.find('.workflow-edit').on('click', () => openEditModal(id));
        scope.find('.workflow-delete').on('click', () => {
            const entity = {
                id,
                title: scope.find('.workflow-title').text(),
                type: 'workflow'
            }
            openConfirmationModal(entity);
        });
        scope.find('.workflow-list-checkbox').change(function () {
            const isChecked = $(this).is(':checked');
            stateMachineManager.getWorkFlowById(id).then(data => {
                const workflow = {
                    id,
                    enabled: isChecked,
                    name: data.name,
                    description: data.description
                }
                updateWorkflow(workflow, false);
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        });
    }

    const deleteWorkflow = id => {
        $(`#${id}.workflow`).remove();
        stateMachineManager.removeWorkflow(id).then(() => {
            $(`#state-machine #${id}.jtk-canvas`).remove();
            toast.notify({ text: window.translate("system_state_machine_deleted_success"), icon: "success" });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    const appendWorkflowToList = workflow => {
        const htmlOutput = workflowTemplate.render(workflow);
        $('#workflow-list').append(htmlOutput);
        attachWorkflowItemActions(workflow.id);
    }

    $('#add-new-workflow').submit(function (e) {
        e.preventDefault();
        const scope = $(this);
        const workflow = {
            name: scope.find('#workflow-title').val(),
            description: scope.find('#workflow-description').val(),
            enabled: scope.find('#workflow-active').is(':checked')
        }
        addNewWorkflow(workflow).then(() => {
            $('#add-new-workflow')[0].reset();
        });
    });

    stateMachineManager.getAllWorkflows().then(workflows => {
        $('#workflow-list').html(null);
        $.each(workflows, function () {
            appendWorkflowToList(this);
        });
        if (loadedWorkflow) {
            loadWorkflow(loadedWorkflow.id);
        }
        else if (workflows[0]) {
            loadWorkflow(workflows[0].id);
        }
    }).catch(e => {
        toast.notifyErrorList(e);
    });

    const initjsPlumb = (workflowId) => {
        jsPlumb.ready(function () {
            // setup some defaults for jsPlumb.
            var instance = jsPlumb.getInstance({
                Endpoint: ["Dot", { radius: 2 }],
                Connector: "StateMachine",
                HoverPaintStyle: { stroke: "#1e8151", strokeWidth: 2 },
                ConnectionOverlays: [
                    ["Arrow", {
                        location: 1,
                        id: "arrow",
                        length: 14,
                        foldback: 0.8
                    }],
                ],
                Container: "canvas"
            });

            instance.registerConnectionType("basic", { anchor: "Continuous", connector: "StateMachine" });

            window.jsp = instance;
            var windows = jsPlumb.getSelector(`.jtk-canvas[data-workflow-id="${workflowId}"] .w`);

            // bind a click listener to each connection; the connection is deleted. you could of course
            // just do this: instance.bind("click", instance.deleteConnection), but I wanted to make it clear what was
            // happening.
            instance.bind("click", function (c) {
                stateMachineManager.getTransitionById(c.id).then(transition => {
                    modal.html(null);
                    loadHelpers().then(helpers => {
                        $.each(helpers.roles, function () {
                            const properties = [
                                {
                                    name: 'roleId',
                                    value: this.id
                                }
                            ];
                            const matchedRole = findObjectsByProperties(transition.transitionRoles, properties)[0];

                            if (matchedRole) {
                                this.enabled = true;
                            }
                            else {
                                this.enabled = false;
                            }
                        });
                        $.each(helpers.actions, function (index, value) {
                            const properties = [
                                {
                                    name: 'actionId',
                                    value: this.id
                                }
                            ];
                            const matchedAction = findObjectsByProperties(transition.transitionActions, properties)[0];

                            if (matchedAction) {
                                this.enabled = true;
                            }
                            else {
                                this.enabled = false;
                            }
                        });
                        const htmlOutput = connectionModal.render(transition, helpers);
                        modal.append(htmlOutput).modal('show');
                        $('.connection-delete').click(function () {
                            deleteConnection(c.id);
                            instance.deleteConnection(c);
                            modal.append(htmlOutput).modal('hide');
                        });
                        $('#edit-transition').submit(function (e) {
                            e.preventDefault();
                            const selections = getSelectedRolesActionsFromTransition();
                            const connName = $('#edit-transition-title').val();
                            stateMachineManager.addOrUpdateTransitionAllowedRoles({
                                transitionId: c.id,
                                roles: selections.roles
                            });
                            stateMachineManager.addOrUpdateTransitionActions({
                                transitionId: c.id,
                                actionHandlers: selections.actions
                            });
                            stateMachineManager.updateTransitionName({
                                transitionId: c.id,
                                newName: connName
                            });
                            c.setLabel(
                                `<div class="connection-label">${connName}</div>`
                            );
                            modal.modal('hide');
                        });
                    }).catch(e => {
                        toast.notifyErrorList(e);
                    });
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
            });

            instance.bind("beforeDrop", function (conn) {
                const sourcePoint = conn.sourceId;
                const targetPoint = conn.targetId;
                if (sourcePoint === targetPoint) {
                    return false;
                }
                else {
                    const transition = {
                        workflowId,
                        name: window.translate('system_state_machine_new_transition'),
                        fromStateId: sourcePoint,
                        toStateId: targetPoint,
                    }
                    stateMachineManager.createTransition(transition).then(transitionId => {
                        conn.id = transitionId;
                        conn.connection.id = transitionId;
                        stateMachineManager.getTransitionById(conn.id).then(transition => {
                            instance.connect({
                                id: conn.id,
                                source: sourcePoint,
                                target: targetPoint,
                                type: "basic",
                                parameters: {
                                    'transition': transition
                                }
                            });
                        }).catch(e => {
                            toast.notifyErrorList(e);
                        });
                    }).catch(e => {
                        toast.notifyErrorList(e);
                    });
                }

                return false;
            });

            instance.bind("connection", function (info) {
                const transition = info.connection.getParameter('transition');
                if (transition) {
                    info.connection.setLabel(
                        `<div class="connection-label">${transition.name}</div>`
                    );
                    info.connection.id = transition.id;
                }
                else {
                    info.connection.setLabel(
                        `<div class="connection-label">${window.translate('system_state_machine_new_transition')}</div>`
                    );
                }
            });

            //
            // initialise element as connection targets and source.
            //
            var initNode = function (el) {

                // initialise draggable elements.
                instance.draggable(el, {
                    stop: function (e) {
                        if (e.pos[0] < 0) {
                            e.pos[0] = 0;
                        }
                        if (e.pos[1] < 0) {
                            e.pos[1] = 0;
                        }
                        const settings = {
                            x: `${e.pos[0]}px`,
                            y: `${e.pos[1]}px`
                        }
                        stateMachineManager.updateStateAdditionalSettings({ stateId: el.id, settings: JSON.stringify(settings) });
                    }
                });

                instance.makeSource(el, {
                    filter: ".ep",
                    anchor: "Continuous",
                    connectorStyle: { stroke: "#5c96bc", strokeWidth: 2, outlineStroke: "transparent", outlineWidth: 4 },
                    connectionType: "basic",
                    extract: {
                        "action": "the-action"
                    },
                });

                instance.makeTarget(el, {
                    dropOptions: { hoverClass: "dragHover" },
                    anchor: "Continuous",
                    allowLoopback: true
                });

                // this is not part of the core demo functionality; it is a means for the Toolkit edition's wrapped
                // version of this demo to find out about new nodes being added.
                //
                instance.fire("jsPlumbDemoNodeAdded", el);
            };

            // suspend drawing and initialise.
            instance.batch(function () {
                for (var i = 0; i < windows.length; i++) {
                    initNode(windows[i], true);
                }

                // and finally, make a few connections
                loadWorkflowTransitions(workflowId, instance);
            });

            jsPlumb.fire("jsPlumbDemoLoaded", instance);

        });
    }
}(window.jQuery);