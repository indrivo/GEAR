$(document).ready(() => {
    const toast = new ToastNotifier();
    const modal = $("#creditCardModal");
    init();
    function init() {
        $("#creditCardBtn").on("click",
            function () {
                modal.modal("show");
            });

        $("#payWithCreditCardBtn").click(function (e) {
            const form = $("#creditCardForm");
            const isValid = form.valid();

            if (!isValid) {
                e.preventDefault();
                return;
            }
            const data = form.serializeArray();
            submitForm(data);
            e.preventDefault();
        });
    }

    function submitForm(params) {
        $.ajax({
            type: "POST",
            url: "/api/CardPayApi/PayOrder",
            data: $.param(params)
        }).done((data) => {
            if (data.is_success) {
                toast.notifySuccess("Info", "Order was payed");
                setTimeout(() => {
                    window.location = `/checkout/success?orderId=${data.result}`;
                }, 1000);
            } else {
                toast.notifyErrorList(data.error_keys);
            }
        }).fail(e => {
            toast.notifyErrorList(e);
        });
    }
});