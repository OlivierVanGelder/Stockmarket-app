import React, { useState } from "react";
import Chart from "react-apexcharts";
import dayjs from "dayjs";

interface CandleStickChartProps {
  dataset: {
    x: Date;
    y: number[];
  }[];
}

const CandleStickChart: React.FC<CandleStickChartProps> = ({ dataset }) => {
  const series = [
    {
      name: "candle",
      data: dataset, // Use the dataset prop here
    },
  ];

  const [options] = useState<any>({
    chart: {
      height: 450,
      type: "candlestick",
      zoom: {
        enabled: true, // Enable zooming
        type: "x", // Allow zooming only on the x-axis
        autoScaleYaxis: true, // Automatically scale y-axis
      },
      toolbar: {
        show: false, // Disable the toolbar (menu button)
      },
    },
    title: {
      text: "CandleStick Chart - Category X-axis",
      align: "left",
      style: {
        color: "#9AA0A6", // Setting the title color
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
      enabled: true, // Enable tooltips
    },
    xaxis: {
      type: "category",
      labels: {
        formatter: function (val: any) {
          return dayjs(val).format("MMM DD HH:mm");
        },
        style: {
          colors: "#9AA0A6", // Setting the x-axis label color
        },
      },
    },
    yaxis: {
      tooltip: {
        enabled: true, // Enable y-axis tooltip
      },
      labels: {
        style: {
          colors: "#9AA0A6", // Setting the y-axis label color
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
