$(document).ready(function ($) {
    const tableId = '#render_{Identifier}';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).dataTable().fnDestroy();
        $(tableId).dataTable().empty();
    }
    $(tableId).DataTable({
        "language": {
            "url": `http://cdn.datatables.net/plug-ins/1.10.19/i18n/${window.getCookie("language")}.json`
        },
        dom: '<"CustomizeColumns">lBfr<"table-responsive"t>ip',
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
        "fnDrawCallback": function () {
            if ($('.dataTables_paginate .pagination li').length > 3) {
                $('.dataTables_paginate').show();
            } else {
                $('.dataTables_paginate').hide();
            }
        }
    });
});