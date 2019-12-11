const notif = new ToastNotifier();
const citySelect = $("#CityId");
$(document).ready(() => {
    $('.dropify').dropify();
    $("#CountryId").select2({
        theme: 'bootstrap'
    });
    citySelect.select2({
        theme: 'bootstrap',
        placeholder: {
            id: '',
            text: window.translate("system_select_city")
        }
    });

    $("#CountryId").change(function () {
        const selectedValue = this.value;
        if ($("#CountryId").prop('selectedIndex') > 0) {
            loadCitiesByCountryId(selectedValue);
        } else {
            citySelect.prop("disabled", true);
            citySelect.empty();
        }
    });
});

function loadCitiesByCountryId(selectedValue, cityId = null) {
    $.ajax({
        type: "GET",
        url: `/Users/GetCityByCountryId/?countryId=${selectedValue}`
    }).done((response) => {
        if (response.is_success) {
            citySelect.empty();
            citySelect.prop("disabled", false);
            for (let city of response.result) {
                citySelect.append(new Option(city.text, city.value, false, city.selected));
            }
            if (cityId) {
                citySelect.val(cityId).trigger('change');
            }
        } else {
            notif.notifyErrorList(response.error_keys);
        }
    }).fail(() => {
        notif.notify({ heading: "Error", text: window.translate("system_internal_error") });
    });
}

class userRolesEdit {
    constructor() {
        this.loadRoles();
    }
    ajaxRequest(requestUrl, requestType, requestData) {
        const baseUrl = '/api/Roles';
        return new Promise((resolve, reject) => {
            $.ajax({
                url: baseUrl + requestUrl,
                type: requestType,
                data: requestData,
                success: (data) => {
                    if (Array.isArray(data)) {
                        resolve(data);
                    }
                    else {
                        if (data.is_success) {
                            resolve(data.result);
                        } else if (!data.is_success) {
                            reject(data.error_keys);
                        } else {
                            resolve(data);
                        }
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    getRoles() {
        const requestUrl = '/GetAllRolesAsync';
        return this.ajaxRequest(requestUrl, 'get', null);
    }

    getUserRoles(id) {
        const requestUrl = '/GetUserRoles';
        return this.ajaxRequest(requestUrl, 'get', { userId: id });
    }

    changeUserRoles(userRoles) {
        const requestUrl = '/ChangeUserRoles';
        return this.ajaxRequest(requestUrl, 'post', userRoles);
    }

    roles = [];
    loadRoles() {
        const scope = this;
        return new Promise((resolve, reject) => {
            if (scope.roles.length > 0) {
                resolve(scope.roles);
            } else {
                scope.getRoles().then(result => {
                    scope.roles = result;
                    resolve(scope.roles);
                }).catch(e => {
                    console.warn(e);
                    notif.notifyErrorList(e);
                });
            }
        });
    }
}
class modalRoles {
    constructor() {
        this.userRolesManager = new userRolesEdit();
        this.editUserRolesModal = $.templates("#edit-user-roles-modal");
        this.modal = $('.company-manage-modal');
    }

    getUserById = id => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/api/Users/GetUserById',
                type: 'get',
                data: { userId: id },
                success: (data) => {
                    if (Array.isArray(data)) {
                        resolve(data);
                    }
                    else {
                        if (data.is_success) {
                            resolve(data.result);
                        } else if (!data.is_success) {
                            reject(data.error_keys);
                        } else {
                            resolve(data);
                        }
                    }
                },
                error: (e) => {
                    reject(e);
                }
            });
        });
    }

    findObjectsByProperties = (array, properties) => {
        return array.filter(obj => {
            let response = true;
            properties.forEach(property => {
                const propertyFound = obj[property.name] === property.value;
                response = response && propertyFound;
            });
            return response;
        });
    }

    addCheckedPropToRoles = userRoles => {
        const scope = this;
        return scope.userRolesManager.loadRoles().then(roles => {
            $.each(roles, function (index, value) {
                const properties = [
                    {
                        name: 'id',
                        value: value.id
                    }
                ];
                const matchedRole = scope.findObjectsByProperties(userRoles, properties)[0];

                if (matchedRole) {
                    value.checked = true;
                }
                else {
                    value.checked = false;
                }
            });
            return roles;
        }).catch(e => {
            notif.notifyErrorList(e);
        });
    }

    updateUserRoles = (config) => {
        const scope = this;
        scope.userRolesManager.changeUserRoles(config).then(() => {
            $('#render_manageCompanyUsers').DataTable().draw(false);
        }).catch(e => {
            notif.notifyErrorList(e);
        });
    }

    getRolesFromCheckboxes = () => {
        let roles = [];
        $.each(this.modal.find('.roles-list .custom-checkbox'), function () {
            const checkbox = $(this).find('input[type="checkbox"]');
            if (checkbox.is(':checked')) {
                roles.push(checkbox.attr('id'));
            }
        });
        return roles;
    }

    addModalEvents = userId => {
        const scope = this;
        scope.modal.find('#edit-user-roles').submit(function (e) {
            e.preventDefault();
            const config = {
                userId,
                roles: scope.getRolesFromCheckboxes(),
            }
            scope.updateUserRoles(config);
            scope.modal.modal('hide');
        });
    }

}

const changeRoles = userIdFromCall => {
    const helperClass = new modalRoles();
    helperClass.userRolesManager.getUserRoles(userIdFromCall).then(userRoles => {
        helperClass.addCheckedPropToRoles(userRoles).then(newRoles => {
            helperClass.getUserById(userIdFromCall).then(user => {
                const htmlOutput = helperClass.editUserRolesModal.render({ roles: newRoles }, user);
                helperClass.modal.html(htmlOutput);
                helperClass.addModalEvents(userIdFromCall);
                helperClass.modal.modal('show');
            })
        });
    });

}