/* Data Injector
 * A plugin for get and set data on api
 *
 * v1.0.0
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
 * Add new item to database
 * @param {any} table
 * @param {any} object
 */
DataInjector.prototype.Add = function (table, object) {
	try {
		return window.load(`/api/DataInjector/Add?tableName=${table}&obj=${JSON.stringify(object)}`, null,
			"post");
	} catch (e) {
		return {
			is_success: false,
			result: null,
			error_keys: [
				{
					key: "exception",
					message: e
				}
			]
		}
	}
}


/**
 * Update new item
 * @param {any} table
 * @param {any} object
 */
DataInjector.prototype.Update = function (table, object) {
	try {
		return window.load(`/api/DataInjector/Update?tableName=${table}&obj=${JSON.stringify(object)}`, null,
			"post");
	} catch (e) {
		return {
			is_success: false,
			result: null,
			error_keys: [
				{
					key: "exception",
					message: e
				}
			]
		}
	}
}


/**
 * Get all items from entity by name
 * @param {any} table
 */
DataInjector.prototype.GetAll = function (table) {
	return window.load("/api/DataInjector/GetAll",
		{
			tableName: table
		});
}


/**
 * Get all items with includes
 * @param {any} table
 */
DataInjector.prototype.GetAllWithInclude = function (table) {
	return window.load("/api/DataInjector/GetAllWithInclude",
		{
			tableName: table
		});
}

/**
 * Get all with filters
 * @param {any} table
 * @param {any} filters
 */
DataInjector.prototype.GetAllWhere = function (table, filters) {
	return window.load("/api/DataInjector/GetAllWhere",
		{
			tableName: table,
			filters: JSON.stringify(filters)
		});
}


/**
 * Get entity row by id
 * @param {any} table
 * @param {any} itemId
 */
DataInjector.prototype.GetById = function (table, itemId) {
	return window.load("/api/DataInjector/GetById",
		{
			tableName: table,
			id: itemId
		});
}

/**
 * Get bey id with include
 * @param {any} table
 * @param {any} itemId
 */
DataInjector.prototype.GetByIdWithInclude = function (table, itemId) {
	return window.load("/api/DataInjector/GetByIdWithInclude",
		{
			tableName: table,
			id: itemId
		});
}









//------------------------------------Async-------------------------------------//

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
}

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
}