"use strict";

window.onload = function () {
    $('.date-rage').daterangepicker({
        opens: 'left'
    });

    const ctx = document.getElementById('canvas').getContext('2d');
    window.myBar = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [{
                label: 'Sales',
                backgroundColor: window.getRandomColor(),
                borderColor: "red",

                data: [
                    40, 10, 60, 90, 30, 70, 30, 25, 15, 75, 30, 90
                ]
            }, {
                label: 'New Users',
                backgroundColor: window.getRandomColor(),
                borderColor: "blue",

                data: [
                    25, 15, 75, 30, 80, 20, 30, 60, 50, 60, 70, 40
                ]
            }]
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

    const ctxDoughnut = document.getElementById('chart-area').getContext('2d');
    $.ajax({
        url: "/api/Orders/GetOrdersGraphInfo",
        success: function (data) {
            if (!data.is_success) return;
            const values = Object.values(data.result);
            $(".total-orders").text(values.reduce(function (a, b) { return a + b; }, 0));
            window.myDoughnut = new Chart(ctxDoughnut, {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: values,
                        backgroundColor: Object.keys(data.result).map(x => window.getRandomColor()),
                        label: 'Orders'
                    }],
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