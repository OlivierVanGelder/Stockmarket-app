import { Line } from "react-chartjs-2";
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from "chart.js";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

function LineChart({ chartData}) {
  return (
    <Line 
      data={chartData} 
      options={{
        elements: {
          point: {
            radius: 2, // Adjust point size if needed
            hoverRadius: 4, // Size of the point when hovered
          },
          line: {
            tension: 0, // Adds a curve to the line (optional, remove if not needed)
          },
        },
        plugins: {
          legend: {
            labels: {
              color: "#9AA0A6", // Legend label color
              font: {
                size: 22, // Font size for legend labels
              },
            },
          },
          tooltip: {
            callbacks: {
              label: function (context) {
                // Format tooltip to show 2 decimal places for points
                return `${context.dataset.label}: ${context.raw.toFixed(2)}`;
              },
            },
          },
        },
        scales: {
          x: {
            border: {
              display: true,
              width: 2,
            },
            ticks: {
              color: "#9AA0A6",
              font: {
                size: 14,
              },
            },
            grid: {
              display: false,
            },
          },
          
          y: {
            border: {
              display: true,
              width: 2,
            },
            ticks: {
              callback: function (value) {
                // Display values with 2 decimal places
                return value.toFixed(2);
              },
              color: "#9AA0A6",
              font: {
                size: 14,
              },
            },
            grid: {
              display: false,
            },
          },
        },
      }}
    />
  );
}

export default LineChart;