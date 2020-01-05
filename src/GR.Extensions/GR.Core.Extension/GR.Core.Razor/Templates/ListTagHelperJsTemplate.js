$(document).ready(function ($) {
    const tableId = '#render_{Identifier}';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).dataTable().fnDestroy();
        $(tableId).dataTable().empty();
    }
    $(tableId).DataTable({
        "language": {
            "url": `http://cdn.datatables.net/plug-ins/1.10.19/i18n/${window.getCookie("language")}.json`,
            "paginate": {
                "previous": '<i class="material-icons">keyboard_arrow_left</i>',
                "next": '<i class="material-icons">keyboard_arrow_right</i>'
            }
        },
        dom: '{Dom}',
        "processing": true,
        "serverSide": true,
        "filter": true,
        "orderMulti": false,
        "destroy": true,
        "ajax": {
            "url": '{DataApi}',
            "type": "{Method}"
        },
        "columns": [
            {RenderColumnsContainer}
            {ListActionsContainer}
        ],
        drawCallback: function () {
            if ($('.table_render_{Identifier} .dataTables_paginate .pagination li').length > 3) {
                $('.table_render_{Identifier} .dataTables_paginate').show();
            } else {
                $('.table_render_{Identifier} .dataTables_paginate').hide();
            }
        }
    });
});