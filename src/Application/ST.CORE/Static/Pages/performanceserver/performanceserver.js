// ============================================================== 
// Gauge chart option
// ============================================================== 
var gaugeChart = echarts.init(document.getElementById('gauge-chart'));
option = {
	tooltip: {
		formatter: "{a} <br/>{b} : {c}%"
	}
	, toolbox: {
		show: false
		, feature: {
			mark: {
				show: true
			}
			, restore: {
				show: true
			}
			, saveAsImage: {
				show: true
			}
		}
	}
	, series: [
		{
			name: ''
			, type: 'gauge'
			, splitNumber: 0,
			axisLine: {
				lineStyle: {
					color: [[0.2, 'green'], [0.8, 'orange'], [1, 'red']]
					, width: 20
				}
			}
			, axisTick: {
				splitNumber: 0,
				length: 12,
				lineStyle: {
					color: 'auto'
				}
			}
			, axisLabel: {
				textStyle: {
					color: 'auto'
				}
			}
			, splitLine: {
				show: false,
				length: 50,
				lineStyle: {
					color: 'auto'
				}
			}
			, pointer: {
				width: 5
				, color: '#54667a'
			}
			, title: {
				show: false
				, offsetCenter: [0, '-40%'],
				textStyle: {
					fontWeight: 'bolder'
				}
			}
			, detail: {
				textStyle: {
					color: 'auto'
					, fontSize: '14'
					, fontWeight: 'bolder'
				}
			}
			, data: [{
				value: 50
				, name: ''
			}]
		}
	]
};
timeTicket = setInterval(function () {
	const data = window.load("/ServerPerformance/GetCpuPercentage");
	option.series[0].data[0].value = data.toFixed(2) - 0;
	gaugeChart.setOption(option, true);
}, 2000)
// use configuration item and data specified to show chart
gaugeChart.setOption(option, true), $(function () {
	function resize() {
		setTimeout(function () {
			gaugeChart.resize()
		}, 100)
	}
	$(window).on("resize", resize), $(".sidebartoggler").on("click", resize)
});