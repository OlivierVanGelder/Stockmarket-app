import React, { useState } from "react";
import Chart from "react-apexcharts";
import dayjs from "dayjs";

interface CandleStickChartProps {
  dataset: {
    x: Date;
    y: number[]; // Open, High, Low, Close
    volume: number; // Volume data
  }[];
}

const CandleStickChart: React.FC<CandleStickChartProps> = ({ dataset }) => {
  const series = [
    {
      name: "candle",
      data: dataset.map((item) => ({
        x: item.x,
        y: item.y, // OHLC values [open, high, low, close]
        volume: item.volume, // Ensure volume is directly mapped
      })),
    },
  ];

  const [options] = useState<any>({
    chart: {
      height: 450,
      type: "candlestick",
      zoom: {
        enabled: true,
        type: "x",
        autoScaleYaxis: true,
      },
      toolbar: {
        show: false,
      },
    },
    title: {
      text: "CandleStick Chart - Category X-axis",
      align: "left",
      style: {
        color: "#9AA0A6",
      },
    },
    annotations: {
      xaxis: [
        {
          x: "Oct 06 14:00",
          borderColor: "#00E396",
          label: {
            borderColor: "#00E396",
            style: {
              fontSize: "12px",
              color: "#fff",
              background: "#00E396",
            },
            orientation: "horizontal",
            offsetY: 7,
            text: "Annotation Test",
          },
        },
      ],
    },
    tooltip: {
      enabled: true,
      custom: function ({ seriesIndex, dataPointIndex, w }: any) {
        const ohlc = w.config.series[seriesIndex].data[dataPointIndex].y; // Get OHLC values
        const volume = w.config.series[seriesIndex].data[dataPointIndex].volume; // Get volume
        console.log(w);
        if (!ohlc) {
          return '<div style="padding: 10px;">No data available</div>';
        }

        return `
          <div style="padding: 10px;">
            <div><strong>Open:</strong> ${ohlc[0]}</div>
            <div><strong>High:</strong> ${ohlc[1]}</div>
            <div><strong>Low:</strong> ${ohlc[2]}</div>
            <div><strong>Close:</strong> ${ohlc[3]}</div>
            <div><strong>Volume:</strong> ${volume}</div>
          </div>
        `;
      },
    },
    xaxis: {
      type: "category",
      labels: {
        formatter: function (val: any) {
          return dayjs(val).format("MMM DD HH:mm");
        },
        style: {
          colors: "#9AA0A6",
        },
      },
    },
    yaxis: {
      tooltip: {
        enabled: true,
      },
      labels: {
        style: {
          colors: "#9AA0A6",
        },
      },
    },
    plotOptions: {
      candlestick: {
        colors: {
          upward: "#089981", // Bullish candle color (green)
          downward: "#f23645", // Bearish candle color (red)
        },
        wick: {
          useFillColor: true, // Makes the wick the same color as the body
        },
      },
    },
  });

  return (
    <div id="chart">
      <Chart
        options={options}
        series={series}
        type="candlestick"
        height={450}
        width={1000}
      />
    </div>
  );
};

export default CandleStickChart;
