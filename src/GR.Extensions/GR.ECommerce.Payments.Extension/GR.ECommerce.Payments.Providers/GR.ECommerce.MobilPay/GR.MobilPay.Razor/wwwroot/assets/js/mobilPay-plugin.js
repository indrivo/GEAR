/* Mobil pay plugin
 * A plugin for pay via mobilPay
 *
 * v1.0.0
 *
 * License: MIT Indrivo Srl
 * Author: Lupei Nicolae
 */

class MobilPayPlugin {

    constructor(orderId) {
        this.orderId = orderId;
        this.services = {};
        this.injectServices();
    }

    /*
     * Inject toast notifier 
     */
    injectServices() {
        this.services.toast = new ToastNotifier();
    }

    getConfiguration() {
        const self = this;
        return new Promise((resolve, reject) => {
            $.ajax({
                method: "get",
                url: "/api/MobilPay/GetConfiguration",
                success: function(response) {
                    resolve(response);
                },
                error: function(e) {
                    self.services.toast.notifyErrorList(e);
                    reject(e);
                }
            });
        });
    }

    /*
     * Request encoded data
     */
    requestEncodedData() {
        const self = this;
        return new Promise((resolve, reject) => {
            $.ajax({
                method: "get",
                url: "/api/MobilPay/RequestInvoiceData",
                data: {
                    orderId: self.orderId
                },
                success: function(response) {
                    if (response.is_success) {
                        resolve(response.result);
                    } else reject(response.error_keys);
                },
                error: function(e) {
                    self.services.toast.notifyErrorList(e);
                    reject(e);
                }
            });
        });
    }

    /**
     * Generate form
     */
    async generateForm() {
        const configuration = await this.getConfiguration();
        const data = await this.requestEncodedData();
        this.configuration = configuration;
        const form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", configuration.mobilPayUrl);
        $.each(Object.keys(data), (i, key) => {
            const el = document.createElement("input");
            el.setAttribute("type", "hidden");
            el.setAttribute("name", key);
            el.value = data[key];
            form.appendChild(el);
        });

        return form;
    }
}