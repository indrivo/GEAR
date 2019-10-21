const toast = new ToastNotifier();
const cms = [];

//TODO: Support for razor
//CodeMirror.defineMIME("xml", {
//    name: "xml",
//    atAnnotations: true,
//    atStrings: true,
//    keywords: keywords("abstract as base bool break byte case catch char checked class const continue decimal" +
//        " default delegate do double else enum event explicit extern false finally fixed float for" +
//        " foreach goto if implicit in int interface internal is lock long namespace new null object" +
//        " operator out override params private protected public readonly ref return sbyte sealed short" +
//        " sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked" +
//        " unsafe ushort using virtual void volatile while add alias ascending descending dynamic from get" +
//        " global group into join let orderby partial remove select set value var yield")
//});

function initCms() {
    const templates = document.querySelectorAll(".templateValue");
    $.each(templates,
        (index, item) => {
            var editor = CodeMirror.fromTextArea(item,
                {
                    lineNumbers: true,
                    mode: "xml",
                    keyMap: "sublime",
                    autoCloseBrackets: true,
                    matchBrackets: true,
                    showCursorWhenSelecting: true,
                    theme: "monokai",
                    tabSize: 2
                });

            //editor.setValue($(item).parent().find(".initialValue").val());

            editor.on('change',
                function () {
                    const value = editor.getValue();
                    const previewContainer = $(item).closest(".sections")
                        .find(".templatePreview");
                    previewContainer.html(value);
                    editor.save();
                });

            editor.setSize(null, 400);
            cms.push(editor);
        });
}

$(() => {
    initCms();
    $('.accordion').on('show.bs.collapse',
        function () {
            window.setTimeout(() => {
                $.each(cms, (i, d) => {
                    d.refresh();
                });
            }, 400);
        });

    $(".saveConfig").on("click",
        function () {
            const data = {
                subject: "",
                template: "",
                event: "",
                roles: []
            };
            const section = $(this).closest(".eventSection");
            data.event = section.data("event");
            data.template = section.find(".templateValue").val();
            data.subject = section.find(".notificationSubject").val();
            if (!data.template) return toast.notify({ heading: "Error", text: "Template can't be null!" });
            if (!data.subject) return toast.notify({ heading: "Error", text: "Subject can't be null!" });
            const roles = section.find(".roles").find("input[type=checkbox]");
            $.each(roles,
                (index, role) => {
                    const isChecked = $(role).is(":checked");
                    if (isChecked) {
                        data.roles.push($(role).data("id"));
                    }
                });
            window.loadAsync("/NotificationSubscriptions/SaveNotificationSubscription", data, "post")
                .then(serverResponse => {
                    if (serverResponse.is_success) {
                        toast.notify({
                            heading: "Info",
                            text: window.translate("system_inline_saved"),
                            icon: "success"
                        });
                    } else {
                        toast.notifyErrorList(serverResponse.error_keys);
                    }
                }).catch(e => {
                    console.warn(e);
                    toast.notify({ heading: "Error", text: "Something went wrong!" });
                });
            return 0;
        });
});