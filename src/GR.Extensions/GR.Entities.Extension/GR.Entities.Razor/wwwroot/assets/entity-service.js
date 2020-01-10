class EntityService {
    routes = {
        deleteTable: "/api/Table/DeleteTable",
        scaffoldPagesAndForms: "/Page/Scaffold"
    }

    constructor() {

    }

    /**
     * Delete table by id
     * @param {any} tableId
     */
    deleteTableByIdWithPrompt(tableId) {
        const self = this;
        swal({
            title: "Are you sure?",
            text: "You will not be able to recover this table!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: window.translate("cancel"),
            closeOnConfirm: false
        }).then(() => {
            $.ajax({
                url: self.routes.deleteTable,
                method: "delete",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: {
                    id: tableId
                },
                success: function (respose) {
                    if (respose.is_success) {
                        swal("Deleted!", "Table has been deleted.", "success");
                        window.location.href = '/Table';
                    } else {
                        swal("Fail!", "Api not respond or not have permissions.", "error");
                    }

                },
                error: function (error) {
                    console.warn(error);
                    swal("Fail!", "Api not respond or not have permissions.", "error");
                }
            });
        });
    }

    /**
     * Scaffold pages and forms
     * @param {any} id
     */
    scaffold(id) {
        const self = this;
        if (!id) {
            Swal.fire({
                type: 'error',
                title: 'Oops...',
                text: 'Something went wrong!'
            });
            return;
        }
        swal({
            title: window.translate("scaffold_modal_title"),
            text: window.translate("scaffold_modal_text"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: window.translate("scaffold_modal_confirm"),
            cancelButtonText: window.translate("cancel")
        }).then((result) => {
            if (result.value) {
                location.href = `${self.routes.scaffoldPagesAndForms}?tableId=${id}`;
            }
        });
    }
}