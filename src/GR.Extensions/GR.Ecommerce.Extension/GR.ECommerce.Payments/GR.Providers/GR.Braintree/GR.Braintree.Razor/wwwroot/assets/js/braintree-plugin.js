/* Braintree plugin
 * A plugin for pay via Braintree
 *
 * v1.0.0
 *
 * License: MIT Indrivo Srl
 * Author: Lupei Nicolae
 */

class BraintreePlugin {
    constructor(options = {
        clientToken: "",
        invokeButtonSelector: "",
        url: ""
    }) {
        this.clientToken = options.clientToken;
        this.invokeButtonSelector = options.invokeButtonSelector;
        this.button = document.querySelector(options.invokeButtonSelector);
        this.url = options.url;
    }

    /**
     * Init
     */
    initAsync() {
        const self = this;
        return new Promise((resolve, reject) => {
            braintree.dropin.create({
                authorization: self.clientToken,
                container: '#dropin-container'
            }, function (createErr, instance) {
                console.log(instance);
                    console.log(createErr);
                self.button.addEventListener('click', function () {
                    instance.requestPaymentMethod(function (requestPaymentMethodErr, payload) {
                        resolve({
                            requestPaymentMethodErr: requestPaymentMethodErr,
                            payload: payload
                        });
                    });
                });
            });
        });
    }

    /**
     * Pay order
     * @param {any} order
     */
    payAsync(order = "", payload = {}) {
        const self = this;
        return new Promise((resolve, reject) => {
            $.ajax({
                type: 'post',
                url: self.url,
                data: {
                    orderId: order,
                    nonce: payload.nonce
                }
            })
                .done(function (data) {
                    resolve(data.result);
                })
                .fail(function (errors) {
                    $('#alertBraintree').html(errors.responseText);
                    $('.alert').toggle();
                    reject(errors.responseText);
                    console.warn(errors.responseText);
                })
                .always(function () { });
        });
    }
}