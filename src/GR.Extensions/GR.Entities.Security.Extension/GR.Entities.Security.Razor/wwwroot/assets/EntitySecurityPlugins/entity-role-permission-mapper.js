$(() => {
    const toast = new ToastNotifier();
    const cards = [];
    $.each($(".card-search"),
        (i, item) => {
            const attr = $(item).data("search").toLowerCase();
            cards.push({
                source: $(item),
                search: attr
            });
        });

    $(".fullControl").on("change", function () {
        const ctx = $(this);
        const isChecked = ctx.is(":checked");
        const checks = ctx.closest(".grantSection").find(".permission");
        $.each(checks,
            (k, v) => {
                if (isChecked) {
                    $(v).attr("disabled", "");
                    v.checked = false;
                }
                else $(v).removeAttr("disabled");
            });
    });

    $("#searchEntity").on("input keypress",
        function () {
            const toFind = $(this).val().toLowerCase();
            if (toFind) {
                const searchResult = cards.filter(x => x.search.indexOf(toFind) !== -1);
                const ignore = cards.filter(function (el) {
                    return !searchResult.includes(el);
                });
                $.each(searchResult,
                    (i, s) => {
                        $(s.source).css("display", "block");
                    });
                $.each(ignore,
                    (i, s) => {
                        $(s.source).css("display", "none");
                    });
            } else {
                $(".card-search").css("display", "block");
            }
        });

    $(".saveConfig").on("click", function () {
        const data = {
            entityId: "",
            roleId: "",
            permissions: []
        };
        const section = $(this).closest(".grantSection");
        data.roleId = section.data("role");
        data.entityId = section.data("entity");
        const checks = section.find("input[type=checkbox]");

        $.each(checks,
            (index, grant) => {
                const isChecked = $(grant).is(":checked");
                if (isChecked) {
                    data.permissions.push($(grant).data("permission"));
                }
            });

        window.loadAsync("/EntitySecurity/SaveEntityMappedPermissions", data, "post")
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
    });
});