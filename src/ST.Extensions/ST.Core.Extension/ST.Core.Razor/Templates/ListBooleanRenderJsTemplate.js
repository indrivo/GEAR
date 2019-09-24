{
    data: null,
        "render": function (data, type, row, meta) {
            return ` <div style="margin-bottom: 1em;" class="custom-control custom-checkbox">
                         <input id="{ColumnName}" disabled type="checkbox" ${row['{ApiIdentifier}'] ? "checked='checked'" : ""} class="custom-control-input vis-check">
                          <label class="custom-control-label" for="{ColumnName}"></label>
                     </div>`;
        }
},