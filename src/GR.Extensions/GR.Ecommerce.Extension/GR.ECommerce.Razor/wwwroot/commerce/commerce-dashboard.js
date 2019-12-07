"use strict";

window.onload = function () {
    const picker = $('.date-rage').daterangepicker({
        opens: 'left'
    }).data('daterangepicker');
    picker.container.addClass('datepicker');

    $("#reloadBtn").on("click",
        function () {
            const { startDate, endDate } = getDates();
            refreshData(startDate, endDate);
        });

    function getDates(addDays = 0) {
        const startDate = moment(picker.startDate.toDate(), "DD/MM/YYYY").add(-addDays, 'days')
            .format("MM.DD.YYYY");
        const endDate = moment(picker.endDate.toDate(), "DD/MM/YYYY").format("MM.DD.YYYY");
        return { startDate, endDate };
    }

    const { startDate, endDate } = getDates(30);
    refreshData(startDate, endDate);

    function refreshData(startDate, endDate) {
        $.ajax({
            url: "/api/Products/GetCommerceGeneralStatistic",
            data: { startDate, endDate },
            success: function (data) {
                if (!data.is_success) {
                    return;
                }
                //orders
                const orderContainer = $("#ordersReceived");
                orderContainer.find("h4").text(data.result.orderReceived.totalOrderReceived);
                orderContainer.find(".progress-bar").css("width", data.result.orderReceived.percentage + "%")
                    .attr("aria-valuenow", data.result.orderReceived.percentage);

                //earnings
                const earningsContainer = $("#earnings");
                earningsContainer.find("h4").text(data.result.totalEarnings.totalEarnings);
                earningsContainer.find(".progress-bar").css("width", data.result.totalEarnings.percentage + "%")
                    .attr("aria-valuenow", data.result.orderReceived.percentage);

                //earnings
                const newCustomersContainer = $("#newCustomers");
                newCustomersContainer.find("h4").text(data.result.newCustomers.newCustomers);
                newCustomersContainer.find(".progress-bar")
                    .css("width", data.result.newCustomers.percentage + "%")
                    .attr("aria-valuenow", data.result.orderReceived.percentage);
            },
            error: function (e) {

            }
        });
    };

    $.ajax({
        url: "/api/Products/GetYearReport",
        success: function (data) {
            if (!data.is_success) return;
            const ctx = document.getElementById('canvas').getContext('2d');
            const users = [];
            const sales = [];
            for (let i = 1; i <= 12; i++) {
                users.push(data.result[i].users);
                sales.push(data.result[i].sales);
            }
            window.myBar = new Chart(ctx,
                {
                    type: 'bar',
                    data: {
                        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                        datasets: [
                            {
                                label: 'Sales',
                                backgroundColor: window.getRandomColor(),
                                borderColor: "red",
                                data: sales
                            }, {
                                label: 'New Users',
                                backgroundColor: window.getRandomColor(),
                                borderColor: "blue",
                                data: users
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',
                            display: false
                        },
                        title: {
                            display: false,
                            text: 'Monthly Sales Report'
                        }
                    }
                });
        }
    });

    const ctxDoughnut = document.getElementById('chart-area').getContext('2d');
    $.ajax({
        url: "/api/Orders/GetOrdersGraphInfo",
        success: function (data) {
            if (!data.is_success) return;
            const values = Object.values(data.result);
            $(".total-orders").text(values.reduce(function (a, b) { return a + b; }, 0));
            window.myDoughnut = new Chart(ctxDoughnut,
                {
                    type: 'doughnut',
                    data: {
                        datasets: [
                            {
                                data: values,
                                backgroundColor: Object.keys(data.result).map(x => window.getRandomColor()),
                                label: 'Orders'
                            }
                        ],
                        labels: Object.keys(data.result)
                    },
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',
                        },
                        title: {
                            display: false,
                            text: 'Chart.js Doughnut Chart'
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        }
                    }
                });
        }
    });
};