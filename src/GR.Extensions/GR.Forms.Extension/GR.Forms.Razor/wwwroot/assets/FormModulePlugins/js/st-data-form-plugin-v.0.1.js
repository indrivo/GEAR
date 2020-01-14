/* Form plugin
 * A plugin for form render in pages
 *
 * v1.0.0
 *
 * License: MIT Indrivo
 * Author: Lupei Nicolae
 */


// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('Data form plugin require JQuery');
}

const form = new Form();

var formPromise = new Promise((resolve, rejected) => {
    var forms = Array.prototype.filter.call(
        document.getElementsByTagName('form'),
        function (el) {
            return el.getAttribute('db-form') != null;
        }
    );
    resolve(forms);
});

formPromise.then(forms => {
    $.each(forms,
        function (index, form) {
            const formId = $(form).attr("db-form");
            const st = new ST();
            const id = `render_${st.newGuid()}`;
            $(form).attr("id", id);
            $(form).html(null);
            renderForm(formId, id);
        });
});

function renderForm(formId, place) {
    var renderFormPromise = new Promise((resolve, rejected) => {
        const data = form.getFormFronServer(formId);
        resolve(data);
    });

    renderFormPromise.then(data => {
        const formRef = `#${place}`;
        if (data) {
            if (data.is_success) {
                const json = form.cleanJson(data.result);

                try {
                    const formeo = new window.Formeo(
                        {
                            allowEdit: false
                        },
                        JSON.stringify(json));

                    const renderContainer = document.querySelector(formRef);

                    $(function () {
                        formeo.render(renderContainer);
                        const selects = Array.prototype.filter.call(
                            renderContainer.getElementsByTagName('select'),
                            function (el) {
                                return el.getAttribute('table-field-id') != null;
                            }
                        );
                        form.populateSelect(selects);
                        form.checkIfIsEditForm(formRef, formId);
                        $(formRef).find(".f-field-group").addClass("form-group");
                        const inputs = $(formRef).find(".f-field-group");
                        for (let y = 0; y < inputs.length; y++) {
                            const data = $(inputs[y]).html();
                            $(inputs[y]).html(null);
                            $(inputs[y]).append(`<div class="controls"></div>`);
                            $(inputs[y]).find(".controls").html(data);
                        }
                        $(formRef).attr("novalidate", "");
                        $(formRef).prepend("<div><ul class='server-errors'></ul></div>");
                        $("input,select,textarea").not("[type=submit]").addClass("form-control");
                        //convert default datetime fields to pretty picker
                        $('input[type=date]').datepicker({
                            format: 'dd/mm/yyyy'
                        }).addClass("datepicker").attr("type", "text");
                        //attach validations for input fields
                        $("input,select,textarea").not("[type=submit]").jqBootstrapValidation(
                            {
                                submitSuccess: function ($form, event) {
                                    event.preventDefault();
                                    const st = new ST();
                                    let isEdit = $($form).attr("isedit");
                                    let systemFields = [];
                                    if (typeof isEdit !== typeof undefined && isEdit !== false) {
                                        const hiddens = $($form).find("input:hidden");
                                        for (var i = 0; i < hiddens.length; i++) {
                                            systemFields.push({
                                                key: $(hiddens[i]).attr("name"),
                                                value: $(hiddens[i]).val()
                                            });
                                        }
                                        isEdit = true;
                                    } else {
                                        isEdit = false;
                                    }
                                    const serialized = st.serializeToJson($($form));

                                    const obj = form.extractOnlyReferenceFields(place, serialized);
                                    const model = {
                                        data: obj,
                                        formId: formId,
                                        isEdit: isEdit,
                                        systemFields: systemFields
                                    };
                                    const errorsBlock = $($form).find(".server-errors");
                                    errorsBlock.html(null);
                                    $.ajax({
                                        url: "/PageRender/PostForm",
                                        type: "post",
                                        data: {
                                            model: model
                                        },
                                        success: function (data) {
                                            if (data) {
                                                if (data.is_success) {
                                                    location.href = data.result.redirectUrl;
                                                } else {
                                                    for (let i = 0; i < data.error_keys.length; i++) {
                                                        errorsBlock.append(
                                                            `<li>${data.error_keys[i].message}</li>`);
                                                    }
                                                }
                                            } else {
                                                errorsBlock.append(`<li>Server not respond!</li>`);
                                            }
                                        }
                                    });
                                }
                            }
                        );
                    });

                } catch (exp) {
                    console.log(exp);
                }
            }
        } else {
            $.toast({
                heading: "Server unavailable!",
                text: "",
                position: 'bottom-right',
                loaderBg: '#ff6849',
                icon: 'error',
                hideAfter: 3500,
                stack: 6
            });
        }
    });
}