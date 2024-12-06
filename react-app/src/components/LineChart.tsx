import React from 'react'
import { Line } from 'react-chartjs-2'
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
} from 'chart.js'

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
)

interface LineGraphItem {
    chartData: {
        label: string
        values: number[]
        backgroundColor: string
        borderColor: string
    }[]
}

const LineChart: React.FC<LineGraphItem> = ({ chartData }) => {
    const data = {
        labels:
            chartData[0]?.values.map((_, index) => `Label ${index + 1}`) || [],
        datasets: chartData.map(item => ({
            label: item.label,
            data: item.values,
            backgroundColor: item.backgroundColor,
            borderColor: item.borderColor,
            fill: false
        }))
    }

    return (
        <Line
            data={data}
            options={{
                elements: {
                    point: {
                        radius: 2, // Adjust point size if needed
                        hoverRadius: 4 // Size of the point when hovered
                    },
                    line: {
                        tension: 0 // Adds a curve to the line (optional, remove if not needed)
                    }
                },
                plugins: {
                    legend: {
                        labels: {
                            color: '#9AA0A6', // Legend label color
                            font: {
                                size: 22 // Font size for legend labels
                            }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                // Format tooltip to show 2 decimal places for points
                                return `${context.dataset.label}: ${(
                                    context.raw as number
                                ).toFixed(2)}`
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        border: {
                            display: true,
                            width: 2
                        },
                        ticks: {
                            color: '#9AA0A6',
                            font: {
                                size: 14
                            }
                        },
                        grid: {
                            display: false
                        }
                    },

                    y: {
                        border: {
                            display: true,
                            width: 2
                        },
                        ticks: {
                            callback: function (value) {
                                // Display values with 2 decimal places
                                return Number(value).toFixed(2)
                            },
                            color: '#9AA0A6',
                            font: {
                                size: 14
                            }
                        },
                        grid: {
                            display: false
                        }
                    }
                }
            }}
        />
    )
}

export default LineChart
