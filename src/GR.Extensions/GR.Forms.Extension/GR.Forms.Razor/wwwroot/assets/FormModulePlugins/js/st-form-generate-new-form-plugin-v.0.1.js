//Functions
$(function () {
    //Declarations
    let renderContainer = document.querySelector('.render-form');
    var fr = new Form();
    var s = new ST();
    let editing = true;
    let toggleEdit = document.getElementById('renderForm');
    let viewDataBtn = document.getElementById('viewData');
    let SaveDataBtn = document.getElementById('saveData');
    let resetDemo = document.getElementById('reloadBtn');


    const id = getTableId();
    init(id);

    function init(id) {
        $.ajax({
            url: "/api/form/GetTableFields",
            method: "get",
            data: { tableId: id },
            success: function (data) {
                $.when(fr.generateJsonForm(data)).then(function () {
                    startFormEditor();
                });
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

    /**
     * Get table id from URL
     * */
    function getTableId() {
        const url = location.href;
        const parsedUrl = new URL(url);
        return parsedUrl.searchParams.get("tableId");
    }

    /**
     * Get form id from URL
     * */
    function getFormTypeId() {
        const url = location.href;
        const parsedUrl = new URL(url);
        const id = parsedUrl.searchParams.get("formType");
        return id;
    }

    function startFormEditor() {
        //fr.printJson();
        const tableId = getTableId();
        const former = new window.Formeo(fr.getOptions('.build-form', tableId), fr.get());
        fr.registerChangeRefEvent(former);
        document.querySelector("#RenderForm").onclick = evt => {
            if (!editing) {
                former.render(renderContainer);
            }

            return editing = !editing;
        };

        document.querySelector("#EditForm").onclick = evt => {
            return editing = !editing;
        };
        resetDemo.onclick = function () {
            window.sessionStorage.removeItem('formData');
            location.reload();
        };

        viewDataBtn.onclick = evt => {
            console.log(former.formData);
        };

        $("#form").on("submit", function (e) {
            let form = JSON.parse(former.formData);
            form = fr.attrsToString(form);
            $.ajax({
                url: "/api/Form/CreateNewForm",
                method: "post",
                data: {
                    form: form,
                    tableId: getTableId(),
                    formTypeId: getFormTypeId(),
                    name: $("#form_name").val(),
                    description: $("#form_description").val(),
                    postUrl: $("#form_postUrl").val(),
                    redirectUrl: $("#form_redirectUrl").val()
                },
                success: function (data) {
                    if (data) {
                        if (data.is_success) {
                            location.href = "/Form";
                        }
                        else {
                            $.toast({
                                heading: data.error_keys[0].message,
                                text: "",
                                position: 'top-right',
                                loaderBg: '#ff6849',
                                icon: 'error',
                                hideAfter: 3500,
                                stack: 6
                            });
                            console.log(data);
                        }
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        });
        SaveDataBtn.onclick = evt => {
            const formObj = $("#form_name");
            const formName = formObj.val();
            if (formName === "") {
                formObj.focus();
                return;
            }
            fr.data = JSON.parse(former.formData);
            $("#form").submit();
        };
    }
});
