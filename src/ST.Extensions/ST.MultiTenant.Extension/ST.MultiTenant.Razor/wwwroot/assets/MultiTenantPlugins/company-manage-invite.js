$(document).ready(() => {
    $('#roleSelect').select2();
    $("#inviteBtn").click(() => {
        swal.fire(
            {
                title: "Invite new user",
                animation: true,
                showCancelButton: true,
                showLoaderOnConfirm: true,
                confirmButtonText: "Confirm",
                html: `<div class="form-group">
						<label class="form-control-label" for="userEmail">Email</label>
						<input type="email" id="userEmail" class="form-control" placeholder="e-mail">
					</div>
					<div class="form-group">
						<label class="form-control-label" for="roleSelect">Role</label>
						<select id="roleSelect" class="swal2-select form-control" multiple="multiple"></select>
					</div>`,
                onOpen: () => {
                    $('#roleSelect').select2({
                        dropdownCssClass: "increased-z-index",
                        data: load("/CompanyManage/GetRoles").map(roles => ({ id: roles.id, text: roles.name }))
                    });
                },
                preConfirm: () => {
                    const email = document.getElementById('userEmail').value;
                    const selectedRoles = $('#roleSelect').select2('data').map(roleId => roleId.id);
                    const info = {
                        'email': email,
                        'roles': selectedRoles
                    }
                    return new Promise((resolve, reject) => {
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
                    }).then(result => {
                        if (!result.is_success) {
                            Swal.showValidationMessage(
                                `Request failed: ${result.error_keys.map(responseMessage => responseMessage.message)
                                    .join(',')}`
                            );
                        }
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.value) {
                    Swal.fire('Email was sanded');
                }
            });
    });
});