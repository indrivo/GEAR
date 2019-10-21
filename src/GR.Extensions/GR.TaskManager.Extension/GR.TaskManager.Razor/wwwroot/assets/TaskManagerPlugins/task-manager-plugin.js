class TaskManager {
    constructor() {

    }

    /**
     * Get task priority list
     */
    getTaskPriorityList() {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/TaskManager/GetTaskPriorityList",
                success: (data) => {
                    resolve(data);
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get task status list
     */
    getTaskStatusList() {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/TaskManager/GetTaskStatusList",
                success: (data) => {
                    resolve(data);
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get users list
     */
    getUsersList() {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/TaskManager/GetUsersList",
                success: (data) => {
                    resolve(data);
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get task
     */
    getTask(taskId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/api/TaskManager/GetTask?id=${taskId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get user tasks
                 * param {bool} deleted
                 * param {int32} page
                 * param {int32} pageSize
                 * param {bool} descending
                 * param {string} attribute
        */
    getUserTasks(userTasks = {
        deleted: false,
        page: 0,
        pageSize: 0,
        descending: 0,
        attribute: ''
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/api/TaskManager/GetUserTasks`,
                data: userTasks,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get user assigend task
                 * param {bool} deleted
                 * param {int32} page
                 * param {int32} pageSize
                 * param {bool} descending
                 * param {string} attribute
        */
    getAssignedTasks(userTasks = {
        deleted: false,
        page: 0,
        pageSize: 0,
        descending: 0,
        attribute: ''
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/TaskManager/GetAssignedTasks",
                data: userTasks,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Get task items
                 * param {strig} taskId
        */
    getTaskItems(taskId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/api/TaskManager/GetTaskItems?id=${taskId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Create task
                 * param {any} task
        */
    createTask(task = {
        TaskItems: [],
        Name: "",
        Description: "",
        StartDate: "",
        EndDate: "",
        UserId: "",
        Files: [],
        UserTeam: [],
        TaskPriority: "",
        Status: ""
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "/api/TaskManager/CreateTask",
                data: task,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }
    
    /**
     * Update task
     * param {any} task
     */
    updateTask(task = {
        Id: "",
        Name: "",
        Description: "",
        DtartDate: "",
        EndDate: "",
        UserId: "",
        UserTeam: [],
        Team: "",
        Files: [],
        TaskPriority: "",
        Status: ""
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "/api/TaskManager/UpdateTask",
                data: task,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Delete task
                 * param {strig} taskId
        */
    deleteTask(taskId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: `/api/TaskManager/DeleteTask?id=${taskId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Delete task permanently
                 * param {strig} taskId
        */
    deleteTaskPermanent(taskId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: `/api/TaskManager/DeleteTaskPermanent?id=${taskId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
     * Restore task
                 * param {strig} taskId
        */
    restoreTask(taskId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: `/api/TaskManager/RestoreTask?id=${taskId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
    * Create task item
        * param {any} taskItem
    */
    createTaskItem(taskItem = {
        taskId: "",
        name: "",
        isDone: false,
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "/api/TaskManager/CreateTaskItem",
                data: taskItem,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
    * Update task item
        * param {any} taskItem
    */
    updateTaskItem(taskItem = {
        Id: "",
        Name: "",
        IsDone: false,
    }) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "/api/TaskManager/UpdateTaskItem",
                data: taskItem,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    /**
    * Delete task item
        * param {string} taskItemId
    */
    deleteTaskItem(taskItemId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                async: true,
                url: `/api/TaskManager/DeleteTaskItem?id=${taskItemId}`,
                success: (data) => {
                    if (data.is_success) {
                        resolve(data.result);
                    } else {
                        reject(data.error_keys);
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    statuses = [];
    getStatusesList() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.statuses.length > 0) {
                resolve(scope.statuses);
            } else {
                scope.getTaskStatusList().then(result => {
                    scope.statuses = result;
                    resolve(scope.statuses);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        })
    }

    priorities = [];
    getPrioritiesList() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.priorities.length > 0) {
                resolve(scope.priorities);
            } else {
                scope.getTaskPriorityList().then(result => {
                    scope.priorities = result;
                    resolve(scope.priorities);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        })
    }

    users = [];
    getUsersLoadedList() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.users.length > 0) {
                resolve(scope.users);
            } else {
                scope.getUsersList().then(result => {
                    scope.users = result;
                    resolve(scope.users);
                }).catch(e => {
                    console.warn(e);
                    toast.notifyErrorList(e);
                });
            }
        })
    }
}