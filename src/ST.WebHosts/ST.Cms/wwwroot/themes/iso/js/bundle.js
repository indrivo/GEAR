/******/ (function (modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if (installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
			/******/
}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
			/******/
};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
		/******/
}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function (exports, name, getter) {
/******/ 		if (!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
			/******/
}
		/******/
};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function (exports) {
/******/ 		if (typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
			/******/
}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
		/******/
};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function (value, mode) {
/******/ 		if (mode & 1) value = __webpack_require__(value);
/******/ 		if (mode & 8) return value;
/******/ 		if ((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if (mode & 2 && typeof value != 'string') for (var key in value) __webpack_require__.d(ns, key, function (key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
		/******/
};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function (module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
		/******/
};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function (object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/js/index.js");
	/******/
})
/************************************************************************/
/******/({

/***/ "./src/js/components/dynamic-filter/dynamic-filter.js":
/*!************************************************************!*\
  !*** ./src/js/components/dynamic-filter/dynamic-filter.js ***!
  \************************************************************/
/*! exports provided: DynamicFilter */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"DynamicFilter\", function() { return DynamicFilter; });\n/* harmony import */ var _list_filter__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./list-filter */ \"./src/js/components/dynamic-filter/list-filter.js\");\n/* harmony import */ var _multi_select_filter__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./multi-select-filter */ \"./src/js/components/dynamic-filter/multi-select-filter.js\");\n/* harmony import */ var _text_filter__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./text-filter */ \"./src/js/components/dynamic-filter/text-filter.js\");\n\r\n\r\n\r\n\r\n/**\r\n * Creates a dynamic filter based on provided arguments.\r\n *\r\n * @param type specifies the type of created dynamic filter. There are three types of available filters:\r\n * 'array', 'date-range' and 'text'. If an invalid argument is provided, returns null.\r\n * @param targetObject the object on which the component will be appended\r\n * @param valuesArray array with the values of dynamic filter\r\n * @param filterOptions object with filter options\r\n * @param positioningOptions object with `Popper` placement options\r\n * @return {FilterBase}\r\n * */\r\nfunction DynamicFilter(type,\r\n                              targetObject,\r\n                              valuesArray = [],\r\n                              filterOptions = {\r\n                                  searchBarPlaceholder: \"Caută\",\r\n                                  addButtonLabel: \"Adaugă\"\r\n                              },\r\n                              positioningOptions = {placement: 'bottom-start'}) {\r\n    switch (type) {\r\n        case 'list': {\r\n            return new _list_filter__WEBPACK_IMPORTED_MODULE_0__[\"ListFilter\"](targetObject, valuesArray, positioningOptions, filterOptions);\r\n        }\r\n        case 'multi-select': {\r\n            return new _multi_select_filter__WEBPACK_IMPORTED_MODULE_1__[\"MultiSelectFilter\"](targetObject, valuesArray, positioningOptions, filterOptions);\r\n        }\r\n        case 'text': {\r\n            return new _text_filter__WEBPACK_IMPORTED_MODULE_2__[\"TextFilter\"](targetObject, positioningOptions, filterOptions);\r\n        }\r\n        default:\r\n            break;\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter/dynamic-filter.js?");

			/***/
}),

/***/ "./src/js/components/dynamic-filter/filter-base.js":
/*!*********************************************************!*\
  !*** ./src/js/components/dynamic-filter/filter-base.js ***!
  \*********************************************************/
/*! exports provided: FilterBase */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"FilterBase\", function() { return FilterBase; });\nclass FilterBase {\r\n\r\n    constructor(target, positioningOptions, filterOptions) {\r\n        this.positioningOptions = positioningOptions;\r\n        this.target = target;\r\n        this.filterOptions = filterOptions;\r\n\r\n        /**\r\n         * Container of the entire component.\r\n         * Always used.\r\n         * */\r\n        this.dynamicSelect = $(this.createDynamicSelect())[0];\r\n\r\n        this.appendFilterPopperToBody();\r\n        this.removeFilterPopupWhenClickAway();\r\n    }\r\n\r\n    static bindFilteringToSearchInput(searchInput) {\r\n        $(searchInput).on('keyup', (event) => {\r\n            const target = $(event.currentTarget);\r\n            const value = target.find('input').val().toLowerCase().trim();\r\n\r\n            $(searchInput).closest('.dynamic-select').find('.item').filter(function () {\r\n                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);\r\n                $(document).trigger('dynamicFilterSearchInput', {\r\n                    value\r\n                });\r\n            });\r\n\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Removes the provided container when `mouseup` event is triggered outside the container\r\n     * */\r\n    removeFilterPopupWhenClickAway() {\r\n        $(document).mouseup((event) => {\r\n            // if the target of the click isn't the container nor a descendant of the container\r\n            if (!$(this.dynamicSelect).is(event.target)\r\n                && $(this.dynamicSelect).has(event.target).length === 0) {\r\n                this.dynamicSelect.remove();\r\n            }\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Takes the ready filter creates a Popper with it and appends it to the target. The positioning options of the\r\n     * Popper are taken from the object's field `positioningOptions`\r\n     * */\r\n    appendFilterPopperToBody() {\r\n        $(document.body).append(this.dynamicSelect);\r\n        new Popper(this.target, this.dynamicSelect, this.positioningOptions);\r\n    }\r\n\r\n    createDynamicSelect() {\r\n        return '<div class=\"dynamic-select\"></div>';\r\n    }\r\n\r\n    createSearchBar() {\r\n        return '<div class=\"search-bar\">\\n' +\r\n            '       <input type=\"text\" placeholder=\"' + this.filterOptions.searchBarPlaceholder + '\">' +\r\n            '       <span class=\"material-icons bg-transparent\">search</span>\\n' +\r\n            '     </div>';\r\n    }\r\n\r\n    createDynamicSelectBody() {\r\n        return '<div class=\"dynamic-select-body\"></div>'\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter/filter-base.js?");

			/***/
}),

/***/ "./src/js/components/dynamic-filter/list-filter.js":
/*!*********************************************************!*\
  !*** ./src/js/components/dynamic-filter/list-filter.js ***!
  \*********************************************************/
/*! exports provided: ListFilter */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"ListFilter\", function() { return ListFilter; });\n/* harmony import */ var _filter_base__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./filter-base */ \"./src/js/components/dynamic-filter/filter-base.js\");\n\r\n\r\nclass ListFilter extends _filter_base__WEBPACK_IMPORTED_MODULE_0__[\"FilterBase\"] {\r\n\r\n    constructor(targetObject,\r\n                valuesArray = [],\r\n                positioningOptions = {placement: 'bottom-start'},\r\n                filterOptions) {\r\n\r\n        super(targetObject, positioningOptions, filterOptions);\r\n        this.valuesArray = valuesArray;\r\n        this.filterOptions = filterOptions;\r\n\r\n        /**\r\n         * Container for items.\r\n         * Always used.\r\n         * */\r\n        this.dynamicSelectBody = $(this.createDynamicSelectBody())[0];\r\n\r\n        /**\r\n         * Container for search bar.\r\n         * */\r\n        this.searchBar = $(this.createSearchBar())[0];\r\n\r\n        /**\r\n         * An array with .item elements\r\n         * */\r\n        this.items = this.createItems();\r\n\r\n        this.createListFilter();\r\n        _filter_base__WEBPACK_IMPORTED_MODULE_0__[\"FilterBase\"].bindFilteringToSearchInput(this.searchBar);\r\n\r\n    }\r\n\r\n    /**\r\n     * Creates list type of filter with search and event emitting\r\n     * */\r\n    createListFilter() {\r\n\r\n        this.buildComponent();\r\n        this.bindToItemsSelect(this.items, this.dynamicSelect, this.filterOptions);\r\n        return this.container;\r\n    }\r\n\r\n    /**\r\n     * Creates an element with class `.item` for each `valuesArray` item and returns the resulted array\r\n     * @return items[] array of .item\r\n     * */\r\n    createItems() {\r\n        const items = [];\r\n        this.valuesArray.forEach((pair) => {\r\n            // Create items and append them\r\n            const item = document.createElement('div');\r\n            $(item).addClass('item');\r\n            $(item).text(pair.value);\r\n            $(item).attr('data-id', pair.id);\r\n            items.push(item);\r\n        });\r\n        return items;\r\n    }\r\n\r\n    /**\r\n     * Assembles all parts of the component together\r\n     * */\r\n    buildComponent() {\r\n\r\n        // Initializes search bar if needed\r\n        if (this.valuesArray.length >= 5) {\r\n            this.dynamicSelect.append(this.searchBar);\r\n        }\r\n\r\n        // Appends each item to dynamicSelectBody\r\n        this.items.forEach(item => this.dynamicSelectBody.append(item));\r\n\r\n        // Assembles the parts\r\n        this.dynamicSelect.append(this.dynamicSelectBody);\r\n\r\n\r\n    }\r\n\r\n    /**\r\n     * Trigger the `filterSelected` event on `.item` click.\r\n     * The event emits the value of item and destroys the component.\r\n     * */\r\n    bindToItemsSelect(items, container, filterOptions) {\r\n        $(items).each((index, item) => {\r\n            $(item).on('click', function () {\r\n                $(this).trigger('filterValueChange', {\r\n                    id: $(item).data('id'),\r\n                    value: $(item).text(),\r\n                    options: filterOptions\r\n                });\r\n                $(container).remove();\r\n            })\r\n        })\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter/list-filter.js?");

			/***/
}),

/***/ "./src/js/components/dynamic-filter/multi-select-filter.js":
/*!*****************************************************************!*\
  !*** ./src/js/components/dynamic-filter/multi-select-filter.js ***!
  \*****************************************************************/
/*! exports provided: MultiSelectFilter */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"MultiSelectFilter\", function() { return MultiSelectFilter; });\n/* harmony import */ var _filter_base__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./filter-base */ \"./src/js/components/dynamic-filter/filter-base.js\");\n\r\n\r\nclass MultiSelectFilter extends _filter_base__WEBPACK_IMPORTED_MODULE_0__[\"FilterBase\"] {\r\n\r\n    constructor(targetObject,\r\n                valuesArray = [],\r\n                positioningOptions = {placement: 'bottom-start'},\r\n                filterOptions) {\r\n\r\n        super(targetObject, positioningOptions, filterOptions);\r\n        this.valuesArray = valuesArray;\r\n        this.filterOptions = filterOptions;\r\n\r\n        /**\r\n         * Container for items.\r\n         * Always used.\r\n         * */\r\n        this.dynamicSelectBody = $(this.createDynamicSelectBody())[0];\r\n\r\n        /**\r\n         * Container for search bar.\r\n         * */\r\n        this.searchBar = $(this.createSearchBar())[0];\r\n\r\n        /**\r\n         * An array with .item elements\r\n         * */\r\n        this.items = this.createItems();\r\n\r\n        this.selectedItems = this.getInitiallySelectedItems();\r\n\r\n        this.createFilter();\r\n        _filter_base__WEBPACK_IMPORTED_MODULE_0__[\"FilterBase\"].bindFilteringToSearchInput(this.searchBar);\r\n    }\r\n\r\n    /**\r\n     * Creates list type of filter with search and event emitting\r\n     * */\r\n    createFilter() {\r\n\r\n        this.buildComponent();\r\n        this.bindCheckingOnItemClick();\r\n        this.bindToCheckboxTrigger();\r\n    }\r\n\r\n    /**\r\n     * Creates an element with class `.item` for each `valuesArray` item and returns the resulted array\r\n     * @return items[] array of .item\r\n     * */\r\n    createItems() {\r\n        const items = [];\r\n        this.valuesArray.forEach((value) => {\r\n            // Create items and append them\r\n            const item = document.createElement('div');\r\n            $(item).addClass('item');\r\n            item.append($(this.createCustomCheckbox(value.id, value.value))[0]);\r\n            this.setCheckboxState(item, value.checked);\r\n            items.push(item);\r\n        });\r\n        return items;\r\n    }\r\n\r\n    /**\r\n     * Checks the child input if `checked` argument is `true` and unchecks otherwise\r\n     * */\r\n    setCheckboxState(item, checked) {\r\n        if (checked === true) {\r\n            $(item).find('input').attr(\"checked\", \"checked\");\r\n        } else {\r\n            $(item).find('input').removeAttr(\"checked\");\r\n        }\r\n    }\r\n\r\n    createCustomCheckbox(id, value) {\r\n        return '<div class=\"custom-control custom-checkbox\">' +\r\n            '<input type=\"checkbox\" class=\"custom-control-input\" id=\"' + id + '\">' +\r\n            '<label class=\"custom-control-label\" for=\"' + id + '\">' + value + '</label>' +\r\n            '</div>'\r\n    }\r\n\r\n    /**\r\n     * Assembles all parts of the component together\r\n     * */\r\n    buildComponent() {\r\n\r\n        // Initializes search bar if needed\r\n        if (this.valuesArray.length >= 5) {\r\n            this.dynamicSelect.append(this.searchBar);\r\n        }\r\n\r\n        // Appends each item to dynamicSelectBody\r\n        this.items.forEach(item => this.dynamicSelectBody.append(item));\r\n\r\n        // Assembles the parts\r\n        this.dynamicSelect.append(this.dynamicSelectBody);\r\n\r\n\r\n    }\r\n\r\n    /**\r\n     * Triggers the checkbox when clicking on any part of .item\r\n     * */\r\n    bindCheckingOnItemClick() {\r\n        this.items.forEach(item => {\r\n            const jItem = $(item);\r\n            jItem.find('label').on('click', event => {\r\n                event.preventDefault();\r\n            });\r\n            jItem.on('mouseup', () => {\r\n                const input = jItem.find('input');\r\n                input.attr('checked', !input.attr('checked'));\r\n            });\r\n        });\r\n    }\r\n\r\n    bindToCheckboxTrigger() {\r\n\r\n        this.items.forEach((item, index) => {\r\n            const jItem = $(item);\r\n\r\n            jItem.on('mouseup', () => {\r\n                const checkbox = jItem.find('input');\r\n                const checked = checkbox.attr('checked');\r\n                const id = checkbox.attr('id');\r\n\r\n                this.selectedItems[index] = (checked === 'checked') ? id : null;\r\n\r\n                this.emmitFilterValueChange({\r\n                    selected: this.selectedItems.filter(item => item),\r\n                    options: this.filterOptions\r\n                });\r\n            });\r\n        });\r\n\r\n    }\r\n\r\n    emmitFilterValueChange(values = {}) {\r\n        $(this.dynamicSelect).trigger('filterValueChange', values);\r\n    }\r\n\r\n    getInitiallySelectedItems() {\r\n        const initiallySelectedItems = [];\r\n        this.valuesArray.forEach((item, index) => {\r\n            if (item.checked) {\r\n                initiallySelectedItems[index] = item.id\r\n            }\r\n        });\r\n        return initiallySelectedItems;\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter/multi-select-filter.js?");

			/***/
}),

/***/ "./src/js/components/dynamic-filter/text-filter.js":
/*!*********************************************************!*\
  !*** ./src/js/components/dynamic-filter/text-filter.js ***!
  \*********************************************************/
/*! exports provided: TextFilter */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"TextFilter\", function() { return TextFilter; });\n/* harmony import */ var _filter_base__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./filter-base */ \"./src/js/components/dynamic-filter/filter-base.js\");\n\r\n\r\nclass TextFilter extends _filter_base__WEBPACK_IMPORTED_MODULE_0__[\"FilterBase\"] {\r\n\r\n    constructor(targetObject,\r\n                positioningOptions = {placement: 'bottom-start'},\r\n                filterOptions) {\r\n\r\n        filterOptions.textEmitEventTimeout = filterOptions.textEmitEventTimeout || 500;\r\n        super(targetObject, positioningOptions, filterOptions);\r\n\r\n        /**\r\n         * Container for search bar.\r\n         * */\r\n        this.searchBar = $(this.createSearchBar())[0];\r\n        this.createFilter();\r\n    }\r\n\r\n    /**\r\n     * Creates list type of filter with search and event emitting\r\n     * */\r\n    createFilter() {\r\n        this.buildComponent();\r\n        this.bindToInput();\r\n    }\r\n\r\n    /**\r\n     * Assembles all parts of the component together\r\n     * */\r\n    buildComponent() {\r\n        this.dynamicSelect.append(this.searchBar)\r\n    }\r\n\r\n    emitFilterValueChange(values = {}) {\r\n        $(this.dynamicSelect).trigger('filterValueChange', values);\r\n    }\r\n\r\n    bindToInput() {\r\n        const input = $(this.searchBar).find('input');\r\n        let timeout = 0;\r\n\r\n        input.on('input', () => {\r\n\r\n            clearTimeout(timeout);\r\n            timeout = setTimeout(\r\n                () => {\r\n                    this.emitFilterValueChange({\r\n                        value: input.val(),\r\n                        options: this.filterOptions\r\n                    });\r\n                }, this.filterOptions.textEmitEventTimeout\r\n            );\r\n        });\r\n    }\r\n\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter/text-filter.js?");

			/***/
}),

/***/ "./src/js/components/info-tooltip.js":
/*!*******************************************!*\
  !*** ./src/js/components/info-tooltip.js ***!
  \*******************************************/
/*! exports provided: TooltipModule */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"TooltipModule\", function() { return TooltipModule; });\n/* harmony import */ var _iso_tooltip__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./iso-tooltip */ \"./src/js/components/iso-tooltip.js\");\n\r\n\r\nclass TooltipModule {\r\n\r\n    constructor() {\r\n        this.bindToInfoTooltipClass();\r\n    }\r\n\r\n    /**\r\n     * Binds to document 'mouseover' event.\r\n     * When the cursor in on top of an element with class '.info-tooltip', a new IsoTooltip is created.\r\n     * As tooltip content is used the value of property 'data-content'\r\n     * */\r\n    bindToInfoTooltipClass() {\r\n        $(document).on('mouseover', function (event) {\r\n            if ($(event.target).hasClass('info-tooltip')) {\r\n                new _iso_tooltip__WEBPACK_IMPORTED_MODULE_0__[\"IsoTooltip\"](event.target, $(event.target).data('content'), {\r\n                    placement: 'bottom-start'\r\n                })\r\n            }\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/info-tooltip.js?");

			/***/
}),

/***/ "./src/js/components/iso-tooltip.js":
/*!******************************************!*\
  !*** ./src/js/components/iso-tooltip.js ***!
  \******************************************/
/*! exports provided: IsoTooltip */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"IsoTooltip\", function() { return IsoTooltip; });\n/**\r\n * Creates a Iso related tooltip.\r\n * @param target        the element on which the tooltip will be appended\r\n * @param textContent   the string content of tooltip\r\n * @param params        list of parameters, inherited from Popper constructor\r\n * */\r\nclass IsoTooltip {\r\n\r\n    constructor(target, textContent, params) {\r\n        const tooltip = target;\r\n        const infoTooltipContentContainer = this.createInfoTooltipContentContainer(textContent);\r\n        this.createTooltip(infoTooltipContentContainer, tooltip, params);\r\n        this.removeWhenMouseNotOver(tooltip, infoTooltipContentContainer);\r\n    }\r\n\r\n    createTooltip(infoTooltipContentContainer, tooltip, params) {\r\n        document.body.appendChild(infoTooltipContentContainer);\r\n        this.popper = new Popper(tooltip, infoTooltipContentContainer, params);\r\n    }\r\n\r\n    removeWhenMouseNotOver(tooltip, infoTooltipContentContainer) {\r\n        $(document).on('mouseover', (nextEvent) => {\r\n            if (nextEvent.target !== tooltip) {\r\n                infoTooltipContentContainer.remove();\r\n            }\r\n        })\r\n    }\r\n\r\n    createInfoTooltipContentContainer(innerText) {\r\n        const infoTooltipContent = document.createElement('div');\r\n        infoTooltipContent.classList = ['info-tooltip-content p-4'];\r\n        infoTooltipContent.innerText = innerText;\r\n        return infoTooltipContent;\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/iso-tooltip.js?");

			/***/
}),

/***/ "./src/js/components/multilevel-collapse.js":
/*!**************************************************!*\
  !*** ./src/js/components/multilevel-collapse.js ***!
  \**************************************************/
/*! exports provided: MultilevelCollapse */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"MultilevelCollapse\", function() { return MultilevelCollapse; });\nclass MultilevelCollapse {\r\n\r\n    constructor() {\r\n        this.bindToCollapseControls();\r\n        this.bindToCollapseParentButton();\r\n        this.bindChangeCollapseHeaderOnToggle();\r\n        this.bindToSpoilerClick();\r\n        collapseChildrenWhenCollapsingParent();\r\n        triggerLinkedCollapse();\r\n    }\r\n\r\n    /**\r\n     * Expands the first collapse child of provided selector\r\n     * */\r\n    static expandCollapse(cardLevelSelector) {\r\n        $(cardLevelSelector).each((index, item) => {\r\n            $(item).find('.collapse').first().collapse('show');\r\n            changeAllCollapseHeaderIcon();\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Binds to collapse controls, which closes or opens all collapses on click\r\n     * */\r\n    bindToCollapseControls() {\r\n        $('#collapse-all').on('click', () => {\r\n            const collapses = $('.collapse');\r\n            collapses.each((index, item) => {\r\n                const collapse = $(item);\r\n                if (!collapse.hasClass('spoiler-collapse')) {\r\n                    collapse.collapse('show');\r\n                    changeAllCollapseHeaderIcon();\r\n                }\r\n            });\r\n        });\r\n\r\n        $('#hide-all').on('click', () => {\r\n            const collapses = $('.collapse');\r\n            collapses.each((index, item) => {\r\n                const collapse = $(item);\r\n                if (!collapse.hasClass('spoiler-collapse')) {\r\n                    collapse.collapse('hide');\r\n                    changeAllCollapseHeaderIcon();\r\n                }\r\n            });\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Finds every collapse, binds to click event and changes the icon whenever the collapse is opened or closed\r\n     * */\r\n    bindChangeCollapseHeaderOnToggle() {\r\n        $('.card-header').each((index, item) => {\r\n\r\n            const button = $(item).find('.btn').first();\r\n            const icon = $(item).find('.material-icons').first();\r\n            let text;\r\n\r\n            button.on('click', () => {\r\n                text = button.hasClass('collapsed')\r\n                    ? 'keyboard_arrow_down'\r\n                    : 'keyboard_arrow_right';\r\n                icon.text(text);\r\n            })\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Binds to .collapse-parent elements and the parent .collapse on click\r\n     * */\r\n    bindToCollapseParentButton() {\r\n        $('.collapse-parent').each((index, button) => {\r\n            $(button).on('click', () => {\r\n                $(button).parents('.collapse').first().collapse('hide');\r\n                changeAllCollapseHeaderIcon();\r\n            });\r\n        });\r\n    }\r\n\r\n    bindToSpoilerClick() {\r\n        $('.spoiler').each((index, item) => {\r\n            $(item).on('click', () => {\r\n                const jItem = $(item);\r\n                const controllerCollapse = jItem.attr('data-target');\r\n                const icon = jItem.find('.material-icons').first();\r\n                const text = $(controllerCollapse).hasClass('show')\r\n                    ? 'add'\r\n                    : 'remove';\r\n                icon.text(text);\r\n            })\r\n        });\r\n    }\r\n\r\n}\r\n\r\n/**\r\n * Checks the state of every collapsible and sets the appropriate header button icon\r\n * */\r\nfunction changeAllCollapseHeaderIcon() {\r\n    $('.card-header').each((index, item) => {\r\n        if (!$(item).find('.btn').hasClass('collapsed')) {\r\n            $(item).find('.material-icons').first().text('keyboard_arrow_down');\r\n        } else {\r\n            $(item).find('.material-icons').first().text('keyboard_arrow_right');\r\n        }\r\n    });\r\n}\r\n\r\n\r\n/**\r\n * This function hides all child collapses when the parent is closed,\r\n * even if they are not in the same container.\r\n * */\r\nfunction collapseChildrenWhenCollapsingParent() {\r\n    const allCollapseTriggers = $('[data-toggle=\"collapse\"]');\r\n\r\n    $(allCollapseTriggers).each((index, trigger) => {\r\n        const assignedCollapse = $(trigger).attr('data-target');\r\n\r\n        $(assignedCollapse).on('hide.bs.collapse', (event) => {\r\n            hideAllChildrenRecursively(event.target);\r\n        });\r\n    });\r\n}\r\n\r\nfunction hideAllChildrenRecursively(collapsible) {\r\n    const jCollapsible = $(collapsible);\r\n    const child = jCollapsible.find('[data-toggle=\"collapse\"]');\r\n\r\n    if (child && child.length !== 0) {\r\n        child.each((index, element) => {\r\n            const collapse = $(element).attr('data-target');\r\n            hideAllChildrenRecursively(element);\r\n            $(collapse).collapse('hide');\r\n        });\r\n    }\r\n}\r\n\r\n/**\r\n * Hides all linked collapses except for the triggered one\r\n * */\r\nfunction triggerLinkedCollapse() {\r\n\r\n    // finding all linked collapse\r\n    const linkedCollapse = $('[data-collapse-linked=\"true\"]');\r\n\r\n    linkedCollapse.each((index, item) => {\r\n        const collapse = $(item);\r\n\r\n        collapse.on('show.bs.collapse', (event) => {\r\n            hideAllLinkedCollapsesExceptCurrent(event.target)\r\n        });\r\n    });\r\n}\r\n\r\nfunction hideAllLinkedCollapsesExceptCurrent(currentCollapse) {\r\n    currentCollapse = $(currentCollapse);\r\n    const linkedCollapse = $('[data-collapse-linked=\"true\"]');\r\n\r\n    linkedCollapse.each((index, item) => {\r\n        const collapse = $(item);\r\n\r\n        if (collapse !== currentCollapse) {\r\n            collapse.collapse('hide');\r\n        }\r\n    });\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/multilevel-collapse.js?");

			/***/
}),

/***/ "./src/js/features/inline-editing-cells.js":
/*!*************************************************!*\
  !*** ./src/js/features/inline-editing-cells.js ***!
  \*************************************************/
/*! exports provided: InlineEditingCells */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"InlineEditingCells\", function() { return InlineEditingCells; });\nclass InlineEditingCells {\r\n\r\n    constructor() {\r\n        this.collapseAllExpandedCells();\r\n        this.bindToInlineEditingCell();\r\n    }\r\n\r\n    /**\r\n     * Gets text from the cell and adds a textarea with it inside\r\n     * @return Object js element (the cell)\r\n     * */\r\n    mutateCell(cell) {\r\n        cell.addClass('expanded-cell');\r\n    }\r\n\r\n    /**\r\n     * Removes the textarea from the cell and adds it's content to the div\r\n     * */\r\n    revertCell(cell) {\r\n        cell.removeClass('expanded-cell');\r\n    }\r\n\r\n    /**\r\n     * Enables the inline editing cell feature\r\n     * */\r\n    bindToInlineEditingCell() {\r\n        this.collapseAllExpandedCells();\r\n\r\n        $('.expandable-cell').each((index, element) => {\r\n            const el = $(element);\r\n            const textarea = el.find('textarea');\r\n\r\n            $(textarea).on('mouseup', () => {\r\n                this.mutateCell(el)\r\n            });\r\n\r\n            $(textarea).on('blur', () => {\r\n                this.revertCell(el);\r\n            })\r\n        });\r\n    }\r\n\r\n    collapseAllExpandedCells() {\r\n        $('expanded-cell').each((index, element) => {\r\n            $(element).removeClass('expanded-cell');\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/features/inline-editing-cells.js?");

			/***/
}),

/***/ "./src/js/index.js":
/*!*************************!*\
  !*** ./src/js/index.js ***!
  \*************************/
/*! no exports provided */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _navigation__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./navigation */ \"./src/js/navigation.js\");\n/* harmony import */ var _features_inline_editing_cells__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./features/inline-editing-cells */ \"./src/js/features/inline-editing-cells.js\");\n/* harmony import */ var _components_info_tooltip__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./components/info-tooltip */ \"./src/js/components/info-tooltip.js\");\n/* harmony import */ var _components_iso_tooltip__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./components/iso-tooltip */ \"./src/js/components/iso-tooltip.js\");\n/* harmony import */ var _components_dynamic_filter_dynamic_filter__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./components/dynamic-filter/dynamic-filter */ \"./src/js/components/dynamic-filter/dynamic-filter.js\");\n/* harmony import */ var _components_multilevel_collapse__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./components/multilevel-collapse */ \"./src/js/components/multilevel-collapse.js\");\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n/**\r\n * Application entry point\r\n * */\r\n$(document).ready(\r\n    () => {\r\n\r\n        // Module declarations\r\n        new _navigation__WEBPACK_IMPORTED_MODULE_0__[\"NavigationModule\"]();\r\n        new _components_info_tooltip__WEBPACK_IMPORTED_MODULE_2__[\"TooltipModule\"]();\r\n        new _components_multilevel_collapse__WEBPACK_IMPORTED_MODULE_5__[\"MultilevelCollapse\"]();\r\n        //addTooltipsForAllLongCells();\r\n\r\n        // Global functions export\r\n        $.Iso = {\r\n            Tooltip: _components_iso_tooltip__WEBPACK_IMPORTED_MODULE_3__[\"IsoTooltip\"],\r\n            DynamicFilter: _components_dynamic_filter_dynamic_filter__WEBPACK_IMPORTED_MODULE_4__[\"DynamicFilter\"],\r\n            InlineEditingCells: _features_inline_editing_cells__WEBPACK_IMPORTED_MODULE_1__[\"InlineEditingCells\"],\r\n        };\r\n    }\r\n);\r\n\r\n\r\n\n\n//# sourceURL=webpack:///./src/js/index.js?");

			/***/
}),

/***/ "./src/js/navigation.js":
/*!******************************!*\
  !*** ./src/js/navigation.js ***!
  \******************************/
/*! exports provided: NavigationModule */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"NavigationModule\", function() { return NavigationModule; });\nclass NavigationModule {\r\n    constructor() {\r\n        this.setInitialStateOfBurgerIcon();\r\n        this.modifyBurgerIconWhenToggleSideMenu();\r\n    }\r\n\r\n    /**\r\n     * Sets the initial direction of the side-menu triggering arrow\r\n     * */\r\n    setInitialStateOfBurgerIcon() {\r\n        const sideMenu = $('aside');\r\n        const burger = $('#isodms-logo');\r\n        if (sideMenu.hasClass('collapsed')) {\r\n            burger.text('arrow_forward');\r\n        } else {\r\n            burger.text('arrow_back');\r\n        }\r\n    }\r\n\r\n    /**\r\n     * Changes the direction of top arrow when opening the side menu\r\n     * */\r\n    modifyBurgerIconWhenToggleSideMenu() {\r\n        $('#isodms-logo').bind(\"click\", () => {\r\n            const sideMenu = $('aside');\r\n            const burger = $('#isodms-logo');\r\n            if (!sideMenu.hasClass('collapsed')) {\r\n                burger.text('arrow_forward');\r\n            } else {\r\n                burger.text('arrow_back');\r\n            }\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/navigation.js?");

			/***/
})

	/******/
});