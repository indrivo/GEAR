/* Data Injector
 * A plugin for get and set data on api
 *
 * v1.0.2
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === "undefined") {
    throw new Error("Data injector plugin require JQuery");
}

function DataInjector() { }

/**
 * Get entity row by id
 * @param {any} table
 * @param {any} itemId
 */
DataInjector.prototype.getByIdWithIncludesAsync = function (entityName, itemId) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            object: itemId
        });
        $.ajax({
            url: `/api/DataInjector/getByIdWithIncludesAsync`,
            data: dataParams,
            method: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

/**
 * Add new row to entity in async mode 
 * @param {any} table
 * @param {any} object
 */
DataInjector.prototype.addAsync = function (entityName, object) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            object: JSON.stringify(object)
        });
        $.ajax({
            url: `/api/DataInjector/AddAsync`,
            data: dataParams,
            method: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};


/**
 * Add new row to entity in async mode 
 * @param {any} table
 * @param {any} object
 */
DataInjector.prototype.addRangeAsync = function (entityName, objectList) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            object: JSON.stringify(objectList)
        });
        $.ajax({
            url: `/api/DataInjector/AddRangeAsync`,
            data: dataParams,
            method: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};

/**
 * Update row in async mode 
 * @param {any} entityName
 * @param {any} object
 */
DataInjector.prototype.updateAsync = function (entityName, object) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            object: JSON.stringify(object)
        });
        $.ajax({
            url: `/api/DataInjector/UpdateAsync`,
            data: dataParams,
            method: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

/**
 * Delete permanent async
 * @param {any} entityName
 * @param {any} filters
 */
DataInjector.prototype.deletePermanentWhereAsync = function (entityName, filters) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: `/api/DataInjector/DeletePermanentWhereAsync`,
            data: dataParams,
            method: "delete",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}


/**
 * Delete where async
 * @param {any} entityName
 * @param {any} filters
 */
DataInjector.prototype.deleteWhereAsync = function (entityName, filters) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: `/api/DataInjector/DeleteWhereAsync`,
            data: dataParams,
            method: "delete",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}


/**
 * Get all with filters async
 * @param {any} table
 * @param {any} filters
 */
DataInjector.prototype.getAllWhereNoIncludesAsync = function (entityName, filters = []) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: "/api/DataInjector/getAllWhereNoIncludesAsync",
            data: dataParams,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            method: "post",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};



/**
 * Get all with filters async
 * @param {any} table
 * @param {any} filters
 */
DataInjector.prototype.getAllWhereNoIncludes = function (entityName, filters = []) {
    const dataParams = JSON.stringify({
        entityName: entityName,
        filters: filters
    });

    let result = {
        is_success: false,
        error_keys: [],
        result: null
    };

    $.ajax({
        url: "/api/DataInjector/getAllWhereNoIncludesAsync",
        data: dataParams,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        method: "post",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (error) {
            result.error_keys.push({ key: "", message: error });
        }
    });

    return result;
};


/**
 * Get all with filters async
 * @param {any} table
 * @param {any} filters
 */
DataInjector.prototype.getAllWhereWithIncludes = function (entityName, filters = []) {
    const dataParams = JSON.stringify({
        entityName: entityName,
        filters: filters
    });

    let result = {
        is_success: false,
        error_keys: [],
        result: null
    };

    $.ajax({
        url: "/api/DataInjector/getAllWhereWithIncludesAsync",
        data: dataParams,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        method: "post",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (error) {
            result.error_keys.push({ key: "", message: error });
        }
    });

    return result;
};


/**
 * Get all with include and filters async
 * @param {any} entityName
 * @param {any} filters
 */
DataInjector.prototype.getAllWhereWithIncludesAsync = function (entityName, filters = []) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: "/api/DataInjector/getAllWhereWithIncludesAsync",
            data: dataParams,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            method: "post",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}



/**
 * Get count by filters
 * @param {any} entityName
 * @param {any} filters
 */
DataInjector.prototype.countAsync = function (entityName, filters = []) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: "/api/DataInjector/countAsync",
            data: dataParams,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            method: "post",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};


/**
 * Get count by filters
 * @param {any} entityName
 * @param {any} filters
 */
DataInjector.prototype.countAllAsync = function (entityName, filters = []) {
    return new Promise((resolve, reject) => {
        const dataParams = JSON.stringify({
            entityName: entityName,
            filters: filters
        });
        $.ajax({
            url: "/api/DataInjector/countAllAsync",
            data: dataParams,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            method: "post",
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
};