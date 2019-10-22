$(document).ready(() => {
    const validator = new Validator();
    $('#roleSelect').select2({
        theme: 'bootstrap'
    });
    $("#inviteBtn").click(() => {
        swal.fire(
            {
                title: window.translate("system_invite_user"),
                animation: true,
                showCancelButton: true,
                showLoaderOnConfirm: true,
                confirmButtonText: window.translate("system_confirm"),
                html: `<div class="form-group">
						<label class="form-control-label" for="userEmail">${window.translate("email")}</label>
						<input type="email" id="userEmail" class="form-control" placeholder="e-mail">
					</div>
					<div class="form-group">
						<label class="form-control-label" for="roleSelect">${window.translate("roles")}</label>
						<select id="roleSelect" class="swal2-select form-control" multiple="multiple"></select>
					</div>`,
                onOpen: () => {
                    $('#roleSelect').select2({
                        theme: 'bootstrap',
                        dropdownCssClass: "increased-z-index",
                        data: load("/CompanyManage/GetRoles").map(roles => ({ id: roles.id, text: roles.name }))
                    });
                },
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        const email = document.getElementById('userEmail').value;
                        if (!validator.isValidEmail(email)) {
                            reject(window.translate("validator_email_incorrect"));
                        } else {
                            const selectedRoles = $('#roleSelect').select2('data').map(roleId => roleId.id);
                            const info = {
                                'email': email,
                                'roles': selectedRoles
                            };
                            $.ajax({
                                url: "/CompanyManage/InviteNewUserAsync",
                                method: "POST",
                                contentType: "application/json; charset=utf-8",
                                data: JSON.stringify(info),
                                success: function (data) {
                                    resolve(data);
                                },
                                error: function (error) {
                                    reject(error);
                                }
                            });
                        }
                    }).then(result => {
                        if (!result.is_success) {
                            Swal.showValidationMessage(
                                result.error_keys.map(responseMessage => responseMessage.message)
                                    .join(',')
                            );
                        }
                    }).catch(e => {
                        Swal.showValidationMessage(e);
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.value) {
                    Swal.fire(window.translate("system_email_sent"));
                }
            });
    });
});