class RiskMatrixBuilder {
	/**
	 * Constructor
	 * @param {any} n
	 * @param {any} cellValues
	 * @param {any} impactValues
	 * @param {any} entities
	 * @param {any} this.isEditable
	 */
    constructor(cellValues = [], impactValues = [], entities = {
        cellValueEntity: undefined,
        matrixEntity: undefined,
        impactDefinitionEntity: undefined
    }, isEditable = true) {
        this.n = 0;
        this.cellValues = cellValues;
        this.impactValues = impactValues;
        this.entities = entities;
        this.isEditable = isEditable;
        this.templateId = undefined;
        this.injectServices();
    }

	/*
	 * Inject services
	 */
    injectServices() {
        this.dbContext = new DataInjector();
        this.toast = new ToastNotifier();
        this.templateManager = new TemplateManager();
    }

	/**
	 * Set size
	 * @param {any} size
	 */
    setSize(size = 0) {
        this.n = size;
    }

	/*
	 * Get template
	 */
    getTemplate() {
        return new Promise((resolve, reject) => {
            this.dbContext.getByIdWithIncludesAsync(this.entities.matrixEntity, this.templateId).then(matrixTemplate => {
                if (matrixTemplate.is_success) {
                    let url = new URL(location.href);
                    let templateId = url.searchParams.get("templateId");
                    if (templateId)
                        $(".sub-nav").find("h1").append(` ${matrixTemplate.result.name}`);
                    this.template = matrixTemplate.result;
                    Promise.all(this.getInitialGetPromises()).then(promisesResult => {
                        const size = parseInt(matrixTemplate.result.impactUnitScale);
                        this.setSize(size);
                        this.cellValues = promisesResult[1];
                        this.impactValues = promisesResult[0];
                        resolve();
                    }).catch(e => {
                        console.warn(e);
                        reject(e);
                    });
                } else {
                    this.toast.notifyErrorList(matrixTemplate.error_keys);
                }
            }).catch(e => {
                console.warn(e);
                reject(e);
                this.toast.notify({ heading: "error on load" });
            });
        });
    }

	/*
	 * Get promises for initial
	 */
    getInitialGetPromises() {
        const promises = [];
        promises.push(
            new Promise((resolve, reject) => {
                const filters = [{ parameter: "TemplateId", value: this.templateId }];
                this.dbContext.getAllWhereNoIncludesAsync(this.entities.impactDefinitionEntity, filters).then(x => {
                    if (x.is_success) {
                        resolve(x.result);
                    } else {
                        reject(x.error_keys);
                    }
                }).catch(e => reject(e));
            })
        );
        promises.push(
            new Promise((resolve, reject) => {
                const filters = [{ parameter: "MatrixId", value: this.templateId }];
                this.dbContext.getAllWhereNoIncludesAsync(this.entities.cellValueEntity, filters).then(x => {
                    if (x.is_success) {
                        resolve(x.result);
                    } else {
                        reject(x.error_keys);
                    }
                }).catch(e => reject(e));
            })
        );
        return promises;
    }

	/**
	 * Set template id
	 * @param {any} templateId
	 */
    setTemplateId(templateId) {
        this.templateId = templateId;
    }

	/*
	 * Get matrix
	 */
    getMatrix() {
        const n = this.n;
        const parentContainer = document.createElement("div");
        parentContainer.setAttribute("class", "risk-matrix-template");
        const container = document.createElement("div");
        const matrixTable = document.createElement("table");
        matrixTable.setAttribute("class", "risk-matrix");
        for (let i = 0; i < 3 + n; i++) {
            const row = document.createElement("tr");
            const hSize = i < 3 ? n : 3 + n;
            //config empty td
            if (i < 3) {
                const emptyTd = document.createElement("td");
                emptyTd.setAttribute("class", "empty-cell");
                emptyTd.setAttribute("colspan", 3);
                row.appendChild(emptyTd);
            }

            for (let j = 0; j < hSize; j++) {
                const cell = document.createElement("td");
                if (i >= 3) {
                    const position = n + 3 - i;
                    const yPosition = i - 2;
                    const exist = this.impactValues.find(g => g.parameterType === 1 && g.position === yPosition);

                    if (j < 3) {
                        if (exist) {
                            cell.setAttribute("data-id", exist.id);
                        }
                        cell.setAttribute("data-position", yPosition);
                        cell.setAttribute("class", "y-cell-size");
                        cell.setAttribute("data-type", "probability");
                    }

                    switch (j) {
                        case 0: {
                            const impactNameEl = document.createElement("span");
                            impactNameEl.setAttribute("class", "head-name");
                            if (exist) {
                                impactNameEl.innerText = exist.name;
                                cell.style.background = exist.color;
                            } else
                                impactNameEl.innerText = window.translate("iso_active_value");
                            cell.appendChild(impactNameEl);
                            const scope = this;
                            if (this.isEditable)
                                cell.addEventListener("click", function () {
                                    scope.onHeadClick(this);
                                });
                        } break;
                        case 1: {
                            const span = document.createElement("span");
                            span.setAttribute("class", "head-description");
                            if (exist) {
                                span.innerText = exist.description;
                            } else
                                span.innerText = window.translate("iso_active_value");
                            cell.appendChild(span);
                            if (this.isEditable)
                                this.bindEventsToDescriptionCell(cell);
                        } break;
                        case 2: {
                            //Points
                            if (this.isEditable)
                                this.bindEventsToHeadPointsCell(cell);
                            if (exist) {
                                cell.innerText = exist.points;
                            } else
                                cell.innerText = position;
                        } break;
                        default: {
                            const x = j - 2;
                            const y = i - 2;
                            const findValue = this.cellValues.find(el => el.x === x && el.y === y);
                            cell.setAttribute("class", "value-cell");
                            cell.setAttribute("data-x", x);
                            cell.setAttribute("data-y", y);

                            const cellPoints = document.createElement("span");
                            cellPoints.setAttribute("class", "cell-points");
                            cellPoints.setAttribute("data-type", "cell");

                            const cellName = document.createElement("h2");
                            cellName.setAttribute("class", "cell-name");
                            cellName.setAttribute("data-type", "cell");
                            const scope = this;
                            if (this.isEditable)
                                cell.addEventListener("click", function () {
                                    scope.onCellValueClick(this);
                                });
                            if (this.cellValues.length === 0 || !findValue) {
                                const yValue = n + 3 - i;
                                const xValue = j - 3 + 1;
                                const prod = xValue * yValue;
                                cellPoints.innerText = `${prod}`;
                                cellName.innerText = window.translate("iso_active_value");
                            } else {
                                cell.setAttribute("data-id", findValue.id);
                                cellPoints.innerText = findValue.riskPoints;
                                cellName.innerText = findValue.name;
                                cell.style.background = findValue.color;
                            }
                            cell.appendChild(cellPoints);
                            cell.appendChild(cellName);
                        }
                    }
                } else {
                    const exist = this.impactValues.find(g => g.parameterType === 0 && g.position === j + 1);
                    if (exist) {
                        cell.setAttribute("data-id", exist.id);
                    }
                    cell.setAttribute("class", "x-cell-size");

                    cell.setAttribute("data-position", j + 1);
                    cell.setAttribute("data-type", "impact");
                    switch (i) {
                        case 0: {
                            //Head Name
                            const impactNameEl = document.createElement("span");
                            impactNameEl.setAttribute("class", "head-name");
                            if (exist) {
                                impactNameEl.innerText = exist.name;
                                cell.style.background = exist.color;
                            } else
                                impactNameEl.innerText = window.translate("iso_active_value");
                            cell.appendChild(impactNameEl);
                            const scope = this;
                            if (this.isEditable)
                                cell.addEventListener("click", function () {
                                    scope.onHeadClick(this);
                                });
                        } break;
                        case 1: {
                            //Head desc
                            const span = document.createElement("span");
                            span.setAttribute("class", "head-description");
                            if (exist) {
                                span.innerText = exist.description;
                            } else
                                span.innerText = window.translate("iso_active_value");
                            cell.appendChild(span);
                            if (this.isEditable)
                                this.bindEventsToDescriptionCell(cell);
                        } break;
                        case 2: {
                            //Points
                            if (this.isEditable)
                                this.bindEventsToHeadPointsCell(cell);
                            if (exist) {
                                cell.innerText = exist.points;
                            } else
                                cell.innerText = j + 1;
                        } break;
                    }
                }
                row.appendChild(cell);
            }
            matrixTable.appendChild(row);
        }
        const impactName = document.createElement("h2");
        impactName.innerText = window.translate("iso_matrix_impact");
        container.appendChild(impactName);
        const groupContainer = document.createElement("div");
        groupContainer.setAttribute("class", "matrix-group");

        groupContainer.appendChild(matrixTable);

        const probabilityName = document.createElement("h2");
        probabilityName.innerText = window.translate("iso_risk_probability");
        groupContainer.appendChild(probabilityName);

        container.append(groupContainer);
        parentContainer.appendChild(container);
        return parentContainer;
    }

	/**
	 * Get input
	 * @param {any} conf
	 */
    getInput(conf = {
        type: "text",
        label: "_",
        value: undefined,
        tagName: "input",
        valueProp: "value",
        onClick: 'javascript:void(0)',
        className: ''
    }) {
        const helper = new ST();
        const c = document.createElement("div");
        const inputId = `name_${helper.newGuid()}`;
        const label = document.createElement("label");
        label.innerHTML = conf.label;
        label.setAttribute("for", inputId);
        c.appendChild(label);
        const input = document.createElement(conf.tagName);
        input.id = inputId;
        input[conf.valueProp] = conf.value;
        input.setAttribute("class", "form-control " + conf.className);
        input.setAttribute("onclick", conf.onClick);
        input.setAttribute("type", conf.type);
        input.style.minHeight = "3em";
        c.appendChild(input);
        return c;
    }

	/**
	 * Bind events to description
	 * @param {any} ctx
	 */
    bindEventsToDescriptionCell(ctx) {
        const scope = this;
        $(ctx).on("click", function () {
            const modalId = "headDescription";
            const c = document.createElement("div");
            const textarea = scope.getInput({
                type: "text",
                label: window.translate("description"),
                tagName: "textarea",
                valueProp: "innerHtml"
            });

            c.appendChild(textarea);
            const modalSettings = {
                id: modalId,
                title: "Edit cell values",
                content: c.outerHTML,
                style: {
                    maxWidth: "40%"
                }
            };

            const modal = scope.templateManager.render("template_bootstrapModal", modalSettings);
            $("body").append(modal);
            const desc = $(ctx).find(".head-description").html();
            $(`#${modalId}`).find("textarea").val(desc).on("keydown", function () {
                $(ctx).find(".head-description").html(this.value);
            });
            $(`#${modalId}`).modal("show");
            $(`#${modalId}`).on('hidden.bs.modal', function () {
                const description = $(this).find("textarea").val();
                $(ctx).find(".head-description").html(description);
                scope.saveImpactProbability(ctx, {
                    description: description
                });
                this.remove();
            });
        });
    }

	/**
	 * Bind events to head points
	 * @param {any} ctx
	 */
    bindEventsToHeadPointsCell(ctx) {
        const scope = this;
        $(ctx).on("click", function () {
            const modalId = "headPoints";
            const c = document.createElement("div");
            const textarea = scope.getInput({
                type: "number",
                label: window.translate("iso_risk_show_param_points"),
                tagName: "input",
                valueProp: "value"
            });

            c.appendChild(textarea);
            const modalSettings = {
                id: modalId,
                title: window.translate("iso_matrix_set_points"),
                content: c.outerHTML,
                style: {
                    maxWidth: "40%"
                }
            };

            const modal = scope.templateManager.render("template_bootstrapModal", modalSettings);
            $("body").append(modal);
            const desc = $(ctx).html();
            $(`#${modalId}`).find("input").val(desc).on("keydown", function () {
                $(ctx).html(this.value);
            });
            $(`#${modalId}`).modal("show");
            $(`#${modalId}`).on('hidden.bs.modal', function () {
                const points = $(this).find("input").val();
                $(ctx).html(points);
                scope.saveImpactProbability(ctx, {
                    points: points
                });
                this.remove();
            });
        });
    }

	/**
	 * On cell value click
	 * @param {any} ctx
	 */
    onCellValueClick(ctx) {
        const c = document.createElement("div");
        const pointsBox = this.getInput({
            type: "number",
            label: window.translate("iso_risk_show_param_points"),
            tagName: "input",
            valueProp: "value"
        });

        const nameBox = this.getInput({
            type: "text",
            label: window.translate("name"),
            tagName: "input",
            valueProp: "value"
        });

        const colorBox = this.getInput({
            type: "text",
            label: window.translate("system_color"),
            tagName: "input",
            valueProp: "value",
            onClick: 'openColorPicker(event)',
            className: 'color-input'
        });
        c.appendChild(pointsBox);
        c.appendChild(nameBox);
        c.appendChild(colorBox);
        const modalId = "headCellModal";
        const modalSettings = {
            id: modalId,
            title: window.translate("iso_matrix_edit_cell_value"),
            content: c.outerHTML,
            style: {
                maxWidth: "40%"
            }
        };
        const modal = this.templateManager.render("template_bootstrapModal", modalSettings);
        $("body").append(modal);
        const name = $(ctx).find(".cell-name").html();
        const points = $(ctx).find(".cell-points").html();
        const modalEl = $(`#${modalId}`);
        modalEl.find("input[type='text']").val(name).on("keydown", function () {
            $(ctx).find(".cell-name").html(this.value);
        });
        modalEl.find("input[type='number']").val(points).on("keydown", function () {
            $(ctx).find(".cell-points").html(this.value);
        });
        modalEl.find("input.color-input")
            .val(new ST().rgbToHex(ctx.style.background))
            .css('background-color', new ST().rgbToHex(ctx.style.background))
            .on("change", function () {
                ctx.style.background = $(this).val();
            });
        modalEl.modal("show");
        const scope = this;
        modalEl.on('hidden.bs.modal', function () {
            const jqCtx = $(ctx);
            const color = $(this).find("input.color-input").val();
            const name = $(this).find("input[type='text']").val();
            const riskPoints = $(this).find("input[type='number']").val();
            jqCtx.find(".cell-name").html(name);
            jqCtx.find(".cell-points").html(riskPoints);
            const x = jqCtx.attr("data-x");
            const y = jqCtx.attr("data-y");
            const table = jqCtx.closest("table");
            const probabilityPoints = table.find(`.y-cell-size[data-position='${y}']`).last().text();
            const impactPoints = table.find(`.x-cell-size[data-position='${x}']`).last().text();
            scope.saveCellValues(ctx, { impactPoints, probabilityPoints, x, y, color, name, riskPoints });
            this.remove();
        });
    }

	/**
	 * Save cell values
	 * @param {any} ctx
	 * @param {any} data
	 */
    saveCellValues(ctx, data) {
        const recordId = $(ctx).attr("data-id");
        if (!recordId) {
            const o = {
                ctx: ctx,
                data: data
            };
            this.addNewCellValue(o).then(x => {
                if (x.is_success) {
                    $(ctx).attr("data-id", x.result);
                } else {
                    this.toast.notifyErrorList(x.error_keys);
                }
            }).catch(e => {
                console.warn(e);
            });
        } else {
            const o = {
                recordId: recordId,
                ctx: ctx,
                data: data
            };
            this.updateCellValue(o).then(u => {
                if (u.is_success) {
                    $(ctx).attr("data-id", u.result);
                } else {
                    this.toast.notifyErrorList(u.error_keys);
                }
            }).catch(e => {
                console.warn(e);
            });
        }
    }

	/**
	 * On head click
	 * @param {any} ctx
	 */
    onHeadClick(ctx) {
        const c = document.createElement("div");
        const nameBox = this.getInput({
            type: "text",
            label: window.translate("name"),
            tagName: "input",
            valueProp: "value"
        });

        const colorBox = this.getInput({
            type: "text",
            label: window.translate("system_color"),
            tagName: "input",
            valueProp: "value",
            onClick: 'openColorPicker(event)',
            className: 'color-input'
        });

        c.appendChild(nameBox);
        c.appendChild(colorBox);

        const modalSettings = {
            id: "headCellModal",
            title: window.translate("iso_matrix_edit_cell_value"),
            content: c.outerHTML,
            style: {
                maxWidth: "40%"
            }
        };
        const modal = this.templateManager.render("template_bootstrapModal", modalSettings);
        $("body").append(modal);
        const name = $(ctx).find(".head-name").html();
        $('#headCellModal').find("input[type='text']").val(name).on("keydown", function () {
            $(ctx).find(".head-name").html(this.value);
        });
        $('#headCellModal').find("input.color-input")
            .val(new ST().rgbToHex(ctx.style.background))
            .css('background-color', new ST().rgbToHex(ctx.style.background))
            .on("change", function () {
                ctx.style.background = $(this).val();
            });
        $(`#headCellModal`).modal("show");
        const scope = this;
        $('#headCellModal').on('hidden.bs.modal', function () {
            const color = $(this).find("input.color-input").val();
            const name = $(this).find("input[type='text']").val();
            $(ctx).find(".head-name").html(name);
            const jqCtx = $(ctx);
            const table = jqCtx.closest("table");
            const position = jqCtx.attr("data-position");
            const dataType = jqCtx.attr("data-type");

            const o = {};
            if (dataType == "impact") {
                o.points = table.find(`.x-cell-size[data-position='${position}']`).last().text();
            } else {
                o.points = table.find(`.y-cell-size[data-position='${position}']`).last().text();
            }

            scope.saveImpactProbability(ctx, Object.assign({
                color: color,
                name: name
            }, o));
            this.remove();
        });
    }

	/**
	 * Add new cell value
	 * @param {any} conf
	 */
    addNewCellValue(conf = {
        data: {},
        ctx: {}
    }) {
        const jqCtx = $(conf.ctx);
        const x = parseInt(jqCtx.attr("data-x"));
        const y = parseInt(jqCtx.attr("data-y"));
        return new Promise((resolve, reject) => {
            const filters = [
                { parameter: "x", value: x },
                { parameter: "MatrixId", value: this.templateId },
                { parameter: "y", value: y }
            ];
            this.dbContext.getAllWhereNoIncludesAsync(this.entities.cellValueEntity, filters).then(x => {
                if (x.is_success) {
                    if (x.result.length >= 1) {
                        const o = Object.assign(x.result[0], conf.data);
                        this.dbContext.updateAsync(this.entities.cellValueEntity, o).then(u => {
                            if (u.is_success) {
                                resolve(u.result);
                            } else {
                                reject(u.error_keys);
                            }
                        }).catch(e => reject(e));
                    } else {
                        const o = Object.assign({
                            x: x,
                            y: y,
                            matrixId: this.templateId
                        }, conf.data);
                        this.dbContext.addAsync(this.entities.cellValueEntity, o).then(a => {
                            if (a.is_success) resolve(a);
                            else reject(a.error_keys);
                        }).catch(e => reject(e));
                    }
                } else {
                    reject(x.error_keys);
                }
            }).catch(e => {
                reject(e);
                console.log(e);
            });
        });
    }

	/**
	 * Update cell value
	 * @param {any} conf
	 */
    updateCellValue(conf = {
        recordId: "",
        data: {},
        ctx: {}
    }) {
        return new Promise((resolve, reject) => {
            this.dbContext.getByIdWithIncludesAsync(this.entities.cellValueEntity, conf.recordId).then(h => {
                if (h.is_success) {
                    const o = Object.assign(h.result, conf.data);
                    this.dbContext.updateAsync(this.entities.cellValueEntity, o).then(u => {
                        if (u.is_success) {
                            resolve(u);
                        } else {
                            reject(u.error_keys);
                        }
                    }).catch(e => reject(e));
                } else {
                    reject(h.error_keys);
                }
            }).catch(e => reject(e));
        });
    }

	/**
	 * Save impact probability
	 * @param {any} ctx
	 * @param {any} data
	 */
    saveImpactProbability(ctx, data) {
        const recordId = $(ctx).attr("data-id");
        if (!recordId) {
            const o = {
                ctx: ctx,
                data: data
            };
            this.addNewRecordForImpactProbability(o).then(x => {
                if (x.is_success) {
                    $(ctx).attr("data-id", x.result);
                    this.toast.notify({ heading: window.translate("iso_matrix_save_conf_message"), icon: "success" });
                } else {
                    this.toast.notifyErrorList(x.error_keys);
                }
            }).catch(e => {
                console.warn(e);
                this.toast.notifyErrorList(e);
            });
        } else {
            const o = {
                recordId: recordId,
                ctx: ctx,
                data: data
            };
            this.updateRecordForImpactProbability(o).then(u => {
                if (u.is_success) {
                    $(ctx).attr("data-id", u.result);
                } else {
                    this.toast.notifyErrorList(u.error_keys);
                }
            }).catch(e => {
                console.warn(e);
            });
        }
    }

	/**
	 * Update record for impact or probability
	 * @param {any} conf
	 */
    updateRecordForImpactProbability(conf = {
        recordId: "",
        data: {},
        ctx: {}
    }) {
        return new Promise((resolve, reject) => {
            this.dbContext.getByIdWithIncludesAsync(this.entities.impactDefinitionEntity, conf.recordId).then(h => {
                if (h.is_success) {
                    const o = Object.assign(h.result, conf.data);
                    this.dbContext.updateAsync(this.entities.impactDefinitionEntity, o).then(u => {
                        if (u.is_success) {
                            this.toast.notify({ heading: window.translate("system_inline_saved"), icon: "success" });
                            resolve(u);
                        } else {
                            reject(u.error_keys);
                        }
                    }).catch(e => reject(e));
                } else {
                    reject(h.error_keys);
                }
            }).catch(e => reject(e));
        });
    }

	/**
	 * Add new record for impact and probability
	 * @param {any} conf
	 */
    addNewRecordForImpactProbability(conf = {
        data: {},
        ctx: {}
    }) {
        const jqCtx = $(conf.ctx);
        const position = parseInt(jqCtx.attr("data-position"));
        const typeAttr = jqCtx.attr("data-type");
        const dataType = typeAttr === "impact" ? 0 : 1;
        return new Promise((resolve, reject) => {
            const filters = [
                { parameter: "Position", value: position },
                { parameter: "TemplateId", value: this.templateId },
                { parameter: "ParameterType", value: dataType }
            ];
            this.dbContext.getAllWhereNoIncludesAsync(this.entities.impactDefinitionEntity, filters).then(x => {
                if (x.is_success) {
                    if (x.result.length >= 1) {
                        const o = Object.assign(x.result[0], conf.data);
                        this.dbContext.updateAsync(this.entities.impactDefinitionEntity, o).then(u => {
                            if (u.is_success) {
                                resolve(u.result);
                            } else {
                                reject(u.error_keys);
                            }
                        }).catch(e => reject(e));
                    } else {
                        const o = Object.assign({
                            position: position,
                            parameterType: dataType,
                            templateId: this.templateId,
                            scale: this.n,
                        }, conf.data);

                        this.dbContext.addAsync(this.entities.impactDefinitionEntity, o).then(a => {
                            if (a.is_success) resolve(a);
                            else reject(a.error_keys);
                        }).catch(e => reject(e));
                    }
                } else {
                    reject(x.error_keys);
                }
            }).catch(e => {
                reject(e);
                console.log(e);
            });
        });
    }
}