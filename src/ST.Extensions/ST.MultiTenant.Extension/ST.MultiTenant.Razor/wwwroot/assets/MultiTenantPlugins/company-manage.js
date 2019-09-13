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