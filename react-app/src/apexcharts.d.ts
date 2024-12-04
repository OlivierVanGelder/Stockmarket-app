declare module 'apexcharts' {
    interface ApexOptions {
        chart?
        title?
        annotations?
        tooltip?
        xaxis?
        yaxis?
        [key: string] // This line allows adding any extra properties dynamically
    }

    const ApexCharts
    export default ApexCharts
}
