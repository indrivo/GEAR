/* Table inline edit
 * A plugin for inline edit
 *
 * v1.0.0
 *
 * License: MIT Soft-Tehnica Srl
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
	throw new Error('Inline edit plugin require JQuery');
}

$(".dynamic-table")
	.on("draw.dt", function (e, settings, json) {
		$(".inline-edit").on("click", inlineEdit);
	});

function completeEditInline() {
	$(this).off("click", completeEditInline);
	const columns = $(this).parent().parent().parent().find(".data-cell");
	for (let i = 0; i < columns.length; i++) {
		const value = $(columns[i]).find(".data-input").val();
		if (value) {
			$(columns[i]).html(value);
			$(columns[i]).find(".inline-update-event").off("blur", onInputBlur);
		}
	}
	$(this).html("Edit inline");
	$(this).addClass("inline-edit");
	$(this).removeClass("inline-complete");
	$(this).on("click", inlineEdit);
}


function inlineEdit() {
	$(this).html("Complete");
	$(this).removeClass("inline-edit");
	$(this).addClass("inline-complete");
	$(this).off("click", inlineEdit);

	const viewModelId = $(this).attr("data-viewmodel");
	const viewModel = load(`/PageRender/GetViewModelColumnTypes?viewModelId=${viewModelId}`);

	const columns = $(this).parent().parent().parent().find(".data-cell");
	for (let i = 0; i < columns.length; i++) {
		const columnId = $(columns[i]).attr("data-column-id");
		const fieldData = viewModel.result.filter(obj => {
			return obj.columnId === columnId;
		});
		if (fieldData.length > 0) {
			//const viewModelId = $(columns[i]).attr("data-viewmodel");
			const cellId = $(columns[i]).attr("data-id");
			const value = $(columns[i]).html();
			let container = value;
			const tableId = fieldData[0].tableId;
			const propId = fieldData[0].id;
			console.log(fieldData[0].dataType);
			switch (fieldData[0].dataType) {
				case "nvarchar":
					{
						container = `<input data-entity="${tableId}" data-prop-id="${propId}" data-id="${cellId}" class="inline-update-event data-input form-control" value="${value}" type="text" />`;
					}
					break;
				case "int32":
					{
						container = `<input data-entity="${tableId}" data-prop-id="${propId}" data-id="${cellId}" class="inline-update-event data-input form-control" value="${value}" type="number" />`;
					}
					break;
				case "uniqueidentifier":
					{

					}
					break;
			}
			$(columns[i]).html(container);
			$(columns[i]).find(".inline-update-event").on("blur", onInputBlur);
		}
	}
	$(this).on("click", completeEditInline);
}

function onInputBlur() {
	const rowId = $(this).attr("data-id");
	const entityId = $(this).attr("data-entity");
	const propId = $(this).attr("data-prop-id");
	const value = $(this).val();
	const req = load(`/PageRender/SaveTableCellData`,
		{
			entityId: entityId,
			propertyId: propId,
			rowId: rowId,
			value: value
		}, "post");
	if (req.is_success) {
		$.toast({
			heading: 'Data was saved with success',
			text: `You change ${value} value`,
			position: 'bottom-right',
			loaderBg: '#ff6849',
			icon: 'success',
			hideAfter: 3500,
			stack: 6
		});
	} else {
		$.toast({
			heading: req.error_keys[0].message,
			text: "",
			position: 'bottom-right',
			loaderBg: '#ff6849',
			icon: 'error',
			hideAfter: 3500,
			stack: 6
		});
	}
}