class GearCommerce {
    constructor() {

    }

    /*
     * Get subscription plans
     */
    getSubscriptions() {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: "/api/Products/GetSubscriptionPlans",
                success: function (response) {
                    if (response.is_success) {
                        resolve(response.result);
                    } else reject(response.error_keys);
                },
                error: function (err) {
                    reject(err);
                }
            });
        });
    }
}