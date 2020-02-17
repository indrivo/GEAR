/* eslint-disable no-undef */
$(function () {
    const manager = new TaskManager();
    const toast = new ToastNotifier();
    const taskDetailsTemplate = $.templates("#taskDetailsTemplate");
    const taskEditTemplate = $.templates("#taskEditTemplate");
    const taskListTemplate = $.templates("#taskListTemplate");
    const taskItemsTemplate = $.templates("#taskItemsTemplate");
    const taskItemsAddTaskTemplate = $.templates("#taskItemsAddTaskTemplate");

    const maxFileSize = 10485760;
    let statuses = [];
    let priorities = [];
    let users = [];
    const sortableTableHeaders = $('.task-manager .task-manager-task-list th.sortable');

    let tableProperties = {
        type: 'active',
        page: 1,
        pageSize: 10,
        descending: false,
        attribute: 'StartDate'
    }

    const promises = [
        manager.getStatusesList(),
        manager.getPrioritiesList(),
        manager.getUsersLoadedList()
    ];

    Promise.all(promises).then(data => {
        statuses = data[0];
        priorities = data[1];
        users = data[2];

        statuses[0].translateKey = "system_taskmanager_notstarted";
        statuses[1].translateKey = "system_taskmanager_inprogress";
        statuses[2].translateKey = 'system_taskmanager_review';
        statuses[3].translateKey = 'system_taskmanager_completed';

        priorities[0].translateKey = 'system_taskmanager_low';
        priorities[1].translateKey = 'system_taskmanager_medium';
        priorities[2].translateKey = 'system_taskmanager_high';
        priorities[3].translateKey = 'system_taskmanager_critical';

        fillLoadedLists();
        dependentInputTeam()
        loadTaskList(tableProperties);
    });

    $('.task-manager .task-manager-items-per-page-control').on('change', function () {
        const tablePropertiesLocal = tableProperties;
        tablePropertiesLocal.pageSize = $(this).val();
        tablePropertiesLocal.page = 1;
        loadTaskList(tablePropertiesLocal);
    });

    $('.task-manager-add-new-task').on('click', function () {
        $('.initial-tab-click').trigger('click');
        $('.add-task-uploaded-files .file-list').html('');
        $('.modal-add-task .file-list').html('');
        addChangeFileInputEvent('add');
        const d = new Date();
        const month = d.getMonth() + 1;
        const day = d.getDate();
        const time = d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
        $('#add-task-start-date, #add-task-end-date').val(time);
    });

    $('#add-task').submit(function (e) {
        e.preventDefault();

        const scope = $(this);
        let taskItemsForm = [];
        let formData = new FormData();
        let fileCount = 0;

        $.each($('#addTaskModal .file-list-item:not(.deleting)'), function () {
            let fileId = $(this).find('.file-name').data('file-id');
            let file = $('#addTaskModal .task-files')[0].files[fileId];
            formData.append("files", file);
        });

        $.each($('#addTaskModal .task-item-name'), function () {
            taskItemsForm.push({ Name: $(this).data('task-name') });
        });

        let task = {
            taskItems: taskItemsForm,
            name: scope.find('#add-task-name').val(),
            description: scope.find('#add-task-description').val(),
            startDate: scope.find('#add-task-start-date').val(),
            endDate: scope.find('#add-task-end-date').val(),
            userId: scope.find('#add-task-users').val(),
            userTeam: scope.find('#add-task-team').val(),
            taskPriority: scope.find('#add-task-priority').val(),
            status: scope.find('#add-task-status').val()
        };

        for (let pair of formData.entries()) {
            fileCount++;
        }

        if (fileCount === 0) {
            task.files = [];
            sendAddTaskObject(task);
        } else {
            uploadMultipleFiles(formData).then(result => {
                task.files = result;
                sendAddTaskObject(task);
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        }
    });

    $('#add-task-item-add-task').submit(function (e) {
        e.preventDefault();
        const name = $(this).find('#new-task-item-name-add-task').val();
        const htmlOutput = taskItemsAddTaskTemplate.render({ name: name });
        $('#add-task-items-tab .task-items-add-task').append(htmlOutput);
        $('.delete-task-item-add-task').off().on('click', function (e) {
            e.preventDefault();
            $(this).closest('.task-item').remove();
        });
        $(this).find('#new-task-item-name-add-task').val('');
        addTaskItemsEvents(null);
    });

    $('.task-manager .task-manager-task-types').on('change', function () {
        tableProperties.type = $(this).val();
        tableProperties.page = 1;
        loadTaskList(tableProperties);
    });

    sortableTableHeaders.on('click', function () {
        let descendingVal = false;
        const attributeVal = $(this).data('attribute');
        cleanTableSort();
        if ($(this)[0].hasAttribute('data-sort')) {
            if ($(this).data('sort') === 'descending') {
                descendingVal = false;
                $(this).data('sort', 'ascending');
                $(this).children('.sort').addClass('ascending');
            } else if ($(this).data('sort') === 'ascending') {
                descendingVal = true;
                $(this).data('sort', 'descending');
                $(this).children('.sort').addClass('descending');
            }
        }
        else {
            $(this).attr('data-sort', 'ascending');
            $(this).children('.sort').addClass('ascending');
            descendingVal = false;
        }

        let tablePropertiesLocal = tableProperties;
        tablePropertiesLocal.descending = descendingVal;
        tablePropertiesLocal.attribute = attributeVal;
        loadTaskList(tablePropertiesLocal);
    });

    function addChangeFileInputEvent(type) {
        $('.task-files').off().on('change', function () {
            const scope = this;
            const wrapper = $(scope).closest('.form-group').find('.file-list');
            const files = getFilesFromInput($(scope));
            if (type === 'add') {
                wrapper.html('');
                for (let i = 0; i < files.length; i++) {
                    if (files[i].size > maxFileSize) {
                        alert("One of files is too big!(Max " + maxFileSize / 1048576 + "MB allowed)");
                        scope.value = "";
                    } else {
                        fileListAppend(files[i].name, wrapper, i);
                    }
                }
            } else if (type === 'edit') {
                if (files[0].size > maxFileSize) {
                    alert("File is too big!(Max " + maxFileSize / 1048576 + "MB allowed)");
                    scope.value = "";
                }
                else {
                    let formData = new FormData();
                    formData.append("files", files[0]);
                    uploadFile(formData).then(fileId => {
                        fileListAppend(files[0].name, wrapper, fileId);
                    });
                }
            }
        });
    }

    function loadEachType(promise, objectConf) {
        $('.task-manager-loader').fadeIn();
        return promise.then(result => {
            $("#task-list-table").html(window.translate('system_taskmanager_no_tasks'));
            $.each(result.result, function () {
                objectConf.user = findUserById(this.userId);
                const htmlOutput = taskListTemplate.render(result.result, objectConf);
                $("#task-list-table").html(htmlOutput);
            });
            addTablePager(result.pageCount, result.currentPage);
            addTaskEvents();
            window.forceTranslate();
            $('.task-manager-loader').fadeOut();
        }).catch(e => {
            console.log(e);
            toast.notifyErrorList(e);
        });
    }

    function loadTaskList(tablePropertiesLocal) {
        tablePropertiesLocal.deleted = false;
        let promise = null;
        let configuration = {
            deleted: false,
            statusesList: statuses,
            prioritiesList: priorities
        }
        if (tablePropertiesLocal.type === "active") {
            promise = manager.getUserTasks(tablePropertiesLocal);
        } else if (tablePropertiesLocal.type === "deleted") {
            configuration.deleted = true;
            tablePropertiesLocal.deleted = true;
            promise = manager.getUserTasks(tablePropertiesLocal);
        } else if (tablePropertiesLocal.type === "assigned") {
            configuration.assigned = true;
            promise = manager.getAssignedTasks(tablePropertiesLocal);
        } else {
            toast.notifyErrorList("An error occured(such type of tasks doesn't exist)");
        }
        if (promise) {
            loadEachType(promise, configuration);
        }
        else {
            toast.notifyErrorList("An error occured(such type of tasks doesn't exist)");
        }
    }

    function dependentInputTeam() {
        let uId = $('.task-users').val();
        let teamDisabledOption = $(`.task-team option[value="${uId}"]`);
        teamDisabledOption.attr('disabled', 'disabled').prop('selected', false).siblings().removeAttr('disabled');
        $('.task-users').on('select2:select', function () {
            uId = $(this).val();
            teamDisabledOption = $(`.task-team option[value="${uId}"]`);
            teamDisabledOption.attr('disabled', 'disabled').prop('selected', false).siblings().removeAttr('disabled');
            $('.task-team').select2();
        });
        $('.task-users').select2();
        $('.task-team').select2();
    }

    function fillLoadedLists() {
        fillSelect(priorities, $(".task-priority"), true);
        fillSelect(users, $(".task-users"), false);
        fillSelect(statuses, $(".task-status"), true);
        fillSelect(users, $(".task-team"), false);
    }

    function fillSelect(options, selectTarget, translatable) {
        $.each(selectTarget, function () {
            const scope = $(this);
            scope.html('');
            $.each(options, function () {
                let newOption = new Option(this.text, this.value);
                if (translatable) {
                    newOption = new Option(window.translate(this.translateKey), this.value);;
                }
                scope.append(newOption);
            });
            window.forceTranslate(scope);
            scope.select2();
        });
    }

    function loadTaskEditModal(taskId) {
        const editModal = $('#editTaskModal');
        editModal.find('#edit-task').attr('data-id', taskId);
        return manager.getTask(taskId).then(result => {
            const htmlOutput = taskEditTemplate.render(result, {
                startDateF: moment(result.startDate, 'DD.MM.YYYY').format('YYYY-MM-DD'),
                endDateF: moment(result.endDate, 'DD.MM.YYYY').format('YYYY-MM-DD')
            });
            $(".edit-task-form-elements").html(htmlOutput);
            fillLoadedLists();
            dependentInputTeam();
            editModal.find('.task-users').val(result.userId).trigger('change');
            editModal.find('.task-status').val(result.status).trigger('change');
            editModal.find('.task-priority').val(result.taskPriority).trigger('change');
            editModal.find(`.task-team option[value="${result.userId}"]`).prop('disabled', true).prop('selected', false).siblings().removeAttr('disabled');

            $.each(result.userTeam, function (index, userTeamId) {
                editModal.find(`.task-team option[value="${userTeamId}"]`).prop('selected', true);
            });
            editModal.find('.task-team').select2();

            $.each(result.files, function () {
                getFilename(this).then(filename => {
                    fileListAppend(filename, $(".modal-edit-task .add-task-uploaded-files .file-list"), this);
                });
            });
            window.forceTranslate();
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function addTaskEditSubmitEvent() {
        $('#edit-task').off().submit(function (e) {
            e.preventDefault();
            const scope = $(this);

            let filesToUpload = [];

            $.each($('.modal-edit-task .file-list-item:not(.deleting)'), function () {
                let fileId = $(this).find('.file-name').data('file-id');
                filesToUpload.push(fileId);
            });

            $.each($('.modal-edit-task .file-list-item.deleting'), function () {
                let fileId = $(this).find('.file-name').data('file-id');
                deleteFilePermanent(fileId);
            });

            let task = {
                id: scope.data('id'),
                name: scope.find('.task-name').val(),
                description: scope.find('.task-description').val(),
                startDate: scope.find('.task-start-date').val(),
                endDate: scope.find('.task-end-date').val(),
                userId: scope.find('.task-users').val(),
                taskPriority: scope.find('.task-priority').val(),
                status: scope.find('.task-status').val(),
                userTeam: scope.find('.task-team').val(),
                files: filesToUpload
            };
            if (!moment(task.startDate).isAfter(task.endDate)) {
                $('.task-manager-loader').fadeIn();
                updateTask(task).then(() => {
                    window.forceTranslate();
                    $('.task-manager-loader').fadeOut();
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
                $('#editTaskModal').modal('hide');
            }
            else {
                toast.notify({ text: window.translate("system_taskmanager_error_date"), icon: "error" });
            }
        });
    }

    function addTaskEvents() {
        $(".delete-task").off().on('click', function (e) {
            e.preventDefault();
            const taskId = $(this).data("id");
            const taskName = $(this).data("task-name");
            e.stopPropagation();
            $('#deleteConfirmModal .task-name').html(taskName);
            $('#deleteConfirmModal').modal('show');

            $('#deleteConfirmModal #submit-task-delete').off().on('click', function () {
                manager.deleteTask(taskId).then(() => {
                    toast.notify({ text: window.translate("system_taskmanager_delete_task_success"), icon: "success" });
                }).catch(e => {
                    toast.notifyErrorList(e);
                });
                $(`.task[data-id="${taskId}"]`).remove();
                $('#deleteConfirmModal').modal('hide');
            });
        });
        $(".edit-task").off().on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            const taskId = $(this).data("id");
            loadTaskEditModal(taskId).then(() => {
                addTaskEditSubmitEvent();
                window.forceTranslate();
                addChangeFileInputEvent('edit');
                $('#editTaskModal').modal('show');
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        });
        $('.restore-task').off().on('click', function (e) {
            e.stopPropagation();
            e.preventDefault();
            const scope = $(this);
            const taskId = scope.data('id');
            restoreTask(taskId).then(() => {
                $(`.task[data-id="${taskId}"]`).remove();
            }).catch(e => {
                console.warn(e);
            });
        });
        $('.view-task').off().on('click', function (e) {
            e.stopPropagation();
            e.preventDefault();
            const scope = $(this);
            const taskId = scope.data('id');
            const assigned = scope.data('assigned');
            if ($('.task-manager .task-manager-task-types').val() === 'deleted') {
                $('.nav-tabs').hide();
            }
            else {
                $('.nav-tabs').show();
            }
            $('.initial-tab-click').trigger('click');
            viewTask(taskId, assigned).then(() => {
                if (assigned) {
                    loadStatusAssignedTask(taskId);
                }
                window.forceTranslate();
                $('#detailsTaskModal').modal('show');
            }).catch(e => {
                console.warn(e);
                console.log('problem');
            });
        });
    }

    function loadStatusAssignedTask(taskId) {
        $("#edit-assigned-task-status").html('');
        $.each(statuses, function () {
            $("#edit-assigned-task-status").append($(`<option value="${this.value}" translate="${this.translateKey}">${this.text}</option>`));
        });
        manager.getTask(taskId).then(result => {
            $("#edit-assigned-task-status").val(result.status);
            $("#edit-assigned-task-status").on('change', function () {
                const scope = $(this);
                let task = {
                    Id: result.id,
                    Name: result.name,
                    Description: result.description,
                    StartDate: result.startDate,
                    EndDate: result.endDate,
                    UserId: result.userId,
                    TaskPriority: result.taskPriority,
                    Status: scope.val()
                };
                updateTask(task).then(() => { });
            });
            window.forceTranslate();
        });
    }

    function viewTask(taskId, assigned) {
        return manager.getTask(taskId).then(result => {
            const user = findUserById(result.userId);
            const htmlOutput = taskDetailsTemplate.render(result, {
                user: user,
                deleted: false,
                assigned: assigned,
                statusesList: statuses,
                prioritiesList: priorities
            });

            $('#detailsTaskModal .task-number').html('#' + result.taskNumber);

            $('#details-tab').html(htmlOutput);
            $('.view-task-files').html('');
            $.each(result.files, function () {
                const scope = this;
                getFilename(scope).then(filename => {
                    $('.view-task-files').append('<div><a href="/api/File/GetFile?id=' + scope + '">' + filename + '</a></div>');
                });
            });

            $('.view-task-team').html('');
            $.each(result.userTeam, function (index, userId) {
                const teamUser = findUserById(userId);
                $('.view-task-team').append(`<div class="task-user-team">${teamUser.text}</div>`);
            });

            window.forceTranslate();
            loadTaskItems(taskId).then(() => { });

            let task = {
                id: result.id,
                name: result.name,
                description: result.description,
                startDate: result.startDate,
                endDate: result.endDate,
                userId: result.userId,
                taskPriority: result.taskPriority,
                status: result.status,
                files: result.files
            };

            if (result.files == null) {
                task.files = [];
            }


            $('#edit-assigned-task-status').off().on('change', function () {
                task.status = $(this).val();
                updateTask(task).then(() => { }).catch(e => {
                    toast.notifyErrorList(e);
                });
            });
        }).catch(e => {
            toast.notifyErrorList(e);
            $("#task-items-tab .task-items").html('');
            $('#details-tab').html('');
        });

    }

    function loadTaskItems(taskId) {
        return manager.getTaskItems(taskId).then(taskItems => {
            manager.getTask(taskId).then(result => {
                let accessLevel = result.accessLevel;
                $.each(taskItems, function () {
                    const htmlOutput = taskItemsTemplate.render(taskItems, { accessLevel: accessLevel });
                    $("#task-items-tab .task-items").html(htmlOutput);
                });
                addTaskItemsEvents(taskId);
            });
        }).catch(() => {
            $("#task-items-tab .task-items").html('');
            addTaskItemsEvents(taskId);
        });
    }

    function sendAddTaskObject(task) {
        if (!moment(task.startDate).isAfter(task.endDate)) {
            $('.task-manager-loader').fadeIn();
            createTask(task).then(() => {
                $('.task-manager-loader').fadeOut();
            }).catch(e => {
                console.warn(e);
            });

            $('#addTaskModal').modal('hide');
            $('#add-task')[0].reset();
            $('#add-task-item-add-task')[0].reset();

            $('#add-task-items-tab .task-items-add-task').html('');
        }
        else {
            toast.notify({ text: window.translate("system_taskmanager_error_date"), icon: "error" });
        }
    }

    function createTask(task) {
        return manager.createTask(task).then(() => {
            toast.notify({ text: window.translate('system_taskmanager_add_task_success'), icon: "success" });
            loadTaskList(tableProperties);
            $('.task-manager-controls .task-manager-task-types').val('active');
            window.forceTranslate();
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function addTaskItemsEvents(taskId) {
        if (taskId !== null) {
            $('#add-new-task-item').off().on('click', function (e) {
                e.preventDefault();
                const name = $(this).siblings('#new-task-item-name').val();
                $(this).siblings('#new-task-item-name').val('');
                addTaskItem(taskId, name);
            });
            $('.task-item-status').off().on('change', function () {
                const taskItemId = $(this).data('id');
                const taskItemName = $(this).siblings('.custom-control-label').text();
                const taskItemStatus = $(this).is(':checked');
                updateTaskItem(taskId, taskItemId, taskItemName, taskItemStatus).then(() => { }).catch(() => {
                });
            });
        }
        $('.task-item-delete').off().on('click', function () {
            const scope = $(this);
            let taskItemId = scope.data('id');
            scope.closest('.task-item').remove();
            if (taskId !== null) {
                deleteTaskItem(taskId, taskItemId).then(() => {
                }).catch(e => {
                    console.warn(e, 'delete task item action');
                });
            }
        });
    }

    function addTaskItem(taskId, taskItemName) {
        manager.createTaskItem({ TaskId: taskId, Name: taskItemName, IsDone: false }).then(result => {
            const htmlOutput = taskItemsTemplate.render({ id: result, name: taskItemName, isDone: false }, { accessLevel: 'Author' });
            $("#task-items-tab .task-items").append(htmlOutput);
            addTaskItemsEvents(taskId);
            refreshTask(taskId).then(() => {

            });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function deleteTaskItem(taskId, taskItemId) {
        return manager.deleteTaskItem(taskItemId).then(() => {
            toast.notify({ text: window.translate("system_taskmanager_delete_success"), icon: "success" });
            refreshTask(taskId).then(() => { });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function refreshTask(taskId) {
        const targetTask = $(`.task[data-id=${taskId}]`);
        return manager.getTask(taskId).then(result => {
            targetTask.find('.task-number').html('#' + result.taskNumber);
            targetTask.find('.task-label').html(result.name);
            targetTask.find('.task-description').html(result.description);
            targetTask.find('.progress .progress-bar').css('width', `calc(100% * ${result.taskItemsCount[0]} / ${result.taskItemsCount[1]})`);
            targetTask.find('.progress .progress-bar').attr('aria-valuenow', result.taskItemsCount[0]);
            targetTask.find('.progress .progress-bar').attr('aria-valuemax', result.taskItemsCount[1]);
            targetTask.find('.task-item-count').html(`<span>${result.taskItemsCount[0]}/${result.taskItemsCount[1]}</span>`);
            targetTask.find('.start-date span').html(result.startDate);
            targetTask.find('.end-date span').html(result.endDate);
            targetTask.find('.status').html(statuses[result.status].text);
            targetTask.find('.priority').html(`<span class="priority-${priorities[result.taskPriority].text}">${priorities[result.taskPriority].text}</span>`);
            targetTask.removeClass('priority-High priority-Critical priority-Low priority-Medium').addClass('priority-' + priorities[result.taskPriority].text);
        }).catch(e => {
            toast.notifyErrorList(e);
            console.log('here is the problem');
        });
    }

    function restoreTask(taskId) {
        return manager.restoreTask(taskId).then(() => {
            toast.notify({ text: window.translate("system_taskmanager_restore_success"), icon: "success" });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function updateTaskItem(taskId, taskItemId, name, isDone) {
        return manager.updateTaskItem({ Id: taskItemId, Name: name, IsDone: isDone }).then(() => {
            refreshTask(taskId).then(() => { });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function updateTask(task) {
        return manager.updateTask(task).then(() => {
            toast.notify({ text: window.translate("system_taskmanager_update_success"), icon: "success" });
            refreshTask(task.id).then(() => {
            }).catch(e => {
                toast.notifyErrorList(e);
            });
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }

    function addTablePager(pageCount, page) {
        $('#task-list-pager').html('');
        if (pageCount > 1) {
            for (let i = 1; i <= pageCount; i++) {
                if (page === i) {
                    $('#task-list-pager').append(`<li class="page-item active"><a class="page-link" data-page="${i}">${i}</a></li>`);
                }
                else {
                    $('#task-list-pager').append(`<li class="page-item"><a class="page-link" data-page="${i}">${i}</a></li>`);
                }
            }

            if (page > 1) {
                $('#task-list-pager').prepend(`<li class="page-item"><a class="page-link previous-page" data-page="${page - 1}"><i class="material-icons">keyboard_arrow_left</i></a></li>`);
            }
            else if (page < pageCount) {
                $('#task-list-pager').append(`<li class="page-item"><a class="page-link previous-page" data-page="${page + 1}"><i class="material-icons">keyboard_arrow_right</i></a></li>`);
            }

            $('#task-list-pager .page-item').off().on('click', function (e) {
                const linkPage = $(this).children('.page-link').data('page');
                e.preventDefault();
                let taskPropertiesLocal = tableProperties;
                taskPropertiesLocal.page = linkPage;
                loadTaskList(taskPropertiesLocal);
            });
        }
    }

    function findUserById(uId) {
        const result = users.find(({ value }) => value === uId);
        if (!result) {
            return {
                disabled: false,
                group: null,
                selected: false,
                text: 'Unknown',
                value: '00000000-0000-0000-0000-000000000000'
            }
        }
        else {
            return result;
        }
    }

    function cleanTableSort() {
        $.each(sortableTableHeaders, function () {
            sortableTableHeaders.children('.sort').removeClass('ascending descending');
        });
    }

    function getFilesFromInput(input) {
        let files = input[0].files;
        return files;
    }

    function fileListAppend(fileName, wrapper, fileId) {
        const restoreBtn = '<button type="button" class="d-none file-list-restore btn btn-outline-warning p-0 ml-auto"><i class="material-icons d-block">restore</i></button>';
        const deleteBtn = '<button type="button" class="file-list-delete btn btn-outline-danger p-0 ml-auto"><i class="material-icons d-block">close</i></button>';
        const listElement = $('<div class="file-list-item d-flex mt-2"><label class="m-0 file-name" data-file-id="' + fileId + '">' + fileName + '</label>' + deleteBtn + restoreBtn + '</div>');
        wrapper.append(listElement);
        addFileListEvents();
    }

    function addFileListEvents() {
        $.each($('.add-task-uploaded-files .file-list-item'), function () {
            const scope = $(this);
            scope.find('.file-list-delete').on('click', function () {
                scope.addClass('deleting');
                scope.find('.file-list-restore').removeClass('d-none');
                scope.find('.file-list-delete').addClass('d-none');
            });
            scope.find('.file-list-restore').on('click', function () {
                scope.removeClass('deleting');
                scope.find('.file-list-delete').removeClass('d-none');
                scope.find('.file-list-restore').addClass('d-none');
            });
        });
    }

    function uploadMultipleFiles(formData) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/File/UploadMultiple',
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    let results = [];
                    $.each(data, function () {
                        if (this.is_success) {
                            results.push(this.result);
                            resolve(results);
                        }
                        else {
                            reject(this.error_keys);
                        }
                    });
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    function uploadFile(formData) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/File/Upload',
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.is_success) {
                        resolve(data.result);
                    }
                    else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    function getFilename(fileId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/api/File/GetFile?id=${fileId}`,
                type: "GET",
                success: function (data, textStatus, request) {
                    if (data) {
                        let filename = '';
                        let disposition = request.getResponseHeader('content-disposition');
                        if (disposition && disposition.indexOf('attachment') !== -1) {
                            let filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                            let matches = filenameRegex.exec(disposition);
                            if (matches != null && matches[1]) {
                                filename = matches[1].replace(/['"]/g, '');
                            }
                        }
                        resolve(filename);
                    } else {
                        reject(textStatus);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    function deleteFilePermanent(fileId) {
        var form = new FormData();
        form.append("id", fileId);
        $.ajax(
            {
                url: '/api/File/DeletePermanent',
                data: form,
                processData: false,
                contentType: false,
                type: "POST",
                success: function () {
                    toast.notify({ text: window.translate("system_taskmanager_file_deleted"), icon: "success" });
                }
            }
        );
    }

});
