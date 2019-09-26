const manager = new TaskManager();
const toast = new ToastNotifier();

const taskDetailsTemplate = $.templates("#taskDetailsTemplate");
const taskEditTemplate = $.templates("#taskEditTemplate");
const taskListTemplate = $.templates("#taskListTemplate");
const taskItemsTemplate = $.templates("#taskItemsTemplate");
const taskItemsAddTaskTemplate = $.templates("#taskItemsAddTaskTemplate");

const templateManager = new TemplateManager();

let statuses = [];
let priorities = [];
let users = [];
let tablePage = 1;
let tablePageSize = 10;
let taskType = $('.task-manager .task-manager-task-types').val();
const sortableTableHeaders = $('.task-manager .task-manager-task-list th.sortable');

Promise.all([manager.getStatusesList(), manager.getPrioritiesList(), manager.getUsersLoadedList()]).then(data => {
    statuses = data[0];
    priorities = data[1];
    users = data[2];

    statuses[0].translateKey = 'system_taskmanager_notstarted';
    statuses[1].translateKey = 'system_taskmanager_inprogress';
    statuses[2].translateKey = 'system_taskmanager_review';
    statuses[3].translateKey = 'system_taskmanager_completed';

    priorities[0].translateKey = 'system_taskmanager_low';
    priorities[1].translateKey = 'system_taskmanager_medium';
    priorities[2].translateKey = 'system_taskmanager_high';
    priorities[3].translateKey = 'system_taskmanager_critical';

    fillLoadedLists();

    $('.task-manager-loader').fadeIn();

    loadTaskList(taskType, tablePage, tablePageSize, false, 'StartDate').then(() => {
        $('.task-manager-loader').fadeOut();
    });
});

$('.task-manager .task-manager-items-per-page-control').off().on('change', function () {
    tablePageSize = $(this).val();
    $('.task-manager-loader').fadeIn();
    loadTaskList(taskType, tablePage, tablePageSize, false, 'StartDate').then(() => {
        $('.task-manager-loader').fadeOut();
    });
});

$('#add-task').submit(function (e) {
    e.preventDefault();

    const scope = $(this);
    let taskItemsForm = [];
    let filesToUpload = [];
    let formData = new FormData();

    $.each($('#addTaskModal .file-list-item:not(.deleting)'), function () {
        let fileId = $(this).find('.file-name').data('file-id');
        let file = $('#addTaskModal .task-files')[0].files[fileId];
        formData.append("files", file);
    });

    $.each($('#addTaskModal .task-item-name'), function () {
        taskItemsForm.push({ Name: $(this).data('task-name') });
    });


    uploadMultipleFiles(formData).then(result => {
        filesToUpload = result;

        task = {
            taskItems: taskItemsForm,
            name: scope.find('#add-task-name').val(),
            description: scope.find('#add-task-description').val(),
            startDate: scope.find('#add-task-start-date').val(),
            endDate: scope.find('#add-task-end-date').val(),
            userId: scope.find('#add-task-users').val(),
            taskPriority: scope.find('#add-task-priority').val(),
            status: scope.find('#add-task-status').val(),
            files: filesToUpload
        };

        if (moment(task.startDate).isBefore(task.endDate)) {
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
            toast.notify({ text: window.translate(""), icon: "error" });
        }
    }).catch(e => {
        toast.notifyErrorList(e);
    });
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
});

$('.task-manager-add-new-task').off().on('click', function () {
    $('.initial-tab-click').trigger('click');
    $('.add-task-uploaded-files .file-list').html('');
    $('.modal-add-task .file-list').html('');
    addChangeFileInputEvent('add');
});

$('.task-manager .task-manager-task-types').off().on('change', function () {
    taskType = $(this).val();
    $('.task-manager-loader').fadeIn();
    loadTaskList(taskType, tablePage, tablePageSize, false, 'StartDate').then(() => {
        $('.task-manager-loader').fadeOut();
    });
});

sortableTableHeaders.off().on('click', function () {
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

    $('.task-manager-loader').fadeIn();
    loadTaskList(taskType, tablePage, tablePageSize, descendingVal, attributeVal).then(() => {
        $('.task-manager-loader').fadeOut();
    });
});

function addChangeFileInputEvent(type) {
    $('.task-files').off().on('change', function () {
        const scope = this;
        const wrapper = $(scope).siblings('.add-task-uploaded-files').find('.file-list');
        const files = getFilesFromInput($(this));
        if (type === 'add') {
            wrapper.html('');
            for (let i = 0; i < files.length; i++) {
                fileListAppend(files[i].name, wrapper, i);
            }
        }
        else if (type === 'edit') {
            let formData = new FormData();
            formData.append("files", files[0]);
            uploadFile(formData).then(fileId => {
                fileListAppend(files[0].name, wrapper, fileId);
            });
        }
    });
};

function loadTaskList(type, page, pageSize, descending, attribute) {
    if (type === 'active') {
        return manager.getUserTasks({ deleted: false, page, pageSize, descending, attribute }).then(result => {
            $.each(result.results, function () {
                const htmlOutput = taskListTemplate.render(result.results, {
                    deleted: false,
                    statusesList: statuses,
                    prioritiesList: priorities,
                });
                $("#task-list-table").html(htmlOutput);
            });
            addTablePager(result.pageCount, result.currentPage);
            addTaskEvents();
            window.forceTranslate();
        }).catch(e => {
            console.log(e);
            toast.notifyErrorList(e);
        });
    }
    else if (type === 'deleted') {
        return manager.getUserTasks({ deleted: true, page, pageSize, descending, attribute }).then(result => {
            $.each(result.results, function () {
                const htmlOutput = taskListTemplate.render(result.results, {
                    deleted: true,
                    statusesList: statuses,
                    prioritiesList: priorities,
                });
                $("#task-list-table").html(htmlOutput);
            });
            addTablePager(result.pageCount, result.CurrentPage);
            addTaskEvents();
            window.forceTranslate();
        }).catch(e => {
            console.log(e);
            toast.notifyErrorList(e);
        });
    }
    else if (type === 'assigned') {
        return manager.getAssignedTasks({ deleted: false, page, pageSize, descending, attribute }).then(result => {
            $.each(result.results, function () {
                const htmlOutput = taskListTemplate.render(result.results, {
                    assigned: true,
                    statusesList: statuses,
                    prioritiesList: priorities,
                });
                $("#task-list-table").html(htmlOutput);
            });
            addTablePager(result.pageCount, result.CurrentPage);
            addTaskEvents();
            window.forceTranslate();
        }).catch(e => {
            toast.notifyErrorList(e);
        });
    }
    else {
        toast.notifyErrorList("An error occured(such type of tasks doesn't exist)");
    }
}

function fillLoadedLists() {

    $(".task-priority").html('');
    $.each(priorities, function () {
        $(".task-priority").append($(`<option value="${this.value}" translate="${this.translateKey}">${this.text}</option>`));
    });

    $(".task-users").html('');
    $.each(users, function () {
        $(".task-users").append($(`<option value="${this.value}">${this.text}</option>`));
    });


    $(".task-status").html('');
    $.each(statuses, function () {
        $(".task-status").append($(`<option value="${this.value}" translate="${this.translateKey}">${this.text}</option>`));
    });
    window.forceTranslate();

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
        editModal.find('.task-user option[value="' + result.userId + '"]').attr('selected', 'selected');
        editModal.find('.task-status option[value="' + result.status + '"]').attr('selected', 'selected');
        editModal.find('.task-priority option[value="' + result.taskPriority + '"]').attr('selected', 'selected');
        $.each(result.files, function () {
            getFilename(this).then(filename => {
                fileListAppend(filename, $('.modal-edit-task .add-task-uploaded-files .file-list'), this);
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
            files: filesToUpload
        };
        $('.task-manager-loader').fadeIn();
        updateTask(task).then(() => {
            window.forceTranslate();
            $('.task-manager-loader').fadeOut();
        }).catch(e => {
            toast.notifyErrorList(e);
        });
        $('#editTaskModal').modal('hide');
    });
}

function addTaskEvents() {
    $(".delete-task").off().on('click', function (e) {
        e.preventDefault();
        const taskId = $(this).data("id");
        const taskName = $(this).data("task-name");
        e.stopPropagation();
        $('#deleteConfirmModal .modal-title .task-name').html(taskName);
        $('#deleteConfirmModal').modal('show');

        $('#deleteConfirmModal #submit-task-delete').off().on('click', function () {
            manager.deleteTask(taskId).then(result => {
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
                Status: scope.val(),
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
            prioritiesList: priorities,
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
        window.forceTranslate();
        loadTaskItems(taskId).then(() => { });
    }).catch(e => {
        toast.notifyErrorList(e);
        $("#task-items-tab .task-items").html('');
        $('#details-tab').html('');
    });

}

function loadTaskItems(taskId) {
    return manager.getTaskItems(taskId).then(taskItems => {
        $.each(taskItems, function () {
            const htmlOutput = taskItemsTemplate.render(taskItems);
            $("#task-items-tab .task-items").html(htmlOutput);
        });
        addTaskItemsEvents(taskId);
    }).catch(e => {
        $("#task-items-tab .task-items").html('');
        addTaskItemsEvents(taskId);
    });
}

function createTask(task) {
    console.log(task);
    return manager.createTask(task).then(result => {
        toast.notify({ text: window.translate("system_taskmanager_add_task_success"), icon: "success" });
        loadTaskList('active', tablePage, tablePageSize, false, 'StartDate').then(() => { });
        $('.task-manager-controls .task-manager-task-types').val('active');
        window.forceTranslate();
    }).catch(e => {
        toast.notifyErrorList(e);
    });
}

function addTaskItemsEvents(taskId) {
    $('#add-new-task-item').off().on('click', function (e) {
        e.preventDefault();
        const name = $(this).siblings('#new-task-item-name').val();
        $(this).siblings('#new-task-item-name').val('');
        addTaskItem(taskId, name);
    });
    $('.delete-task-item').off().on('click', function () {
        const scope = $(this);
        taskItemId = scope.data('id');
        deleteTaskItem(taskId, taskItemId).then(() => {
            scope.closest('.task-item').remove();
        }).catch(e => {
            console.warn(e, 'delete task item action');
        });
    });
    $('.task-item-status').off().on('change', function () {
        const taskItemId = $(this).data('id');
        const taskItemName = $(this).siblings('.custom-control-label').text();
        const taskItemStatus = $(this).is(':checked');
        updateTaskItem(taskId, taskItemId, taskItemName, taskItemStatus).then(() => { }).catch(e => {
        });
    });
}

function addTaskItem(taskId, taskItemName) {
    manager.createTaskItem({ TaskId: taskId, Name: taskItemName, IsDone: false }).then(result => {
        const htmlOutput = taskItemsTemplate.render({ id: result, name: taskItemName, isDone: false });
        $("#task-items-tab .task-items").append(htmlOutput);
        addTaskItemsEvents(taskId);
        refreshTask(taskId).then(() => {

        });
    }).catch(e => {
        toast.notifyErrorList(e);
    });
}

function deleteTaskItem(taskId, taskItemId) {
    return manager.deleteTaskItem(taskItemId).then(result => {
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
        if (result.status == 3) {
            targetTask.find('.status').html('<i class="material-icons d-block mr-1">done_outline</i>' + statuses[result.status].text);
        }
        else {
            targetTask.find('.status').html(statuses[result.status].text);
        }
        targetTask.find('.priority').html(`<span class="priority-${priorities[result.taskPriority].text}">${priorities[result.taskPriority].text}</span>`);
        targetTask.removeClass('prioriry-High priority-Critical priority-Low priority-Medium').addClass('priority-' + priorities[result.taskPriority].text);

    }).catch(e => {
        toast.notifyErrorList(e);
        console.log('here is the problem');
    });
}

function restoreTask(taskId) {
    return manager.restoreTask(taskId).then(result => {
        toast.notify({ text: window.translate("system_taskmanager_restore_success"), icon: "success" });
    }).catch(e => {
        toast.notifyErrorList(e);
    });
}

function updateTaskItem(taskId, taskItemId, name, isDone) {
    return manager.updateTaskItem({ Id: taskItemId, Name: name, IsDone: isDone }).then(result => {
        refreshTask(taskId).then(() => { });
    }).catch(e => {
        toast.notifyErrorList(e);
    });
}

function updateTask(task) {
    return manager.updateTask(task).then(result => {
        toast.notify({ text: window.translate("system_taskmanager_update_success"), icon: "success" });
        refreshTask(result.id).then(() => {
        }).catch(e => {
        });
    }).catch(e => {
        toast.notifyErrorList(e);
    });
}

function addTablePager(pageCount, page) {
    $('#task-list-pager').html('');
    if (pageCount > 1) {
        for (i = 1; i <= pageCount; i++) {
            if (page == i) {
                $('#task-list-pager').append(`<li class="page-item active"><a class="page-link" data-page="` + i + `">` + i + `</a></li>`);
            }
            else {
                $('#task-list-pager').append(`<li class="page-item"><a class="page-link" data-page="` + i + `">` + i + `</a></li>`);
            }
        }
        $('#task-list-pager .page-item').off().on('click', function (e) {
            const linkPage = $(this).children('.page-link').data('page');
            e.preventDefault();
            $('.task-manager-loader').fadeIn();
            loadTaskList(taskType, linkPage, tablePageSize, false, 'StartDate').then(() => {
                $('.task-manager-loader').fadeOut();
            });
        });
    }
}

function findUserById(UId) {
    const result = users.find(user => {
        return user.value == UId;
    });
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
};

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

function downloadFile(fileId) {
    window.location.href = "/api/File/GetFile?id=" + fileId;
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
            success: function (data) {
                toast.notify({ text: window.translate("system_taskmanager_file_deleted"), icon: "success" });
            }
        }
    );
}
