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

/***/ "./src/js/components/dynamic-filter.js":
/*!*********************************************!*\
  !*** ./src/js/components/dynamic-filter.js ***!
  \*********************************************/
/*! exports provided: DynamicFilter */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"DynamicFilter\", function() { return DynamicFilter; });\n/**\r\n * Creates a dynamic filter based on provided constructor arguments.\r\n * */\r\nclass DynamicFilter {\r\n\r\n    /**\r\n     *\r\n     * @param type specifies the type of created dynamic filter. There are three types of available filters:\r\n     * 'array', 'date-range' and 'text'. If an invalid argument is provided, returns null.\r\n     * @param targetObject the object on which the component will be appended\r\n     * @param valuesArray array with the values of dynamic filter\r\n     * @param filterOptions object with filter options\r\n     * @param positioningOptions object with `Popper` placement options\r\n     * */\r\n    constructor(type,\r\n                targetObject,\r\n                valuesArray = [],\r\n                filterOptions = {},\r\n                positioningOptions = {placement: 'bottom-start'}) {\r\n        this.type = type;\r\n        this.targetObject = targetObject;\r\n        this.valuesArray = valuesArray;\r\n        this.filterOptions = filterOptions;\r\n        this.filterOptions['filterButtonLabel'] = this.filterOptions['filterButtonLabel'] || 'Filter';\r\n        this.positioningOptions = positioningOptions;\r\n        this.container = this.createCommonContainer();\r\n        this.createFilterPopup();\r\n        this.removeFilterPopupWhenClickAway();\r\n    }\r\n\r\n    createCommonContainer() {\r\n        const dynamicSelectBody = document.createElement('div');\r\n        $(dynamicSelectBody).addClass('dynamic-select-body');\r\n        return dynamicSelectBody;\r\n    }\r\n\r\n    createSearchBarWithItemSearchBinding() {\r\n        // Create search bar\r\n        const searchBar = document.createElement('div');\r\n        searchBar.classList = ['search-bar'];\r\n\r\n        // Create search icon\r\n        const searchIcon = document.createElement('span');\r\n        searchIcon.classList = ['material-icons bg-transparent'];\r\n        searchIcon.innerText = 'search';\r\n        searchBar.appendChild(searchIcon);\r\n\r\n        // Create search input\r\n        const searchInput = document.createElement('input');\r\n        searchInput.type = 'text';\r\n        searchBar.appendChild(searchInput);\r\n\r\n        // Bind filtering to the search input\r\n        $(searchInput).on('keyup', (event) => {\r\n            const value = $(event.currentTarget).val().toLowerCase();\r\n            $('.dynamic-select-body').first().find('.item').filter(function () {\r\n                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);\r\n                $(document).trigger('dynamicFilterSearchInput', {\r\n                    value\r\n                });\r\n            });\r\n\r\n        });\r\n        return searchBar;\r\n    }\r\n\r\n    /**\r\n     * Creates list type of filter with search and event emitting\r\n     * */\r\n    createListFilter() {\r\n\r\n        // Append search bar\r\n        if (this.valuesArray.length >= 5) {\r\n            $(this.container).append(this.createSearchBarWithItemSearchBinding());\r\n        }\r\n\r\n        const listItems = addItems(this.container, this.valuesArray);\r\n\r\n        /**\r\n         * Creates an element with class `.item` for each `valuesArray` item.\r\n         * The elements are appended to the function argument.\r\n         * @return items[] array of .item\r\n         * */\r\n        function addItems(container, values) {\r\n            const items = [];\r\n            values.forEach((value) => {\r\n                // Create items and append them\r\n                const item = document.createElement('div');\r\n                item.classList = ['item'];\r\n                $(item).text(value);\r\n                items.push(item);\r\n                $(container).append(item);\r\n            });\r\n            return items;\r\n        }\r\n\r\n        /**\r\n         * Trigger the `filterSelected` event on `.item` click.\r\n         * The event emits the value of item and destroys the component.\r\n         * */\r\n        function bindToItemsSelect(items, container) {\r\n            $(items).each(function (index, item) {\r\n                $(item).on('click', function () {\r\n                    $(container).trigger('filterValueChange', {value: $(item).text()});\r\n                    $(container).remove();\r\n                })\r\n            })\r\n        }\r\n\r\n        bindToItemsSelect(listItems, this.container);\r\n        return this.container;\r\n    }\r\n\r\n    createTextFilter() {\r\n        $(this.container).append(this.createSearchBarWithItemSearchBinding());\r\n\r\n        const submitButton = document.createElement('button');\r\n        $(submitButton).addClass('btn btn-primary');\r\n        $(submitButton).text(this.filterOptions['filterButtonLabel']);\r\n        $(this.container).append(submitButton);\r\n\r\n        return this.container;\r\n    }\r\n\r\n    factorAFilterBasedOnType() {\r\n        switch (this.type) {\r\n            case 'list': {\r\n                return this.createListFilter()\r\n            }\r\n            case 'text': {\r\n                return this.createTextFilter();\r\n            }\r\n            default:\r\n                break;\r\n        }\r\n    }\r\n\r\n    createFilterPopup() {\r\n        const element = this.factorAFilterBasedOnType();\r\n        $(document.body).append(element);\r\n        new Popper(this.targetObject, element, this.positioningOptions);\r\n    }\r\n\r\n    removeFilterPopupWhenClickAway() {\r\n        $(document).mouseup((event) => {\r\n            // if the target of the click isn't the container nor a descendant of the container\r\n            if (!$(this.container).is(event.target)\r\n                && $(this.container).has(event.target).length === 0) {\r\n                $(this.container).remove();\r\n            }\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-filter.js?");

			/***/
}),

/***/ "./src/js/components/dynamic-select.js":
/*!*********************************************!*\
  !*** ./src/js/components/dynamic-select.js ***!
  \*********************************************/
/*! exports provided: DynamicSelect */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"DynamicSelect\", function() { return DynamicSelect; });\nfunction dynamicSelect(sourceElementId, newElementId) {\r\n\r\n    function createDiv() {\r\n        return document.createElement('div');\r\n    }\r\n\r\n    // Search the source element. If it is not found, return null.\r\n    var sourceElement = $('#' + sourceElementId);\r\n    if (!sourceElement) {\r\n        return null;\r\n    }\r\n\r\n    // Create main container\r\n    var dynamicSelect = createDiv();\r\n    dynamicSelect.id = newElementId;\r\n    dynamicSelect.classList = ['dynamic-select-all'];\r\n\r\n    // Create select body\r\n    var dynamicSelectBody = createDiv();\r\n    dynamicSelectBody.classList = ['dynamic-select-body flex-column'];\r\n    dynamicSelect.appendChild(dynamicSelectBody);\r\n\r\n    // Create search bar\r\n    var searchBar = createDiv();\r\n    searchBar.classList = ['search-bar d-flex'];\r\n    dynamicSelectBody.appendChild(searchBar);\r\n\r\n    // Create search icon\r\n    var searchIcon = document.createElement('span');\r\n    searchIcon.classList = ['material-icons bg-transparent'];\r\n    searchIcon.innerText = 'search';\r\n    searchBar.appendChild(searchIcon);\r\n\r\n    // Create search input\r\n    var searchInput = document.createElement('input');\r\n    searchInput.type = 'text';\r\n    searchBar.appendChild(searchInput);\r\n\r\n    // Bind filtering to the search input\r\n    $(searchInput).on('keyup', function () {\r\n        var value = $(this).val().toLowerCase();\r\n        $('.dynamic-select-body').first().find('.item').filter(function () {\r\n            $(this).toggle($(this).find('input').val().toLowerCase().indexOf(value) > -1)\r\n        });\r\n    });\r\n\r\n    $('#' + sourceElementId + ' option').each(function (index, element) {\r\n\r\n        // Create items and append them\r\n        var item = createDiv();\r\n        item.classList = ['item'];\r\n        dynamicSelectBody.appendChild(item);\r\n\r\n        // Create item input\r\n        var itemInput = document.createElement('input');\r\n        itemInput.type = 'text';\r\n        itemInput.readOnly = true;\r\n        itemInput.value = element.innerText;\r\n        $(itemInput).data('id', element.value);\r\n        item.appendChild(itemInput);\r\n\r\n        // Create buttons and their icons\r\n        var editButton = document.createElement('button');\r\n        editButton.classList = ['edit-button'];\r\n        item.appendChild(editButton);\r\n\r\n        var removeButton = document.createElement('button');\r\n        removeButton.classList = ['remove-button'];\r\n        item.appendChild(removeButton);\r\n\r\n        var editIcon = document.createElement('span');\r\n        editIcon.classList = ['material-icons bg-transparent'];\r\n        editIcon.innerText = 'edit';\r\n        editButton.appendChild(editIcon);\r\n\r\n        var clearIcon = document.createElement('span');\r\n        clearIcon.classList = ['material-icons bg-transparent'];\r\n        clearIcon.innerText = 'clear';\r\n        removeButton.appendChild(clearIcon);\r\n\r\n        // Bind to select input\r\n        $(itemInput).bind('click', function () {\r\n            if (!$(itemInput).is('[readonly]')) {\r\n                return null;\r\n            }\r\n            var itemValue = $(itemInput).data('id');\r\n            $('#' + sourceElementId).val(itemValue);\r\n            dynamicSelect.remove();\r\n        });\r\n\r\n        // Bind to edit button\r\n        $(editButton).bind('click', function () {\r\n            // Switch icons\r\n            if (editIcon.innerText === 'edit') {\r\n                editIcon.innerText = 'checked';\r\n            } else {\r\n                editIcon.innerText = 'edit';\r\n            }\r\n\r\n            // Set option value\r\n            if (!$(itemInput).is('[readonly]')) {\r\n                element.innerText = itemInput.value;\r\n            }\r\n\r\n            // Switch readonly\r\n            $(itemInput).prop('readonly', !$(itemInput).is('[readonly]'));\r\n            if (!$(itemInput).is('[readonly]')) {\r\n                $(itemInput).focus();\r\n            }\r\n        });\r\n\r\n        // Bind to remove button\r\n        $(removeButton).bind('click', function () {\r\n            item.remove();\r\n            element.remove();\r\n        });\r\n\r\n    });\r\n\r\n    document.body.appendChild(dynamicSelect);\r\n    return new Popper(sourceElement[0], dynamicSelect, {\r\n        placement: 'center'\r\n    });\r\n}\r\n\r\nfunction DynamicSelect() {\r\n    bindToAllDynamicSelectElements();\r\n\r\n    /**\r\n     * Function for dynamically adding dynamic select element\r\n     * */\r\n    function bindToAllDynamicSelectElements() {\r\n        function dynamicSelectItem(element) {\r\n            return $('#' + element.id + 'dselect');\r\n        }\r\n\r\n        $('.dynamic-select').each(function (index, element) {\r\n            $(element).bind('click', function () {\r\n                dynamicSelectItem(element).remove();\r\n                dynamicSelect(element.id, element.id + 'dselect');\r\n                $(element).blur();\r\n\r\n                $(document).mouseup(function (event) {\r\n                    // if the target of the click isn't the container nor a descendant of the container\r\n                    if (!dynamicSelectItem(element).is(event.target)\r\n                        && dynamicSelectItem(element).has(event.target).length === 0) {\r\n                        dynamicSelectItem(element).remove();\r\n                    }\r\n                })\r\n            });\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/dynamic-select.js?");

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
/*! exports provided: MultilevelCollapseModule */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"MultilevelCollapseModule\", function() { return MultilevelCollapseModule; });\nclass MultilevelCollapseModule {\r\n\r\n    constructor() {\r\n        this.bindToCollapseControls();\r\n        this.bindToCollapseParentButton();\r\n        this.bindChangeCollapseHeaderOnToggle();\r\n        this.bindToSpoilerClick();\r\n    }\r\n\r\n    /**\r\n     * Expands the first collapse child of provided selector\r\n     * */\r\n    static expandCollapse(cardLevelSelector) {\r\n        $(cardLevelSelector).each((index, item) => {\r\n            $(item).children('.collapse').first().collapse('show');\r\n            this.changeAllCollapseHeaderIcon();\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Checks the state of every collapsible and sets the appropriate header button icon\r\n     * */\r\n    changeAllCollapseHeaderIcon() {\r\n        $('.card-header').each((index, item) => {\r\n            if (!$(item).find('.btn').hasClass('collapsed')) {\r\n                $(item).find('.material-icons').first().text('keyboard_arrow_down');\r\n            } else {\r\n                $(item).find('.material-icons').first().text('keyboard_arrow_right');\r\n            }\r\n        });\r\n    }\r\n\r\n    changeAllSpoilerIcons() {\r\n        $('.spoiler').each((index, item) => {\r\n            const targetId = $(item).data('target');\r\n\r\n            if (!$('' + targetId).hasClass('show')) {\r\n                $(item).find('.material-icons').first().text('remove');\r\n            } else {\r\n                $(item).find('.material-icons').first().text('add');\r\n            }\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Binds to collapse controls, which closes or opens all collapses on click\r\n     * */\r\n    bindToCollapseControls() {\r\n        $('#collapse-all').on('click', () => {\r\n            $('.collapse').collapse('show');\r\n            this.changeAllCollapseHeaderIcon();\r\n            this.changeAllSpoilerIcons();\r\n        });\r\n\r\n        $('#hide-all').on('click', () => {\r\n            $('.collapse').collapse('hide');\r\n            this.changeAllCollapseHeaderIcon();\r\n            this.changeAllSpoilerIcons();\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Finds every collapse, binds to click event and changes the icon whenever the collapse is opened or closed\r\n     * */\r\n    bindChangeCollapseHeaderOnToggle() {\r\n        // find every card header\r\n        $('.card-header').each((index, item) => {\r\n            // binds to .btn click event\r\n            $(item).find('.btn').first().on('click', () => {\r\n                if ($(item).find('.btn').hasClass('collapsed')) {\r\n                    $(item).find('.material-icons').first().text('keyboard_arrow_down');\r\n                } else {\r\n                    $(item).find('.material-icons').first().text('keyboard_arrow_right');\r\n                }\r\n            })\r\n        });\r\n    }\r\n\r\n    /**\r\n     * Binds to .collapse-parent elements and the parent .collapse on click\r\n     * */\r\n    bindToCollapseParentButton() {\r\n        $('.collapse-parent').each((index, button) => {\r\n            $(button).on('click', () => {\r\n                $(button).parents('.collapse').first().collapse('hide');\r\n                this.changeAllCollapseHeaderIcon();\r\n            });\r\n        });\r\n    }\r\n\r\n    bindToSpoilerClick() {\r\n        $('.spoiler').each((index, item) => {\r\n            $(item).on('click', () => {\r\n                this.changeAllSpoilerIcons();\r\n            })\r\n        });\r\n    }\r\n\r\n}\r\n\r\n\n\n//# sourceURL=webpack:///./src/js/components/multilevel-collapse.js?");

			/***/
}),

/***/ "./src/js/components/tables.js":
/*!*************************************!*\
  !*** ./src/js/components/tables.js ***!
  \*************************************/
/*! exports provided: addTooltipsForAllLongCells */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"addTooltipsForAllLongCells\", function() { return addTooltipsForAllLongCells; });\n/* harmony import */ var _iso_tooltip__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./iso-tooltip */ \"./src/js/components/iso-tooltip.js\");\n\r\n\r\nfunction addTooltipsForAllLongCells() {\r\n    $(document).on('mouseover', (event) => {\r\n        const target = $(event.target);\r\n        if (target.is('td div, th div')\r\n            && target.text().trim().length > 64) {\r\n            new _iso_tooltip__WEBPACK_IMPORTED_MODULE_0__[\"IsoTooltip\"](target[0], target.text(), {\r\n                placement: 'bottom-start',\r\n                positionFixed: true\r\n            })\r\n        }\r\n    });\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/components/tables.js?");

			/***/
}),

/***/ "./src/js/index.js":
/*!*************************!*\
  !*** ./src/js/index.js ***!
  \*************************/
/*! no exports provided */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _navigation__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./navigation */ \"./src/js/navigation.js\");\n/* harmony import */ var _components_dynamic_select__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./components/dynamic-select */ \"./src/js/components/dynamic-select.js\");\n/* harmony import */ var _inline_editing_cells__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./inline-editing-cells */ \"./src/js/inline-editing-cells.js\");\n/* harmony import */ var _components_info_tooltip__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./components/info-tooltip */ \"./src/js/components/info-tooltip.js\");\n/* harmony import */ var _components_iso_tooltip__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./components/iso-tooltip */ \"./src/js/components/iso-tooltip.js\");\n/* harmony import */ var _components_tables__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./components/tables */ \"./src/js/components/tables.js\");\n/* harmony import */ var _components_multilevel_collapse__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./components/multilevel-collapse */ \"./src/js/components/multilevel-collapse.js\");\n/* harmony import */ var _components_dynamic_filter__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./components/dynamic-filter */ \"./src/js/components/dynamic-filter.js\");\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n/**\r\n * Application entry point\r\n * */\r\n$(document).ready(\r\n    () => {\r\n\r\n        // Module declarations\r\n        new _navigation__WEBPACK_IMPORTED_MODULE_0__[\"NavigationModule\"]();\r\n        Object(_components_dynamic_select__WEBPACK_IMPORTED_MODULE_1__[\"DynamicSelect\"])();\r\n        new _inline_editing_cells__WEBPACK_IMPORTED_MODULE_2__[\"InlineEditingCells\"]();\r\n        new _components_info_tooltip__WEBPACK_IMPORTED_MODULE_3__[\"TooltipModule\"]();\r\n        new _components_multilevel_collapse__WEBPACK_IMPORTED_MODULE_6__[\"MultilevelCollapseModule\"]();\r\n        Object(_components_tables__WEBPACK_IMPORTED_MODULE_5__[\"addTooltipsForAllLongCells\"])();\r\n\r\n        // Global functions export\r\n        $.Iso = {\r\n            Tooltip: _components_iso_tooltip__WEBPACK_IMPORTED_MODULE_4__[\"IsoTooltip\"],\r\n            DynamicFilter: _components_dynamic_filter__WEBPACK_IMPORTED_MODULE_7__[\"DynamicFilter\"]\r\n        };\r\n    }\r\n);\r\n\r\n\r\n\n\n//# sourceURL=webpack:///./src/js/index.js?");

			/***/
}),

/***/ "./src/js/inline-editing-cells.js":
/*!****************************************!*\
  !*** ./src/js/inline-editing-cells.js ***!
  \****************************************/
/*! exports provided: InlineEditingCells */
/***/ (function (module, __webpack_exports__, __webpack_require__) {

			"use strict";
			eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"InlineEditingCells\", function() { return InlineEditingCells; });\nclass InlineEditingCells {\r\n\r\n    constructor() {\r\n        this.bindToInlineEditingCell();\r\n        this.isMutated = false;\r\n    }\r\n\r\n    /**\r\n     * Gets text from the cell and adds a textarea with it inside\r\n     * @return Object js element (the cell)\r\n     * */\r\n    mutateCell(cell) {\r\n        if (!$(cell).is('td.inline-editing-cell') || this.isMutated) {\r\n            return;\r\n        }\r\n        // Storing cell text in a variable and removing it from the cell\r\n        const cellContent = $(cell).text().trim();\r\n        $(cell).text('');\r\n        $(cell).addClass('inline-editing-cell-container');\r\n        // Append textarea to the cell\r\n        $(cell).append('<textarea id=\"inline-editing-textarea\">' + cellContent + '</textarea>');\r\n        $(cell).find('textarea').focus();\r\n        this.isMutated = true;\r\n        return cell;\r\n    }\r\n\r\n    /**\r\n     * Removes the textarea from the cell and adds it's content to the div\r\n     * */\r\n    revertCell(cell) {\r\n        if (!$(cell).is('td.inline-editing-cell')) {\r\n            return;\r\n        }\r\n\r\n        const textarea = $(cell).find('textarea');\r\n\r\n        // Getting textarea text\r\n        const textareaContent = textarea.val();\r\n\r\n        // Removing class\r\n        $(cell).removeClass('inline-editing-cell-container');\r\n\r\n        // Creating .data-cell div\r\n        const dataCell = document.createElement('div');\r\n        $(dataCell).addClass('data-cell');\r\n\r\n        // Placing the text in the cell\r\n        $(dataCell).text(textareaContent);\r\n\r\n        // Placing the cell in the td\r\n        $(cell).append(dataCell);\r\n\r\n        // Removing textarea from the dom\r\n        textarea.remove();\r\n        this.isMutated = false;\r\n    }\r\n\r\n    /**\r\n     * Enables the inline editing cell feature\r\n     * */\r\n    bindToInlineEditingCell() {\r\n        $(document).on('click', (event) => {\r\n            let cell;\r\n            const isTargetElement = $(event.target).is('.inline-editing-cell')\r\n                || $(event.target).parent().is('.inline-editing-cell');\r\n            if (isTargetElement) {\r\n                const targetElement = $(event.target).is('.inline-editing-cell')\r\n                    ? event.target : $(event.target).parent()[0];\r\n                cell = this.mutateCell(targetElement);\r\n            }\r\n            $(document).mouseup((event) => {\r\n                // if the target of the click isn't the container nor a descendant of the container\r\n                if (!$(cell).find('textarea').is(event.target)) {\r\n                    this.revertCell(cell);\r\n                }\r\n            });\r\n        });\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/js/inline-editing-cells.js?");

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