declare module "apexcharts" {
    interface ApexOptions {
      chart?: any;
      title?: any;
      annotations?: any;
      tooltip?: any;
      xaxis?: any;
      yaxis?: any;
      [key: string]: any; // This line allows adding any extra properties dynamically
    }
  
    const ApexCharts: any;
    export default ApexCharts;
  }  