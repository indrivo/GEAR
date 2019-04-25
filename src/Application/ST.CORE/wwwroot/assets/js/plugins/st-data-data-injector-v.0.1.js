/* Data Injector
 * A plugin for get and set data on api
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Data injector plugin require JQuery');
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
 * Get all with filters
 * @param {any} table
 * @param {any} filters
 */
DataInjector.prototype.GetAllWhere = function (table, filters) {
	return window.load("/api/DataInjector/GetAllWhere",
		{
			tableName: table,
			filters: filters
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